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


		public int LogPurgeJobIntervalSeconds
		{
			get { return 60*60; }
		}

		public int? LogPurgeTraceAgeMinutes
		{
			get { return 60; }
		}

		public int? LogPurgeDebugAgeMinutes
		{
			get { return 60; }
		}

		public int? LogPurgeInfoAgeMinutes
		{
			get { return 60*24; }
		}

		public int? LogPurgeWarnAgeMinutes
		{
			get { return 60*24*7; }
		}

		public int? LogPurgeErrorAgeMinutes
		{
			get { return 60*24*30; }
		}

		public int? LogPurgeFatalAgeMinutes
		{
			get { return 60*24*30; }
		}
	}
}
