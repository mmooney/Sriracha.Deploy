﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using ServiceStack.Common.Web;
using ServiceStack.ServiceClient.Web;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Utility;
using Sriracha.Deploy.Data.Dto.Build;
using System.Net;
using System.Diagnostics;
using MMDB.Shared;
using Sriracha.Deploy.Data.Dto.Deployment;
using Sriracha.Deploy.Data.Dto.Project;

namespace Sriracha.Deploy.Data.Build.BuildImpl
{
	public class BuildPublisher : IBuildPublisher
	{
		private readonly IZipper _zipper;
		private readonly Logger _logger;
		private readonly IRegexResolver _regexResolver;

		public BuildPublisher(IZipper zipper, Logger logger, IRegexResolver regexResolver)
		{
			_zipper = DIHelper.VerifyParameter(zipper);
			_logger = DIHelper.VerifyParameter(logger);
			_regexResolver = DIHelper.VerifyParameter(regexResolver);
		}

		public void PublishDirectory(BuildPublishOptions options)
		{
			if(string.IsNullOrEmpty(options.Directory))
			{
				throw new ArgumentNullException("Missing DirectoryPath");
			}
			if(string.IsNullOrEmpty(options.ApiUrl))
			{
				throw new ArgumentNullException("Missing ApiUrl");
			}
			if(string.IsNullOrEmpty(options.ProjectId))
			{
				throw new ArgumentNullException("Missing ProjectId");
			}
			if(string.IsNullOrEmpty(options.ComponentId))
			{
				throw new ArgumentNullException("Missing ComponentId");
			}
			if(string.IsNullOrEmpty(options.BranchId))
			{
				throw new ArgumentNullException("Missing BranchId");
			}
			if(string.IsNullOrEmpty(options.Version))
			{
				throw new ArgumentNullException("Missing Version");
			}
			_logger.Info("Start publishing directory {0} to URL {1}", options.Directory, options.ApiUrl);
			_regexResolver.ResolveValues(options);
			string zipPath = Path.ChangeExtension(Path.GetTempFileName(), ".zip");
			if(!Directory.Exists(options.Directory))
			{
				throw new DirectoryNotFoundException(string.Format("Publish directory \"{0}\" not found", options.Directory));
			}
			_zipper.ZipDirectory(options.Directory, zipPath);

			var buildData = PublishZip(options.ApiUrl, options.ProjectId, options.ComponentId, options.BranchId, options.Version, zipPath, options.UserName, options.Password);
            DeployBuilds(options.ApiUrl, options.UserName, options.Password, options.DeployEnvironmentName, buildData);

			try 
			{
				File.Delete(zipPath);
			}
			catch(Exception err)
			{
				_logger.ErrorException(string.Format("Failed to delete ZIP file {0}: {1}",  zipPath, err.ToString()), err);
			}
			_logger.Info("Done publishing directory {0} to URL {1}", options.Directory, options.ApiUrl);
		}

