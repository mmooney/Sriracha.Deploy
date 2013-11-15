using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Build;
using Sriracha.Deploy.Data.Dto.Project;
using Sriracha.Deploy.Data.Dto.Build;
using Sriracha.Deploy.Data.Deployment;

namespace Sriracha.Deploy.Data.Tasks
{
	public abstract class BaseDeployTaskExecutor<TaskDefinition> : IDeployTaskExecutor 
		where TaskDefinition: IDeployTaskDefinition
	{
		private readonly IParameterEvaluator _buildParameterEvaluator;

		public BaseDeployTaskExecutor(IParameterEvaluator buildParamterEvaluator)
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
			return _buildParameterEvaluator.EvaluateBuildParameter(parameterName, build);
		}

		protected string GetDeployParameterValue(string parameterName, RuntimeSystemSettings runtimeSystemSettings, DeployMachine machine, DeployComponent component)
		{
			return _buildParameterEvaluator.EvaluateDeployParameter(parameterName, runtimeSystemSettings, machine, component);
		}

		protected abstract DeployTaskExecutionResult InternalExecute(string deployStateId, IDeployTaskStatusManager statusManager, TaskDefinition definition, DeployComponent component, DeployEnvironmentConfiguration environmentComponent, DeployMachine machine, DeployBuild build, RuntimeSystemSettings runtimeSystemSettings);
	}
}
