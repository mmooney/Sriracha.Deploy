using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.Shared;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Tasks;

namespace Sriracha.Deploy.Data.Impl
{
	public class DeployComponentRunner : IDeployComponentRunner
	{
		private readonly IDeployTaskFactory _deployTaskFactory;
		
		public DeployComponentRunner(IDeployTaskFactory deployTaskFactory)
		{
			this._deployTaskFactory = DIHelper.VerifyParameter(deployTaskFactory);
		}

		public void Run(string deployStateId, IDeployTaskStatusManager statusManager, List<IDeployTaskDefinition> taskDefinitionList, DeployEnvironmentComponent environmentComponent, RuntimeSystemSettings runtimeSystemSettings)
		{
			int stepCounter = 0;
			foreach(var taskDefinition in taskDefinitionList)
			{
				stepCounter++;
				statusManager.Info(deployStateId, string.Format("Step {0}: Starting {1}", stepCounter, taskDefinition.TaskDefintionName));
				var executor = _deployTaskFactory.CreateTaskExecutor(taskDefinition.GetTaskExecutorType());
				var result = executor.Execute(deployStateId, statusManager, taskDefinition, environmentComponent, runtimeSystemSettings);
				switch(result.Status)
				{
					case EnumDeployTaskExecutionResultStatus.Success:
						statusManager.Info(deployStateId, string.Format("Step {0}: End {1}, completed successfully", stepCounter, taskDefinition.TaskDefintionName));
						break;
					case EnumDeployTaskExecutionResultStatus.Error:
						statusManager.Info(deployStateId, string.Format("Step {0}: End {1}, failed", stepCounter, taskDefinition.TaskDefintionName));
						return;
						break;
					case EnumDeployTaskExecutionResultStatus.Warning:
						statusManager.Info(deployStateId, string.Format("Step {0}: End {1}, completed with warnings", stepCounter, taskDefinition.TaskDefintionName));
						break;
					default:
						throw new UnknownEnumValueException(result.Status);
				}
			}
		}
	}
}
