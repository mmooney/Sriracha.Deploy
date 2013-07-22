using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Triggers;

namespace Sriracha.Deploy.Data.ServiceJobs.ServiceJobImpl
{
	public class JobScheduler : IJobScheduler
	{
		private readonly IScheduler _quartzScheduler;
		private readonly Logger _logger;
		private readonly ISystemSettings _systemSettings;

		public JobScheduler(IScheduler quartzScheduler, Logger logger, ISystemSettings systemSettings)
		{
			this._quartzScheduler = DIHelper.VerifyParameter(quartzScheduler);
			this._logger = DIHelper.VerifyParameter(logger);
			this._systemSettings = DIHelper.VerifyParameter(systemSettings);

			var jobDetail = new JobDetailImpl("Run Deployments", typeof(IRunDeploymentJob));
		}
		public void StartJobs()
		{
			this._logger.Info("Starting jobs");
			this._logger.Info("Done starting jobs");

			var jobDetail = new JobDetailImpl("RunDeployment", typeof(IRunDeploymentJob));
			var trigger = new SimpleTriggerImpl("RunDeploymentTrigger", SimpleTriggerImpl.RepeatIndefinitely, TimeSpan.FromSeconds(_systemSettings.RunDeploymentPollingIntervalSeconds));
			this._quartzScheduler.ScheduleJob(jobDetail, trigger);

			this._quartzScheduler.Start();
		}
	}
}
