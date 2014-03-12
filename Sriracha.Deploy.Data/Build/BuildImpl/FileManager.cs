using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Repository;
using Sriracha.Deploy.Data.Utility;
using Sriracha.Deploy.Data.Dto.Build;
using System.IO;

namespace Sriracha.Deploy.Data.Build.BuildImpl
{
	public class FileManager : IFileManager
	{
		private readonly IFileRepository _fileRepository;
		private readonly IFileWriter _fileWriter;

		public FileManager(IFileRepository fileRepository, IFileWriter fileWriter)
		{
			this._fileRepository = DIHelper.VerifyParameter(fileRepository);
			this._fileWriter = DIHelper.VerifyParameter(fileWriter);
		}

		public IEnumerable<DeployFile> GetFileList()
		{
			return _fileRepository.GetFileList();
		}

		public DeployFile CreateFile(string fileName, byte[] fileData)
		{
            if(string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException("Missing fileName");
            }
            if(fileData == null)
            {
                throw new ArgumentNullException("Missing fileData");
            }
            if(fileData.Length == 0)
            {
                throw new ArgumentException("fileData is empty");
            }
			return _fileRepository.CreateFile(fileName, fileData);
		}

		public DeployFile UpdateFile(string fileId, string fileName, byte[] fileData)
		{
            if(string.IsNullOrEmpty(fileId))
            {
                throw new ArgumentNullException("Missing fileId");
            }
            if(string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException("Missing fileName");
            }
            if(fileData == null)
            {
                throw new ArgumentNullException("Missing fileData");
            }
            if(fileData.Length == 0)
            {
                throw new ArgumentException("fileData is empty");
            }
			return _fileRepository.UpdateFile(fileId, fileName, fileData);
		}

		public DeployFile GetFile(string fileId)
		{
            if(string.IsNullOrEmpty(fileId))
            {
                throw new ArgumentNullException("Missing fileId");
            }
			return _fileRepository.GetFile(fileId);
		}

        public byte[] GetFileData(string fileId)
        {
            if (string.IsNullOrEmpty(fileId))
            {
                throw new ArgumentNullException("Missing fileId");
            }
            return _fileRepository.GetFileData(fileId);
        }

        public Stream GetFileDataStream(string fileId)
        {
            if (string.IsNullOrEmpty(fileId))
            {
                throw new ArgumentNullException("Missing fileId");
            }
            return _fileRepository.GetFileDataStream(fileId);
        }

		public void DeleteFile(string fileId)
		{
            if (string.IsNullOrEmpty(fileId))
            {
                throw new ArgumentNullException("Missing fileId");
            }
            _fileRepository.DeleteFile(fileId);
		}

		public void ExportFile(string fileId, string targetFilePath)
		{
            if (string.IsNullOrEmpty(fileId))
            {
                throw new ArgumentNullException("Missing fileId");
            }
            var data = _fileRepository.GetFileData(fileId);
            if (data == null)
            {
                throw new ArgumentException("File not found for ID " + fileId);
            }
            _fileWriter.WriteBytes(targetFilePath, data);
		}
    }
}
