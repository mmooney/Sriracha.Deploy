using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Tasks;
using Sriracha.Deploy.Data.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Tasks.Common.DeployWindowsService
{
    [TaskDefinitionMetadata("Deploy Windows Service", "WindowsServiceTaskOptionsView")]
    //[TaskDefinitionMetadata("Deploy Windows Service")]
    public class DeployWindowsServiceTaskDefinition : BaseDeployTaskDefinition<DeployWindowsServiceTaskOptions, DeployWindowsServiceTaskExecutor>
    {
        public DeployWindowsServiceTaskDefinition(IParameterParser parameterParser) : base(parameterParser)
        {

        }

        public override string TaskDefintionName
        {
            get { return "Deploy Windows Service"; }
        }
    }
}
