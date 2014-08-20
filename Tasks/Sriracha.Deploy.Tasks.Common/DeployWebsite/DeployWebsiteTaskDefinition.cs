using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Tasks;
using Sriracha.Deploy.Data.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Tasks.Common.DeployWebsite
{
    [TaskDefinitionMetadata("Deploy Website")]
    public class DeployWebsiteTaskDefinition : BaseDeployTaskDefinition<DeployWebsiteTaskOptions, DeployWebsiteTaskExecutor>
    {
        public DeployWebsiteTaskDefinition(IParameterParser parameterParser) : base(parameterParser)
        {

        }

    }
}
