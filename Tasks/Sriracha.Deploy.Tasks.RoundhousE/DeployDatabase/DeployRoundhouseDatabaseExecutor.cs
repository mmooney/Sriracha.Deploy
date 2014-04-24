using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sriracha.Deploy.Data.Tasks;
using Sriracha.Deploy.Data.Deployment;

namespace Sriracha.Deploy.Tasks.RoundhousE.DeployDatabase
{
    public class DeployRoundhouseDatabaseExecutor : BaseDeployTaskExecutor<DeployRoundhouseDatabaseTaskDefinition, DeployRoundhouseDatabaseTaskOptions>
    {
        public DeployRoundhouseDatabaseExecutor(IParameterEvaluator parameterEvaluator, IDeploymentValidator validator) : base(parameterEvaluator, validator)
        {

        }

        protected override DeployTaskExecutionResult InternalExecute(TaskExecutionContext<DeployRoundhouseDatabaseTaskDefinition, DeployRoundhouseDatabaseTaskOptions> context)
        {
            throw new NotImplementedException();
        }
    }
}
