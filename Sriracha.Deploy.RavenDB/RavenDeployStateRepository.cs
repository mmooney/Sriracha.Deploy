using MMDB.Shared;
using PagedList;
using Raven.Client;
using Raven.Client.Linq;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Dto;
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

        public DeployState CreateDeployState(DeployBuild build, DeployProjectBranch branch, DeployEnvironment environment, DeployComponent component, IEnumerable<DeployMachine> machineList, string deployBatchRequestItemId)
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
            if (string.IsNullOrEmpty(buildId))
            {
                throw new ArgumentNullException("Missing build ID");
            }
            if (string.IsNullOrEmpty(environmentId))
            {
                throw new ArgumentNullException("Missing environment ID");
            }
            if(string.IsNullOrEmpty(machineId))
            {
                throw new ArgumentNullException("Missing machine ID");
            }
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

        public DeployState AddDeploymentMessage(string deployStateId, string message)
        {
            if(string.IsNullOrEmpty(message))
            {
                throw new ArgumentNullException("Missing message");
            }
            var deployStateMessage = CreateDeploymentMessage(deployStateId, message);
            var state = _documentSession.LoadEnsure<DeployState>(deployStateId);
            state.MessageList.Add(deployStateMessage);
            state.UpdatedDateTimeUtc = DateTime.UtcNow;
            state.UpdatedByUserName = _userIdentity.UserName;
            this._documentSession.SaveEvict(state);
            return state;
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

        public PagedSortedList<ComponentDeployHistory> GetComponentDeployHistory(ListOptions listOptions = null, List<string> projectIdList = null, List<string> branchIdList = null,
                                                                            List<string> componentIdList = null, List<string> buildIdList = null, List<string> environmentIdList = null,
                                                                            List<string> environmentNameList = null, List<string> machineIdList = null, List<string> machineNameList = null,
                                                                            List<string> statusList = null)
        {
            var baseQuery = _documentSession.Query<DeployState>().Customize(i => i.WaitForNonStaleResultsAsOfNow());
            RavenQueryStatistics stats;
            baseQuery = baseQuery.Statistics(out stats);
            if (projectIdList != null)
            {
                baseQuery = baseQuery.Where(i => i.ProjectId.In(projectIdList));
            }
            if (branchIdList != null)
            {
                baseQuery = baseQuery.Where(i => i.Build.ProjectBranchId.In(branchIdList));
            }
            if (componentIdList != null)
            {
                baseQuery = baseQuery.Where(i => i.Build.ProjectComponentId.In(componentIdList));
            }
            if (buildIdList != null)
            {
                //baseQuery = baseQuery.Where(i => i.Build.Id == buildIdList[0]);
                baseQuery = baseQuery.Where(i=>i.Build.Id.In(buildIdList));
            }
            if (environmentIdList != null)
            {
                baseQuery = baseQuery.Where(i => i.Environment.Id.In(environmentIdList));
            }
            if (environmentNameList != null)
            {
                baseQuery = baseQuery.Where(i => i.Environment.EnvironmentName.In(environmentNameList));
            }
            if (machineIdList != null)
            {
                baseQuery = baseQuery.Where(i => i.MachineList.Any(j => j.Id.In(machineIdList)));
            }
            if (machineNameList != null)
            {
                baseQuery = baseQuery.Where(i => i.MachineList.Any(j => j.MachineName.In(machineNameList)));
            }
            if (statusList != null)
            {
                List<EnumDeployStatus> statusEnumList = (from i in statusList
                                                         select EnumHelper.Parse<EnumDeployStatus>(i)).ToList();
                baseQuery = baseQuery.Where(i => i.Status.In(statusEnumList));
            }

            var query = GetComponentDeployHistoryBaseQuery(baseQuery);
            IPagedList<ComponentDeployHistory> pagedList;
            listOptions = ListOptions.SetDefaults(listOptions, 20, 1, "DeploymentStartedDateTimeUtc", false);
            switch (listOptions.SortField.ToLower())
            {
                case "deploymentstarteddatetimeutc":
                    pagedList = query.PageAndSort(listOptions, i => i.DeploymentStartedDateTimeUtc);
                    break;
                case "version":
                    pagedList = query.PageAndSort(listOptions, i => i.SortableVersion);
                    break;
                default:
                    throw new ArgumentException("Unrecognized sort field: " + listOptions.SortField);
            }
            return new PagedSortedList<ComponentDeployHistory>(pagedList, listOptions.SortField, listOptions.SortAscending.GetValueOrDefault());
        }

        private IRavenQueryable<ComponentDeployHistory> GetComponentDeployHistoryBaseQuery(IRavenQueryable<DeployState> baseQuery)
        {
            var query = (from i in baseQuery
                         select new ComponentDeployHistory
                         {
                             DeployStateId = i.Id,
                             DeployBatchRequestItemId = i.DeployBatchRequestItemId,
                             Status = i.Status,
                             StatusDisplayValue = i.StatusDisplayValue,
                             ErrorDetails = i.ErrorDetails,
                             DeploymentStartedDateTimeUtc = i.DeploymentStartedDateTimeUtc,
                             DeploymentCompleteDateTimeUtc = i.DeploymentCompleteDateTimeUtc,

                             ProjectId = i.Build.ProjectId,
                             ProjectName = i.Build.ProjectName,
                             ProjectComponentId = i.Component.Id,
                             ProjectComponentName = i.Component.ComponentName,
                             ProjectBranchId = i.Build.ProjectBranchId,
                             ProjectBranchName = i.Build.ProjectBranchName,

                             BuildId = i.Build.Id,
                             FileId = i.Build.FileId,
                             Version = i.Build.Version,
                             SortableVersion = i.Build.SortableVersion,

                             EnvironmentId = i.Environment.Id,
                             EnvironmentName = i.Environment.EnvironmentName,
                             MachineList = i.MachineList
                             //MachineId = i.MachineList.Select(j=>j.Id).FirstOrDefault(),
                             //MachineName = i.MachineList.Select(j => j.MachineName).FirstOrDefault()
                         });
            return query;
        }

        public List<DeployStateSummary> GetDeployStateSummaryListByDeployBatchRequestItemId(string deployBatchRequestItemId)
        {
            if(string.IsNullOrEmpty(deployBatchRequestItemId))
            {
                throw new ArgumentNullException("Missing deploy batch request item ID");
            }
#if true
            var list = (from i in _documentSession.QueryNoCache<DeployState>()
                        where i.DeployBatchRequestItemId == deployBatchRequestItemId
                        select new DeployStateSummary
                        {
                            Id = i.Id,
                            Branch = i.Branch,
                            Build = i.Build,
                            Component = i.Component,
                            CreatedByUserName = i.CreatedByUserName,
                            CreatedDateTimeUtc = i.CreatedDateTimeUtc,
                            DeployBatchRequestItemId = i.DeployBatchRequestItemId,
                            DeploymentCompleteDateTimeUtc = i.DeploymentCompleteDateTimeUtc,
                            DeploymentStartedDateTimeUtc = i.DeploymentStartedDateTimeUtc,
                            Environment = i.Environment,
                            ErrorDetails = i.ErrorDetails,
                            MachineList = i.MachineList,
                            ProjectId = i.ProjectId,
                            Status = i.Status,
                            SubmittedDateTimeUtc = i.SubmittedDateTimeUtc,
                            UpdatedByUserName = i.UpdatedByUserName,
                            UpdatedDateTimeUtc = i.UpdatedDateTimeUtc,
                            //UserName = i.UserName 
                        }).ToList();
            return list;
#else 
			var list = (from i in _documentSession.Query<DeployState>()
						where i.DeployBatchRequestItemId == deployBatchRequestItemId
						select i).ToList();
			var returnList = new List<DeployStateSummary>();
			foreach (var dbItem in list)
			{
				var returnItem = Mapper.Map(dbItem, new DeployStateSummary());
				returnList.Add(returnItem);
			}
			return returnList;
#endif
        }
    }

}
