using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.Shared;
using Raven.Client;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Repository;

namespace Sriracha.Deploy.RavenDB
{
	public class RavenFileRepository : IFileRepository
	{
		private readonly IDocumentSession _documentSession;
		private readonly IFileStorage _fileStorage;

		public RavenFileRepository(IDocumentSession documentSession, IFileStorage fileStorage)
		{
			this._documentSession = DIHelper.VerifyParameter(documentSession);
			this._fileStorage = DIHelper.VerifyParameter(fileStorage);
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
			string fileId = _fileStorage.StoreFile(fileData);
			var file = new DeployFile
			{
				Id = Guid.NewGuid().ToString(),
				FileName = fileName,
				FileStorageId = fileId
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
			var file = this._documentSession.Load<DeployFile>(fileId);
			if(file == null)
			{
				throw new RecordNotFoundException(typeof(DeployFile), "Id", fileId);
			}
			return file;
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
			//file.FileData = fileData;
			this._documentSession.SaveChanges();

			_fileStorage.UpdateFile(file.FileStorageId, fileData);
			return file;
		}

		public void DeleteFile(string fileId)
		{
			var file = this.GetFile(fileId);
			this._documentSession.Delete(file);
			this._documentSession.SaveChanges();
		}


		public byte[] GetFileData(string fileId)
		{
			var file = this.GetFile(fileId);
			return _fileStorage.GetFile(file.FileStorageId);
		}
	}
}
