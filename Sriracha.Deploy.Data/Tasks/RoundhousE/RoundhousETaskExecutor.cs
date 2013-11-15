using Sriracha.Deploy.Data.Build;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Dto.Build;
using Sriracha.Deploy.Data.Dto.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Deployment;

namespace Sriracha.Deploy.Data.Tasks.RoundhousE
{
	public class RoundhousETaskExecutor : BaseDeployTaskExecutor<RoundhousETask>
	{
		public RoundhousETaskExecutor(IParameterEvaluator buildParameterEvaluator) : base(buildParameterEvaluator)
		{

		}

		protected override DeployTaskExecutionResult InternalExecute(string deployStateId, IDeployTaskStatusManager statusManager, RoundhousETask definition, DeployComponent component, DeployEnvironmentConfiguration environmentComponent, DeployMachine machine, DeployBuild build, RuntimeSystemSettings runtimeSystemSettings)
		{
			throw new NotImplementedException();
		}
	}
}
