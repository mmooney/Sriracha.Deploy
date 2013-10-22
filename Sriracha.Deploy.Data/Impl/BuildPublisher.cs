using System;
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

namespace Sriracha.Deploy.Data.Impl
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

			PublishZip(options.ApiUrl, options.ProjectId, options.ComponentId, options.BranchId, options.Version, zipPath);

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
			foreach(var fileBatch in batchedFileListList)
			{
				_logger.Info("- {0}", string.Join(",", fileBatch));
				var task = Task.Factory.StartNew(()=>
				{
					foreach(var filePath in fileBatch)
					{
						//var fileOptions = AutoMapper.Mapper.Map(options, new BuildPublishOptions());
						// Don't try to use automapper here.  Something about it evaluating it later during the task causes
						//	it to pollute values between threads.  Not sure why, but ouch ouch ouch
						var fileOptions = new BuildPublishOptions
						{
							File = filePath,
							ApiUrl = options.ApiUrl,
							BranchId = options.BranchId,
							ComponentId = options.ComponentId,
							Directory = options.Directory,
							FilePattern = null,
							NewFileName = options.NewFileName,
							ProjectId = options.ProjectId,
							Version = options.Version
						};
						fileOptions.File = filePath;
						fileOptions.FilePattern = null;
						this.PublishFile(fileOptions);
					}
				});
				taskList.Add(task);
			}
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

			PublishZip(options.ApiUrl, options.ProjectId, options.ComponentId, options.BranchId, options.Version, zipPath);

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
			using (var client = new JsonServiceClient(url))
			{
				_logger.Debug("Posting file {0} to URL {1}", zipPath, url);
				//var x = client.Send<DeployFileDto>(deployFile);
				var fileToUpload = new FileInfo(zipPath);
				string fileUrl = url + "file";
				client.Credentials = System.Net.CredentialCache.DefaultCredentials;
				client.Timeout = TimeSpan.FromMinutes(2);
				client.ReadWriteTimeout = TimeSpan.FromMinutes(2);
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
				try 
				{
					var buildResponse = client.Post<DeployBuild>(buildUrl, build);
				}
				catch(Exception err)
				{
					throw;
				}
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
