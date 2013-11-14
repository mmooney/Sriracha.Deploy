using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MMDB.Shared;
using Sriracha.Deploy.Data.Dto.Deployment;
using Sriracha.Deploy.Data.Dto.Deployment.Plan;
using Sriracha.Deploy.Data.Tasks;

namespace Sriracha.Deploy.Data.Deployment.DeploymentImpl
{
	public class DeployBatchParallelRunner : IDeployBatchRunner
	{
		private readonly NLog.Logger _logger;
		private readonly ISystemSettings _systemSettings;
		private readonly IDeployStateManager _deployStateManager;
		private readonly IDeployRequestManager _deployRequestManager;
		private readonly IDeployQueueManager _deployQueueManager;
		private readonly IDeploymentPlanBuilder _deploymentPlanBuilder;
		private readonly IDIFactory _diFactory;
		private readonly ICleanupManager _cleanupManager;

		public DeployBatchParallelRunner(NLog.Logger logger, ISystemSettings systemSettings, IDeployStateManager deployStateManager, IDeployQueueManager deployQueueManager, IDeployRequestManager deployRequestManager, IDeploymentPlanBuilder deploymentPlanBuilder, IDIFactory diFactory, ICleanupManager cleanupManager)
		{
			_logger = DIHelper.VerifyParameter(logger);
			_systemSettings = DIHelper.VerifyParameter(systemSettings);
			_deployStateManager = DIHelper.VerifyParameter(deployStateManager);
			_deployQueueManager = DIHelper.VerifyParameter(deployQueueManager);
			_deployRequestManager = DIHelper.VerifyParameter(deployRequestManager);
			_deploymentPlanBuilder = DIHelper.VerifyParameter(deploymentPlanBuilder);
			_diFactory = DIHelper.VerifyParameter(diFactory);
			_cleanupManager = DIHelper.VerifyParameter(cleanupManager);
		}

		public void ForceRunDeployment(string deployBatchRequestId)
		{
			var deployBatchRequest = _deployRequestManager.GetDeployBatchRequest(deployBatchRequestId);
			try 
			{
				_deployStateManager.MarkBatchDeploymentResumed(deployBatchRequest.Id, "Force run");
				RunDeployment(deployBatchRequest);
			}
			catch (Exception err)
			{
				_deployStateManager.MarkBatchDeploymentFailed(deployBatchRequest.Id, err);
				throw;
			}
		}

		private void RunDeployment(DeployBatchRequest deployBatchRequest)
		{
			string subDirName = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + deployBatchRequest.Id;
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
			_cleanupManager.QueueFolderForCleanup(runtimeSettings.LocalDeployDirectory, _systemSettings.DeploymentFolderCleanupMinutes);
			var plan = _deploymentPlanBuilder.Build(deployBatchRequest);
			//_deployStateManager.SaveDeploymentPlan(plan);
			foreach (var parallelBatchList in plan.ParallelBatchList)
			{
				var taskList = new List<Task>();
				foreach (var machineQueue in parallelBatchList.MachineQueueList)
				{
					string machineQueueId = machineQueue.Id;
					var task = Task.Factory.StartNew(() => DeployMachineQueue(plan, machineQueueId, runtimeSettings));
					taskList.Add(task);
				}
				var taskArray = taskList.ToArray();
				Task.WaitAll(taskArray);
				if (_deployRequestManager.IsStopped(deployBatchRequest.Id))
				{
					break;
				}
			}
			_deployStateManager.MarkBatchDeploymentSuccess(deployBatchRequest.Id);

			this._logger.Info("Deployment complete: " + deployBatchRequest.Id);
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
					this._logger.Info("Found pending deployment: " + nextDeploymentBatch.Id);

					if(nextDeploymentBatch.CancelRequested)
					{
						_deployStateManager.MarkBatchDeploymentCancelled(nextDeploymentBatch.Id, nextDeploymentBatch.CancelMessage);
						return;
					}
					else if (nextDeploymentBatch.ResumeRequested)
					{
						_deployStateManager.MarkBatchDeploymentResumed(nextDeploymentBatch.Id, nextDeploymentBatch.ResumeMessage);
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

					RunDeployment(nextDeploymentBatch);
				}
				catch (Exception err)
				{
					_deployStateManager.MarkBatchDeploymentFailed(nextDeploymentBatch.Id, err);
					throw;
				}

			}
		}

		public void DeployMachineQueue(DeploymentPlan plan, string machineQueueId, RuntimeSystemSettings runtimeSettings)
		{
			string deployBatchRequestId = plan.DeployBatchRequestId;
			var machineQueue = plan.ParallelBatchList.SelectMany(i=>i.MachineQueueList.Where(j=>j.Id == machineQueueId)).SingleOrDefault();
			if(machineQueue == null)
			{
				throw new RecordNotFoundException(typeof(DeploymentPlanMachineQueue), "Id", machineQueueId);
			}
			//Sooooo, we're in some threads now.  And some of our repository types (Raven) doesn't like sharing sessions between threads.
			//	So let's create a new instance now for this method
			var localDeployRequestManager = _diFactory.CreateInjectedObject<IDeployRequestManager>();
			var localDeployRunner = _diFactory.CreateInjectedObject<IDeployRunner>();
			var localDeployStateManager = _diFactory.CreateInjectedObject<IDeployStateManager>();

			foreach(var machineQueueItem in machineQueue.MachineQueueItemList)
			{
				if (localDeployRequestManager.HasCancelRequested(deployBatchRequestId))
				{
					localDeployStateManager.MarkBatchDeploymentCancelled(deployBatchRequestId, null);
					return;
				}
				else if (localDeployRequestManager.IsStopped(deployBatchRequestId))
				{
					return;
				}
				var machine = machineQueueItem.DeployBatchRequestItem.MachineList.FirstOrDefault(i=>i.Id == machineQueueItem.MachineId);
				if(machine == null)
				{
					throw new Exception("Failed to find machine " + machineQueueItem.MachineId);
				}
				var deployState = localDeployStateManager.GetOrCreateDeployState(machineQueueItem.DeployBatchRequestItem.Build.ProjectId, machineQueueItem.DeployBatchRequestItem.Build.Id, machine.EnvironmentId, machine.Id, deployBatchRequestId);
				if (deployState.Status != EnumDeployStatus.Success)
				{
					try
					{
						localDeployStateManager.MarkDeploymentInProcess(deployState.Id);
						var machineIdList = new List<string> { machine.Id };
						localDeployRunner.Deploy(deployState.Id, machine.EnvironmentId, machineQueueItem.DeployBatchRequestItem.Build.Id, machineIdList, runtimeSettings);
						localDeployStateManager.MarkDeploymentSuccess(deployState.Id);
					}
					catch (Exception err)
					{
						localDeployStateManager.MarkDeploymentFailed(deployState.Id, err);
						throw;
					}
				}
			}
		}
	}
}
