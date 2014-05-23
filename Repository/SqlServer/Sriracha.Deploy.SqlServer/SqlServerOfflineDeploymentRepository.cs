using MMDB.Shared;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Dto.Deployment.Offline;
using Sriracha.Deploy.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.SqlServer
{
    public class SqlServerOfflineDeploymentRepository : IOfflineDeploymentRepository
    {
        private readonly ISqlConnectionInfo _sqlConnectionInfo;
        private readonly IUserIdentity _userIdentity;

        [PetaPoco.TableName("OfflineDeployment")]
        private class SqlOfflineDeployment : BaseDto
        {
            public string DeployBatchRequestId { get; set; }
            [PetaPoco.Column("EnumOfflineDeploymentStatusID")]
            public EnumOfflineDeploymentStatus Status { get; set; }
            public string FileId { get; set; }
            public string CreateErrorDetails { get; set; }
            public string ResultFileId { get; set; }

            public static SqlOfflineDeployment FromDto(OfflineDeployment item)
            {
                return AutoMapper.Mapper.Map(item, new SqlOfflineDeployment());
            }

            public OfflineDeployment ToDto()
            {
                return AutoMapper.Mapper.Map(this, new OfflineDeployment());
            }
        }

        static SqlServerOfflineDeploymentRepository()
        {
            AutoMapper.Mapper.CreateMap<SqlOfflineDeployment, OfflineDeployment>();
            AutoMapper.Mapper.CreateMap<OfflineDeployment, SqlOfflineDeployment>();
        }

        public SqlServerOfflineDeploymentRepository(ISqlConnectionInfo sqlConnectionInfo, IUserIdentity userIdentity)
        {
            _sqlConnectionInfo = DIHelper.VerifyParameter(sqlConnectionInfo);
            _userIdentity = DIHelper.VerifyParameter(userIdentity);
        }

        private SqlOfflineDeployment GetSqlItem(string offlineDeploymentId)
        {
            if (string.IsNullOrEmpty(offlineDeploymentId))
            {
                throw new ArgumentNullException("offlineDeploymentId");
            }
            using (var db = _sqlConnectionInfo.GetDB())
            {
                var item = db.FirstOrDefault<SqlOfflineDeployment>("FROM OfflineDeployment WHERE ID=@0", offlineDeploymentId);
                if (item == null)
                {
                    throw new RecordNotFoundException(typeof(OfflineDeployment), "Id", offlineDeploymentId);
                }
                return item;
            }
        }

        public OfflineDeployment CreateOfflineDeployment(string deployBatchRequestId, EnumOfflineDeploymentStatus initialStatus)
        {
            if(string.IsNullOrEmpty(deployBatchRequestId))
            {
                throw new ArgumentNullException("deployBatchRequestId");
            }
            var dbItem = new SqlOfflineDeployment
            {
                Id = Guid.NewGuid().ToString(),
                DeployBatchRequestId = deployBatchRequestId,
                Status = initialStatus
            };
            dbItem.SetCreatedFields(_userIdentity.UserName);
            using(var db = _sqlConnectionInfo.GetDB())
            {
                db.Insert("OfflineDeployment", "ID", false, dbItem);
            }
            return this.GetOfflineDeployment(dbItem.Id);
        }

        public OfflineDeployment GetOfflineDeployment(string offlineDeploymentId)
        {
            return GetSqlItem(offlineDeploymentId).ToDto();
        }


        public OfflineDeployment GetOfflineDeploymentForDeploymentBatchRequestId(string deployBatchRequestId)
        {
            if (string.IsNullOrEmpty(deployBatchRequestId))
            {
                throw new ArgumentNullException("deployBatchRequestId");
            }
            using (var db = _sqlConnectionInfo.GetDB())
            {
                var item = db.FirstOrDefault<SqlOfflineDeployment>("FROM OfflineDeployment WHERE DeployBatchRequestID=@0", deployBatchRequestId);
                if (item == null)
                {
                    throw new RecordNotFoundException(typeof(OfflineDeployment), "DeployBatchRequestId", deployBatchRequestId);
                }
                return item.ToDto();
            }
        }

        public OfflineDeployment SetReadyForDownload(string offlineDeploymentId, string fileId)
        {
            if(string.IsNullOrEmpty(fileId))
            {
                throw new ArgumentNullException("fileId");
            }
            var item = this.GetSqlItem(offlineDeploymentId);
            item.Status = EnumOfflineDeploymentStatus.ReadyForDownload;
            item.FileId = fileId;
            item.SetUpdatedFields(_userIdentity.UserName);
            using(var db = _sqlConnectionInfo.GetDB())
            {
                db.Update("OfflineDeployment", "ID", item, item.Id);
            }
            return this.GetOfflineDeployment(offlineDeploymentId);
        }

        public OfflineDeployment UpdateStatus(string offlineDeploymentId, EnumOfflineDeploymentStatus status, Exception err = null)
        {
            var item = this.GetSqlItem(offlineDeploymentId);
            item.Status = status;
            if(err != null)
            {
                item.CreateErrorDetails = err.ToString();
            }
            item.SetUpdatedFields(_userIdentity.UserName);
            using (var db = _sqlConnectionInfo.GetDB())
            {
                db.Update("OfflineDeployment", "ID", item, item.Id);
            }
            return this.GetOfflineDeployment(offlineDeploymentId);
        }

        public OfflineDeployment PopNextOfflineDeploymentToCreate()
        {
            var sql = PetaPoco.Sql.Builder
                        .Append("DECLARE @@id AS NVARCHAR(50);")
                        .Append("SET @@id = (SELECT TOP 1 ID FROM OfflineDeployment WHERE EnumOfflineDeploymentStatusID=@0 ORDER BY CreatedDateTimeUtc ASC);", EnumOfflineDeploymentStatus.CreateRequested)
                        .Append("IF(@@id IS NOT NULL) UPDATE OfflineDeployment SET EnumOfflineDeploymentStatusID=@0, UpdatedDateTimeUtc=GETUTCDATE(), UpdatedByUserName=@1 WHERE ID=@@id;", EnumOfflineDeploymentStatus.CreateInProcess, _userIdentity.UserName)
                        .Append("SELECT @@id");
            using (var db = _sqlConnectionInfo.GetDB())
            {
                var id = db.ExecuteScalar<string>(sql);
                if (!string.IsNullOrEmpty(id))
                {
                    return this.GetOfflineDeployment(id);
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