        private DeployBatchRequest DeployBuilds(string apiUrl, string userName, string password, string environmentName, params DeployBuild[] buildData)
        {
            if(string.IsNullOrEmpty(environmentName))
            {
                return null;
            }
            string url = apiUrl;
            if (!url.EndsWith("/"))
            {
                url += "/";
            }
            if (!url.EndsWith("/api/", StringComparison.CurrentCultureIgnoreCase))
            {
                url += "api/";
            }

            Cookie authCookie = null;
            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
            {
                authCookie = GetAuthCookie(apiUrl, userName, password);
            }
            using (var client = new JsonServiceClient(url))
            {
                //var x = client.Send<DeployFileDto>(deployFile);
                client.Credentials = System.Net.CredentialCache.DefaultCredentials;
                client.Timeout = TimeSpan.FromMinutes(5);
                client.ReadWriteTimeout = TimeSpan.FromMinutes(5);
                if (authCookie != null)
                {
                    client.CookieContainer.Add(authCookie);
                }

                var projectIdList = buildData.Select(i=>i.ProjectId).Distinct().ToList();
                var componentIdMachines = new Dictionary<string, DeployMachine>();
                foreach(var build in buildData)
                {
                    string projectUrl = url + "project/?format=json&id=" + System.Uri.EscapeDataString(build.ProjectId);
                    var project = client.Get<DeployProject>(projectUrl);
                    var environment = project.EnvironmentList.FirstOrDefault(i=> environmentName.Equals(i.EnvironmentName, StringComparison.CurrentCultureIgnoreCase));
                    if(environment == null)
                    {
                        throw new Exception("Unable to find environment " + environmentName + " in project " + project.ProjectName);
                    }
                    var component = project.GetComponent(build.ProjectComponentId);
                    if(component.UseConfigurationGroup && !string.IsNullOrEmpty(component.ConfigurationId))
                    {
                        var environmentConfiguration = environment.GetConfigurationItem(component.ConfigurationId);
                        foreach(var machine in environmentConfiguration.MachineList)
                        {
                            componentIdMachines.Add(component.Id, machine);
                        }
                    }
                    else 
                    {
                        var environmentComponent = environment.GetComponentItem(component.Id);
                        foreach(var machine in environmentComponent.MachineList)
                        {
                            componentIdMachines.Add(component.Id, machine);
                        }
                    }
                }
                var deployBatchRequest = new DeployBatchRequest
                {
                    DeploymentLabel = "Auto-Deploy " + DateTime.UtcNow.ToString(),
                    Status = EnumDeployStatus.Requested, 
                    ItemList = (from b in buildData
                                select new DeployBatchRequestItem
                                {
                                    Build = b,
                                    MachineList = componentIdMachines.Where(i=>i.Key == b.ProjectComponentId).Select(i=>i.Value).ToList()
                                }).ToList()
                };

                var batchRequestUrl = url + "deploy/batch/request";
                _logger.Debug("Posting DeployBatchRequest object to URL {0}, sending{1}", batchRequestUrl, deployBatchRequest.ToJson());
                DeployBatchRequest response;
                try
                {
                    response = client.Post<DeployBatchRequest>(batchRequestUrl, deployBatchRequest);
                }
                catch (Exception err)
                {
                    _logger.WarnException(string.Format("Error posting DeployBatchRequest object to URL {0}: {1}, ERROR: {2}", url, deployBatchRequest.ToJson(), err.ToString()), err);
                    throw;
                }
                _logger.Debug("Posting DeployBuild object to URL {0}, returned {1}", url, response.ToJson());

                return response;
            }
        }


		public void PublishFilePattern(BuildPublishOptions options)
		{
			if(string.IsNullOrEmpty(options.FilePattern))
			{
				throw new ArgumentNullException("Missing FilePattern");
			}
			var parts = options.FilePattern.Split(new char[] {'|'},StringSplitOptions.RemoveEmptyEntries);
			string directory = parts[0];
			string searchPattern = null;
			int filePatternBatchSize = 4;
			SearchOption searchOption = SearchOption.AllDirectories;
			if(parts.Length > 1)
			{
				searchPattern = parts[1];
			}
			if(parts.Length > 2)
			{
				bool recurseDirectories;
				if(!bool.TryParse(parts[2], out recurseDirectories))
				{
					throw new ArgumentException("Invalid boolean value for 3rd FilePattern value (recurseDirectories): " + parts[2]);
				}
				if(recurseDirectories)
				{
					searchOption = SearchOption.AllDirectories;
				}
				else 
				{
					searchOption = SearchOption.TopDirectoryOnly;
				}
			}
			if(parts.Length > 3)
			{
				if(!int.TryParse(parts[3], out filePatternBatchSize))
				{
					throw new ArgumentException("Invalid int value for 4th FilePattern value (batchSize): " + parts[3]);
				}
			}
			var fileList = Directory.EnumerateFiles(directory, searchPattern, searchOption);
			var batchedFileListList = fileList.Select((x, i) => new { Index = i, Value = x })
										   .GroupBy(x => x.Index / filePatternBatchSize)
										   .Select(x => x.Select(v => v.Value).ToList())
										   .ToList();
			_logger.Info("Publishing file pattern {0}, the following {1} files were found:", options.FilePattern, fileList.Count());
			var taskList = new  List<System.Threading.Tasks.Task>();
			foreach(var filePath in fileList)
			{
				string x = filePath;
				_logger.Info("- {0}", x);
				var task = Task.Factory.StartNew(() =>
				{
					//var fileOptions = AutoMapper.Mapper.Map(options, new BuildPublishOptions());
					// Don't try to use automapper here.  Something about it evaluating it later during the task causes
					//	it to pollute values between threads.  Not sure why, but ouch ouch ouch
					var fileOptions = new BuildPublishOptions
					{
						File = x,
						ApiUrl = options.ApiUrl,
						BranchId = options.BranchId,
						ComponentId = options.ComponentId,
						Directory = options.Directory,
						FilePattern = null,
						NewFileName = options.NewFileName,
						ProjectId = options.ProjectId,
						Version = options.Version,
                        DeployEnvironmentName = options.DeployEnvironmentName
					};
					fileOptions.FilePattern = null;
					this.PublishFile(fileOptions);
				});
				taskList.Add(task);
			}
			//foreach(var fileBatch in batchedFileListList)
			//{
			//	_logger.Info("- {0}", string.Join(",", fileBatch));
			//	var task = Task.Factory.StartNew(()=>
			//	{
			//		foreach(var filePath in fileBatch)
			//		{
			//			//var fileOptions = AutoMapper.Mapper.Map(options, new BuildPublishOptions());
			//			// Don't try to use automapper here.  Something about it evaluating it later during the task causes
			//			//	it to pollute values between threads.  Not sure why, but ouch ouch ouch
			//			var fileOptions = new BuildPublishOptions
			//			{
			//				File = filePath,
			//				ApiUrl = options.ApiUrl,
			//				BranchId = options.BranchId,
			//				ComponentId = options.ComponentId,
			//				Directory = options.Directory,
			//				FilePattern = null,
			//				NewFileName = options.NewFileName,
			//				ProjectId = options.ProjectId,
			//				Version = options.Version
			//			};
			//			fileOptions.File = filePath;
			//			fileOptions.FilePattern = null;
			//			this.PublishFile(fileOptions);
			//		}
			//	});
			//	taskList.Add(task);
			//}
			Task.WaitAll(taskList.ToArray());
			var exceptionList = taskList.Where(i=>i.Exception != null).Select(i=>i.Exception);
			foreach(var exception in exceptionList)
			{
				_logger.ErrorException(exception.ToString(), exception);
			}
			if(exceptionList.Any())
			{
				throw new Exception("Publish exceptions occurred");
			}
		}

