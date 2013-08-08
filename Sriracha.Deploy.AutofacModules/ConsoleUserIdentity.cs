using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data;

namespace Sriracha.Deploy.AutofacModules
{
	public class ConsoleUserIdentity : IUserIdentity
	{
		public string UserName
		{
			get { return Environment.UserName; }
		}
	}
}
