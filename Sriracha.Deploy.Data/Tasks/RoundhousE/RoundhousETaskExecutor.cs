using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Tasks.RoundhousE
{
	public class RoundhousETaskExecutor : BaseDeployTaskExecutor<RoundhousETask>
	{
		protected override DeployTaskExecutionResult InternalExecute(string deployStateId, IDeployTaskStatusManager statusManager, RoundhousETask definition, Dto.DeployEnvironmentConfiguration environmentComponent, Dto.DeployMachine machine, RuntimeSystemSettings runtimeSystemSettings)
		{
			throw new NotImplementedException();
		}
	}
}
