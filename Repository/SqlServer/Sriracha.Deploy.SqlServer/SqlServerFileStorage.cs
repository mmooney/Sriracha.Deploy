using MMDB.Shared;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Build;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.SqlServer
{
    public class SqlServerFileStorage : IFileStorage
    {
        private readonly ISqlConnectionInfo _sqlConnectionInfo;
        private readonly IUserIdentity _userIdentity;

        public SqlServerFileStorage(ISqlConnectionInfo sqlConnectionInfo, IUserIdentity userIdentity)
        {
            _sqlConnectionInfo = DIHelper.VerifyParameter(sqlConnectionInfo);
            _userIdentity = DIHelper.VerifyParameter(userIdentity);
        }

        public string StoreFile(byte[] fileData)
        {
            if(fileData == null || fileData.Length == 0)
            {
                throw new ArgumentNullException("fileData");
            }
            var id = Guid.NewGuid().ToString();
            var sql = PetaPoco.Sql.Builder
                    .Append("INSERT INTO DeployFileStorage (ID, FileData)")
                    .Append("VALUES (@0, @1)", id, fileData);
            using(var db = _sqlConnectionInfo.GetDB())
            {
                db.Execute(sql);
            }
            return id;
        }

        private void VerifyExists(string fileStorageId)
        {
            if(string.IsNullOrEmpty(fileStorageId))
            {
                throw new ArgumentNullException("fileStorageID");
            }
            using(var db = _sqlConnectionInfo.GetDB())
            {
                var count = db.ExecuteScalar<int>("SELECT COUNT(*) FROM DeployFileStorage WHERE ID=@0", fileStorageId);
                if(count == 0)
                {
                    throw new RecordNotFoundException(typeof(byte[]), "FileStorageId", fileStorageId);
                }
            }
        }

        public void UpdateFile(string fileStorageId, byte[] fileData)
        {
            VerifyExists(fileStorageId);
            using(var db = _sqlConnectionInfo.GetDB())
            {
                db.Execute("UPDATE DeployFileStorage SET FileData=@0 WHERE ID=@1", fileData, fileStorageId);
            }
        }

        public byte[] GetFile(string fileStorageId)
        {
            if (string.IsNullOrEmpty(fileStorageId))
            {
                throw new ArgumentNullException("fileStorageID");
            }
            using (var db = _sqlConnectionInfo.GetDB())
            {
                var data = db.FirstOrDefault<byte[]>("SELECT FileData FROM DeployFileStorage WHERE ID=@0", fileStorageId);
                if (data == null)
                {
                    throw new RecordNotFoundException(typeof(byte[]), "FileStorageId", fileStorageId);
                }
                return data;
            }
        }

        public System.IO.Stream GetFileStream(string fileStorageId)
        {
            var data = this.GetFile(fileStorageId);
            return new MemoryStream(data);
        }

        public void DeleteFile(string fileStorageId)
        {
            VerifyExists(fileStorageId);
            using(var db = _sqlConnectionInfo.GetDB())
            {
                db.Execute("DELETE FROM DeployFileStorage WHERE ID=@0", fileStorageId);
            }
        }
    }
}
