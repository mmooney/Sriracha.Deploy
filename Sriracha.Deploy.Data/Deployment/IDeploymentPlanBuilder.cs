using Sriracha.Deploy.Data.Dto.Deployment;
using Sriracha.Deploy.Data.Dto.Deployment.Plan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Deployment
{
    public interface IDeploymentPlanBuilder
    {
        DeploymentPlan Build(DeployBatchRequest deployBatchRequest);
    }
}
