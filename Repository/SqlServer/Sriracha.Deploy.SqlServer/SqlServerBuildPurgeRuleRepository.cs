using MMDB.Shared;
using Newtonsoft.Json;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Dto.BuildPurgeRules;
using Sriracha.Deploy.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.SqlServer
{
    public class SqlServerBuildPurgeRuleRepository : IBuildPurgeRuleRepository
    {
        [PetaPoco.TableName("DeployBuildPurgeRule")]
        private class SqlBuildPurgeRule : BuildPurgeRule
        {
            private string ToJson(List<string> list)
            {
 	            if(list == null)
                {
                    return null;
                }
                else 
                {
                    return list.ToJson();
                }
            }

            public List<string> FromJson(string value)
            {
                if(string.IsNullOrEmpty(value))
                {
                    return new List<string>();
                }
                else 
                {
                    return JsonConvert.DeserializeObject<List<string>>(value);
                }
            }

            public void SetLists(List<string> environmentIdList, List<string> environmentNameList, List<string> machineIdList, List<string> machineNameList)
            {
                this.EnvironmentIdList = environmentIdList;
                this.EnvironmentNameList = environmentNameList;
                this.MachineIdList = machineIdList;
                this.MachineNameList = machineNameList;
            }

            public BuildPurgeRule CreateDto()
            {
                return new BuildPurgeRule
                {
                    Id = this.Id,
                    ProjectId = this.ProjectId,
                    BuildRetentionMinutes = this.BuildRetentionMinutes,
                    EnvironmentIdList = this.EnvironmentIdList,
                    EnvironmentNameList = this.EnvironmentNameList,
                    MachineIdList = this.MachineIdList, 
                    MachineNameList = this.MachineNameList,
                    CreatedDateTimeUtc = this.CreatedDateTimeUtc,
                    CreatedByUserName = this.CreatedByUserName,
                    UpdatedByUserName = this.UpdatedByUserName, 
                    UpdatedDateTimeUtc = this.UpdatedDateTimeUtc
                };
            }

            new private string DisplayValue { get; set; }

            new private List<string> EnvironmentIdList { get; set; }
            public string EnvironmentIdListJson
            {
                get { return this.ToJson(this.EnvironmentIdList); }
                set { this.EnvironmentIdList = this.FromJson(value); }
            }

            new private List<string> EnvironmentNameList { get; set; }
            public string EnvironmentNameListJson
            {
                get { return this.ToJson(this.EnvironmentNameList); }
                set { this.EnvironmentNameList = this.FromJson(value); }
            }

            new private List<string> MachineIdList { get; set; }
            public string MachineIdListJson
            {
                get { return this.ToJson(this.MachineIdList); }
                set { this.MachineIdList = this.FromJson(value); }
            }

            new private List<string> MachineNameList { get; set; }
            public string MachineNameListJson
            {
                get { return this.ToJson(this.MachineNameList); }
                set { this.MachineNameList = this.FromJson(value); }
            }
        }

        private readonly ISqlConnectionInfo _sqlConnectionInfo;
        private readonly IUserIdentity _userIdentity;

        public SqlServerBuildPurgeRuleRepository(ISqlConnectionInfo sqlConnectionInfo, IUserIdentity userIdentity)
        {
            _sqlConnectionInfo = DIHelper.VerifyParameter(sqlConnectionInfo);
            _userIdentity = DIHelper.VerifyParameter(userIdentity);
        }


        private BuildPurgeRule InternalCreate(SqlBuildPurgeRule dbItem)
        {
            if(string.IsNullOrEmpty(dbItem.Id))
            {
                dbItem.Id = Guid.NewGuid().ToString();
            }
            using(var db = _sqlConnectionInfo.GetDB())
            {
                db.Insert("DeployBuildPurgeRule", "ID", dbItem);
            }
            if(string.IsNullOrEmpty(dbItem.ProjectId))
            {
                return this.GetSystemBuildPurgeRule(dbItem.Id);
            }
            else 
            {
                return this.GetProjectBuildPurgeRule(dbItem.Id, dbItem.ProjectId);
            }
        }

        private void VerifySystemRuleExists(string id)
        {
            if(string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("Missing ID");
            }
            using(var db = _sqlConnectionInfo.GetDB())
            {
                int count = db.ExecuteScalar<int>("SELECT COUNT(*) FROM DeployBuildPurgeRule WHERE ID=@0", id);
                if(count == 0)
                {
                    throw new RecordNotFoundException(typeof(BuildPurgeRule), "ID", id);
                }
            }
        }

        private void VerifyProjectRuleExists(string id, string projectId)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("Missing ID");
            }
            if(string.IsNullOrEmpty(projectId))
            {
                throw new ArgumentNullException("Missing Project ID");
            }
            using (var db = _sqlConnectionInfo.GetDB())
            {
                int count = db.ExecuteScalar<int>("SELECT COUNT(*) FROM DeployBuildPurgeRule WHERE ID=@0 AND ProjectID=@1", id, projectId);
                if (count == 0)
                {
                    throw new RecordNotFoundException(typeof(BuildPurgeRule), "ID_ProjectID", id + "_" + projectId);
                }
            }
        }

        public List<BuildPurgeRule> GetSystemBuildPurgeRuleList()
        {
            using(var db = _sqlConnectionInfo.GetDB())
            {
                return db.Query<SqlBuildPurgeRule>("WHERE ProjectID IS NULL").Select(i=>i.CreateDto()).ToList();
            }
        }

        public BuildPurgeRule CreateSystemBuildPurgeRule(int? buildRetentionMinutes, List<string> environmentNameList, List<string> environmentIdList, List<string> machineNameList, List<string> machineIdList)
        {
            var dbItem = new SqlBuildPurgeRule
            {
                ProjectId = null,
                BuildRetentionMinutes = buildRetentionMinutes,
                CreatedByUserName = _userIdentity.UserName,
                CreatedDateTimeUtc = DateTime.UtcNow,
                UpdatedByUserName = _userIdentity.UserName,
                UpdatedDateTimeUtc = DateTime.UtcNow
            };
            dbItem.SetLists(environmentIdList, environmentNameList, machineIdList, machineNameList);
            return this.InternalCreate(dbItem);
        }

        public BuildPurgeRule GetSystemBuildPurgeRule(string id)
        {
            this.VerifySystemRuleExists(id);
            using(var db = _sqlConnectionInfo.GetDB())
            {
                var item = db.FirstOrDefault<SqlBuildPurgeRule>("FROM DeployBuildPurgeRule WHERE ID=@0",id);
                if(item == null)
                {   
                    throw new RecordNotFoundException(typeof(BuildPurgeRule), "ID", id);
                }
                return item.CreateDto();
            }
        }

        public BuildPurgeRule UpdateSystemBuildPurgeRule(string id, int? buildRetentionMinutes, List<string> environmentNameList, List<string> environmentIdList, List<string> machineNameList, List<string> machineIdList)
        {
            VerifySystemRuleExists(id);
            var dbItem = new SqlBuildPurgeRule
            {
                Id = id,
                BuildRetentionMinutes = buildRetentionMinutes,
                UpdatedDateTimeUtc = DateTime.UtcNow,
                UpdatedByUserName = _userIdentity.UserName
            };
            dbItem.SetLists(environmentIdList, environmentNameList, machineIdList, machineNameList);
            using (var db = _sqlConnectionInfo.GetDB())
            {
                db.Update("DeployBuildPurgeRule", "ID", dbItem, new[] { "BuildRetentionMinutes", "UpdatedDateTimeUtc", "UpdatedByUserName", "EnvironmentIdListJson", "EnvironmentNameListJson", "MachineIdListJson", "MachineNameListJson" });
            }
            return this.GetSystemBuildPurgeRule(id);
        }

        public BuildPurgeRule DeleteSystemBuildPurgeRule(string id)
        {
            var itemToDelete = GetSystemBuildPurgeRule(id);
            using (var db = _sqlConnectionInfo.GetDB())
            {
                db.Delete<SqlBuildPurgeRule>("WHERE ID=@0", id);
            }
            return itemToDelete;
        }

        public List<BuildPurgeRule> GetProjectBuildPurgeRuleList(string projectId)
        {
            if(string.IsNullOrEmpty(projectId))
            {
                throw new ArgumentNullException("Missing Project ID");
            }
            using(var db = _sqlConnectionInfo.GetDB())
            {
                var list = db.Query<SqlBuildPurgeRule>("WHERE ProjectID=@0", projectId);
                return list.Select(i=>i.CreateDto()).ToList();
            }
        }

        public BuildPurgeRule CreateProjectBuildPurgeRule(string projectId, int? buildRetentionMinutes, List<string> environmentNameList, List<string> environmentIdList, List<string> machineNameList, List<string> machineIdList)
        {
            if(string.IsNullOrEmpty(projectId))
            {
                throw new ArgumentNullException("Missing Project ID");
            }
            var dbItem = new SqlBuildPurgeRule
            {
                ProjectId = projectId,
                BuildRetentionMinutes = buildRetentionMinutes,
                CreatedByUserName = _userIdentity.UserName,
                CreatedDateTimeUtc = DateTime.UtcNow,
                UpdatedByUserName = _userIdentity.UserName,
                UpdatedDateTimeUtc = DateTime.UtcNow
            };
            dbItem.SetLists(environmentIdList, environmentNameList, machineIdList, machineNameList);
            return this.InternalCreate(dbItem);
        }

        public BuildPurgeRule GetProjectBuildPurgeRule(string id, string projectId)
        {
            this.VerifyProjectRuleExists(id, projectId);
            using (var db = _sqlConnectionInfo.GetDB())
            {
                var item = db.FirstOrDefault<SqlBuildPurgeRule>("FROM DeployBuildPurgeRule WHERE ID=@0 AND ProjectID=@1", id, projectId);
                if (item == null)
                {
                    throw new RecordNotFoundException(typeof(BuildPurgeRule), "ID", id);
                }
                return item.CreateDto();
            }
        }

        public BuildPurgeRule UpdateProjectBuildPurgeRule(string id, string projectId, int? buildRetentionMinutes, List<string> environmentNameList, List<string> environmentIdList, List<string> machineNameList, List<string> machineIdList)
        {
            VerifyProjectRuleExists(id, projectId);
            var dbItem = new SqlBuildPurgeRule
            {
                Id = id,
                ProjectId = projectId,
                BuildRetentionMinutes = buildRetentionMinutes,
                UpdatedDateTimeUtc = DateTime.UtcNow,
                UpdatedByUserName = _userIdentity.UserName
            };
            dbItem.SetLists(environmentIdList, environmentNameList, machineIdList, machineNameList);
            using(var db = _sqlConnectionInfo.GetDB())
            {
                db.Update("DeployBuildPurgeRule", "ID", dbItem, new [] {"BuildRetentionMinutes", "UpdatedDateTimeUtc", "UpdatedByUserName", "EnvironmentIdListJson", "EnvironmentNameListJson", "MachineIdListJson", "MachineNameListJson"}  );
            }
            return this.GetProjectBuildPurgeRule(id, projectId);
        }

        public BuildPurgeRule DeleteProjectBuildPurgeRule(string id, string projectId)
        {
            var itemToDelete = GetProjectBuildPurgeRule(id, projectId);
            using(var db = _sqlConnectionInfo.GetDB())
            {
                db.Delete<SqlBuildPurgeRule>("WHERE ID=@0 AND ProjectID=@1", id, projectId);
            }
            return itemToDelete;
        }
    }
}
