using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.Shared;
using NLog;
using Raven.Client;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Repository;
using Sriracha.Deploy.Data.Build;
using Sriracha.Deploy.Data.Dto.Build;
using System.IO;

namespace Sriracha.Deploy.RavenDB
{
	public class RavenFileRepository : IFileRepository
	{
		private readonly IDocumentSession _documentSession;
		private readonly IFileStorage _fileStorage;
		private readonly IUserIdentity _userIdentity;
		private readonly Logger _logger;

		public RavenFileRepository(IDocumentSession documentSession, IFileStorage fileStorage, IUserIdentity userIdentity, Logger logger)
		{
			_documentSession = DIHelper.VerifyParameter(documentSession);
			_fileStorage = DIHelper.VerifyParameter(fileStorage);
			_userIdentity = DIHelper.VerifyParameter(userIdentity);
			_logger = DIHelper.VerifyParameter(logger);
		}

		public List<DeployFile> GetFileList()
		{
			return _documentSession.QueryNoCache<DeployFile>().ToList();
		}

		public DeployFile CreateFile(string fileName, byte[] fileData, FileManifest fileManifest)
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
				FileStorageId = fileId,
                Manifest = fileManifest,
				CreatedDateTimeUtc = DateTime.UtcNow,
				CreatedByUserName = _userIdentity.UserName,
				UpdatedDateTimeUtc = DateTime.UtcNow,
				UpdatedByUserName = _userIdentity.UserName
			};
            return _documentSession.StoreSaveEvict(file);
		}

		public DeployFile GetFile(string fileId)
		{
            return _documentSession.LoadEnsureNoCache<DeployFile>(fileId);
		}

		public DeployFile UpdateFile(string fileId, string fileName, byte[] fileData, FileManifest fileManifest)
		{
            if(string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException("fileName");
            }
            if(fileData == null || fileData.Length == 0)
            {
                throw new ArgumentNullException("fileData");
            }
            var file = _documentSession.LoadEnsure<DeployFile>(fileId);
			file.FileName = fileName;
			file.UpdatedDateTimeUtc = DateTime.UtcNow;
			file.UpdatedByUserName = _userIdentity.UserName;
            file.Manifest = fileManifest;
            _fileStorage.UpdateFile(file.FileStorageId, fileData);
            return this._documentSession.SaveEvict(file);
		}

		public DeployFile DeleteFile(string fileId)
		{
			_logger.Info("User {0} deleting file {1}", _userIdentity.UserName, fileId);
			var file = _documentSession.LoadEnsure<DeployFile>(fileId);
            this._fileStorage.DeleteFile(file.FileStorageId);
            return _documentSession.DeleteSaveEvict(file);
		}


        public byte[] GetFileData(string fileId)
        {
            var file = _documentSession.LoadEnsureNoCache<DeployFile>(fileId);
            return _fileStorage.GetFile(file.FileStorageId);
        }

        public Stream GetFileDataStream(string fileId)
        {
            var file = _documentSession.LoadEnsureNoCache<DeployFile>(fileId);
            return _fileStorage.GetFileStream(file.FileStorageId);
        }
    }
}
