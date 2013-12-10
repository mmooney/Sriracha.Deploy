using Sriracha.Deploy.Data.Dto.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Dto.Deployment.Offline
{
    public class OfflineComponentSelection
    {
        public DeployBatchRequestItem BatchRequestItem { get; set; }
        public List<DeployMachine> SelectedMachineList { get; set; }

        public OfflineComponentSelection()
        {
            this.SelectedMachineList = new List<DeployMachine>();
        }
    }
}
