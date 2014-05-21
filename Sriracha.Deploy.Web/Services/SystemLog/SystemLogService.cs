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
			return _systemLogRepository.GetList(request.BuildListOptions());
		}

	}
}