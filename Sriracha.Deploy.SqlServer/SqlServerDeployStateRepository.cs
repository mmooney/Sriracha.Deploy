using MMDB.Shared;
using Newtonsoft.Json;
using PagedList;
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

namespace Sriracha.Deploy.SqlServer
{
    public class SqlServerDeployStateRepository : IDeployStateRepository
    {
        private readonly ISqlConnectionInfo _sqlConnectionInfo;
        private readonly IUserIdentity _userIdentity;

        private class SqlDeployState
        {
            public string ID { get; set; }
            public string DeployBatchRequestItemID { get; set; }
            public int EnumDeployStatusID { get; set; }
            public string ProjectID { get; set; }
            public string BranchID { get; set; }
            public string BuildID { get; set; }
            public string EnvironmentID { get; set; }
            public string EnvironmentName { get; set; }
            public string ComponentID { get; set; }
            public string BranchJson { get; set; }
            public string BuildJson { get; set; }
            public string EnvironmentJson { get; set; }
            public string ComponentJson { get; set; }
            public string MessageListJson { get; set; }
            public string SortableVersion { get; set; }
            public DateTime SubmittedDateTimeUtc { get; set; }
            public DateTime? DeploymentStartedDateTimeUtc { get; set; }
            public DateTime? DeploymentCompleteDateTimeUtc { get; set; }
            public string ErrorDetails { get; set; }
            public string CreatedByUserName { get; set; }
            public DateTime CreatedDateTimeUtc { get; set; }
            public string UpdatedByUserName { get; set; }
            public DateTime UpdatedDateTimeUtc { get; set; }
        }

        private class SqlDeployStateMachine
        {
            public string ID { get; set; }
            public string DeployStateID { get; set; }
            public string MachineID { get; set; }
            public string MachineName { get; set; }
            public string MachineJson { get; set; }
            public string CreatedByUserName { get; set; }
            public DateTime CreatedDateTimeUtc { get; set; }
            public string UpdatedByUserName { get; set; }
            public DateTime UpdatedDateTimeUtc { get; set; }
        }

        public SqlServerDeployStateRepository(ISqlConnectionInfo sqlConnectionInfo, IUserIdentity userIdentity)
        {
            _sqlConnectionInfo = DIHelper.VerifyParameter(sqlConnectionInfo);
            _userIdentity = DIHelper.VerifyParameter(userIdentity);
        }

