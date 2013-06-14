using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Tasks.RoundhousE
{
	public enum EnumRoundhousEDBType
	{
		Unknown,
		SqlServer,
		Postgres,
		MySql,
		Oracle,
		Other
	}

	public class RoundhousETaskOptions
	{
		public EnumRoundhousEDBType DBType { get; set; }
		public string DBTypeOtherValue { get; set; }
	}
}
