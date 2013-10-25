using MMDB.Shared;
using Sriracha.Deploy.Data.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Deployment.DeploymentImpl
{
	public class DeployBatchRunner : IDeployBatchRunner
	{
		private readonly NLog.Logger _logger;
		private readonly ISystemSettings _systemSettings;
		private readonly IDeployStateManager _deployStateManager;
		private readonly IDeployRequestManager _deployRequestManager;
		private IDeployRunner _deployRunner;
		private IDeployQueueManager _deployQueueManager;

		public DeployBatchRunner(NLog.Logger logger, ISystemSettings systemSettings, IDeployStateManager deployStateManager, IDeployRunner deployRunner, IDeployQueueManager deployQueueManager, IDeployRequestManager deployRequestManager)
		{
			_logger = DIHelper.VerifyParameter(logger);
			_systemSettings = DIHelper.VerifyParameter(systemSettings);
			_deployStateManager = DIHelper.VerifyParameter(deployStateManager);
			_deployRunner = DIHelper.VerifyParameter(deployRunner);
			_deployQueueManager = DIHelper.VerifyParameter(deployQueueManager);
			_deployRequestManager = DIHelper.VerifyParameter(deployRequestManager);
		}

		public void TryRunNextDeployment()
		{
			var nextDeploymentBatch = _deployQueueManager.PopNextBatchDeployment();
			if (nextDeploymentBatch == null)
			{
				this._logger.Trace("No pending deployment found");

			}
			else
			{
				try
				{
					bool isResume = false;
					this._logger.Info("Found pending deployment: " + nextDeploymentBatch.Id);

					if(nextDeploymentBatch.CancelRequested)
					{
						_deployStateManager.MarkBatchDeploymentCancelled(nextDeploymentBatch.Id, nextDeploymentBatch.CancelMessage);
						return;
					}
					else if (nextDeploymentBatch.ResumeRequested)
					{
						_deployStateManager.MarkBatchDeploymentResumed(nextDeploymentBatch.Id, nextDeploymentBatch.ResumeMessage);
						isResume = true;
					}
					List<string> environmentIds = nextDeploymentBatch.ItemList.SelectMany(i=>i.MachineList.Select(j=>j.EnvironmentId)).ToList();
					var existingDeployments = _deployQueueManager
												.GetQueue(null, EnumDeployStatus.InProcess.ListMe(), environmentIds, includeResumeRequested:false)
												.Items.Where(i=>i.Id != nextDeploymentBatch.Id);
					if(existingDeployments.Any())
					{
						var existingDeploymentEnvironmentIds = existingDeployments.SelectMany(i=>i.ItemList.SelectMany(j=>j.MachineList.Select(k=>k.EnvironmentId)));
						var environmentNames = nextDeploymentBatch.ItemList
													.SelectMany(i => i.MachineList	
																	.Where(j=>existingDeploymentEnvironmentIds.Contains(j.EnvironmentId))
																	.Select(j=>StringHelper.IsNullOrEmpty(j.EnvironmentName, j.EnvironmentId))).Distinct().ToList();
						string message = "Waiting to start deployment, other deployments in process to the following environments: " + string.Join(", ",environmentNames);
						_deployQueueManager.RequeueDeployment(nextDeploymentBatch.Id, message);
						return;
					}

					string subDirName = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + nextDeploymentBatch.Id;
					string deployDirectory = Path.Combine(_systemSettings.DeployWorkingDirectory, subDirName);
					if (Directory.Exists(deployDirectory))
					{
						//if directory already exists, start adding "_1", "_2", until we get a directory that does not exist
						int counter = 1;
						string newDeployDirectory = deployDirectory;
						while (Directory.Exists(newDeployDirectory))
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
					foreach (var item in nextDeploymentBatch.ItemList)
					{
						foreach (var machine in item.MachineList)
						{
							if(_deployRequestManager.HasCancelRequested(nextDeploymentBatch.Id))
							{
								_deployStateManager.MarkBatchDeploymentCancelled(nextDeploymentBatch.Id, nextDeploymentBatch.CancelMessage);
								return;
							}
							var deployState = _deployStateManager.GetOrCreateDeployState(item.Build.ProjectId, item.Build.Id, machine.EnvironmentId, machine.Id, nextDeploymentBatch.Id);
							if(deployState.Status != EnumDeployStatus.Success)
							{
								try
								{
									_deployStateManager.MarkDeploymentInProcess(deployState.Id);
									var machineIdList = new List<string> { machine.Id };
									_deployRunner.Deploy(deployState.Id, machine.EnvironmentId, item.Build.Id, machineIdList, runtimeSettings);
									_deployStateManager.MarkDeploymentSuccess(deployState.Id);
								}
								catch (Exception err)
								{
									_deployStateManager.MarkDeploymentFailed(deployState.Id, err);
									throw;
								}
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
	}
}
