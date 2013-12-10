using MMDB.Shared;
using Newtonsoft.Json;
using Sriracha.Deploy.Data.Dto.Build;
using Sriracha.Deploy.Data.Dto.Deployment;
using Sriracha.Deploy.Data.Dto.Deployment.Offline;
using Sriracha.Deploy.Data.Dto.Project;
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

        public DeployProject TryGetProject(string projectId)
        {
            var projectDirectory = Path.Combine(_workingDirectory, "projects");
            var projectFilePath = Path.Combine(projectDirectory, projectId + ".json");
            if(Directory.Exists(projectDirectory) &&  File.Exists(projectFilePath))
            {
                var projectJson = File.ReadAllText(projectFilePath);
                return JsonConvert.DeserializeObject<DeployProject>(projectJson);
            }
            else 
            {
                return null;
            }
        }


        public DeployFile GetFile(string fileId)
        {
            var packageDirectory = Path.Combine(_workingDirectory, "packages");
            var filePath = Path.Combine(packageDirectory, fileId + ".json");
            if(!File.Exists(filePath))
            {
                throw new RecordNotFoundException(typeof(DeployFile), "Id", fileId);
            }
            var fileJson = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<DeployFile>(fileJson);
        }

        public byte[] GetFileData(string fileId)
        {
            var file = GetFile(fileId);
            var packageDirectory = Path.Combine(_workingDirectory, "packages");
            var binaryPath = Path.Combine(packageDirectory, fileId + ".data");
            if(!File.Exists(binaryPath))
            {
                throw new FileNotFoundException("Package file data not found", binaryPath);
            }
            return File.ReadAllBytes(binaryPath);
        }


        public DeployState TryGetDeployState(string projectId, string buildId, string environmentId, string machineId, string deployBatchRequestItemId)
        {
            string deployStateDirectory = Path.Combine(_workingDirectory, "states");
            if(Directory.Exists(deployStateDirectory))
            {
                foreach(var fileName in Directory.GetFiles(deployStateDirectory, "*.json"))
                {
                    var json = File.ReadAllText(fileName);
                    var item = JsonConvert.DeserializeObject<DeployState>(json);
                    if(item.ProjectId == projectId && item.Build.Id == buildId && item.Environment.Id == environmentId
                                && item.MachineList.Any(i=>i.Id == machineId) && item.DeployBatchRequestItemId == deployBatchRequestItemId)
                    {
                        return item;
                    }
                }
            }
            return null;
        }

        public void SaveDeployState(DeployState deployState)
        {
            string json = deployState.ToJson();

            string deployStateDirectory = Path.Combine(_workingDirectory, "states");
            if(!Directory.Exists(deployStateDirectory))
            {
                Directory.CreateDirectory(deployStateDirectory);
            }
            string filePath = Path.Combine(deployStateDirectory, deployState.Id + ".json");
            File.WriteAllText(filePath, json);
        }


        public DeployState GetDeployState(string deployStateId)
        {
            string deployStateDirectory = Path.Combine(_workingDirectory, "states");
            var filePath = Path.Combine(deployStateDirectory, deployStateId + ".json");
            if(!File.Exists(filePath))
            {
                throw new RecordNotFoundException(typeof(DeployState), "Id", deployStateId);
            }
            var json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<DeployState>(json);
        }


        public void WriteLog(string formattedMessage)
        {
            string logFileName = Path.Combine(_workingDirectory, "log.log");
            File.AppendAllLines(logFileName, formattedMessage.ListMe());
        }
    }
}
