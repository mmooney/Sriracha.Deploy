using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PagedList;
using Sriracha.Deploy.Data.Dto;

namespace Sriracha.Deploy.Data.Repository
{
	public interface ISystemLogRepository
	{
		SystemLog LogMessage(EnumSystemLogType logType, string userName, DateTime messageDateTime, string message, string loggerName);

        SystemLog GetMessage(string id);

        PagedSortedList<SystemLog> GetList(ListOptions listOptions = null, List<EnumSystemLogType> systemLogTypeList=null);

        void PurgeLogMessages(DateTime utcNow, EnumSystemLogType systemLogType, int? ageMinutes);


    }
}