		public void PublishFile(BuildPublishOptions options)
		{
			if(string.IsNullOrEmpty(options.File))
			{
				throw new ArgumentNullException("Missing FilePath");
			}
			if(string.IsNullOrEmpty(options.ApiUrl))
			{
				throw new ArgumentNullException("Missing ApiUrl");
			}
			if(string.IsNullOrEmpty(options.ProjectId))
			{
				throw new ArgumentNullException("Missing ProjectId");
			}
			if(string.IsNullOrEmpty(options.ComponentId))
			{
				throw new ArgumentNullException("Missing ComponentId");
			}
			if(string.IsNullOrEmpty(options.BranchId))
			{
				throw new ArgumentNullException("Missing BranchId");
			}
			if(string.IsNullOrEmpty(options.Version))
			{
				throw new ArgumentNullException("Missing Version");
			}
			bool deleteFile = false;
			string directoryToDelete = null;
			_logger.Info("Start publishing file {0} to URL {1}", options.File, options.ApiUrl);
			_regexResolver.ResolveValues(options);
			string zipPath = Path.ChangeExtension(Path.GetTempFileName(), ".zip");
			if (!File.Exists(options.File))
			{
				throw new DirectoryNotFoundException(string.Format("File directory \"{0}\" not found", options.File));
			}
			if(!string.IsNullOrWhiteSpace(options.NewFileName))
			{
				_logger.Info("New file name {0} provided", options.NewFileName);
				string oldFilePath = options.File;
				directoryToDelete = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
				Directory.CreateDirectory(directoryToDelete);
				options.File = Path.Combine(directoryToDelete, options.NewFileName);
				_logger.Info("Copying {0} to {1}", oldFilePath, options.File);
				File.Copy(oldFilePath, options.File);
				deleteFile = true;
			}
			_zipper.ZipFile(options.File, zipPath);

			PublishZip(options.ApiUrl, options.ProjectId, options.ComponentId, options.BranchId, options.Version, zipPath, options.UserName, options.Password);

			try
			{
				File.Delete(zipPath);
			}
			catch (Exception err)
			{
				_logger.WarnException(string.Format("Failed to delete ZIP file {0}: {1}", zipPath, err.ToString()), err);
			}

			if(deleteFile)
			{
				try 
				{
					File.Delete(options.File);
				}
				catch(Exception err)
				{
					_logger.WarnException(string.Format("Failed to delete file {0}: {1}", options.File, err.ToString()), err);
				}
			}
			if(!string.IsNullOrEmpty(directoryToDelete))
			{
				try 
				{
					Directory.Delete(directoryToDelete);
				}
				catch(Exception err)
				{
					_logger.WarnException(string.Format("Failed to delete directory {0}: {1}", options.File, err.ToString()), err);
				}
			}

			_logger.Info("Done publishing file {0} to URL {1}", options.File, options.ApiUrl);
		}

