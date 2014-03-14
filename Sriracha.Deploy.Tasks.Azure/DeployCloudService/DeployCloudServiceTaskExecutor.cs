using Sriracha.Deploy.Data.Deployment;
using Sriracha.Deploy.Data.Dto.Build;
using Sriracha.Deploy.Data.Dto.Project;
using Sriracha.Deploy.Data.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Tasks.Azure.DeployCloudService
{
    public class DeployCloudServiceTaskExecutor : BaseDeployTaskExecutor<DeployCloudServiceTaskDefinition>
    {
        public DeployCloudServiceTaskExecutor(IParameterEvaluator parameterEvaluator) : base(parameterEvaluator)
        {

        }
        protected override DeployTaskExecutionResult InternalExecute(string deployStateId, IDeployTaskStatusManager statusManager, DeployCloudServiceTaskDefinition definition, DeployComponent component, DeployEnvironmentConfiguration environmentComponent, DeployMachine machine, DeployBuild build, RuntimeSystemSettings runtimeSystemSettings)
        {
            throw new NotImplementedException();
        }
    }
}
