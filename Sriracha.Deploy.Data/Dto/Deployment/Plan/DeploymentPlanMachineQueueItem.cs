using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Dto.Deployment.Plan
{
    public class DeploymentPlanMachineQueueItem
    {
        public string MachineId { get; set; }
        public DeployBatchRequestItem DeployBatchRequestItem { get; set; }
    }
}
