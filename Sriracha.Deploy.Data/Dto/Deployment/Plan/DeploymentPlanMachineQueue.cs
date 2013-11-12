using Sriracha.Deploy.Data.Dto.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Dto.Deployment.Plan
{
    public class DeploymentPlanMachineQueue 
    {
		public string Id { get; set; }
		public string MachineName { get; set; }
        public List<DeploymentPlanMachineQueueItem> MachineQueueItemList { get; set; }

        public DeploymentPlanMachineQueue ()
	    {
            this.MachineQueueItemList = new List<DeploymentPlanMachineQueueItem>();
	    }
	}
}
