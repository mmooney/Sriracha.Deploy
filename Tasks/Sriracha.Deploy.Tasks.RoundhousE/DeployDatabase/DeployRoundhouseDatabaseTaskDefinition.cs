using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Tasks;
using Sriracha.Deploy.Data.Utility;

namespace Sriracha.Deploy.Tasks.RoundhousE.DeployDatabase
{
    public class DeployRoundhouseDatabaseTaskDefinition : BaseDeployTaskDefinition<DeployRoundhouseDatabaseTaskOptions, DeployRoundhouseDatabaseExecutor>
    {
        public DeployRoundhouseDatabaseTaskDefinition(IParameterParser parameterParser) : base(parameterParser)
        {

        }
    }
}
