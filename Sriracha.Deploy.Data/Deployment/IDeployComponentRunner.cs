using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Tasks;
using Sriracha.Deploy.Data.Dto.Project;

namespace Sriracha.Deploy.Data.Deployment
{
	public interface IDeployComponentRunner
	{
		void Run(string deployStateId, IDeployTaskStatusManager statusManager, List<IDeployTaskDefinition> taskDefinitionList, DeployComponent component, DeployEnvironmentConfiguration environmentComponent, DeployMachine machine,  DeployBuild build, RuntimeSystemSettings runtimeSystemSettings);
	}
}
