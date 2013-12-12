using MMDB.Shared;
using ServiceStack.Common.Web;
using ServiceStack.ServiceClient.Web;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Dto.Build;
using Sriracha.Deploy.Data.Dto.Deployment;
using Sriracha.Deploy.Data.Dto.Deployment.Offline;
using Sriracha.Deploy.Data.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace Sriracha.Deploy.Offline
{
    public partial class ExportHistoryForm : Form
    {
        private DeployBatchRequest _batchRequest;
        private string _workingDirectory;
        private IDIFactory _diFactory;

        public ExportHistoryForm(IDIFactory diFactory, DeployBatchRequest batchRequest, string workingDirectory)
        {
            _diFactory = diFactory;
            _batchRequest = batchRequest;
            _workingDirectory = workingDirectory;
            InitializeComponent();
        }

        private void ExportHistoryForm_Load(object sender, EventArgs e)
        {
            _txtOutputFileName.Text = Path.Combine(_workingDirectory, "HistoryExport.zip");
            _chkPublishToWebsite.Enabled = true;
            _txtPublishUrl.Text = AppSettingsHelper.GetSetting("SiteUrl");
        }

        private void _chkPublishToWebsite_CheckedChanged(object sender, EventArgs e)
        {
            _txtPublishUrl.Enabled = 
                _btnTestUrl.Enabled = 
                _txtUserName.Enabled = 
                _txtPassword.Enabled = 
            _chkPublishToWebsite.Checked;
        }

        private void _btnTestUrl_Click(object sender, EventArgs e)
        {
            string rootUrl = _txtPublishUrl.Text;
            string fullUrl = rootUrl;
            if(!fullUrl.EndsWith("/"))
            {
                fullUrl += "/";
            }
            fullUrl += "api/deploy/offline?format=json&deployBatchRequestId=" + _batchRequest.Id;
            try 
            {
                Cookie authCookie = null;
                if (!string.IsNullOrEmpty(_txtUserName.Text) && !string.IsNullOrEmpty(_txtPassword.Text))
                {
                    authCookie = GetAuthCookie(rootUrl, _txtUserName.Text, _txtPassword.Text);
                }

                OfflineDeployment result;
                //result = WinFormsHelper.GetJsonUrl<OfflineDeployment>(fullUrl);
                using(var client = new JsonServiceClient(rootUrl))
                {
                    client.AllowAutoRedirect = false;
					client.Credentials = System.Net.CredentialCache.DefaultCredentials;
					if (authCookie != null)
                    {
                        client.CookieContainer.Add(authCookie);
                    }
                    result = client.Get<OfflineDeployment>(fullUrl);
                }
                if(result == null)
                {
                    throw new Exception("Failed to retrieve offline deployment for deploy batch request ID " + _batchRequest.Id);
                }
                if(result.DeployBatchRequestId != _batchRequest.Id)
                {
                    throw new Exception("Invalid data received for offline deployment for deploy batch request ID " + _batchRequest.Id);
                }
                MessageBox.Show("Connection Successful");
            }
            catch(Exception err)
            {
                var newErr = new Exception("Error testing connection: " + fullUrl, err);
                WinFormsHelper.DisplayError(err.Message, newErr);
            }

        }
        private Cookie GetAuthCookie(string url, string userName, string password)
        {
            string logonUrl = url;
            if (logonUrl.EndsWith("/"))
            {
                logonUrl = logonUrl.Substring(0, logonUrl.Length - 1);
            }
            if (logonUrl.EndsWith("/api", StringComparison.CurrentCultureIgnoreCase))
            {
                logonUrl = logonUrl.Substring(0, logonUrl.Length - "/api".Length);
            }
            if (!logonUrl.EndsWith("/"))
            {
                logonUrl += "/";
            }
            logonUrl += "Account/LogOnJson";
            var request = HttpWebRequest.Create(logonUrl);
            request.Method = "POST";
            request.ContentType = "application/json";
            using (var requestStream = request.GetRequestStream())
            using (var requestWriter = new StreamWriter(requestStream))
            {
                var postObject = new { UserName = userName, Password = password };
                string postJson = postObject.ToJson();
                requestWriter.Write(postJson);
                requestWriter.Flush();
            }
            using (var response = request.GetResponse())
            using (var responseStream = response.GetResponseStream())
            using (var responseReader = new StreamReader(responseStream))
            {
                var responseString = responseReader.ReadToEnd();
                var authResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<Sriracha.Deploy.Data.Account.AuthResponse>(responseString);
                if (!authResponse.Success)
                {
                    throw new Exception("Failed to authenticate user " + userName + ": " + StringHelper.IsNullOrEmpty(authResponse.ErrorMessage, "Unknown Error"));
                }
                return new Cookie(authResponse.CookieName, authResponse.CookieValue, authResponse.CookiPath, authResponse.CookieDomain);
            }
        }

        private void _btnExport_Click(object sender, EventArgs e)
        {
            try 
            {
                if (string.IsNullOrEmpty(_txtOutputFileName.Text))
                {
                    MessageBox.Show("Please enter an output file name");
                    return;
                }
                if(_chkPublishToWebsite.Checked && string.IsNullOrEmpty(_txtPublishUrl.Text))
                {
                    MessageBox.Show("Please enter a publish URL");
                    return;
                }

                var deployStateDirectory = Path.Combine(_workingDirectory,"states");
                if(!Directory.Exists(deployStateDirectory))
                {
                    MessageBox.Show("No deploy history found");
                    return;
                }
                var stateFileList = Directory.GetFiles(deployStateDirectory, "*.json");
                if(stateFileList.Length == 0)
                {
                    MessageBox.Show("No deploy history found");
                    return;
                }
                var exportDirectory = Path.Combine(_workingDirectory, "exports", Guid.NewGuid().ToString());
                if (!Directory.Exists(exportDirectory))
                {
                    Directory.CreateDirectory(exportDirectory);
                }
                foreach (var fileName in stateFileList)
                {
                    string exportFileName = Path.Combine(exportDirectory, Path.GetFileName(fileName));
                    File.Copy(fileName, exportFileName);
                }
                string zipPath = _txtOutputFileName.Text;
                var zipper = _diFactory.CreateInjectedObject<IZipper>();
                zipper.ZipDirectory(exportDirectory, zipPath);

                if(_chkPublishToWebsite.Checked)
                {
                    var url = _txtPublishUrl.Text;
                    if (!url.EndsWith("/"))
                    {
                        url += "/";
                    }
                    if (!url.EndsWith("/api/", StringComparison.CurrentCultureIgnoreCase))
                    {
                        url += "api/";
                    }
                    var deployFile = new DeployFileDto
                    {
                        FileData = File.ReadAllBytes(zipPath),
                        FileName = Path.GetFileName(zipPath)
                    };

                    Cookie authCookie = null;
                    if (!string.IsNullOrEmpty(_txtUserName.Text) && !string.IsNullOrEmpty(_txtPassword.Text))
                    {
                        authCookie = GetAuthCookie(url, _txtUserName.Text, _txtPassword.Text);
                    }
                
                    using (var client = new JsonServiceClient(url))
			        {
				        var fileToUpload = new FileInfo(zipPath);
						client.AllowAutoRedirect = false;
				        client.Credentials = System.Net.CredentialCache.DefaultCredentials;
				        client.Timeout = TimeSpan.FromMinutes(2);
				        client.ReadWriteTimeout = TimeSpan.FromMinutes(2);
                        if(authCookie != null)
                        {  
                            client.CookieContainer.Add(authCookie);
                        }

                        var offlineDeployment = client.Get<OfflineDeployment>(url + "deploy/offline?deployBatchRequestId=" + _batchRequest.Id);
                        if(offlineDeployment == null)
                        {
                            throw new Exception("Could not find offline deployment record for batch request ID " + _batchRequest.Id);
                        }

				        var fileResponse = client.PostFile<DeployFileDto>(url + "file", fileToUpload, MimeTypes.GetMimeType(fileToUpload.Name));

                        var updateRequest = new OfflineDeployment
                        {
                            Id = offlineDeployment.Id,
                            DeployBatchRequestId = _batchRequest.Id,
                            ResultFileId = fileResponse.Id
                        };
                        client.Post<OfflineDeployment>(url + "deploy/offline", updateRequest);
                    }
                }
                MessageBox.Show("Export Complete");
            }
            catch (Exception err)
            {
                WinFormsHelper.DisplayError("Error exporting history", err);
            }
        }
    }
}
