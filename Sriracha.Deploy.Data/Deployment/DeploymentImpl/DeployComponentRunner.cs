using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.Shared;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Tasks;
using Sriracha.Deploy.Data.Credentials;
using Sriracha.Deploy.Data.Dto.Project;

namespace Sriracha.Deploy.Data.Deployment.DeploymentImpl
{
	public class DeployComponentRunner : IDeployComponentRunner
	{
		private readonly IDeployTaskFactory _deployTaskFactory;
		private readonly IImpersonator _impersonator;
		
		public DeployComponentRunner(IDeployTaskFactory deployTaskFactory, IImpersonator impersonator)
		{
			_deployTaskFactory = DIHelper.VerifyParameter(deployTaskFactory);
			_impersonator = DIHelper.VerifyParameter(impersonator);
		}

		public void Run(string deployStateId, IDeployTaskStatusManager statusManager, List<IDeployTaskDefinition> taskDefinitionList, DeployComponent component, DeployEnvironmentConfiguration environmentComponent, DeployMachine machine, DeployBuild build, RuntimeSystemSettings runtimeSystemSettings)
		{
			int stepCounter = 0;
			foreach(var taskDefinition in taskDefinitionList)
			{
				stepCounter++;
				statusManager.Info(deployStateId, string.Format("Step {0}: Starting {1}", stepCounter, taskDefinition.TaskDefintionName));
				DeployTaskExecutionResult result;
				using (var impersontator = BeginImpersonation(deployStateId, statusManager, environmentComponent))
				{
					var executor = _deployTaskFactory.CreateTaskExecutor(taskDefinition.GetTaskExecutorType());
					result = executor.Execute(deployStateId, statusManager, taskDefinition, component, environmentComponent, machine, build, runtimeSystemSettings);
				}
				switch(result.Status)
				{
					case EnumDeployTaskExecutionResultStatus.Success:
						statusManager.Info(deployStateId, string.Format("Step {0}: End {1}, completed successfully", stepCounter, taskDefinition.TaskDefintionName));
						break;
					case EnumDeployTaskExecutionResultStatus.Error:
						statusManager.Info(deployStateId, string.Format("Step {0}: End {1}, failed", stepCounter, taskDefinition.TaskDefintionName));
						return;	//error error eject!
						//break;
					case EnumDeployTaskExecutionResultStatus.Warning:
						statusManager.Info(deployStateId, string.Format("Step {0}: End {1}, completed with warnings", stepCounter, taskDefinition.TaskDefintionName));
						break;
					default:
						throw new UnknownEnumValueException(result.Status);
				}
			}
		}

		private ImpersonationContext BeginImpersonation(string deployStateId, IDeployTaskStatusManager statusManager, DeployEnvironmentConfiguration environmentComponent)
		{
			if(!string.IsNullOrEmpty(environmentComponent.DeployCredentialsId))
			{
				var context = _impersonator.BeginImpersonation(environmentComponent.DeployCredentialsId);
				statusManager.Info(deployStateId, "Starting impersonation of " + context.Credentials.DisplayValue);
				return context;
			}
			else 
			{
				statusManager.Info(deployStateId, "No impersonation");
				return null;
			}
		}
	}
}
