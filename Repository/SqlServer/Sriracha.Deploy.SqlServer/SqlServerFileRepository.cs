using MMDB.Shared;
using Newtonsoft.Json;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Build;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Dto.Build;
using Sriracha.Deploy.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.SqlServer
{
    public class SqlServerFileRepository : IFileRepository
    {
        private readonly ISqlConnectionInfo _sqlConnectionInfo;
        private readonly IUserIdentity _userIdentity;
        private readonly IFileStorage _fileStorage;

        public SqlServerFileRepository(ISqlConnectionInfo sqlConnectionInfo, IUserIdentity userIdentity, IFileStorage fileStorage)
        {
            _sqlConnectionInfo = DIHelper.VerifyParameter(sqlConnectionInfo);
            _userIdentity = DIHelper.VerifyParameter(userIdentity);
            _fileStorage = DIHelper.VerifyParameter(fileStorage);
        }


        [PetaPoco.TableName("DeployFile")]
        public class SqlDeployFile : BaseDto
        {
            static SqlDeployFile()
            {
                AutoMapper.Mapper.CreateMap<SqlDeployFile, DeployFile>();
                AutoMapper.Mapper.CreateMap<DeployFile, SqlDeployFile>();
            }

            public string FileName { get; set; }
            public string FileStorageID { get; set; }
            public string FileManifestJson { get; set; }

            public DeployFile ToDto()
            {
                var returnValue = AutoMapper.Mapper.Map(this, new DeployFile());
                if(!string.IsNullOrEmpty(this.FileManifestJson))
                {
                    returnValue.Manifest = JsonConvert.DeserializeObject<FileManifest>(this.FileManifestJson);
                }
                return returnValue;
            }
        }

        private void VerifyExists(string fileId)
        {
            if (string.IsNullOrEmpty(fileId))
            {
                throw new ArgumentNullException("fileId");
            }
            using (var db = _sqlConnectionInfo.GetDB())
            {
                var count = db.ExecuteScalar<int>("SELECT COUNT(*) FROM DeployFile WHERE ID=@0", fileId);
                if (count == 0)
                {
                    throw new RecordNotFoundException(typeof(DeployFile), "Id", fileId);
                }
            }
        }

        public List<DeployFile> GetFileList()
        {
            using(var db = _sqlConnectionInfo.GetDB())
            {
                return db.Query<SqlDeployFile>("").Select(i=>i.ToDto()).ToList();
            }
        }

        public DeployFile CreateFile(string fileName, byte[] data, FileManifest fileManifest)
        {
            if(string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException("fileName");
            }
            if(data == null || data.Length == 0)
            {
                throw new ArgumentNullException("data");
            }
            string fileStorageId = _fileStorage.StoreFile(data);
            var dbItem = new SqlDeployFile
            {
                Id = Guid.NewGuid().ToString(),
                FileName = fileName,
                FileStorageID = fileStorageId
            };

            dbItem.SetCreatedFields(_userIdentity.UserName);
            if(fileManifest != null)
            {
                dbItem.FileManifestJson = fileManifest.ToJson();
            }
            using(var db = _sqlConnectionInfo.GetDB())
            {
                db.Insert("DeployFile", "ID", false, dbItem);
            }
            return this.GetFile(dbItem.Id);
        }

        public DeployFile GetFile(string fileId)
        {
            if(string.IsNullOrEmpty(fileId))
            {
                throw new ArgumentNullException("fileId");
            }
            using(var db = _sqlConnectionInfo.GetDB())
            {
                var item = db.FirstOrDefault<SqlDeployFile>("WHERE ID=@0", fileId);
                if(item == null)
                {
                    throw new RecordNotFoundException(typeof(DeployFile), "ID", fileId);
                }
                return item.ToDto();
            }
        }

        public DeployFile UpdateFile(string fileId, string fileName, byte[] fileData, FileManifest fileManifest)
        {
            if(string.IsNullOrEmpty(fileId))
            {
                throw new ArgumentNullException("fileId");
            }
            if(string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException("fileName");
            }
            if(fileData == null || fileData.Length == 0)
            {
                throw new ArgumentNullException("fileData");
            }
            string fileStorageId;
            using(var db = _sqlConnectionInfo.GetDB())
            {
                var dbItem = db.FirstOrDefault<SqlDeployFile>("WHERE ID=@0", fileId);
                if(dbItem == null)
                {
                    throw new RecordNotFoundException(typeof(DeployFile),"Id", fileId);
                }
                dbItem.FileName = fileName;
                if(fileManifest != null)
                {
                    dbItem.FileManifestJson = fileManifest.ToJson();
                }
                else 
                {
                    dbItem.FileManifestJson = null;
                }
                dbItem.SetUpdatedFields(this._userIdentity.UserName);
                var sql = PetaPoco.Sql.Builder
                            .Append("UPDATE DeployFile")
                            .Append("SET FileName=@FileName, FileManifestJson=@FileManifestJson, UpdatedDateTimeUtc=@UpdatedDateTimeUtc, UpdatedByUserName=@UpdatedByUserName", dbItem)
                            .Append("WHERE ID=@Id", dbItem);
                db.Execute(sql);
                fileStorageId = dbItem.FileStorageID;
            }
            _fileStorage.UpdateFile(fileStorageId, fileData);
            return this.GetFile(fileId);
        }


        public DeployFile DeleteFile(string fileId)
        {
            var item = this.GetFile(fileId);
            _fileStorage.DeleteFile(item.FileStorageId);
            using(var db = _sqlConnectionInfo.GetDB())
            {
                db.Execute("DELETE FROM DeployFile WHERE ID=@0", fileId);
            }
            return item;
        }

        public byte[] GetFileData(string fileId)
        {
            var item = this.GetFile(fileId);
            return _fileStorage.GetFile(item.FileStorageId);
        }

        public System.IO.Stream GetFileDataStream(string fileId)
        {
            var item = this.GetFile(fileId);
            return _fileStorage.GetFileStream(item.FileStorageId);
        }
    }
}
