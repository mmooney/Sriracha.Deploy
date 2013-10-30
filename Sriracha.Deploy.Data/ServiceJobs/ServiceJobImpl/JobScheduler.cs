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
		public void StartJobs(bool thrashMode=false)
		{
			if(thrashMode)
			{
				this._logger.Info("Starting jobs in thrash mode, running jobs every 10 seconds");
				_systemSettings.EmailSenderPollingIntervalSeconds
					= _systemSettings.RunDeploymentPollingIntervalSeconds
					= _systemSettings.LogPurgeJobIntervalSeconds
					= _systemSettings.BuildPurgeJobIntervalSeconds = 10;
				_systemSettings.GCFlushJobIntervalSeconds = 60*60*25;

				//this.ScheduleJob("EmailSender", typeof(IEmailSenderJob), _systemSettings.EmailSenderPollingIntervalSeconds);
				//this.ScheduleJob("RunBatchDeployment", typeof(IRunBatchDeploymentJob), _systemSettings.RunDeploymentPollingIntervalSeconds);
				//this.ScheduleJob("PurgeSystemLogs", typeof(IPurgeSystemLogJob), _systemSettings.LogPurgeJobIntervalSeconds);
				this.ScheduleJob("PurgeBuilds", typeof(IPurgeBuildJob), _systemSettings.BuildPurgeJobIntervalSeconds);
				//this.ScheduleJob("GCFlushJob", typeof(IGCFlushJob), _systemSettings.GCFlushJobIntervalSeconds, 5 * 60);
			}
			else 
			{
				this._logger.Info("Starting jobs");
				this.ScheduleJob("EmailSender", typeof(IEmailSenderJob), _systemSettings.EmailSenderPollingIntervalSeconds);
				this.ScheduleJob("RunBatchDeployment", typeof(IRunBatchDeploymentJob), _systemSettings.RunDeploymentPollingIntervalSeconds);
				this.ScheduleJob("PurgeSystemLogs", typeof(IPurgeSystemLogJob), _systemSettings.LogPurgeJobIntervalSeconds);
				this.ScheduleJob("PurgeBuilds", typeof(IPurgeBuildJob), _systemSettings.BuildPurgeJobIntervalSeconds);
				this.ScheduleJob("GCFlushJob", typeof(IGCFlushJob), _systemSettings.GCFlushJobIntervalSeconds, 5 * 60);
			}


			this._quartzScheduler.Start();

			this._logger.Info("Done starting jobs");
		}

		public void StopJobs()
		{
			this._logger.Info("Stopping jobs");

			_quartzScheduler.Shutdown(true);

			this._logger.Info("Done stopping jobs");
		}
		private void ScheduleJob(string jobName, Type jobType, int intervalSeconds, int delayStartSeconds=0)
		{
			var jobDetail = new JobDetailImpl(jobName, jobType);
			var trigger = new SimpleTriggerImpl(jobName + "Trigger", DateBuilder.FutureDate(delayStartSeconds, IntervalUnit.Second), null,  SimpleTriggerImpl.RepeatIndefinitely, TimeSpan.FromSeconds(intervalSeconds));
			this._quartzScheduler.ScheduleJob(jobDetail, trigger);
		}
	}
}
