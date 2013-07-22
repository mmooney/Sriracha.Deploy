using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NLog;
using Quartz;
using Sriracha.Deploy.Data.Tasks;

namespace Sriracha.Deploy.Data.ServiceJobs.ServiceJobImpl
{
	public class RunDeploymentJob : IRunDeploymentJob
	{
		private readonly Logger _logger;
		private readonly IDeployStateManager _deployStateManager;
		private readonly IDeployRunner _deployRunner;
		private readonly ISystemSettings _systemSettings;

		public RunDeploymentJob(Logger logger, IDeployStateManager deployStateManager, IDeployRunner deployRunner, ISystemSettings systemSettings)
		{
			_logger = DIHelper.VerifyParameter(logger);
			_deployStateManager = DIHelper.VerifyParameter(deployStateManager);
			_deployRunner = DIHelper.VerifyParameter(deployRunner);
			_systemSettings = DIHelper.VerifyParameter(systemSettings);
		}

		public void Execute(IJobExecutionContext context)
		{
			this._logger.Trace("Starting RunDeploymentJob.Execute");
			try 
			{
				var nextDeployment = this._deployStateManager.PopNextDeployment();
				if(nextDeployment == null)
				{
					this._logger.Trace("No pending deployment found");

				}
				else 
				{
					this._logger.Info("Found pending deployment: " + nextDeployment.Id);

					string deployDirectory = Path.Combine(_systemSettings.DeployWorkingDirectory, nextDeployment.Id + "_" + Guid.NewGuid().ToString());
					var runtimeSettings = new RuntimeSystemSettings
					{
						LocalDeployDirectory = deployDirectory
					};
					Directory.CreateDirectory(runtimeSettings.LocalDeployDirectory);
					_deployRunner.Deploy(nextDeployment.Id, nextDeployment.Environment.Id, nextDeployment.Build.Id, runtimeSettings);

					this._logger.Info("Deployment complete: " + nextDeployment.Id);
				}
			}
			catch(Exception err)
			{
				this._logger.ErrorException("RunDeploymentJob failed: " + err.ToString(), err);
			}
			this._logger.Trace("Done RunDeploymentJob.Execute");
		}
	}
}
