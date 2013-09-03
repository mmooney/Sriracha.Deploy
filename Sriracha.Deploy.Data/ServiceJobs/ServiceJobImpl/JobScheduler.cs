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

		}
		public void StartJobs()
		{
			this._logger.Info("Starting jobs");

			//this.ScheduleJob("RunDeployment", typeof(IRunDeploymentJob), _systemSettings.RunDeploymentPollingIntervalSeconds);
			this.ScheduleJob("RunBatchDeployment", typeof(IRunBatchDeploymentJob), _systemSettings.RunDeploymentPollingIntervalSeconds);
			this.ScheduleJob("PurgeSystemLogs", typeof(IPurgeSystemLogJob), _systemSettings.LogPurgeJobIntervalSeconds);
			this.ScheduleJob("PurgeBuilds", typeof(IPurgeBuildJob), _systemSettings.BuildPurgeJobIntervalSeconds);

			this._quartzScheduler.Start();

			this._logger.Info("Done starting jobs");
		}

		public void StopJobs()
		{
			this._logger.Info("Stopping jobs");

			_quartzScheduler.Shutdown(true);

			this._logger.Info("Done stopping jobs");
		}
		private void ScheduleJob(string jobName, Type jobType, int intervalSeconds)
		{
			var jobDetail = new JobDetailImpl(jobName, jobType);
			var trigger = new SimpleTriggerImpl(jobName + "Trigger", SimpleTriggerImpl.RepeatIndefinitely, TimeSpan.FromSeconds(intervalSeconds));
			this._quartzScheduler.ScheduleJob(jobDetail, trigger);
		}
	}
}