		private DeployBuild PublishZip(string apiUrl, string projectId, string componentId, string branchId, string version, string zipPath, string userName, string password)
		{
			string url = apiUrl;
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
			branchId = FormatBranch(branchId, version);

            Cookie authCookie = null;
            if(!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
            {
                authCookie = GetAuthCookie(apiUrl, userName, password);
            }
			using (var client = new JsonServiceClient(url))
			{
				_logger.Debug("Posting file {0} to URL {1}", zipPath, url);
				//var x = client.Send<DeployFileDto>(deployFile);
				var fileToUpload = new FileInfo(zipPath);
				string fileUrl = url + "file";
				client.Credentials = System.Net.CredentialCache.DefaultCredentials;
				client.Timeout = TimeSpan.FromMinutes(5);
				client.ReadWriteTimeout = TimeSpan.FromMinutes(5);
                if(authCookie != null)
                {  
                    client.CookieContainer.Add(authCookie);
                }
				var fileResponse = client.PostFile<DeployFileDto>(fileUrl, fileToUpload, MimeTypes.GetMimeType(fileToUpload.Name));
				_logger.Debug("Done posting file {0} to URL {1}, returned fileId {2} and fileStorageId {3}", zipPath, url, fileResponse.Id, fileResponse.FileStorageId);

				var buildRequest = new DeployBuild
				{
					FileId = fileResponse.Id,
					ProjectId = projectId,
					ProjectBranchId = branchId,
					ProjectComponentId = componentId,
					Version = version
				};
				_logger.Debug("Posting DeployBuild object to URL {0}, sending{1}", url, buildRequest.ToJson());
				string buildUrl = url + "build";
				try 
				{
					var buildResponse = client.Post<DeployBuild>(buildUrl, buildRequest);
                    _logger.Debug("Posting DeployBuild object to URL {0}, returned {1}", url, buildRequest.ToJson());
                    return buildResponse;
                }
				catch(Exception err)
				{
					_logger.WarnException(string.Format("Error posting DeployBuild object to URL {0}: {1}, ERROR: {2}", url, buildRequest.ToJson(), err.ToString()), err);
					throw;
				}
			}

		}

        private Cookie GetAuthCookie(string apiUrl, string userName, string password)
        {
            string logonUrl = apiUrl;
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

		private string FormatBranch(string branchId, string version)
		{
			if (string.IsNullOrWhiteSpace(branchId) && !string.IsNullOrWhiteSpace(version))
			{
				branchId = version.Substring(0, version.LastIndexOf("."));
				_logger.Info("No branch provided, defaulting to " + branchId);
			}
			else if (branchId.Equals("[[DefaultVersionThreeDigits]]", StringComparison.CurrentCultureIgnoreCase))
			{
				int index = version.IndexOf('.');
				if(index >= 0)
				{
					index = version.IndexOf('.',index+1);
					if(index >= 0)
					{
						index = version.IndexOf('.',index+1);
					}
					branchId = version.Substring(0, index);
				}
				else
				{
					branchId = version;
				}

				_logger.Info("Branch == [[DefaultVersionThreeDigits]], defaulting to " + branchId);
			}
			else if (branchId.Equals("[[DefaultVersionTwoDigits]]", StringComparison.CurrentCultureIgnoreCase))
			{
				int index = version.IndexOf('.');
				if(index >= 0)
				{
					index = version.IndexOf('.',index+1);
					if(index >= 0)
					{
						branchId = version.Substring(0, index);
					}
				}
				else
				{
					branchId = version;
				}

				_logger.Info("Branch == [[DefaultVersionTwoDigits]], defaulting to " + branchId);
			}
			return branchId;
		}
	}
}
