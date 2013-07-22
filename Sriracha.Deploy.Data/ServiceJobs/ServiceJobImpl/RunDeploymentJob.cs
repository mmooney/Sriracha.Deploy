using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;
using Quartz;

namespace Sriracha.Deploy.Data.ServiceJobs.ServiceJobImpl
{
	public class RunDeploymentJob : IRunDeploymentJob
	{
		private readonly Logger _logger;
		private readonly IDeployStateManager _deployStateManager;

		public RunDeploymentJob(Logger logger, IDeployStateManager deployStateManager)
		{
			_logger = DIHelper.VerifyParameter(logger);
			_deployStateManager = DIHelper.VerifyParameter(deployStateManager);
		}

		public void Execute(IJobExecutionContext context)
		{
			this._logger.Trace("Starting RunDeploymentJob.Execute");
			this._logger.Trace("Done RunDeploymentJob.Execute");
		}
	}
}
