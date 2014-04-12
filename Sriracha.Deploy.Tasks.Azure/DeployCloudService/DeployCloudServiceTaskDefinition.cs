using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Tasks;
using Sriracha.Deploy.Data.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Tasks.Azure.DeployCloudService
{
    [TaskDefinitionMetadata("Azure - Deploy Cloud Service", "DeployCloudServiceTaskOptionsView")]
    public class DeployCloudServiceTaskDefinition : BaseDeployTaskDefinition<DeployCloudServiceTaskOptions, DeployCloudServiceTaskExecutor>
    {
        public DeployCloudServiceTaskDefinition(IParameterParser parameterParser) : base(parameterParser)
        {
        }

        public override string TaskDefintionName
        {
            get { return "Deploy Cloud Service"; }
        }
    }
}
