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
        private readonly IBuildPurgeRuleManager _buildPurgeRuleManager;

        public BuildPurger(ISystemSettings systemSettings, IBuildManager buildManager, Logger logger, IDIFactory diFactory, IBuildPurgeRuleManager buildPurgeRuleManager)
		{
			_systemSettings = DIHelper.VerifyParameter(systemSettings);
			_buildManager = DIHelper.VerifyParameter(buildManager);
			_logger = DIHelper.VerifyParameter(logger);
			_diFactory = DIHelper.VerifyParameter(diFactory);
            _buildPurgeRuleManager = DIHelper.VerifyParameter(buildPurgeRuleManager);
		}

		public void PurgeBuild(DeployBuild build)
		{
			_logger.Info("Purging build {0}", build.DisplayValue);
			_buildManager.DeleteBuild(build.Id);
			_logger.Info("Done purging build {0}", build.DisplayValue);
		}

		public void PurgeBuildIfNecessary(DeployBuild build)
		{
            int? rentionMinutes = _buildPurgeRuleManager.CalculateRentionMinutes(build);
			if(rentionMinutes.HasValue)
			{
				DateTime cutoffDate = DateTime.UtcNow.AddMinutes(0 - rentionMinutes.Value);
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
