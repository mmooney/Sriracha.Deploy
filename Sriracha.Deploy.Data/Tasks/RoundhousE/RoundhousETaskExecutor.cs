using Sriracha.Deploy.Data.Build;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Dto.Project;
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

		protected override DeployTaskExecutionResult InternalExecute(string deployStateId, IDeployTaskStatusManager statusManager, RoundhousETask definition, DeployComponent component, DeployEnvironmentConfiguration environmentComponent, DeployMachine machine, DeployBuild build, RuntimeSystemSettings runtimeSystemSettings)
		{
			throw new NotImplementedException();
		}
	}
}
