using Microsoft.WindowsAzure.Storage;
using MMDB.Shared;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Tasks.Azure.AzureDto;
using Sriracha.Deploy.Tasks.Azure.AzureDto.AzureCloudService;
using Sriracha.Deploy.Tasks.Azure.AzureDto.AzureLocation;
using Sriracha.Deploy.Tasks.Azure.AzureDto.AzureStorage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;

namespace Sriracha.Deploy.Tasks.Azure
{
    public class AzureClient
    {
        private readonly string _subscriptionIdentifier;
        private readonly string _managementCertificate;

        public AzureClient(string subscriptionIdentifier, string managementCertificate)
        {
            _subscriptionIdentifier = subscriptionIdentifier;
            _managementCertificate = managementCertificate;
        }

        private static HttpWebRequest CreateRequest(string url, string managementCertificate)
        {
            var cert = new System.Security.Cryptography.X509Certificates.X509Certificate2(Convert.FromBase64String(managementCertificate));
            var request = (HttpWebRequest)HttpWebRequest.CreateDefault(new Uri(url));

            request.Headers.Add("x-ms-version", "2013-11-01");
            request.Method = "GET";
            request.ContentType = "application/xml";

            // Attach the certificate to the request.
            request.ClientCertificates.Add(cert);

            return request;
        }

        public List<HostedService> GetCloudServiceList()
        {
            string url = string.Format("https://management.core.windows.net/{0}/services/hostedservices", _subscriptionIdentifier);
            var request = CreateRequest(url, _managementCertificate);
            using (var response = request.GetResponse())
            using (var responseStream = response.GetResponseStream())
            using (var reader = new StreamReader(responseStream))
            {
                var xml = reader.ReadToEnd();
                var serializer = new XmlSerializer(typeof(HostedServiceListResponse), "http://schemas.microsoft.com/windowsazure");
                var returnValue = (HostedServiceListResponse)serializer.Deserialize(new StringReader(xml));
                return returnValue.HostedServiceList;
            }
        }

        public void CreateCloudService(string serviceName, string label=null, string location=null, string affinityGroup=null)
        {
            if(!string.IsNullOrEmpty(location) && !string.IsNullOrEmpty(affinityGroup))
            {
                throw new ArgumentException("Location and AffinityGroup cannot both be specified");
            }
            if(string.IsNullOrEmpty(location) && string.IsNullOrEmpty(location))
            {
                var locationItem = GetDefaultLocation();
                if(locationItem == null)
                {
                    throw new Exception("Unable to find default location");
                }
                location = locationItem.Name;
            }
            string base64Label;
            if(!string.IsNullOrEmpty(label))
            {
                base64Label = Convert.ToBase64String(Encoding.UTF8.GetBytes(label));
            }
            else 
            {
                base64Label = Convert.ToBase64String(Encoding.UTF8.GetBytes(serviceName));
            }
            var requestData = new CreateHostedServiceRequest
            {
                ServiceName = serviceName,
                Label = base64Label,
                Location = location,
                AffinityGroup = affinityGroup
            };
            string requestXml;
            using (var writer = new Utf8StringWriter())
            {
                var serializer = new XmlSerializer(typeof(CreateHostedServiceRequest), "http://schemas.microsoft.com/windowsazure");
                serializer.Serialize(writer, requestData);
                requestXml = writer.ToString();
            }
            string url = string.Format("https://management.core.windows.net/{0}/services/hostedservices", _subscriptionIdentifier);
            var request = CreateRequest(url, _managementCertificate);
            request.Method = "POST";
            using(var stream = request.GetRequestStream())
            using(var writer = new StreamWriter(stream))
            {
                writer.Write(requestXml);
            }
            
            try 
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (var responseStream = response.GetResponseStream())
                using (var reader = new StreamReader(responseStream))
                {
                    string responseData = reader.ReadToEnd();
                    if(response.StatusCode != HttpStatusCode.Created)
                    {
                        string message = string.Format("Failed to create cloud service, return status {0}", response.StatusCode);
                        if(!string.IsNullOrEmpty(responseData))
                        {
                            message += ", Details: " + responseData;
                        }
                        throw new Exception(message);
                    }
                }
            }
            catch(WebException webEx)
            {
                var resp = webEx.Response;
                var responseDetails = (new System.IO.StreamReader(resp.GetResponseStream(), true)).ReadToEnd();
                if(!string.IsNullOrEmpty(responseDetails))
                {
                    throw new Exception("Failed to create cloud service, Details: " + responseDetails, webEx);
                }
                else 
                {
                    throw;
                }
            }
        }

