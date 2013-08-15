using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NLog;
using ServiceStack.Common.Web;
using ServiceStack.ServiceClient.Web;
using Sriracha.Deploy.Data.Dto;

namespace Sriracha.Deploy.Data.Impl
{
	public class BuildPublisher : IBuildPublisher
	{
		private readonly IZipper _zipper;
		private readonly Logger _logger;

		public BuildPublisher(IZipper zipper, Logger logger)
		{
			_zipper = DIHelper.VerifyParameter(zipper);
			_logger = DIHelper.VerifyParameter(logger);
		}

		public void PublishDirectory(string directoryPath, string apiUrl, string projectId, string componentId, string branchId, string version)
		{
			_logger.Info("Start publishing directory {0} to URL {1}", directoryPath, apiUrl);
			string zipPath = Path.ChangeExtension(Path.GetTempFileName(), ".zip");
			if(!Directory.Exists(directoryPath))
			{
				throw new DirectoryNotFoundException(string.Format("Publish directory \"{0}\" not found", directoryPath));
			}
			_zipper.ZipDirectory(directoryPath, zipPath);

			PublishZip(apiUrl, projectId, componentId, branchId, version, zipPath);

			try 
			{
				File.Delete(zipPath);
			}
			catch(Exception err)
			{
				_logger.ErrorException(string.Format("Failed to delete ZIP file {0}: {1}",  zipPath, err.ToString()), err);
			}
			_logger.Info("Done publishing directory {0} to URL {1}", directoryPath, apiUrl);
		}

		public void PublishFile(string filePath, string apiUrl, string projectId, string componentId, string branchId, string version)
		{
			_logger.Info("Start publishing file {0} to URL {1}", filePath, apiUrl);
			string zipPath = Path.ChangeExtension(Path.GetTempFileName(), ".zip");
			if (!File.Exists(filePath))
			{
				throw new DirectoryNotFoundException(string.Format("File directory \"{0}\" not found", filePath));
			}
			_zipper.ZipFile(filePath, zipPath);

			PublishZip(apiUrl, projectId, componentId, branchId, version, zipPath);

			try
			{
				File.Delete(zipPath);
			}
			catch (Exception err)
			{
				_logger.ErrorException(string.Format("Failed to delete ZIP file {0}: {1}", zipPath, err.ToString()), err);
			}

			_logger.Info("Done publishing file {0} to URL {1}", filePath, apiUrl);
		}

		private void PublishZip(string apiUrl, string projectId, string componentId, string branchId, string version, string zipPath)
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
			string fileId;
			using (var client = new JsonServiceClient(url))
			{
				_logger.Debug("Posting file {0} to URL {1}", zipPath, url);
				//var x = client.Send<DeployFileDto>(deployFile);
				var fileToUpload = new FileInfo(zipPath);
				string fileUrl = url + "file";
				var fileResponse = client.PostFile<DeployFileDto>(fileUrl, fileToUpload, MimeTypes.GetMimeType(fileToUpload.Name));
				_logger.Debug("Done posting file {0} to URL {1}, returned fileId {2} and fileStorageId {3}", zipPath, url, fileResponse.Id, fileResponse.FileStorageId);

				DeployBuild build = new DeployBuild
				{
					FileId = fileResponse.Id,
					ProjectId = projectId,
					ProjectBranchId = branchId,
					ProjectComponentId = componentId,
					Version = version
				};
				_logger.Debug("Posting DeployBuild object to URL {0}, sending{1}", url, build.ToJson());
				string buildUrl = url + "build";
				var buildResponse = client.Post<DeployBuild>(buildUrl, build);
				_logger.Debug("Posting DeployBuild object to URL {0}, returned {1}", url, build.ToJson());
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