        private void VerifyDeployStateExists(string deployStateId)
        {
            if(string.IsNullOrEmpty(deployStateId))
            {
                throw new ArgumentNullException("Missing deploy state ID");
            }
            using(var db = _sqlConnectionInfo.GetDB())
            {
                var sql = PetaPoco.Sql.Builder.Append("SELECT COUNT(*) FROM DeployState WHERE ID=@0", deployStateId);
                int count = db.ExecuteScalar<int>(sql);
                if(count == 0)
                {
                    throw new RecordNotFoundException(typeof(DeployState), "Id", deployStateId);
                }
            }
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
            var sqlDeployState = new SqlDeployState
            {
                ID = Guid.NewGuid().ToString(),
                ProjectID = build.ProjectId,
                BranchID = branch.Id,
                BranchJson = branch.ToJson(),
                BuildID = build.Id,
                BuildJson = build.ToJson(),
                ComponentID = component.Id,
                ComponentJson = component.ToJson(),
                EnvironmentID = environment.Id,
                EnvironmentName = environment.EnvironmentName,
                EnvironmentJson = environment.ToJson(),
                DeployBatchRequestItemID = deployBatchRequestItemId,
                DeploymentCompleteDateTimeUtc = null,
                DeploymentStartedDateTimeUtc = null,
                ErrorDetails = null,
                SortableVersion = build.SortableVersion,
                EnumDeployStatusID = (int)EnumDeployStatus.NotStarted,
                CreatedByUserName = _userIdentity.UserName,
                CreatedDateTimeUtc = DateTime.UtcNow,
                UpdatedByUserName = _userIdentity.UserName,
                UpdatedDateTimeUtc = DateTime.UtcNow,
                SubmittedDateTimeUtc = DateTime.UtcNow
            };
            var branchJson = branch.ToJson();
            var buildJson = build.ToJson();
            var componentJson = component.ToJson();
            var environmentJson = environment.ToJson();
            using(var db = _sqlConnectionInfo.GetDB())
            {
                var deployStateSql = PetaPoco.Sql.Builder
                            .Append("INSERT INTO DeployState (ID, DeployBatchRequestItemID, EnumDeployStatusID, ProjectID, BranchID, BuildID, EnvironmentID, EnvironmentName, ComponentID,")
                                            .Append("BranchJson, BuildJson, EnvironmentJson, ComponentJson, SubmittedDateTimeUtc, DeploymentStartedDateTimeUtc, DeploymentCompleteDateTimeUtc, ")
                                            .Append("ErrorDetails, SortableVersion, CreatedByUserName, CreatedDateTimeUtc, UpdatedByUserName, UpdatedDateTimeUtc)")
                            .Append("VALUES (@ID, @DeployBatchRequestItemID, @EnumDeployStatusID, @ProjectID, @BranchID, @BuildID, @EnvironmentID, @EnvironmentName, @ComponentID,", sqlDeployState)
                                .Append("@BranchJson, @BuildJson, @EnvironmentJson, @ComponentJson, @SubmittedDateTimeUtc, @DeploymentStartedDateTimeUtc, @DeploymentCompleteDateTimeUtc,", sqlDeployState)
                                .Append("@ErrorDetails, @SortableVersion, @CreatedByUserName, @CreatedDateTimeUtc, @UpdatedByUserName, @UpdatedDateTimeUtc)", sqlDeployState);
                db.Execute(deployStateSql);

                foreach(var machine in machineList)
                {
                    var sqlMachine = new SqlDeployStateMachine
                    {
                        ID = Guid.NewGuid().ToString(),
                        DeployStateID = sqlDeployState.ID,
                        MachineID = machine.Id,  
                        MachineName = machine.MachineName,
                        MachineJson = machine.ToJson(),
                        CreatedByUserName = _userIdentity.UserName,
                        CreatedDateTimeUtc = DateTime.UtcNow,
                        UpdatedByUserName = _userIdentity.UserName,
                        UpdatedDateTimeUtc = DateTime.UtcNow
                    };
                    var machineSql = PetaPoco.Sql.Builder  
                            .Append("INSERT INTO DeployStateMachine (ID, DeployStateID, MachineID, MachineName, MachineJson, CreatedByUserName, CreatedDateTimeUtc, UpdatedByUserName, UpdatedDateTimeUtc)")
                            .Append("VALUES (@ID, @DeployStateID, @MachineID, @MachineName, @MachineJson, @CreatedByUserName, @CreatedDateTimeUtc, @UpdatedByUserName, @UpdatedDateTimeUtc)", sqlMachine);
                    db.Execute(machineSql);
                }
            }
            return GetDeployState(sqlDeployState.ID);
        }

        public DeployState GetDeployState(string deployStateId)
        {
            VerifyDeployStateExists(deployStateId);
            using(var db = _sqlConnectionInfo.GetDB())
            {
                var sql = GetBaseDeployStateQuery().Append("WHERE ID=@0", deployStateId);
                var item = db.SingleOrDefault<SqlDeployState>(sql);
                if(item == null)
                {
                    throw new RecordNotFoundException(typeof(DeployState), "Id", deployStateId);
                }
                var returnValue = PopulateDeployState(item, true);
                return returnValue;
            }
        }

        private void PopulateDeployStateChildren(DeployStateSummary deployState)
        {
            deployState.MachineList = GetDeployStateMachineList(deployState.Id);
        }

