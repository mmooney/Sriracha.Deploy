using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.ConnectionSettings
{
	public class FtpConnectionSettings : ConnectionSettingBase
	{
		public string FtpUserName { get; set; }
		public string FtpPassword { get; set; }
		public string FtpHost { get; set; }
		public bool SecureFTP { get; set; }
	}
}
