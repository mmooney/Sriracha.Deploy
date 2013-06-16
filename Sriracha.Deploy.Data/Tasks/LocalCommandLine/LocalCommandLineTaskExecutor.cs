using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Dto;

namespace Sriracha.Deploy.Data.Tasks.LocalCommandLine
{
	public class LocalCommandLineTaskExecutor : BaseDeployTaskExecutor<LocalCommandLineTaskDefinition>
	{
		protected override DeployTaskExecutionResult InternalExecute(IDeployTaskStatusManager statusManager, LocalCommandLineTaskDefinition definition, DeployEnvironmentComponent environmentComponent, RuntimeSystemSettings runtimeSystemSettings)
		{
			throw new NotImplementedException();
		}
	}
}
