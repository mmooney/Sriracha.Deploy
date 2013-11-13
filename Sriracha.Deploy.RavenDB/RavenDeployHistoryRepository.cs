using MMDB.Shared;
using PagedList;
using Raven.Client;
using Raven.Client.Linq;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Dto.Deployment;
using Sriracha.Deploy.Data.Repository;
using Sriracha.Deploy.RavenDB.Indexes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.RavenDB
{
    public class RavenDeployHistoryRepository : IDeployHistoryRepository
    {
        private readonly IDocumentSession _documentSession;

        public RavenDeployHistoryRepository(IDocumentSession documentSession)
        {
            _documentSession = DIHelper.VerifyParameter(documentSession);
        }
        public PagedSortedList<ComponentDeployHistory> GetComponentDeployHistory(ListOptions listOptions, List<string> projectIdList, List<string> branchIdList, List<string> componentIdList, 
                                                                            List<string> buildIdList, List<string> environmentIdList, List<string> environmentNameList, 
                                                                            List<string> machineIdList, List<string> machineNameList, List<string> statusList)
        {
            var baseQuery = _documentSession.Query<DeployState>().Customize(i=>i.WaitForNonStaleResultsAsOfNow());
            RavenQueryStatistics stats;
            baseQuery = baseQuery.Statistics( out stats);
            if(projectIdList != null)
            {
                baseQuery = baseQuery.Where(i=>i.ProjectId.In(projectIdList));
            }
            if(branchIdList != null)
            {
                baseQuery = baseQuery.Where(i=>i.Build.ProjectBranchId.In(branchIdList));
            }
            if(componentIdList != null)
            {
                baseQuery = baseQuery.Where(i=>i.Build.ProjectComponentId.In(componentIdList));
            }
            if(buildIdList != null)
            {
                baseQuery = baseQuery.Where(i=>i.Build.Id == buildIdList[0]);
                //baseQuery = baseQuery.Where(i=>i.Build.Id.In(buildIdList));
            }
            if(environmentIdList != null)
            {
                baseQuery = baseQuery.Where(i=>i.Environment.Id.In(environmentIdList));
            }
            if(environmentNameList != null)
            {
                baseQuery = baseQuery.Where(i=>i.Environment.EnvironmentName.In(environmentNameList));
            }
            if(machineIdList != null)
            {
                baseQuery = baseQuery.Where(i=>i.MachineList.Any(j=>j.Id.In(machineIdList)));
            }
            if(machineNameList != null)
            {
                baseQuery = baseQuery.Where(i=>i.MachineList.Any(j=>j.MachineName.In(machineNameList)));
            }
            if(statusList != null)
            {
                List<EnumDeployStatus> statusEnumList = (from i in statusList
                                                            select EnumHelper.Parse<EnumDeployStatus>(i)).ToList();
                baseQuery = baseQuery.Where(i=>i.Status.In(statusEnumList));
            }

            var query = GetComponentDeployHistoryBaseQuery(baseQuery);
            IPagedList<ComponentDeployHistory> pagedList;
            switch(listOptions.SortField.ToLower())
            {
                case "deploymentstarteddatetimeutc":
                    pagedList = query.PageAndSort(listOptions, i=>i.DeploymentStartedDateTimeUtc);
                    break;
                case "version":
                    pagedList = query.PageAndSort(listOptions, i=>i.SortableVersion);
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
    }
}