        private List<DeployMachine> GetDeployStateMachineList(string deployStateId)
        {
            using(var db = _sqlConnectionInfo.GetDB())
            {
                var sql = GetBaseMachineQuery().Append("WHERE DeployStateID=@0", deployStateId);
                var sqlMachineList = db.Fetch<SqlDeployStateMachine>(sql);
                var returnList = (from i in sqlMachineList
                                    select JsonConvert.DeserializeObject<DeployMachine>(i.MachineJson)
                                  ).ToList();
                return returnList;
            }
        }

        private PetaPoco.Sql GetBaseMachineQuery()
        {
            return PetaPoco.Sql.Builder
                .Append("SELECT ID, DeployStateID, MachineID, MachineName, MachineJson, CreatedByUserName, CreatedDateTimeUtc, UpdatedByUserName, UpdatedDateTimeUtc")
                .Append("FROM DeployStateMachine");
        }

        private DeployStateSummary PopulateDeployStateSummary(SqlDeployState item, bool loadChildren)
        {
            var returnValue = new DeployStateSummary();
            PopulateDeployState(item, loadChildren, returnValue);
            return returnValue;
        }

        private DeployState PopulateDeployState(SqlDeployState item, bool loadChildren)
        {
            var returnValue = new DeployState();
            PopulateDeployState(item, loadChildren, returnValue);
            return returnValue;
        }

        private void PopulateDeployState(SqlDeployState source, bool loadChildren, DeployStateSummary target)
        {
            target.Id = source.ID;
            target.DeployBatchRequestItemId = source.DeployBatchRequestItemID;
            target.DeploymentStartedDateTimeUtc = source.DeploymentStartedDateTimeUtc;
            target.Status = (EnumDeployStatus)source.EnumDeployStatusID;
            target.ErrorDetails = source.ErrorDetails;
            target.ProjectId = source.ProjectID;
            target.DeploymentCompleteDateTimeUtc = source.DeploymentCompleteDateTimeUtc;
            target.SubmittedDateTimeUtc = source.SubmittedDateTimeUtc;
            target.CreatedByUserName = source.CreatedByUserName;
            target.CreatedDateTimeUtc = source.CreatedDateTimeUtc;
            target.UpdatedByUserName = source.UpdatedByUserName;
            target.UpdatedDateTimeUtc = source.UpdatedDateTimeUtc;
            if(!string.IsNullOrEmpty(source.BranchJson))
            {
                target.Branch = JsonConvert.DeserializeObject<DeployProjectBranch>(source.BranchJson);
            }
            if(!string.IsNullOrEmpty(source.BuildJson))
            {
                target.Build = JsonConvert.DeserializeObject<DeployBuild>(source.BuildJson);
            }
            if(!string.IsNullOrEmpty(source.ComponentJson))
            {
                target.Component = JsonConvert.DeserializeObject<DeployComponent>(source.ComponentJson);
            }
            if(!string.IsNullOrEmpty(source.EnvironmentJson))
            {
                target.Environment = JsonConvert.DeserializeObject<DeployEnvironment>(source.EnvironmentJson);
            }
            if(!string.IsNullOrEmpty(source.MessageListJson) && target is DeployState)
            {
                var deployState = (DeployState)target;
                deployState.MessageList = JsonConvert.DeserializeObject<List<DeployStateMessage>>(source.MessageListJson);
            }
            if(loadChildren)
            {
                PopulateDeployStateChildren(target);
            }
        }

