using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data
{
	public interface ISystemSettings
	{
		int RunDeploymentPollingIntervalSeconds { get; }
		string DeployWorkingDirectory { get; }

		int LogPurgeJobIntervalSeconds { get; }
		int? LogPurgeTraceAgeMinutes { get; }
		int? LogPurgeDebugAgeMinutes { get; }
		int? LogPurgeInfoAgeMinutes { get; }
		int? LogPurgeWarnAgeMinutes { get; }
		int? LogPurgeErrorAgeMinutes { get; }
		int? LogPurgeFatalAgeMinutes { get; }

	}
}
