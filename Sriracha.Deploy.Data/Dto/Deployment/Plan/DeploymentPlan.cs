using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Dto.Deployment.Plan
{
    public class DeploymentPlan
    {
        public string DeployBatchRequestId { get; set; }
        public List<DeploymentPlanParallelBatch> ParallelBatchList { get; set; }

        public DeploymentPlan()
        {
            this.ParallelBatchList = new List<DeploymentPlanParallelBatch>();
        }
    }
}
