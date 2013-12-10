using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Dto.Deployment;
using Sriracha.Deploy.Data.Dto.Deployment.Offline;

namespace Sriracha.Deploy.Data.Repository
{
	public interface IOfflineDeploymentRepository
	{
		OfflineDeployment CreateOfflineDeployment(string deployBatchRequestId, EnumOfflineDeploymentStatus initialStatus);
        OfflineDeployment GetOfflineDeployment(string offlineDeploymentId);
        OfflineDeployment SetReadyForDownload(string offlineDeploymentId, string fileId);
        OfflineDeployment UpdateStatus(string offlineDeploymentId, EnumOfflineDeploymentStatus status, Exception err = null);
        OfflineDeployment PopNextOfflineDeploymentToCreate();
    }
}
