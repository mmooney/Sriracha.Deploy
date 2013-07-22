using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Dto;

namespace Sriracha.Deploy.Data.Tasks
{
	public interface IDeployTaskExecutor
	{
		DeployTaskExecutionResult Execute(string deployStateId, IDeployTaskStatusManager statusManager, IDeployTaskDefinition definition, DeployEnvironmentComponent environmentComponent, RuntimeSystemSettings runtimeSystemSettings);		
	}
}
