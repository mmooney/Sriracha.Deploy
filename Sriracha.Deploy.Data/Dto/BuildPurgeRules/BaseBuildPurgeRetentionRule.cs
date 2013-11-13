using Sriracha.Deploy.Data.Dto.Build;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Dto.BuildPurgeRules
{
	public abstract class BaseBuildPurgeRetentionRule
	{
		public int? BuildRetentionMinutes { get; set; }
		public abstract bool MatchesRule(DeployBuild build, IDIFactory diFactory);
		public abstract string DisplayValue { get; }
	}
}
