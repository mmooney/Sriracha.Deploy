using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Build;
using Sriracha.Deploy.Data.Dto.Project;

namespace Sriracha.Deploy.Data.Tasks
{
	public abstract class BaseDeployTaskExecutor<TaskDefinition> : IDeployTaskExecutor 
		where TaskDefinition: IDeployTaskDefinition
	{
		private readonly IBuildParameterEvaluator _buildParameterEvaluator;

		public BaseDeployTaskExecutor(IBuildParameterEvaluator buildParamterEvaluator)
		{
			_buildParameterEvaluator = DIHelper.VerifyParameter(buildParamterEvaluator);
		}

		public DeployTaskExecutionResult Execute(string deployStateId, IDeployTaskStatusManager statusManager, IDeployTaskDefinition definition, DeployComponent component, DeployEnvironmentConfiguration environmentComponent, DeployMachine machine, DeployBuild build, RuntimeSystemSettings runtimeSystemSettings)
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
			return this.InternalExecute(deployStateId, statusManager, typedDefinition, component, environmentComponent, machine, build, runtimeSystemSettings);
		}

		protected string GetBuildParameterValue(string parameterName, DeployBuild build)
		{
			return _buildParameterEvaluator.Evaluate(parameterName, build);
		}

		protected abstract DeployTaskExecutionResult InternalExecute(string deployStateId, IDeployTaskStatusManager statusManager, TaskDefinition definition, DeployComponent component, DeployEnvironmentConfiguration environmentComponent, DeployMachine machine, DeployBuild build, RuntimeSystemSettings runtimeSystemSettings);
	}
}
