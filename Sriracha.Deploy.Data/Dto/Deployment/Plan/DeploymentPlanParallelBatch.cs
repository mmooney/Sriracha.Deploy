using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Dto.Deployment.Plan
{
    public class DeploymentPlanParallelBatch
    {
        public List<DeploymentPlanMachineQueue> MachineQueueList { get; set; }
        public EnumDeploymentIsolationType IsolationType { get; set; }

        public DeploymentPlanParallelBatch()
        {
            this.MachineQueueList = new List<DeploymentPlanMachineQueue>();
        }
    }
}