        private PetaPoco.Sql GetBaseDeployStateQuery(bool includeMessageList=true)
        {
            var sql = PetaPoco.Sql.Builder
                        .Append("SELECT ID, DeployBatchRequestItemID, EnumDeployStatusID, ProjectID, BranchID, BuildID, EnvironmentID, EnvironmentName,  ComponentID,")
                            .Append("BranchJson, BuildJson, EnvironmentJson, ComponentJson, SubmittedDateTimeUtc, DeploymentStartedDateTimeUtc, DeploymentCompleteDateTimeUtc, ")
                            .Append("ErrorDetails, SortableVersion, CreatedByUserName, CreatedDateTimeUtc, UpdatedByUserName, UpdatedDateTimeUtc");
            if(includeMessageList)
            {
                sql = sql.Append(", MessageListJson");
            }
            sql = sql.Append("FROM DeployState");

            return sql;
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
            using(var db = _sqlConnectionInfo.GetDB())
            {
                var sql = GetBaseDeployStateQuery().Append("WHERE BuildID=@0 AND EnvironmentID=@1", buildId, environmentId);
                var dbList = db.Fetch<SqlDeployState>(sql);
                return dbList.Select(i=>PopulateDeployState(i, true)).ToList();
            }
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
            if (string.IsNullOrEmpty(machineId))
            {
                throw new ArgumentNullException("Missing machine ID");
            }
            using (var db = _sqlConnectionInfo.GetDB())
            {
                var sql = GetBaseDeployStateQuery()
                            .Append("WHERE BuildID=@0 AND EnvironmentID=@1", buildId, environmentId)
                            .Append("AND ID IN (SELECT x.DeployStateID FROM DeployStateMachine x WHERE x.MachineID=@0)", machineId);
                var dbList = db.Fetch<SqlDeployState>(sql);
                return dbList.Select(i => PopulateDeployState(i, true)).ToList();
            }
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
            using (var db = _sqlConnectionInfo.GetDB())
            {
                var sql = GetBaseDeployStateQuery()
                            .Append("WHERE BuildID=@0 AND EnvironmentID=@1 AND DeployBatchRequestItemID=@2 AND ProjectID=@3", buildId, environmentId, deployBatchRequestItemId, projectId)
                            .Append("AND ID IN (SELECT x.DeployStateID FROM DeployStateMachine x WHERE x.MachineID=@0)", machineId);
                var dbItem = db.FirstOrDefault<SqlDeployState>(sql);
                if(dbItem == null)
                {
                    return null;
                }
                else 
                {
                    return PopulateDeployState(dbItem, true);
                }
            }
        }

        public DeployState UpdateDeploymentStatus(string deployStateId, EnumDeployStatus status, Exception err = null)
        {
            VerifyDeployStateExists(deployStateId);
            using(var db = _sqlConnectionInfo.GetDB())
            {
                var sql = PetaPoco.Sql.Builder
                                .Append("UPDATE DeployState")
                                .Append("SET EnumDeployStatusID=@0, UpdatedByUserName=@1, UpdatedDateTimeUtc=@2", (int)status, _userIdentity.UserName, DateTime.UtcNow);
                if(err != null)
                {
                    sql = sql.Append(", ErrorDetails=@0", err.ToString());
                }
                sql = sql.Append("WHERE ID=@0", deployStateId);
                db.Execute(sql);
            }
            return GetDeployState(deployStateId);
        }

        public DeployStateMessage AddDeploymentMessage(string deployStateId, string message)
        {
            if(string.IsNullOrEmpty(message))
            {
                throw new ArgumentNullException("Missing message");
            }
            VerifyDeployStateExists(deployStateId);
            var deployStateMessage = new DeployStateMessage
            {
                Id = Guid.NewGuid().ToString(),
                DeployStateId = deployStateId,
                Message = message,
                DateTimeUtc = DateTime.UtcNow,
                MessageUserName = _userIdentity.UserName
            };
            var deployState = GetDeployState(deployStateId);
            deployState.MessageList.Add(deployStateMessage);
            var messageListJson = deployState.MessageList.ToJson();
            using(var db = _sqlConnectionInfo.GetDB())
            {
                var sql = PetaPoco.Sql.Builder
                            .Append("UPDATE DeployState")
                            .Append("SET MessageListJson=@0, UpdatedByUserName=@1, UpdatedDateTimeUtc=@2", messageListJson, _userIdentity.UserName, DateTime.UtcNow)
                            .Append("WHERE ID=@0", deployStateId);
                db.Execute(sql);
            }
            return deployStateMessage;
        }



