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

		void PurgeLogMessages(DateTime utcNow, EnumSystemLogType systemLogType, int? ageMinutes);

		IPagedList<SystemLog> GetList(int pageSize, int pageNumber, EnumSystemLogSortField sortField, bool sortAscending);
	}
}
