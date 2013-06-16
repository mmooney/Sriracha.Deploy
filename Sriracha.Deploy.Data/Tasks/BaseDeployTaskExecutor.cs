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
		public DeployTaskExecutionResult Execute(IDeployTaskStatusManager statusManager, IDeployTaskDefinition definition, DeployEnvironmentComponent environmentComponent, RuntimeSystemSettings runtimeSystemSettings)
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
			return this.InternalExecute(statusManager, typedDefinition, environmentComponent, runtimeSystemSettings);
		}

		protected abstract DeployTaskExecutionResult InternalExecute(IDeployTaskStatusManager statusManager, TaskDefinition definition, DeployEnvironmentComponent environmentComponent, RuntimeSystemSettings runtimeSystemSettings);
	}
}
