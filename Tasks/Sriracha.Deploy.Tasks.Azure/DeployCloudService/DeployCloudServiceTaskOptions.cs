using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Tasks.Azure.DeployCloudService
{
    public class DeployCloudServiceTaskOptions
    {
        public string AzureSubscriptionIdentifier { get; set; }
        public string AzureManagementCertificate { get; set; }
        public string ServiceName { get; set; }
        public string StorageAccountName { get; set; }

        public string DeploymentName { get; set; }
        public string DeploymentSlot { get; set; }

        public string AzurePackagePath { get; set; }
        public string AzureConfigPath { get; set; }

        public int? AzureTimeoutMinutes { get; set; }
    }
}
