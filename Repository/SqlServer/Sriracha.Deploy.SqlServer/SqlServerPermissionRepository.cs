using MMDB.Shared;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Dto.Project.Roles;
using Sriracha.Deploy.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.SqlServer
{
    public class SqlServerPermissionRepository : IPermissionRepository
    {
        private readonly ISqlConnectionInfo _sqlConnectionInfo;
        private readonly IUserIdentity _userIdentity;

        [PetaPoco.TableName("DeployProjectRoleUser")]
        private class SqlDeployProjectRoleUser
        {
            public string ID { get; set; }
            public string DeployProjectRoleID { get; set; }
            public string UserName { get; set; }
        }

        [PetaPoco.TableName("DeployProjectRole")]
        private class SqlDeployProjectRole : BaseDto
        {
            public string ProjectId { get; set; }
            public string ProjectName { get; set; }
            public string RoleName { get; set; }
            public bool EveryoneRoleIndicator { get; set; }

            public string PermissionsJson { get; set; }
            [PetaPoco.Ignore]
            public List<SqlDeployProjectRoleUser> UserList { get; set; }

            public SqlDeployProjectRole()
            {
                this.UserList = new List<SqlDeployProjectRoleUser>();
            }

            public static SqlDeployProjectRole FromDto(DeployProjectRole item)
            {
                var returnValue = AutoMapper.Mapper.Map(item, new SqlDeployProjectRole());
                if(item.Permissions != null)
                {
                    returnValue.PermissionsJson = item.Permissions.ToJson();
                }
                if(item.Assignments != null && item.Assignments.UserNameList != null)
                {
                    returnValue.UserList = item.Assignments.UserNameList.Select(i=> new SqlDeployProjectRoleUser { DeployProjectRoleID=item.Id, UserName=i }).ToList();
                }
                return returnValue;
            }

            public DeployProjectRole ToDto()
            {
                var returnValue = AutoMapper.Mapper.Map(this, new DeployProjectRole());
                if(!string.IsNullOrEmpty(this.PermissionsJson))
                {
                    returnValue.Permissions = this.PermissionsJson.FromJson<DeployProjectRolePermissions>();
                }
                if(this.UserList != null)
                {
                    returnValue.Assignments = new DeployProjectRoleAssignments
                    {
                        UserNameList = this.UserList.Select(i=>i.UserName).ToList()
                    };
                }
                return returnValue;
            }
        }

        static SqlServerPermissionRepository()
        {
            AutoMapper.Mapper.CreateMap<DeployProjectRole, SqlDeployProjectRole>();
            AutoMapper.Mapper.CreateMap<SqlDeployProjectRole, DeployProjectRole>();
        }

        public SqlServerPermissionRepository(ISqlConnectionInfo sqlConnectionInfo, IUserIdentity userIdentity)
        {
            _sqlConnectionInfo = DIHelper.VerifyParameter(sqlConnectionInfo);
            _userIdentity = DIHelper.VerifyParameter(userIdentity);
        }

        private SqlDeployProjectRole GetDBItem(string projectRoleId)
        {
            if (string.IsNullOrEmpty(projectRoleId))
            {
                throw new ArgumentNullException("projectRoleID");
            }
            using(var db = _sqlConnectionInfo.GetDB())
            {
                var item = db.FirstOrDefault<SqlDeployProjectRole>("FROM DeployProjectRole WHERE ID=@0", projectRoleId);
                if(item == null)
                {
                    throw new RecordNotFoundException(typeof(DeployProjectRole), "Id", projectRoleId);
                }
                item.UserList = db.Query<SqlDeployProjectRoleUser>("FROM DeployProjectRoleUser WHERE DeployProjectRoleID=@0", projectRoleId).ToList();
                return item;
            }
        }

        public DeployProjectRole GetProjectRole(string projectRoleId)
        {
            return GetDBItem(projectRoleId).ToDto();;
        }

        public List<DeployProjectRole> GetProjectRoleList(string projectId)
        {
            if (string.IsNullOrEmpty(projectId))
            {
                throw new ArgumentNullException("projectId");
            }
            using (var db = _sqlConnectionInfo.GetDB())
            {
                var list = db.Query<SqlDeployProjectRole>("FROM DeployProjectRole WHERE ProjectID=@0", projectId).ToList();
                foreach(var item in list)
                {
                    item.UserList = db.Query<SqlDeployProjectRoleUser>("FROM DeployProjectRoleUser WHERE DeployProjectRoleID=@0", item.Id).ToList();
                }
                return list.Select(i=>i.ToDto()).ToList();
            }
        }

        public List<DeployProjectRole> GetProjectRoleListForUser(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentNullException("userName");
            }
            using (var db = _sqlConnectionInfo.GetDB())
            {
                var list = db.Query<SqlDeployProjectRole>("FROM DeployProjectRole WHERE ID IN (SELECT DeployProjectRoleID FROM DeployProjectRoleUser WHERE UserName=@0)", userName).ToList();
                foreach (var item in list)
                {
                    item.UserList = db.Query<SqlDeployProjectRoleUser>("FROM DeployProjectRoleUser WHERE DeployProjectRoleID=@0", item.Id).ToList();
                }
                return list.Select(i => i.ToDto()).ToList();
            }
        }

        public DeployProjectRole CreateProjectRole(string projectId, string projectName, string roleName, DeployProjectRolePermissions permissions, DeployProjectRoleAssignments assignments, bool everyoneRoleIndicator)
        {
            if (string.IsNullOrEmpty(projectId))
            {
                throw new ArgumentNullException("projectId");
            }
            if (string.IsNullOrEmpty(projectName))
            {
                throw new ArgumentNullException("projectName");
            }
            if (string.IsNullOrEmpty(roleName))
            {
                throw new ArgumentNullException("roleName");
            }
            using (var db = _sqlConnectionInfo.GetDB())
            {
                var count = db.ExecuteScalar<int>("SELECT COUNT(*) FROM DeployProjectRole WHERE ProjectID=@0 AND RoleName=@1", projectId, roleName);
                if (count != 0)
                {
                    throw new ArgumentException(string.Format("DeployProjectRole already exists for Project {0} and RoleName {1}", projectId, roleName));
                }
            }
            permissions = permissions ?? new DeployProjectRolePermissions();
            assignments = assignments ?? new DeployProjectRoleAssignments();
            var dbItem = new SqlDeployProjectRole
            {
                Id = Guid.NewGuid().ToString(),
                ProjectId = projectId,
                ProjectName = projectName,
                RoleName = roleName, 
                PermissionsJson = permissions.ToJson(),
                EveryoneRoleIndicator = everyoneRoleIndicator
            };
            dbItem.UserList = assignments.UserNameList.Select(i=>
                                    new SqlDeployProjectRoleUser 
                                    {
                                        ID = Guid.NewGuid().ToString(),
                                        DeployProjectRoleID = dbItem.Id,
                                        UserName = i
                                    }).ToList();
            dbItem.SetCreatedFields(_userIdentity.UserName);
            using(var db = _sqlConnectionInfo.GetDB())
            {
                db.Insert("DeployProjectRole", "ID", false, dbItem);
                foreach(var item in dbItem.UserList)
                {
                    db.Insert("DeployProjectRoleUser", "ID", false, item);
                }
            }
            return this.GetProjectRole(dbItem.Id);
        }

        public DeployProjectRole UpdateProjectRole(string roleId, string projectId, string projectName, string roleName, DeployProjectRolePermissions permissions, DeployProjectRoleAssignments assignments, bool everyoneRoleIndicator)
        {
            if (string.IsNullOrEmpty(projectId))
            {
                throw new ArgumentNullException("projectId");
            }
            if (string.IsNullOrEmpty(projectName))
            {
                throw new ArgumentNullException("projectName");
            }
            if (string.IsNullOrEmpty(roleName))
            {
                throw new ArgumentNullException("roleName");
            }
            permissions = permissions ?? new DeployProjectRolePermissions();
            assignments = assignments ?? new DeployProjectRoleAssignments();

            using(var db = _sqlConnectionInfo.GetDB())
            {
                var count = db.ExecuteScalar<int>("SELECT COUNT(*) FROM DeployProjectRole WHERE ID<>@0 AND ProjectID=@1 AND RoleName=@2", roleId, projectId, roleName);
                if(count != 0)
                {
                    throw new ArgumentException(string.Format("DeployProjectRole already exists for Project {0} and RoleName {1}", projectId, roleName));
                }
            }

            var dbItem = this.GetDBItem(roleId);
            dbItem.ProjectId = projectId;
            dbItem.ProjectName = projectName;
            dbItem.RoleName = roleName;
            dbItem.EveryoneRoleIndicator = everyoneRoleIndicator;
            dbItem.PermissionsJson = permissions.ToJson();
            dbItem.SetUpdatedFields(_userIdentity.UserName);
            
            var itemsToAdd = new List<string>();
            var itemsToDelete = new List<SqlDeployProjectRoleUser>();
            foreach(var userName in assignments.UserNameList)
            {
                if(!dbItem.UserList.Any(i=>i.UserName.Equals(userName)))
                {
                    itemsToAdd.Add(userName);
                }
            }
            foreach(var item in dbItem.UserList)
            {
                if(!assignments.UserNameList.Contains(item.UserName, StringComparer.CurrentCultureIgnoreCase))
                {
                    itemsToDelete.Add(item);
                }
            }
            using(var db = _sqlConnectionInfo.GetDB())
            {
                db.Update("DeployProjectRole", "ID", dbItem, dbItem.Id);

                foreach(var userName in itemsToAdd)
                {
                    var newItem = new SqlDeployProjectRoleUser
                    {
                        ID = Guid.NewGuid().ToString(),
                        DeployProjectRoleID = dbItem.Id,
                        UserName = userName 
                    };
                    db.Insert("DeployProjectRoleUser", "ID", false, newItem);
                }
                foreach(var item in itemsToDelete)
                {
                    db.Execute("DELETE FROM DeployProjectRoleUser WHERE ID=@0", item.ID);
                }
            }
            return this.GetProjectRole(roleId);
        }

        public DeployProjectRole TryGetProjectEveryoneRole(string projectId)
        {
            if (string.IsNullOrEmpty(projectId))
            {
                throw new ArgumentNullException("projectId");
            }
            using (var db = _sqlConnectionInfo.GetDB())
            {
                var item = db.FirstOrDefault<SqlDeployProjectRole>("FROM DeployProjectRole WHERE ProjectID=@0 AND EveryoneRoleIndicator=1", projectId);
                if (item == null)
                {
                    return null;
                }
                item.UserList = db.Query<SqlDeployProjectRoleUser>("FROM DeployProjectRoleUser WHERE DeployProjectRoleID=@0", item.Id).ToList();
                return item.ToDto();
            }
        }

        public DeployProjectRole DeleteProjectRole(string roleId)
        {
            var itemToDelete = this.GetProjectRole(roleId);
            using(var db = _sqlConnectionInfo.GetDB())
            {
                db.Execute("DELETE FROM DeployProjectRoleUser WHERE DeployProjectRoleID=@0;DELETE FROM DeployProjectRole WHERE ID=@0", roleId);
            }
            return itemToDelete;
        }

    }
}
