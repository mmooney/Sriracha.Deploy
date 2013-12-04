using MMDB.Shared;
using Raven.Client;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Dto.Build;
using Sriracha.Deploy.Data.Dto.Deployment;
using Sriracha.Deploy.Data.Dto.Project;
using Sriracha.Deploy.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.RavenDB
{
    public class RavenDeployStateRepository : IDeployStateRepository
    {
        private readonly IDocumentSession _documentSession;
        private readonly IUserIdentity _userIdentity;

        public RavenDeployStateRepository(IDocumentSession documentSession, IUserIdentity userIdentity)
        {
            _documentSession = DIHelper.VerifyParameter(documentSession);
            _userIdentity = DIHelper.VerifyParameter(userIdentity);
        }

        public DeployState CreateDeployment(DeployBuild build, DeployProjectBranch branch, DeployEnvironment environment, DeployComponent component, IEnumerable<DeployMachine> machineList, string deployBatchRequestItemId)
        {
            if (build == null)
            {
                throw new ArgumentNullException("Missing build");
            }
            if (branch == null)
            {
                throw new ArgumentNullException("Missing branch");
            }
            if (component == null)
            {
                throw new ArgumentNullException("Missing component");
            }
            if (environment == null)
            {
                throw new ArgumentNullException("Missing environment");
            }
            if (machineList == null)
            {
                throw new ArgumentNullException("Missing machineList");
            }
            if (deployBatchRequestItemId == null)
            {
                throw new ArgumentNullException("Missing deployBatchRequestItemId");
            }
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
            _documentSession.StoreSaveEvict(deployState);
            return deployState;
        }

        public DeployState GetDeployState(string deployStateId)
        {
            if (string.IsNullOrWhiteSpace(deployStateId))
            {
                throw new ArgumentNullException("Missing Deploy State ID");
            }
            var returnValue = _documentSession.LoadNoCache<DeployState>(deployStateId);
            if (returnValue == null)
            {
                throw new RecordNotFoundException(typeof(DeployState), "Id", deployStateId);
            }
            return returnValue;
        }

        public List<DeployState> FindDeployStateListForEnvironment(string buildId, string environmentId)
        {
            if(string.IsNullOrEmpty(buildId))
            {
                throw new ArgumentNullException("Missing build ID");
            }
            if(string.IsNullOrEmpty(environmentId))
            {
                throw new ArgumentNullException("Missing environment ID");
            }
            return _documentSession.QueryNoCache<DeployState>().Where(i => i.Build.Id == buildId && i.Environment.Id == environmentId).ToList();
        }


        public List<DeployState> FindDeployStateListForMachine(string buildId, string environmentId, string machineId)
        {
            var tempList = _documentSession.QueryNoCache<DeployState>().Where(i => i.Build.Id == buildId && i.Environment.Id == environmentId).ToList();
            return tempList.Where(i => i.MachineList.Any(j => j.Id == machineId)).ToList(); ;
        }

        public DeployState TryGetDeployState(string projectId, string buildId, string environmentId, string machineId, string deployBatchRequestItemId)
        {
            if (string.IsNullOrEmpty(projectId))
            {
                throw new ArgumentNullException("Missing project ID");
            }
            if (string.IsNullOrEmpty(buildId))
            {
                throw new ArgumentNullException("Missing build ID");
            }
            if (string.IsNullOrEmpty(environmentId))
            {
                throw new ArgumentNullException("Missing environment ID");
            }
            if (string.IsNullOrEmpty(machineId))
            {
                throw new ArgumentNullException("Missing machine ID");
            }
            if (string.IsNullOrEmpty(deployBatchRequestItemId))
            {
                throw new ArgumentNullException("Missing deploy batch request item ID");
            }
            return (from i in _documentSession.QueryNoCache<DeployState>()
                    where i.ProjectId == projectId
                        && i.Build.Id == buildId
                        && i.Environment.Id == environmentId
                        && i.MachineList.Any(j => j.Id == machineId)
                        && i.DeployBatchRequestItemId == deployBatchRequestItemId
                    select i)
                    .FirstOrDefault();
        }

        public DeployState UpdateDeploymentStatus(string deployStateId, EnumDeployStatus status, Exception err = null)
        {
            var state = _documentSession.LoadEnsure<DeployState>(deployStateId);
            if (state.MessageList == null || state.MessageList.Count == 0)
            {
                _documentSession.Advanced.Evict(state);
                state = _documentSession.LoadEnsure<DeployState>(deployStateId);
            }
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
                state.ErrorDetails = err.ToString();
            }
            state.UpdatedDateTimeUtc = DateTime.UtcNow;
            state.UpdatedByUserName = _userIdentity.UserName;
            this._documentSession.SaveEvict(state);
            return state;
        }

        public DeployStateMessage AddDeploymentMessage(string deployStateId, string message)
        {
            var deployStateMessage = CreateDeploymentMessage(deployStateId, message);
            var state = _documentSession.LoadEnsure<DeployState>(deployStateId);
            state.MessageList.Add(deployStateMessage);
            this._documentSession.SaveEvict(state);
            return deployStateMessage;
        }

        private DeployStateMessage CreateDeploymentMessage(string deployStateId, string message)
        {
            var deployStateMessage = new DeployStateMessage
            {
                Id = Guid.NewGuid().ToString(),
                DeployStateId = deployStateId,
                Message = message,
                DateTimeUtc = DateTime.UtcNow,
                MessageUserName = _userIdentity.UserName
            };
            return deployStateMessage;
        }
    }
}
