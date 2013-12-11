using Sriracha.Deploy.Data.Dto.Build;
using Sriracha.Deploy.Data.Dto.Deployment;
using Sriracha.Deploy.Data.Dto.Deployment.Offline;
using Sriracha.Deploy.Data.Dto.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Deployment.Offline
{
    public interface IOfflineDataProvider
    {
        void Initialize(string offlineDeploymentRunId, DeployBatchRequest deployBatchRequest, List<OfflineComponentSelection> selectionList, string workingDirectory);

        DeployBatchRequest GetBatchRequest();
        List<OfflineComponentSelection> GetSelectionList();

        DeployBatchRequest UpdateBatchDeploymentStatus(string deployBatchRequestId, EnumDeployStatus status, Exception err = null, string statusMessage = null, bool addToMessageHistory = true);

        DeployProject TryGetProject(string projectId);
        DeployFile GetFile(string fileId);
        byte[] GetFileData(string fileId);

        DeployState TryGetDeployState(string projectId, string buildId, string environmentId, string machineId, string deployBatchRequestItemId);
        void SaveDeployState(DeployState deployState);

        DeployState GetDeployState(string deployStateId);

        void WriteLog(string formattedMessage);

        List<OfflineDeploymentRun> GetDeployHistoryList(string _workingDirectory);
    }
}