        private Location GetDefaultLocation()
        {
            var locationList = GetLocationList();
            if(locationList != null)
            {
                string defaultDefault = "East US";
                var acceptableItemList = locationList.Where(i=>i.AvailableServiceList != null 
                                                                && i.AvailableServiceList.Contains(Location.EnumAvailableService.Compute)
                                                                && i.AvailableServiceList.Contains(Location.EnumAvailableService.Storage));
                var returnValue = acceptableItemList.FirstOrDefault(i=>i.Name == defaultDefault);
                if(returnValue == null)
                {
                    returnValue = acceptableItemList.FirstOrDefault();
                }
                return returnValue;
            }
            return null;
        }

        private List<Location> GetLocationList()
        {
            string url = string.Format("https://management.core.windows.net/{0}/locations", _subscriptionIdentifier);
            var request = CreateRequest(url, _managementCertificate);
            using (var response = request.GetResponse())
            using (var responseStream = response.GetResponseStream())
            using (var reader = new StreamReader(responseStream))
            {
                var xml = reader.ReadToEnd();
                var serializer = new XmlSerializer(typeof(LocationsResponse), "http://schemas.microsoft.com/windowsazure");
                var returnValue = (LocationsResponse)serializer.Deserialize(new StringReader(xml));
                return returnValue.LocationList;
            }
        }

        public bool CheckCloudServiceNameAvailability(string serviceName, out string message)
        {
            string url = string.Format("https://management.core.windows.net/{0}/services/hostedservices/operations/isavailable/{1}", _subscriptionIdentifier, Uri.EscapeUriString(serviceName));
            var request = CreateRequest(url, _managementCertificate);
            using (var response = request.GetResponse())
            using (var responseStream = response.GetResponseStream())
            using (var reader = new StreamReader(responseStream))
            {
                var xml = reader.ReadToEnd();
                var serializer = new XmlSerializer(typeof(AvailabilityResponse), "http://schemas.microsoft.com/windowsazure");
                var returnValue = (AvailabilityResponse)serializer.Deserialize(new StringReader(xml));
                message = returnValue.Reason;
                return returnValue.Result;
            }
        }

