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

		public void PublishDirectory(string directoryPath, string apiUrl)
		{
			_logger.Info("Start publishing directory {0} to URL {1}", directoryPath, apiUrl);
			string zipPath = Path.ChangeExtension(Path.GetTempFileName(), ".zip");
			if(!Directory.Exists(directoryPath))
			{
				throw new DirectoryNotFoundException(string.Format("Publish directory \"{0}\" not found", directoryPath));
			}
			_zipper.ZipDirectory(directoryPath, zipPath);

			string url = apiUrl;
			if(!url.EndsWith("/"))
			{
				url += "/";
			}
			if(!url.EndsWith("/api/", StringComparison.CurrentCultureIgnoreCase))
			{
				url += "api/";
			}
			url += "file";
			var deployFile = new DeployFileDto
			{
				FileData = File.ReadAllBytes(zipPath),
				FileName = Path.GetFileName(zipPath)
			};
			string fileId;
			using(var client = new JsonServiceClient(url))
			{
				_logger.Debug("Posting file {0} to URL {1}", zipPath, url);
				//var x = client.Send<DeployFileDto>(deployFile);
				var fileToUpload = new FileInfo(zipPath);
				var response = client.PostFile<DeployFileDto>(url,
					fileToUpload, MimeTypes.GetMimeType(fileToUpload.Name));
				_logger.Debug("Done posting file {0} to URL {1}, returned fileId {2} and fileStorageId {3}", zipPath, url, response.Id, response.FileStorageId);
			}

			throw NotImplementedException("Still need to create build object...");

			_logger.Info("Done publishing directory {0} to URL {1}", directoryPath, apiUrl);
		}
	}
}
