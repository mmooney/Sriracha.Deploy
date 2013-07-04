using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Tasks;

namespace Sriracha.Deploy.Data
{
	public interface IDeployComponentRunner
	{
		void Run(IDeployTaskStatusManager statusManager, List<IDeployTaskDefinition> taskDefinitionList, DeployEnvironmentComponent environmentComponent, RuntimeSystemSettings runtimeSystemSettings);
	}
}