        public HostedService GetHostedService(string serviceName)
        {
            string url = string.Format("https://management.core.windows.net/{0}/services/hostedservices/{1}?embed-details=true", _subscriptionIdentifier, Uri.EscapeDataString(serviceName));
            var request = CreateRequest(url, _managementCertificate);
            try 
            {
                using (var response = request.GetResponse())
                using (var responseStream = response.GetResponseStream())
                using (var reader = new StreamReader(responseStream))
                {
                    var xml = reader.ReadToEnd();
                    var serializer = new XmlSerializer(typeof(HostedService), "http://schemas.microsoft.com/windowsazure");
                    var returnValue = (HostedService)serializer.Deserialize(new StringReader(xml));
                    return returnValue;
                }
            }
            catch(WebException ex)
            {
                if(((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }
                throw;
            }
        }

        public List<StorageService> GetStorageAccountList()
        {
            string url = string.Format("https://management.core.windows.net/{0}/services/storageservices", _subscriptionIdentifier);
            var request = CreateRequest(url, _managementCertificate);
            using (var response = request.GetResponse())
            using (var responseStream = response.GetResponseStream())
            using (var reader = new StreamReader(responseStream))
            {
                var xml = reader.ReadToEnd();
                var serializer = new XmlSerializer(typeof(StorageServiceListResponse), "http://schemas.microsoft.com/windowsazure");
                var returnValue = (StorageServiceListResponse)serializer.Deserialize(new StringReader(xml));
                return returnValue.StorageServiceList;
            }
        }

        public StorageService GetStorageAccount(string storageAccountName)
        {
            string url = string.Format("https://management.core.windows.net/{0}/services/storageservices/{1}", _subscriptionIdentifier, Uri.EscapeUriString(storageAccountName));
            var request = CreateRequest(url, _managementCertificate);
            try
            {
                using (var response = request.GetResponse())
                using (var responseStream = response.GetResponseStream())
                using (var reader = new StreamReader(responseStream))
                {
                    var xml = reader.ReadToEnd();
                    var serializer = new XmlSerializer(typeof(StorageService), "http://schemas.microsoft.com/windowsazure");
                    var returnValue = (StorageService)serializer.Deserialize(new StringReader(xml));
                    return returnValue;
                }
            }
            catch (WebException ex)
            {
                if (((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }

                var resp = ex.Response;
                var responseDetails = (new System.IO.StreamReader(resp.GetResponseStream(), true)).ReadToEnd();
                if (!string.IsNullOrEmpty(responseDetails))
                {
                    throw new Exception("Failed to create storage account, Details: " + responseDetails, ex);
                }
                else
                {
                    throw;
                }
            }
        }

        public void CreateStorageAccount(string storageAccountName, string label=null, string location=null, string affinityGroup=null)
        {
            if (!string.IsNullOrEmpty(location) && !string.IsNullOrEmpty(affinityGroup))
            {
                throw new ArgumentException("Location and AffinityGroup cannot both be specified");
            }
            if (string.IsNullOrEmpty(location) && string.IsNullOrEmpty(location))
            {
                var locationItem = GetDefaultLocation();
                if (locationItem == null)
                {
                    throw new Exception("Unable to find default location");
                }
                location = locationItem.Name;
            }
            string base64Label;
            if (!string.IsNullOrEmpty(label))
            {
                base64Label = Convert.ToBase64String(Encoding.UTF8.GetBytes(label));
            }
            else
            {
                base64Label = Convert.ToBase64String(Encoding.UTF8.GetBytes(storageAccountName));
            }
            var requestData = new CreateStorageServiceRequest
            {
                ServiceName = storageAccountName,
                Label = base64Label,
                Location = location,
                AffinityGroup = affinityGroup,
                GeoReplicationEnabled = false,
                SecondaryReadEnabled = false
            };
            string requestXml;
            using (var writer = new Utf8StringWriter())
            {
                var serializer = new XmlSerializer(typeof(CreateStorageServiceRequest), "http://schemas.microsoft.com/windowsazure");
                serializer.Serialize(writer, requestData);
                requestXml = writer.ToString();
            }
            string url = string.Format("https://management.core.windows.net/{0}/services/storageservices", _subscriptionIdentifier);
            var request = CreateRequest(url, _managementCertificate);
            request.Method = "POST";
            using (var stream = request.GetRequestStream())
            using (var writer = new StreamWriter(stream))
            {
                writer.Write(requestXml);
            }

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (var responseStream = response.GetResponseStream())
                using (var reader = new StreamReader(responseStream))
                {
                    string responseData = reader.ReadToEnd();
                    if (response.StatusCode != HttpStatusCode.Accepted)
                    {
                        string message = string.Format("Failed to create storage account, return status {0}", response.StatusCode);
                        if (!string.IsNullOrEmpty(responseData))
                        {
                            message += ", Details: " + responseData;
                        }
                        throw new Exception(message);
                    }
                }
            }
            catch (WebException webEx)
            {
                var resp = webEx.Response;
                var responseDetails = (new System.IO.StreamReader(resp.GetResponseStream(), true)).ReadToEnd();
                if (!string.IsNullOrEmpty(responseDetails))
                {
                    throw new Exception("Failed to create storage account, Details: " + responseDetails, webEx);
                }
                else
                {
                    throw;
                }
            }
        }

        public bool CheckStorageAccountNameAvailability(string storageAccountName)
        {
            return true;
        }

        public StorageServiceKeys GetStorageAccountKeys(string storageAccountName)
        {
            string url = string.Format("https://management.core.windows.net/{0}/services/storageservices/{1}/keys", _subscriptionIdentifier, Uri.EscapeUriString(storageAccountName));
            var request = CreateRequest(url, _managementCertificate);
            using (var response = request.GetResponse())
            using (var responseStream = response.GetResponseStream())
            using (var reader = new StreamReader(responseStream))
            {
                var xml = reader.ReadToEnd();
                var serializer = new XmlSerializer(typeof(StorageService), "http://schemas.microsoft.com/windowsazure");
                var returnValue = (StorageService)serializer.Deserialize(new StringReader(xml));
                return returnValue.StorageServiceKeys;
            }
        }

        public string UploadBlobFile(string storageAccountName, string accessKey, string localPath, string containerName)
        {
            var connectionString = GetBlogConnectionString(storageAccountName, accessKey);
            var account = CloudStorageAccount.Parse(connectionString);
            var blobClient = account.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(containerName);
            container.CreateIfNotExists();
            
            string targetFileName =  string.Format("{0}_{1}_{2}__{3}_{4}_{5}__{6}__{7}",
                                                        DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day,
                                                        DateTime.UtcNow.Hour, DateTime.UtcNow.Minute, DateTime.UtcNow.Second, DateTime.UtcNow.Ticks,
                                                        Path.GetFileName(localPath));
            var blob = container.GetBlockBlobReference(targetFileName);
            blob.UploadFromFile(localPath, FileMode.Open);

            return blob.Uri.ToString();
        }

        private string GetBlogConnectionString(string storageAccountName, string accessKey)
        {
            return string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}", storageAccountName, accessKey);
        }

        public void CreateCloudServiceDeployment(string serviceName, string blobUrl, string configurationData, string deploymentSlot)
        {
            string base64Label = Convert.ToBase64String(Encoding.UTF8.GetBytes(serviceName));
            string base64Configuation = Convert.ToBase64String(Encoding.UTF8.GetBytes(configurationData));
            var requestData = new CreateDeploymentRequest
            {
                Name = serviceName + deploymentSlot,
                Label = base64Label,
                PackageUrl = blobUrl,
                Configuration = base64Configuation,
                StartDeployment = true,
                TreatWarningsAsError = true
            };
            string requestXml;
            using (var writer = new Utf8StringWriter())
            {
                var serializer = new XmlSerializer(typeof(CreateDeploymentRequest), "http://schemas.microsoft.com/windowsazure");
                serializer.Serialize(writer, requestData);
                requestXml = writer.ToString();
            }
            string url = string.Format("https://management.core.windows.net/{0}/services/hostedservices/{1}/deploymentslots/{2}", _subscriptionIdentifier, Uri.EscapeUriString(serviceName), Uri.EscapeUriString(deploymentSlot));
            var request = CreateRequest(url, _managementCertificate);
            request.Method = "POST";
            using (var stream = request.GetRequestStream())
            using (var writer = new StreamWriter(stream))
            {
                writer.Write(requestXml);
            }

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (var responseStream = response.GetResponseStream())
                using (var reader = new StreamReader(responseStream))
                {
                    string responseData = reader.ReadToEnd();
                    if (response.StatusCode != HttpStatusCode.Accepted)
                    {
                        string message = string.Format("Failed to create cloud service deployment, return status {0}", response.StatusCode);
                        if (!string.IsNullOrEmpty(responseData))
                        {
                            message += ", Details: " + responseData;
                        }
                        throw new Exception(message);
                    }
                }
            }
            catch (WebException webEx)
            {
                var resp = webEx.Response;
                var responseDetails = (new System.IO.StreamReader(resp.GetResponseStream(), true)).ReadToEnd();
                if (!string.IsNullOrEmpty(responseDetails))
                {
                    throw new Exception("Failed to create cloud service deployment, Details: " + responseDetails, webEx);
                }
                else
                {
                    throw;
                }
            }
        }

        public DeploymentItem GetCloudServiceDeployment(string serviceName, string deploymentSlot)
        {
            string url = string.Format("https://management.core.windows.net/{0}/services/hostedservices/{1}/deploymentslots/{2}", _subscriptionIdentifier, serviceName, deploymentSlot);
            var request = CreateRequest(url, _managementCertificate);
            try
            {
                using (var response = request.GetResponse())
                using (var responseStream = response.GetResponseStream())
                using (var reader = new StreamReader(responseStream))
                {
                    var xml = reader.ReadToEnd();
                    Debug.WriteLine("");
                    Debug.WriteLine(xml);
                    Debug.WriteLine("");
                    var serializer = new XmlSerializer(typeof(DeploymentItem), "http://schemas.microsoft.com/windowsazure");
                    var returnValue = (DeploymentItem)serializer.Deserialize(new StringReader(xml));
                    return returnValue;
                }
            }
            catch (WebException ex)
            {
                if (((HttpWebResponse)ex.Response).StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }
                throw;
            }
        }

        public StorageService WaitForStorageAccountStatus(string storageAccountName, StorageServiceProperties.EnumStorageServiceStatus status, TimeSpan timeout)
        {
            DateTime start = DateTime.UtcNow;
            DateTime killTime = start.Add(timeout);
            StorageServiceProperties.EnumStorageServiceStatus lastStatus = StorageServiceProperties.EnumStorageServiceStatus.Unknown;
            while(DateTime.UtcNow < killTime)
            {
                var storageAccount = GetStorageAccount(storageAccountName);
                if(storageAccount == null || storageAccount.StorageServiceProperties == null)
                {
                    throw new Exception("Failed to find storage account " + storageAccountName);
                }
                if(storageAccount.StorageServiceProperties.Status == status)
                {
                    return storageAccount;
                }
                lastStatus = storageAccount.StorageServiceProperties.Status.GetValueOrDefault();
                Thread.Sleep(1000);
            }
            throw new TimeoutException(string.Format("Timeout (0 seconds) waiting for status {1}, last status {2}", timeout.TotalSeconds, status, lastStatus));
        }

        public DeploymentItem WaitForCloudServiceDeploymentStatus(string serviceName, string deploymentSlot, DeploymentItem.EnumDeploymentItemStatus status, TimeSpan timeout)
        {
            DateTime start = DateTime.UtcNow;
            DateTime killTime = start.Add(timeout);
            DeploymentItem.EnumDeploymentItemStatus lastStatus = DeploymentItem.EnumDeploymentItemStatus.Unknown;
            while (DateTime.UtcNow < killTime)
            {
                var deployment = this.GetCloudServiceDeployment(serviceName, deploymentSlot);
                if (deployment == null)
                {
                    throw new Exception("Failed to find deployment " + serviceName + " for slot " + deploymentSlot);
                }
                if (deployment.Status == status)
                {
                    return deployment;
                }
                lastStatus = deployment.Status.GetValueOrDefault();
                Thread.Sleep(1000);
            }
            throw new TimeoutException(string.Format("Timeout (0 seconds) waiting for status {1}, last status {2}", timeout.TotalSeconds, status, lastStatus));
        }

        public DeploymentItem WaitForAllCloudServiceInstanceStatus(string serviceName, string deploymentSlot, RoleInstance.EnumInstanceStatus status, TimeSpan timeout)
        {
            DateTime start = DateTime.UtcNow;
            DateTime killTime = start.Add(timeout);
            List<RoleInstance.EnumInstanceStatus> lastStatusList = new List<RoleInstance.EnumInstanceStatus>();
            while (DateTime.UtcNow < killTime)
            {
                var deployment = this.GetCloudServiceDeployment(serviceName, deploymentSlot);
                if (deployment == null)
                {
                    throw new Exception("Failed to find deployment " + serviceName + " for slot " + deploymentSlot);
                }
                if(deployment.RoleInstanceList != null && deployment.RoleInstanceList.Count != 0)
                {
                    lastStatusList = deployment.RoleInstanceList.Select(i=>i.InstanceStatus.GetValueOrDefault()).ToList();
                    if(!lastStatusList.Any(i=>i != status))
                    {
                        return deployment;
                    }
                }
                Thread.Sleep(1000);
            }
            throw new TimeoutException(string.Format("Timeout (0 seconds) waiting for status {1}, last statuses {2}", timeout.TotalSeconds, status, string.Join("|", lastStatusList.Select(i=>i.ToString()))));
        }
    }
}