        public PagedSortedList<ComponentDeployHistory> GetComponentDeployHistory(ListOptions listOptions, List<string> projectIdList, List<string> branchIdList, List<string> componentIdList, List<string> buildIdList, List<string> environmentIdList, List<string> environmentNameList, List<string> machineIdList, List<string> machineNameList, List<string> statusList)
        {
            listOptions = ListOptions.SetDefaults(listOptions, 20, 1, "DeploymentStartedDateTimeUtc", false);
            using(var db = _sqlConnectionInfo.GetDB())
            {
                var deployStateSql = GetBaseDeployStateQuery().Append("WHERE 1=1");
                if(projectIdList != null && projectIdList.Count > 0)
                {
                    deployStateSql = deployStateSql.Append("AND ProjectID IN (@idList)", new { idList = projectIdList });
                }
                if(branchIdList != null && branchIdList.Count > 0)
                {
                    deployStateSql = deployStateSql.Append("AND BranchID IN (@idList)", new { idList = branchIdList });
                }
                if(componentIdList != null && componentIdList.Count > 0)
                {
                    deployStateSql = deployStateSql.Append("AND ComponentID IN (@idList)", new { idList = componentIdList });
                }
                if(buildIdList != null && buildIdList.Count > 0)
                {
                    deployStateSql = deployStateSql.Append("AND BuildID IN (@idList)", new { idList = buildIdList });
                }
                if(environmentIdList != null && environmentIdList.Count > 0)
                {
                    deployStateSql = deployStateSql.Append("AND EnvironmentID IN (@idList)", new { idList = environmentIdList });
                }
                if (environmentNameList != null && environmentNameList.Count > 0)
                {
                    deployStateSql = deployStateSql.Append("AND EnvironmentName IN (@nameList)", new { nameList = environmentNameList });
                }
                if (machineIdList != null && machineIdList.Count > 0)
                {
                    deployStateSql = deployStateSql.Append("AND DeployState.ID IN (SELECT x.DeployStateID FROM DeployStateMachine x WHERE x.MachineID IN (@idList))", new { idList = machineIdList });
                }
                if (machineNameList != null && machineNameList.Count > 0)
                {
                    deployStateSql = deployStateSql.Append("AND DeployState.ID IN (SELECT x.DeployStateID FROM DeployStateMachine x WHERE x.MachineName IN (@nameList))", new { nameList = machineNameList });
                }
                if (statusList != null && statusList.Count > 0)
                {
                    List<int> statusIdList = (from i in statusList
                                                             select (int)EnumHelper.Parse<EnumDeployStatus>(i)).ToList();
                    deployStateSql = deployStateSql.Append("AND EnumDeployStatusID IN (@statusIdList)", new { statusIdList = statusIdList });
                }
                if (!string.IsNullOrEmpty(listOptions.SortField))
                {
                    switch(listOptions.SortField.ToLower())
                    {
                        case "deploymentstarteddatetimeutc":
                            //OK
                            break;
                        case "version":
                            listOptions.SortField = "SortableVersion";
                            break;
                        default:
                            throw new ArgumentException("Unrecognized sort field: " + listOptions.SortField);
                    }
                }
                var dbStateList = db.PageAndSort<SqlDeployState>(listOptions, deployStateSql);

                var idList = dbStateList.Items.Select(i=>i.ID);
                var dbMachineList = new List<SqlDeployStateMachine>();
                if(idList.Count() > 0)
                {
                    var machineSql = GetBaseMachineQuery().Append("WHERE DeployStateID IN (@deployStateIdList)", new { deployStateIdList=idList });
                    dbMachineList = db.Fetch<SqlDeployStateMachine>(machineSql).ToList();
                }
                var castedList = (from i in dbStateList.Items
                                    select PopulateComponentDeployHistory(i, dbMachineList)
                                 ).ToList();
                var castedPagedList = new StaticPagedList<ComponentDeployHistory>(castedList, dbStateList.PageNumber, dbStateList.PageSize, dbStateList.TotalItemCount);
                return new PagedSortedList<ComponentDeployHistory>(castedPagedList, listOptions.SortField, listOptions.SortAscending.GetValueOrDefault());
            }
        }

