using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Repository;

namespace Sriracha.Deploy.RavenDB
{
	public class RavenFileRepository : IFileRepository
	{
		private readonly IDocumentSession _documentSession;

		public RavenFileRepository(IDocumentSession documentSession)
		{
			this._documentSession = DIHelper.VerifyParameter(documentSession);
		}

		public IEnumerable<DeployFile> GetFileList()
		{
			return _documentSession.Query<DeployFile>();
		}

		public DeployFile CreateFile(string fileName, byte[] fileData)
		{
			if(string.IsNullOrEmpty(fileName))
			{
				throw new ArgumentNullException("Missing file name");
			}
			if(fileData == null || fileData.Length == 0)
			{
				throw new ArgumentNullException("Missing file data");
			}
			var file = new DeployFile
			{
				Id = Guid.NewGuid().ToString(),
				FileName = fileName,
				FileData = fileData
			};
			this._documentSession.Store(file);
			this._documentSession.SaveChanges();
			return file;
		}

		public DeployFile GetFile(string fileId)
		{
			if(string.IsNullOrEmpty(fileId))
			{
				throw new ArgumentNullException("Missing file ID");
			}
			return this._documentSession.Load<DeployFile>(fileId);
		}

		public DeployFile UpdateFile(string fileId, string fileName, byte[] fileData)
		{
			if (string.IsNullOrEmpty(fileName))
			{
				throw new ArgumentNullException("Missing file name");
			}
			if (fileData == null || fileData.Length == 0)
			{
				throw new ArgumentNullException("Missing file data");
			}
			var file = this.GetFile(fileId);
			file.FileName = fileName;
			file.FileData = fileData;
			this._documentSession.SaveChanges();
			return file;
		}

		public void DeleteFile(string fileId)
		{
			var file = this.GetFile(fileId);
			this._documentSession.Delete(file);
			this._documentSession.SaveChanges();
		}
	}
}
