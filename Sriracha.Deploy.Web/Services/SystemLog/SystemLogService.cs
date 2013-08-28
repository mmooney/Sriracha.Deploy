using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack.ServiceInterface;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Repository;

namespace Sriracha.Deploy.Web.Services.SystemLog
{
	public class SystemLogService : Service
	{
		private readonly ISystemLogRepository _systemLogRepository;

		public SystemLogService(ISystemLogRepository systemLogRepository)
		{
			_systemLogRepository = DIHelper.VerifyParameter(systemLogRepository);
		}

		public object Get(SystemLogRequest request)
		{
			int pageSize = request.PageSize.GetValueOrDefault(50);
			int pageNumber = request.PageNumber.GetValueOrDefault(1);
			var sortField = request.SortField.GetValueOrDefault(EnumSystemLogSortField.MessageDate);
			bool sortAscending;
			if(!request.SortAscending.HasValue)
			{
				if(sortField == EnumSystemLogSortField.MessageDate)
				{
					sortAscending = false;
				}
				else 
				{
					sortAscending = true;
				}
			}
			else 
			{
				sortAscending = request.SortAscending.Value;
			}
			var pagedList = _systemLogRepository.GetList(pageSize, pageNumber, sortField, sortAscending);
			return new PagedSortedList<Sriracha.Deploy.Data.Dto.SystemLog>(pagedList, sortField.ToString(), sortAscending);
		}

	}
}