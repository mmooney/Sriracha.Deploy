using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Dto.Deployment.Offline
{
    public class OfflineDeploymentRun
    {
        public string Id { get; set; }
        public DeployBatchRequest DeployBatchRequest { get; set; }
        public List<OfflineComponentSelection> SelectionList { get; set; }
    }
}
