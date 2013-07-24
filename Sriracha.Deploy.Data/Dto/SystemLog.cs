using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Dto
{
	public enum EnumSystemLogType
	{
		Trace = 0,
		Debug = 1,
		Info = 2,
		Warn = 3,
		Error = 4,
		Fatal = 5,
		Off = 6
	}

	public class SystemLog
	{
		public string MessageText;
		public string Id { get; set; }
		public EnumSystemLogType EnumSystemLogTypeID { get; set; }
		public string UserName { get; set; }
		public DateTime MessageDateTimeUtc { get; set; }
	}
}
