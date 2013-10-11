using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.ConnectionSettings
{
	public class EmailConnectionSettings : ConnectionSettingBase
	{
		public string Host { get; set; }
		public string Password { get; set; }
		public int? Port { get; set; }
		public string UserName { get; set; }
	}
}
