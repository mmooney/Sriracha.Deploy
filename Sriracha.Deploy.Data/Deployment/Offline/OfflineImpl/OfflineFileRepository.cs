﻿using MMDB.Shared;
using Sriracha.Deploy.Data.Dto.Build;
using Sriracha.Deploy.Data.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Deployment.Offline.OfflineImpl
{
    public class OfflineFileRepository : IFileRepository
    {
        private readonly IOfflineDataProvider _offlineDataProvider;

        public OfflineFileRepository(IOfflineDataProvider offlineDataProvider)
        {
            _offlineDataProvider = offlineDataProvider;
        }

        public IEnumerable<DeployFile> GetFileList()
        {
            throw new NotSupportedException();
        }

        public DeployFile CreateFile(string fileName, byte[] data)
        {
            throw new NotSupportedException();
        }

        public DeployFile GetFile(string fileId)
        {
            return _offlineDataProvider.GetFile(fileId);
        }

        public DeployFile UpdateFile(string fileId, string fileName, byte[] fileData)
        {
            throw new NotSupportedException();
        }

        public void DeleteFile(string fileId)
        {
            throw new NotSupportedException();
        }

        public byte[] GetFileData(string fileId)
        {
            return _offlineDataProvider.GetFileData(fileId);
        }


        public Stream GetFileDataStream(string fileId)
        {
            return new MemoryStream(_offlineDataProvider.GetFileData(fileId));
        }
    }
}