        private ComponentDeployHistory PopulateComponentDeployHistory(SqlDeployState dbState, List<SqlDeployStateMachine> dbMachineList)
        {
            DeployBuild build = null;
            if(!string.IsNullOrEmpty(dbState.BuildJson))
            {
                build = JsonConvert.DeserializeObject<DeployBuild>(dbState.BuildJson);
            }
            DeployEnvironment environment = null;
            if(!string.IsNullOrEmpty(dbState.EnvironmentJson))
            {
                environment = JsonConvert.DeserializeObject<DeployEnvironment>(dbState.EnvironmentJson);
            }
            DeployProjectBranch branch = null;
            if(!string.IsNullOrEmpty(dbState.BranchJson))
            {
                branch = JsonConvert.DeserializeObject<DeployProjectBranch>(dbState.BranchJson);
            }
            DeployComponent component = null;
            if(!string.IsNullOrEmpty(dbState.ComponentJson))
            {
                component = JsonConvert.DeserializeObject<DeployComponent>(dbState.ComponentJson);
            }
            var machineList = (from m in dbMachineList
                                where m.DeployStateID == dbState.ID && !string.IsNullOrEmpty(m.MachineJson)
                                select JsonConvert.DeserializeObject<DeployMachine>(m.MachineJson)
                              ).ToList();

            return new ComponentDeployHistory
            {
                DeployStateId = dbState.ID,
                DeployBatchRequestItemId = dbState.DeployBatchRequestItemID,
                ProjectId = dbState.ProjectID, 
                ProjectName = (build != null)
                                ? build.ProjectName
                                : null,
                BuildId = dbState.BuildID,
                ProjectBranchId = dbState.BranchID,
                ProjectBranchName = (branch != null)
                                    ? branch.BranchName
                                    : null,
                ProjectComponentId = dbState.ComponentID,
                ProjectComponentName = (component != null)
                                        ? component.ComponentName
                                        : null,
                Status = (EnumDeployStatus)dbState.EnumDeployStatusID,
                StatusDisplayValue = EnumHelper.GetDisplayValue((EnumDeployStatus)dbState.EnumDeployStatusID),
                EnvironmentId = dbState.EnvironmentID,
                EnvironmentName = (environment != null) 
                                    ? environment.EnvironmentName 
                                    : null,
                DeploymentStartedDateTimeUtc = dbState.DeploymentStartedDateTimeUtc,
                DeploymentCompleteDateTimeUtc = dbState.DeploymentCompleteDateTimeUtc,
                FileId = (build != null)
                            ? build.FileId
                            : null,
                Version = (build != null)
                            ? build.Version
                            : null,
                SortableVersion = (build != null)
                            ? DeployBuild.GetSortableVersion(build.Version)
                            : null,
                MachineList = machineList,
                ErrorDetails = dbState.ErrorDetails,
            };
        }


        public List<DeployStateSummary> GetDeployStateSummaryListByDeployBatchRequestItemId(string deployBatchRequestItemId)
        {
            if(string.IsNullOrEmpty(deployBatchRequestItemId))
            {
                throw new ArgumentNullException("Missing deploy batch request item ID");
            }
            using(var db = _sqlConnectionInfo.GetDB())
            {
                var sql = GetBaseDeployStateQuery(false).Append("WHERE DeployBatchRequestItemID=@0", deployBatchRequestItemId);
                var dbList = db.Fetch<SqlDeployState>(sql);
                return dbList.Select(i => PopulateDeployStateSummary(i, true)).ToList();
            }
        }

    }
}
