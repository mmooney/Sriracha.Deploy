using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sriracha.Deploy.Data.Dto;

namespace Sriracha.Deploy.Web.Services.SystemLog
{
	public class SystemLogRequest
	{
		public int? PageSize { get; set; }
		public int? PageNumber { get; set; }
		public EnumSystemLogSortField? SortField { get; set; }
		public bool? SortAscending { get; set; }
	}
}