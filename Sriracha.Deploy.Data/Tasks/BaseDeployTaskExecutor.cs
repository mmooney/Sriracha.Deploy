using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Dto;

namespace Sriracha.Deploy.Data.Tasks
{
	public abstract class BaseDeployTaskExecutor<TaskDefinition> : IDeployTaskExecutor 
		where TaskDefinition: IDeployTaskDefinition
	{
		public DeployTaskExecutionResult Execute(string deployStateId, IDeployTaskStatusManager statusManager, IDeployTaskDefinition definition, DeployEnvironmentComponent environmentComponent, DeployMachine machine, RuntimeSystemSettings runtimeSystemSettings)
		{
			if(definition == null)
			{
				throw new ArgumentNullException("Missing definition parameter");
			}
			if(!(definition is TaskDefinition))
			{
				throw new ArgumentException(string.Format("Task definition must be {0}, found {1}", typeof(TaskDefinition).FullName, definition.GetType().FullName));
			}
			var typedDefinition = (TaskDefinition)definition;
			return this.InternalExecute(deployStateId, statusManager, typedDefinition, environmentComponent, machine, runtimeSystemSettings);
		}

		protected abstract DeployTaskExecutionResult InternalExecute(string deployStateId, IDeployTaskStatusManager statusManager, TaskDefinition definition, DeployEnvironmentComponent environmentComponent, DeployMachine machine, RuntimeSystemSettings runtimeSystemSettings);
	}
}
