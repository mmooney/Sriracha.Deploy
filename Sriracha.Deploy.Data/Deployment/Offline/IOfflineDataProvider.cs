using Sriracha.Deploy.Data.Dto.Deployment;
using Sriracha.Deploy.Data.Dto.Deployment.Offline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Deployment.Offline
{
    public interface IOfflineDataProvider
    {
        void Initialize(DeployBatchRequest deployBatchRequest, List<OfflineComponentSelection> selectionList, string workingDirectory);

        DeployBatchRequest GetBatchRequest();
        List<OfflineComponentSelection> GetSelectionList();

        DeployBatchRequest UpdateBatchDeploymentStatus(string deployBatchRequestId, EnumDeployStatus status, Exception err = null, string statusMessage = null, bool addToMessageHistory = true);
    }
}
