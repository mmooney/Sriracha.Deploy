using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Tasks;

namespace Sriracha.Deploy.Data
{
	public interface IDeploymentValidator
	{
		TaskDefinitionValidationResult ValidateTaskDefinition(IDeployTaskDefinition taskDefinition, DeployEnvironmentComponent environmentComponent);

		DeploymentValidationResult ValidateDeployment(DeployComponent component, DeployEnvironment environment);
	}
}
