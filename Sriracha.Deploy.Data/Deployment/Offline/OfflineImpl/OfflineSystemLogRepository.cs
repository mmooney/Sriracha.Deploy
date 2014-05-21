using PagedList;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Deployment.Offline.OfflineImpl
{
    public class OfflineSystemLogRepository : ISystemLogRepository
    {
        private readonly IOfflineDataProvider _offlineDataProvider;

        public OfflineSystemLogRepository(IOfflineDataProvider offlineDataProvider)
        {
            _offlineDataProvider = DIHelper.VerifyParameter(offlineDataProvider);
        }

        public SystemLog GetMessage(string id)
        {
            throw new NotImplementedException();
        }

        public SystemLog LogMessage(EnumSystemLogType logType, string userName, DateTime messageDateTime, string message, string loggerName)
        {
            string formattedMessage = String.Format("{0} {1} {2}: {3}", logType, messageDateTime, userName, message);
            _offlineDataProvider.WriteLog(formattedMessage);
            return null;
        }

        public void PurgeLogMessages(DateTime utcNow, EnumSystemLogType systemLogType, int? ageMinutes)
        {
            throw new NotSupportedException();
        }

        public PagedSortedList<SystemLog> GetList(ListOptions listOptions, List<EnumSystemLogType> systemLogTypeList = null)
        {
            throw new NotSupportedException();
        }


    }
}
