using Sriracha.Deploy.Data.Build;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Tasks.RoundhousE
{
	public class RoundhousETaskExecutor : BaseDeployTaskExecutor<RoundhousETask>
	{
		public RoundhousETaskExecutor(IBuildParameterEvaluator buildParameterEvaluator) : base(buildParameterEvaluator)
		{

		}

		protected override DeployTaskExecutionResult InternalExecute(string deployStateId, IDeployTaskStatusManager statusManager, RoundhousETask definition, Dto.DeployComponent component, Dto.DeployEnvironmentConfiguration environmentComponent, Dto.DeployMachine machine, Dto.DeployBuild build, RuntimeSystemSettings runtimeSystemSettings)
		{
			throw new NotImplementedException();
		}
	}
}
