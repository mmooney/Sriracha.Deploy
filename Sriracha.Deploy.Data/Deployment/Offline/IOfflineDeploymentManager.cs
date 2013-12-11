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
        OfflineDeployment BeginCreateOfflineDeployment(string deployBatchRequestId);
        OfflineDeployment GetOfflineDeployment(string offlineDeploymentId);
        OfflineDeployment GetOfflineDeploymentForDeploymentBatchRequestId(string deployBatchRequestId);

        OfflineDeployment PopNextOfflineDeploymentToCreate();
        void CreateOfflineDeploymentPackage(string offlineDeploymentId);

        OfflineDeployment ImportHistory(string offlineDeploymentId, string fileId);
    }
}
