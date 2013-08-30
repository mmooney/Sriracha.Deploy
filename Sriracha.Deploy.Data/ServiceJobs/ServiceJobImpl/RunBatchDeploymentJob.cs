using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NLog;
using Sriracha.Deploy.Data.Tasks;

namespace Sriracha.Deploy.Data.ServiceJobs.ServiceJobImpl
{
	public class RunBatchDeploymentJob : IRunBatchDeploymentJob
	{
		private readonly Logger _logger;
		private readonly IDeployStateManager _deployStateManager;
		private readonly ISystemSettings _systemSettings;
		private readonly IDeployRunner _deployRunner; 
		private static volatile bool _isRunning = false;

		public RunBatchDeploymentJob(Logger logger, IDeployStateManager deployStateManager, ISystemSettings systemSettings, IDeployRunner deployRunner)
		{
			_logger = DIHelper.VerifyParameter(logger);
			_deployStateManager = DIHelper.VerifyParameter(deployStateManager);
			_systemSettings = DIHelper.VerifyParameter(systemSettings);
			_deployRunner = DIHelper.VerifyParameter(deployRunner);
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
				var nextDeploymentBatch = this._deployStateManager.PopNextBatchDeployment();
				if (nextDeploymentBatch == null)
				{
					this._logger.Trace("No pending deployment found");

				}
				else
				{
					try
					{
						this._logger.Info("Found pending deployment: " + nextDeploymentBatch.Id);

						string subDirName = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + nextDeploymentBatch.Id;
						string deployDirectory = Path.Combine(_systemSettings.DeployWorkingDirectory, subDirName);
						if(Directory.Exists(deployDirectory))
						{
							//if directory already exists, start adding "_1", "_2", until we get a directory that does not exist
							int counter = 1;
							string newDeployDirectory = deployDirectory;
							while(Directory.Exists(newDeployDirectory))
							{
								newDeployDirectory = deployDirectory + "_" + counter.ToString();
								counter++;
							}
							deployDirectory = newDeployDirectory;
						}
						var runtimeSettings = new RuntimeSystemSettings
						{
							LocalDeployDirectory = deployDirectory
						};
						Directory.CreateDirectory(runtimeSettings.LocalDeployDirectory);
						foreach(var item in nextDeploymentBatch.ItemList)
						{
							foreach(var machine in item.MachineList)
							{
								var deployState = _deployStateManager.CreateDeployState(item.Build.ProjectId, item.Build.Id, machine.EnvironmentId, machine.Id, nextDeploymentBatch.Id);
								try 
								{
									_deployStateManager.MarkDeploymentInProcess(deployState.Id);
									var machineIdList = new List<string> { machine.Id };
									_deployRunner.Deploy(deployState.Id, machine.EnvironmentId, item.Build.Id, machineIdList, runtimeSettings);
									_deployStateManager.MarkDeploymentSuccess(deployState.Id);
								}
								catch(Exception err)
								{
									_deployStateManager.MarkDeploymentFailed(deployState.Id, err);
									throw;
								}
							}
						}
	
						_deployStateManager.MarkBatchDeploymentSuccess(nextDeploymentBatch.Id);

						this._logger.Info("Deployment complete: " + nextDeploymentBatch.Id);
					}
					catch (Exception err)
					{
						_deployStateManager.MarkBatchDeploymentFailed(nextDeploymentBatch.Id, err);
						throw;
					}
				}
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
