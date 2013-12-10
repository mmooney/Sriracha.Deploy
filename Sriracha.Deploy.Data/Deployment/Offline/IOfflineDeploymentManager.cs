using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Dto.Deployment;
using Sriracha.Deploy.Data.Dto.Deployment.Offline;

namespace Sriracha.Deploy.Data.Deployment.Offline
{
	public interface IOfflineDeploymentManager
	{
        OfflineDeployment BeginCreateOfflineDeployment(List<DeployBatchRequestItem> itemList, string deploymentLabel);
        OfflineDeployment GetOfflineDeployment(string offlineDeploymentId);

        OfflineDeployment PopNextOfflineDeploymentToCreate();
        void CreateOfflineDeploymentPackage(string offlineDeploymentId);
    }
}
