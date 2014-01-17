using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Dto.Build;
using Sriracha.Deploy.Data.SystemSettings;

namespace Sriracha.Deploy.Data.Build.BuildImpl
{
	public class BuildPurger : IBuildPurger
	{
		private readonly ISystemSettings _systemSettings;
		private readonly IBuildManager _buildManager;
		private readonly Logger _logger;
		private readonly IDIFactory _diFactory;

		public BuildPurger(ISystemSettings systemSettings, IBuildManager buildManager, Logger logger, IDIFactory diFactory)
		{
			_systemSettings = DIHelper.VerifyParameter(systemSettings);
			_buildManager = DIHelper.VerifyParameter(buildManager);
			_logger = DIHelper.VerifyParameter(logger);
			_diFactory = DIHelper.VerifyParameter(diFactory);
		}

		public void PurgeBuild(DeployBuild build)
		{
			_logger.Info("Purging build {0}", build.DisplayValue);
			_buildManager.DeleteBuild(build.Id);
			_logger.Info("Done purging build {0}", build.DisplayValue);
		}

		public void PurgeBuildIfNecessary(DeployBuild build)
		{
			var ruleList = _systemSettings.BuildPurgeRetentionRuleList;
			int maxRetentionMinutes = _systemSettings.DefaultBuildRetentionMinutes;
			bool keepForever = false;
			if(ruleList != null)
			{
				foreach(var rule in ruleList)
				{
					if (rule.MatchesRule(build, _diFactory))
					{
						if(!rule.BuildRetentionMinutes.HasValue)
						{
							_logger.Trace("Build \"{0}\" matched rule \"{1}\", keep forever", build.DisplayValue, rule.DisplayValue);
							keepForever = true;
						}
						else 
						{
							_logger.Trace("Build \"{0}\" matched rule \"{1}\", build retention minutes: \"{2}\"", build.DisplayValue, rule.DisplayValue, rule.BuildRetentionMinutes.Value);
							maxRetentionMinutes = Math.Max(maxRetentionMinutes, rule.BuildRetentionMinutes.Value);
						}
					}
				}
			}
			if (keepForever)
			{
				_logger.Trace("Per rules, keeping build {0} forever", build.DisplayValue);
			}
			else if(maxRetentionMinutes <= 0)
			{
				_logger.Trace("In absence of any rules or defaults, keeping build {0} forever", build.DisplayValue);
			}
			else 
			{
				DateTime cutoffDate = DateTime.UtcNow.AddMinutes(0 - maxRetentionMinutes);
				if(build.UpdatedDateTimeUtc < cutoffDate)
				{
					_logger.Info("Build \"{0}\" updated date {1} is less than cutoff date {2}, purging build", build.DisplayValue, build.UpdatedDateTimeUtc, cutoffDate);
					this.PurgeBuild(build);
				}
				else
				{
					_logger.Trace("Build \"{0}\" updated date {1} is greater than cutoff date {2}, leaving as is", build.DisplayValue, build.UpdatedDateTimeUtc, cutoffDate);
				}
			}
		}
	}
}
