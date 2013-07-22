using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Sriracha.Deploy.Data.Impl
{
	public class DefaultSystemSettings : ISystemSettings
	{
		public int RunDeploymentPollingIntervalSeconds
		{
			get
			{
				return 60;
			}
		}


		public string DeployWorkingDirectory
		{
			get { return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "WorkingDirectory"); }
		}
	}
}
