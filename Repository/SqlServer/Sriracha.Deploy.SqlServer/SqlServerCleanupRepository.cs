using MMDB.Shared;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.SqlServer
{
    public class SqlServerCleanupRepository : ICleanupRepository
    {
        private readonly ISqlConnectionInfo _sqlConnectionInfo;
        private readonly IUserIdentity _userIdentity;

        public SqlServerCleanupRepository(ISqlConnectionInfo sqlConnectionInfo, IUserIdentity userIdentity)
        {
            _sqlConnectionInfo = DIHelper.VerifyParameter(sqlConnectionInfo);
            _userIdentity = DIHelper.VerifyParameter(userIdentity);
        }

        private PetaPoco.Sql GetBaseQuery()
        {
            return PetaPoco.Sql.Builder 
                .Append("SELECT ID, EnumCleanupTaskTypeID AS TaskType, MachineName, FolderPath, AgeMinutes, TargetCleanupDateTimeUtc, EnumQueueStatusID AS Status,")
                .Append("StartedDateTimeUtc, CompletedDateTimeUtc, ErrorDetails, CreatedDateTimeUtc, CreatedByUserName, UpdatedDateTimeUtc, UpdatedByUserName")
                .Append("FROM DeployCleanupTaskData");
        }

        private void VerifyExists(string taskId)
        {
            using(var db = _sqlConnectionInfo.GetDB())
            {
                int count = db.ExecuteScalar<int>("SELECT COUNT(*) FROM DeployCleanupTaskData WHERE ID=@0", taskId);
                if(count == 0)
                {
                    throw new RecordNotFoundException(typeof(CleanupTaskData), "ID", taskId);
                }
            }
        }

        public CleanupTaskData CreateCleanupTask(string machineName, EnumCleanupTaskType taskType, string folderPath, int ageMinutes)
        {
            if(string.IsNullOrEmpty(machineName))
            {
                throw new ArgumentNullException("Missing machine name");
            }
            if(string.IsNullOrEmpty(folderPath))
            {
                throw new ArgumentNullException("Missing folder path");
            }
            var dbItem = new CleanupTaskData
            {
                Id = Guid.NewGuid().ToString(),
                TaskType = taskType,
                MachineName = machineName,
                FolderPath = folderPath,
                AgeMinutes = ageMinutes,
                Status = EnumQueueStatus.New,
                TargetCleanupDateTimeUtc = DateTime.UtcNow.AddMinutes(ageMinutes),
                CreatedByUserName = _userIdentity.UserName,
                CreatedDateTimeUtc = DateTime.UtcNow,
                UpdatedByUserName = _userIdentity.UserName,
                UpdatedDateTimeUtc = DateTime.UtcNow
            };
            using(var db = _sqlConnectionInfo.GetDB())
            {
                var sql = PetaPoco.Sql.Builder
                                .Append("INSERT INTO DeployCleanupTaskData (ID, EnumCleanupTaskTypeID, MachineName, FolderPath, AgeMinutes, EnumQueueStatusID, TargetCleanupDateTimeUtc, CreatedByUserName, CreatedDateTimeUtc, UpdatedByUserName, UpdatedDateTimeUtc)")
                                .Append("VALUES (@Id, @TaskType, @MachineName, @FolderPath, @AgeMinutes, @Status, @TargetCleanupDateTimeUtc, @CreatedByUserName, @CreatedDateTimeUtc, @UpdatedByUserName, @UpdatedDateTimeUtc)", dbItem);
                db.Execute(sql);
            }
            return this.GetCleanupTask(dbItem.Id);
        }

        public CleanupTaskData GetCleanupTask(string taskId)
        {
            if(string.IsNullOrEmpty(taskId))
            {
                throw new ArgumentNullException("Missing task ID");
            }
            var sql = GetBaseQuery();
            sql = sql.Append("WHERE ID=@0", taskId);
            using(var db = _sqlConnectionInfo.GetDB())
            {
                var item = db.FirstOrDefault<CleanupTaskData>(sql);
                if(item == null)
                {
                    throw new RecordNotFoundException(typeof(CleanupTaskData), "ID", taskId);
                }
                return item;
            }
        }

        public CleanupTaskData PopNextFolderCleanupTask(string machineName)
        {
            if(string.IsNullOrEmpty(machineName))
            {
                throw new ArgumentNullException("Missing machine name");
            }
            var sql = PetaPoco.Sql.Builder 
                        .Append("DECLARE @@id AS NVARCHAR(50);")
                        .Append("SET @@id = (SELECT TOP 1 ID FROM DeployCleanupTaskData WHERE EnumQueueStatusID=0 AND MachineName=@0 AND TargetCleanupDateTimeUtc < GETUTCDATE());", machineName)
                        .Append("IF(@@id IS NOT NULL) UPDATE DeployCleanupTaskData SET EnumQueueStatusID=1, StartedDateTimeUtc=GETUTCDATE(), UpdatedDateTimeUtc=GETUTCDATE(), UpdatedByUserName=@0 WHERE ID=@@id;", _userIdentity.UserName)
                        .Append("SELECT @@id");
            using(var db = _sqlConnectionInfo.GetDB())
            {
                var id = db.ExecuteScalar<string>(sql);
                if(!string.IsNullOrEmpty(id))
                {
                    return GetCleanupTask(id);
                }
                else 
                {
                    return null;
                }
            }
        }

        public CleanupTaskData MarkItemSuccessful(string taskId)
        {
            if(string.IsNullOrEmpty(taskId))
            {
                throw new ArgumentNullException("Missing task ID");
            }
            VerifyExists(taskId);
            var sql = PetaPoco.Sql.Builder
                        .Append("UPDATE DeployCleanupTaskData")
                        .Append("SET EnumQueueStatusID=2, CompletedDateTimeUtc=GETUTCDATE(), UpdatedByUserName=@0, UpdatedDateTimeUtc=GETUTCDATE()", _userIdentity.UserName)
                        .Append("WHERE ID=@0", taskId);
            using(var db = _sqlConnectionInfo.GetDB())
            {
                db.Execute(sql);
            }
            return GetCleanupTask(taskId);
        }


        public CleanupTaskData MarkItemFailed(string taskId, Exception err)
        {
            if (string.IsNullOrEmpty(taskId))
            {
                throw new ArgumentNullException("Missing task ID");
            }
            VerifyExists(taskId);
            var sql = PetaPoco.Sql.Builder
                        .Append("UPDATE DeployCleanupTaskData")
                        .Append("SET EnumQueueStatusID=3, CompletedDateTimeUtc=GETUTCDATE(), UpdatedByUserName=@0, UpdatedDateTimeUtc=GETUTCDATE()", _userIdentity.UserName);
            if(err != null)
            {
                sql = sql.Append(", ErrorDetails=@0", err.ToString());
            }
            sql = sql.Append("WHERE ID=@0", taskId);
            using (var db = _sqlConnectionInfo.GetDB())
            {
                db.Execute(sql);
            }
            return GetCleanupTask(taskId);
        }
    }
}
