using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Dto.BuildPurgeRules;

namespace Sriracha.Deploy.Data
{
	public interface ISystemSettings
	{
		int RunDeploymentPollingIntervalSeconds { get; set; }
		string DeployWorkingDirectory { get; set; }

		int LogPurgeJobIntervalSeconds { get; set; }
		int? LogPurgeTraceAgeMinutes { get; set; }
		int? LogPurgeDebugAgeMinutes { get; set; }
		int? LogPurgeInfoAgeMinutes { get; set; }
		int? LogPurgeWarnAgeMinutes { get; set; }
		int? LogPurgeErrorAgeMinutes { get; set; }
		int? LogPurgeFatalAgeMinutes { get; set; }

		int BuildPurgeJobIntervalSeconds { get; set; }
		int? DefaultBuildRetentionMinutes { get; set; }
		List<BaseBuildPurgeRetentionRule> BuildPurgeRetentionRuleList { get; set; }
	}
}
