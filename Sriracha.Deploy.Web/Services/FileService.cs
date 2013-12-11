using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using MMDB.Shared;
using ServiceStack.ServiceInterface;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Build;
using Sriracha.Deploy.Data.Dto.Build;
using ServiceStack.Common.Web;

namespace Sriracha.Deploy.Web.Services
{
	public class FileService : Service
	{
		private readonly IFileManager _fileManager;

		public FileService(IFileManager fileManager)
		{
			this._fileManager = DIHelper.VerifyParameter(fileManager);
		}

		public object Get(DeployFileDto request)
		{
			if (!string.IsNullOrEmpty(request.Id))
			{
				var file = this._fileManager.GetFile(request.Id);
                if(request.RawData)
                {
                    var result = new HttpResult(_fileManager.GetFileDataStream(request.Id), "application/octet-stream");
                    result.Headers.Add("Content-Disposition", "attachment; filename=" + file.FileName);
                    return result;
                }
                else 
                {
				    return file;
                }
			}
			else
			{
				return this._fileManager.GetFileList();
			}
		}

		public object Post(DeployFileDto file)
		{
			return this.PutPost(file);
		}

		public object Put(DeployFileDto file)
		{
			return this.PutPost(file);
		}

		private object PutPost(DeployFileDto file)
		{
			if (this.RequestContext.Files.Length == 0)
			{
				throw new Exception("Missing file data");
			}
			if (this.RequestContext.Files.Length > 1)
			{
				throw new Exception("Multiple file upload not supported");
			}
			var fileData = TempStreamHelper.ReadAllBytes(this.RequestContext.Files[0].InputStream);
			string fileName = this.RequestContext.Files[0].FileName;
			if (string.IsNullOrEmpty(file.Id) || file.Id.Equals("upload", StringComparison.CurrentCultureIgnoreCase))
			{
				return this._fileManager.CreateFile(fileName, fileData);
			}
			else
			{
				return this._fileManager.UpdateFile(file.Id, fileName, fileData);
			}
		}

	}
}