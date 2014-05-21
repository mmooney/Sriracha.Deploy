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
    public class SqlServerSystemLogRepository : ISystemLogRepository
    {
        private readonly ISqlConnectionInfo _sqlConnectionInfo;
        private readonly IUserIdentity _userIdentity;

        public SqlServerSystemLogRepository(ISqlConnectionInfo sqlConnectionInfo, IUserIdentity userIdentity)
        {
            _sqlConnectionInfo = DIHelper.VerifyParameter(sqlConnectionInfo);
            _userIdentity = DIHelper.VerifyParameter(userIdentity);
        }


        public SystemLog LogMessage(EnumSystemLogType logType, string userName, DateTime messageDateTime, string message, string loggerName)
        {
            var item = new SystemLog
            {
                Id = Guid.NewGuid().ToString(),
                EnumSystemLogTypeID = logType,
                UserName = userName,
                MessageDateTimeUtc = messageDateTime,
                MessageText = message,
                LoggerName = loggerName 
            };

            using(var db = _sqlConnectionInfo.GetDB())
            {
                db.Insert("SystemLog", "ID", false, item);
            }
            return this.GetMessage(item.Id);
        }

        public SystemLog GetMessage(string id)
        {
            if(string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("id");
            }
            using(var db = _sqlConnectionInfo.GetDB())
            {
                var item = db.FirstOrDefault<SystemLog>("FROM SystemLog WHERE ID=@0", id);
                if(item == null)
                {
                    throw new RecordNotFoundException(typeof(SystemLog), "Id", id);
                }
                return item;
            }
        }

        public void PurgeLogMessages(DateTime utcNow, EnumSystemLogType systemLogType, int? ageMinutes)
        {
            if (ageMinutes.HasValue)
            {
                DateTime maxDate = utcNow.AddMinutes(0 - ageMinutes.Value);
                var sql = PetaPoco.Sql.Builder
                            .Append("DELETE FROM SystemLog")
                            .Append("WHERE EnumSystemLogTypeID=@0", systemLogType)
                            .Append("AND MessageDateTimeUtc < @0", maxDate);
                using(var db = _sqlConnectionInfo.GetDB())
                {
                    db.Execute(sql);
                }
            }
        }

        public PagedSortedList<SystemLog> GetList(ListOptions listOptions, List<EnumSystemLogType> systemLogTypeList = null)
        {
            listOptions = ListOptions.SetDefaults(listOptions, 20, 1, "MessageDateTimeUtc", false);
            var sql = PetaPoco.Sql.Builder
                        .Append("FROM SystemLog WHERE 0=0");
            if(systemLogTypeList != null && systemLogTypeList.Any())
            {
                sql = sql.Append("AND EnumSystemLogTypeID IN (@systemLogTypeList)", new { systemLogTypeList });
            }
            using(var db = _sqlConnectionInfo.GetDB())
            {
                return db.PageAndSort<SystemLog>(listOptions, sql);
            }
        }


    }
}
