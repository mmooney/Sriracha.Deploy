using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Dto.Build;
using Sriracha.Deploy.Data.Dto.Deployment;
using Sriracha.Deploy.Data.Dto.Project;
using Sriracha.Deploy.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Deployment.Offline.OfflineImpl
{
    public class OfflineDeployStateRepository : IDeployStateRepository
    {
        private readonly IOfflineDataProvider _offlineDataProvider;
        private readonly IUserIdentity _userIdentity;

        public OfflineDeployStateRepository(IOfflineDataProvider offlineDataProvider, IUserIdentity userIdentity)
        {
            _offlineDataProvider = DIHelper.VerifyParameter(offlineDataProvider);
            _userIdentity = DIHelper.VerifyParameter(userIdentity);
        }

        public DeployState CreateDeployState(DeployBuild build, DeployProjectBranch branch, DeployEnvironment environment, DeployComponent component, IEnumerable<DeployMachine> machineList, string deployBatchRequestItemId)
        {
            var deployState = new DeployState
            {
                Id = Guid.NewGuid().ToString(),
                ProjectId = environment.ProjectId,
                Build = build,
                Branch = branch,
                Environment = environment,
                Component = component,
                MachineList = machineList.ToList(),
                Status = EnumDeployStatus.NotStarted,
                SubmittedDateTimeUtc = DateTime.UtcNow,
                DeployBatchRequestItemId = deployBatchRequestItemId,
                CreatedDateTimeUtc = DateTime.UtcNow,
                CreatedByUserName = _userIdentity.UserName,
                UpdatedDateTimeUtc = DateTime.UtcNow,
                UpdatedByUserName = _userIdentity.UserName
            };
            _offlineDataProvider.SaveDeployState(deployState);
            return deployState;
        }

        public DeployState TryGetDeployState(string projectId, string buildId, string environmentId, string machineId, string deployBatchRequestItemId)
        {
            return _offlineDataProvider.TryGetDeployState(projectId, buildId, environmentId, machineId, deployBatchRequestItemId);
        }

        public DeployState GetDeployState(string deployStateId)
        {
            return _offlineDataProvider.GetDeployState(deployStateId);
        }

        public List<DeployState> FindDeployStateListForEnvironment(string buildId, string environmentId)
        {
            throw new NotImplementedException();
        }

        public List<DeployState> FindDeployStateListForMachine(string buildId, string environmentId, string machineId)
        {
            throw new NotImplementedException();
        }

        public DeployState AddDeploymentMessage(string deployStateId, string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentNullException("Missing message");
            }
            var deployStateMessage = new DeployStateMessage
            {
                Id = Guid.NewGuid().ToString(),
                DeployStateId = deployStateId,
                Message = message,
                DateTimeUtc = DateTime.UtcNow,
                MessageUserName = _userIdentity.UserName
            };

            var state = _offlineDataProvider.GetDeployState(deployStateId);
            state.MessageList.Add(deployStateMessage);
            state.UpdatedDateTimeUtc = DateTime.UtcNow;
            state.UpdatedByUserName = _userIdentity.UserName;
            _offlineDataProvider.SaveDeployState(state);
            return state;
        }

        public DeployState UpdateDeploymentStatus(string deployStateId, EnumDeployStatus status, Exception err = null)
        {
            var state = _offlineDataProvider.GetDeployState(deployStateId);
            state.Status = status;
            switch (status)
            {
                case EnumDeployStatus.Success:
                case EnumDeployStatus.Error:
                    state.DeploymentCompleteDateTimeUtc = DateTime.UtcNow;
                    break;
            }
            if (err != null)
            {
                var message = new DeployStateMessage
                {
                    Id = Guid.NewGuid().ToString(),
                    DateTimeUtc = DateTime.UtcNow,
                    DeployStateId = state.Id,
                    Message = "ERROR: " + err.ToString(),
                    MessageUserName = _userIdentity.UserName
                };
                state.MessageList.Add(message);
                state.ErrorDetails = err.ToString();
            }
            state.UpdatedDateTimeUtc = DateTime.UtcNow;
            state.UpdatedByUserName = _userIdentity.UserName;
            _offlineDataProvider.SaveDeployState(state);
            return state;
        }

        public PagedSortedList<ComponentDeployHistory> GetComponentDeployHistory(Dto.ListOptions listOptions = null, List<string> projectIdList = null, List<string> branchIdList = null, List<string> componentIdList = null, List<string> buildIdList = null, List<string> environmentIdList = null, List<string> environmentNameList = null, List<string> machineIdList = null, List<string> machineNameList = null, List<string> statusList = null)
        {
            throw new NotImplementedException();
        }

        public List<DeployStateSummary> GetDeployStateSummaryListByDeployBatchRequestItemId(string deployBatchRequestItemId)
        {
            throw new NotImplementedException();
        }
    }
}
