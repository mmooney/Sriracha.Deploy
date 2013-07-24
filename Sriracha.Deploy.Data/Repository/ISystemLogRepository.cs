using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Dto;

namespace Sriracha.Deploy.Data.Repository
{
	public interface ISystemLogRepository
	{
		SystemLog LogMessage(EnumSystemLogType logType, string userName, DateTime messageDateTime, string message);
	}
}
