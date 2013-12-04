using MMDB.Shared;
using Newtonsoft.Json;
using Sriracha.Deploy.Data;
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
            public string ComponentID { get; set; }
            public string BranchJson { get; set; }
            public string BuildJson { get; set; }
            public string EnvironmentJson { get; set; }
            public string ComponentJson { get; set; }
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
                EnvironmentJson = environment.ToJson(),
                DeployBatchRequestItemID = deployBatchRequestItemId,
                DeploymentCompleteDateTimeUtc = null,
                DeploymentStartedDateTimeUtc = null,
                ErrorDetails = null,
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
                            .Append("INSERT INTO DeployState (ID, DeployBatchRequestItemID, EnumDeployStatusID, ProjectID, BranchID, BuildID, EnvironmentID, ComponentID,")
                                            .Append("BranchJson, BuildJson, EnvironmentJson, ComponentJson, SubmittedDateTimeUtc, DeploymentStartedDateTimeUtc, DeploymentCompleteDateTimeUtc, ")
                                            .Append("ErrorDetails, CreatedByUserName, CreatedDateTimeUtc, UpdatedByUserName, UpdatedDateTimeUtc)")
                            .Append("VALUES (@ID, @DeployBatchRequestItemID, @EnumDeployStatusID, @ProjectID, @BranchID, @BuildID, @EnvironmentID, @ComponentID,", sqlDeployState)
                                .Append("@BranchJson, @BuildJson, @EnvironmentJson, @ComponentJson, @SubmittedDateTimeUtc, @DeploymentStartedDateTimeUtc, @DeploymentCompleteDateTimeUtc,", sqlDeployState)
                                .Append("@ErrorDetails, @CreatedByUserName, @CreatedDateTimeUtc, @UpdatedByUserName, @UpdatedDateTimeUtc)", sqlDeployState);
                db.Execute(deployStateSql);

                foreach(var machine in machineList)
                {
                    var sqlMachine = new SqlDeployStateMachine
                    {
                        ID = Guid.NewGuid().ToString(),
                        DeployStateID = sqlDeployState.ID,
                        MachineID = machine.Id,  
                        MachineName = machine.CreatedByUserName,
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

        private void PopulateDeployStateChildren(DeployState deployState)
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

        private DeployState PopulateDeployState(SqlDeployState item, bool loadChildren)
        {
            var returnValue = new DeployState
            {
                Id = item.ID,
                DeployBatchRequestItemId = item.DeployBatchRequestItemID,
                DeploymentStartedDateTimeUtc = item.DeploymentStartedDateTimeUtc,
                Status = (EnumDeployStatus)item.EnumDeployStatusID,
                ErrorDetails = item.ErrorDetails,
                ProjectId = item.ProjectID,
                DeploymentCompleteDateTimeUtc = item.DeploymentCompleteDateTimeUtc,
                SubmittedDateTimeUtc = item.SubmittedDateTimeUtc,
                CreatedByUserName = item.CreatedByUserName,
                CreatedDateTimeUtc = item.CreatedDateTimeUtc,
                UpdatedByUserName = item.UpdatedByUserName,
                UpdatedDateTimeUtc = item.UpdatedDateTimeUtc
            };
            returnValue.Branch = JsonConvert.DeserializeObject<DeployProjectBranch>(item.BranchJson);
            returnValue.Build = JsonConvert.DeserializeObject<DeployBuild>(item.BuildJson);
            returnValue.Component = JsonConvert.DeserializeObject<DeployComponent>(item.ComponentJson);
            returnValue.Environment = JsonConvert.DeserializeObject<DeployEnvironment>(item.EnvironmentJson);
            if(loadChildren)
            {
                PopulateDeployStateChildren(returnValue);
            }
            return returnValue;
        }

        private PetaPoco.Sql GetBaseDeployStateQuery()
        {
            return PetaPoco.Sql.Builder
                        .Append("SELECT ID, DeployBatchRequestItemID, EnumDeployStatusID, ProjectID, BranchID, BuildID, EnvironmentID, ComponentID,")
                            .Append("BranchJson, BuildJson, EnvironmentJson, ComponentJson, SubmittedDateTimeUtc, DeploymentStartedDateTimeUtc, DeploymentCompleteDateTimeUtc, ")
                            .Append("ErrorDetails, CreatedByUserName, CreatedDateTimeUtc, UpdatedByUserName, UpdatedDateTimeUtc")
                        .Append("FROM DeployState");

        }

        public DeployState TryGetDeployState(string projectId, string buildId, string environmentId, string machineId, string deployBatchRequestItemId)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public DeployStateMessage AddDeploymentMessage(string deployStateId, string message)
        {
            throw new NotImplementedException();
        }

        public DeployState UpdateDeploymentStatus(string deployStateId, Data.EnumDeployStatus enumDeployStatus, Exception err = null)
        {
            throw new NotImplementedException();
        }
    }
}
