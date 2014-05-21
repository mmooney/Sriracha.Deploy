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
            throw new NotImplementedException();
        }

        public SystemLog GetMessage(string id)
        {
            throw new NotImplementedException();
        }

        public void PurgeLogMessages(DateTime utcNow, EnumSystemLogType systemLogType, int? ageMinutes)
        {
            throw new NotImplementedException();
        }

        public PagedSortedList<SystemLog> GetList(ListOptions listOptions, List<EnumSystemLogType> systemLogTypeList = null)
        {
            throw new NotImplementedException();
        }


    }
}
