using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NLog;
using Sriracha.Deploy.Data.Notifications;
using Sriracha.Deploy.Data.Tasks;
using Sriracha.Deploy.Data.Deployment;

namespace Sriracha.Deploy.Data.ServiceJobs.ServiceJobImpl
{
	public class RunBatchDeploymentJob : IRunBatchDeploymentJob
	{
		private readonly Logger _logger;
		private static IDeployBatchRunner _deployBatchRunner;
		private static volatile bool _isRunning = false;
		
		public RunBatchDeploymentJob(Logger logger, IDeployBatchRunner deployBatchRunner)
		{
			_logger = DIHelper.VerifyParameter(logger);
			_deployBatchRunner = DIHelper.VerifyParameter(deployBatchRunner);
		}

		public void Execute(Quartz.IJobExecutionContext context)
		{
			this._logger.Trace("Starting RunDeploymentJob.Execute");
			lock (typeof(RunBatchDeploymentJob))
			{
				if (_isRunning)
				{
					this._logger.Info("RunDeploymentJob already running");
					return;
				}
				else
				{
					_isRunning = true;
				}
			}
			try
			{
				_deployBatchRunner.TryRunNextDeployment();
			}
			catch (Exception err)
			{
				this._logger.ErrorException("RunDeploymentJob failed: " + err.ToString(), err);
			}
			finally
			{
				_isRunning = false;
			}
			this._logger.Trace("Done RunDeploymentJob.Execute");
		}
	}
}
