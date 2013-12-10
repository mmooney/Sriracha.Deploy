using Sriracha.Deploy.Data.Dto.Deployment;
using Sriracha.Deploy.Data.Dto.Deployment.Offline;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Deployment.Offline.OfflineImpl
{
    public class OfflineDataProvider : IOfflineDataProvider
    {
        private readonly IUserIdentity _userIdentity;
        private OfflineDeploymentRun _deploymentRun;
        private List<OfflineComponentSelection> _selectionList;
        private string _workingDirectory;
        private static object _fileLocker = new Object();

        public OfflineDataProvider(IUserIdentity userIdentity)
        {
            _userIdentity = DIHelper.VerifyParameter(userIdentity);
        }

        public void Initialize(DeployBatchRequest deployBatchRequest, List<OfflineComponentSelection> selectionList, string workingDirectory)
        {
            _workingDirectory = workingDirectory;
            _deploymentRun = new OfflineDeploymentRun
            {
                Id = Guid.NewGuid().ToString(),
                DeployBatchRequest = deployBatchRequest,
                SelectionList = selectionList
            };
        }

        public DeployBatchRequest GetBatchRequest()
        {
            return _deploymentRun.DeployBatchRequest;
        }

        public List<OfflineComponentSelection> GetSelectionList()
        {
            return _deploymentRun.SelectionList;
        }

        public DeployBatchRequest UpdateBatchDeploymentStatus(string deployBatchRequestId, EnumDeployStatus status, Exception err = null, string statusMessage = null, bool addToMessageHistory = true)
        {
            lock(_fileLocker)
            {
                var batchRequest = _deploymentRun.DeployBatchRequest;
                var oldStatus = batchRequest.Status;
                batchRequest.Status = status;
                switch (status)
                {
                    case EnumDeployStatus.Success:
                    case EnumDeployStatus.Error:
                        batchRequest.CompleteDateTimeUtc = DateTime.UtcNow;
                        break;
                    case EnumDeployStatus.InProcess:
                        batchRequest.ResumeRequested = false;
                        break;
                    case EnumDeployStatus.Cancelled:
                        batchRequest.CancelRequested = false;
                        break;
                }
                if (err != null)
                {
                    batchRequest.ErrorDetails = err.ToString();
                }
                if (addToMessageHistory)
                {
                    batchRequest.MessageList.Add(statusMessage);
                }
                batchRequest.LastStatusMessage = statusMessage;
                batchRequest.UpdatedDateTimeUtc = DateTime.UtcNow;
                batchRequest.UpdatedByUserName = _userIdentity.UserName;
                this.SaveRunToFile();
            }
            return _deploymentRun.DeployBatchRequest;
        }

        private void SaveRunToFile()
        {
            string runDirectory = Path.Combine(_workingDirectory, "runs");
            if(!Directory.Exists(runDirectory))
            {
                Directory.CreateDirectory(runDirectory);
            }
            string filePath = Path.Combine(runDirectory, _deploymentRun.Id.ToString() + ".json");
            string jsonData = _deploymentRun.ToJson();
            File.WriteAllText(filePath, jsonData);
        }
    }
}
