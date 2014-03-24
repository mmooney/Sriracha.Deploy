using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Tasks.Azure.DeployCloudService
{
    public class DeployCloudServiceTaskTaskOptions
    {
        public string AzureSubscriptionIdentifier { get; set; }
        public string AzureManagementCertificate { get; set; }
        public string ServiceName { get; set; }
    }
}
