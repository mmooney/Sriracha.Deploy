using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Tasks;

namespace Sriracha.Deploy.Data.Deployment
{
	public interface IDeploymentValidator
	{
		TaskDefinitionValidationResult ValidateMachineTaskDefinition(IDeployTaskDefinition taskDefinition, DeployEnvironmentConfiguration environmentComponent, DeployMachine machine);
		TaskDefinitionValidationResult ValidateTaskDefinition(IDeployTaskDefinition taskDefinition, DeployEnvironmentConfiguration environmentComponent);
		DeploymentValidationResult ValidateDeployment(DeployProject project, DeployComponent component, DeployEnvironment environment);
		ComponentConfigurationDefinition GetComponentConfigurationDefinition(List<DeployStep> deploymentStepList);
		void ValidateStatusTransition(EnumDeployStatus oldStatus, EnumDeployStatus newStatus);
	}
}
