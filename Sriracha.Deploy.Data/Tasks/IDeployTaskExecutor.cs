using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Dto;

namespace Sriracha.Deploy.Data.Tasks
{
	public interface IDeployTaskExecutor
	{
		void Execute(IDeployTaskDefinition definition, DeployEnvironmentComponent environmentComponent, RuntimeSystemSettings runtimeSystemSettings);		
	}
}
