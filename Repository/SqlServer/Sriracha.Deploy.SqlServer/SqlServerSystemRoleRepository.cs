using MMDB.Shared;
using Newtonsoft.Json;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Dto.Account;
using Sriracha.Deploy.Data.Exceptions;
using Sriracha.Deploy.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.SqlServer
{
    public class SqlServerSystemRoleRepository : ISystemRoleRepository
    {
        private readonly ISqlConnectionInfo _sqlConnectionInfo;
        private readonly IUserIdentity _userIdentity;

        [PetaPoco.TableName("SystemRole")]
        private class SqlSystemRole : BaseDto
        {
            public string RoleName { get; set; }
            [PetaPoco.Column("EnumSystemRoleTypeID")]
            public EnumSystemRoleType RoleType { get; set; }

            public string PermissionsJson { get; set; }

            [PetaPoco.Ignore]
            public List<SqlSystemRoleUser> SystemRoleUserList { get; set; }

            public SqlSystemRole()
            {
                this.SystemRoleUserList = new List<SqlSystemRoleUser>();
            }

            public static SqlSystemRole FromDto(SystemRole role)
            {
                var returnValue = AutoMapper.Mapper.Map(role, new SqlSystemRole());
                if(role.Permissions != null)
                {
                    returnValue.PermissionsJson = role.Permissions.ToJson();
                }
                if (role.Assignments != null && role.Assignments.UserNameList != null)
                {
                    returnValue.SystemRoleUserList = role.Assignments.UserNameList.Select(i=>new SqlSystemRoleUser { UserName=i }).ToList();
                }
                return returnValue;
            }

            public SystemRole ToDto()
            {
                var returnValue = AutoMapper.Mapper.Map(this, new SystemRole());
                if(!string.IsNullOrEmpty(this.PermissionsJson))
                {
                    returnValue.Permissions = JsonConvert.DeserializeObject<SystemRolePermissions>(this.PermissionsJson);
                }
                if (this.SystemRoleUserList != null)
                {
                    returnValue.Assignments = new SystemRoleAssignments
                    {
                        UserNameList = this.SystemRoleUserList.Select(i=>i.UserName).ToList()
                    };
                }
                return returnValue;
            }
        }

        [PetaPoco.TableName("SystemRoleUser")]
        private class SqlSystemRoleUser
        {
            public string Id { get; set; }
            public string SystemRoleId { get; set; }
            public string UserName { get; set; }
        }

        static SqlServerSystemRoleRepository()
        {
            AutoMapper.Mapper.CreateMap<SqlSystemRole, SystemRole>();
            AutoMapper.Mapper.CreateMap<SystemRole, SqlSystemRole>();
        }

        public SqlServerSystemRoleRepository(ISqlConnectionInfo sqlConnectionInfo, IUserIdentity userIdentity)
        {
            _sqlConnectionInfo = DIHelper.VerifyParameter(sqlConnectionInfo);
            _userIdentity = DIHelper.VerifyParameter(userIdentity);
        }

        public PagedSortedList<SystemRole> GetSystemRoleList(ListOptions listOptions)
        {
            listOptions = ListOptions.SetDefaults(listOptions, 20, 1, "RoleName", true);
            switch(listOptions.SortField)
            {
                case "RoleName":
                    //Have a nice day
                    break;
                default:
                    throw new UnrecognizedSortFieldException<SystemRole>(listOptions);
            }
            using(var db = _sqlConnectionInfo.GetDB())
            {
                var dbList = db.PageAndSort<SqlSystemRole>(listOptions, new PetaPoco.Sql(""));
                return dbList.Cast<SystemRole>(i=>i.ToDto());
            }
        }

        public SystemRole CreateSystemRole(string roleName, EnumSystemRoleType roleType, SystemRolePermissions permissions, SystemRoleAssignments assignments)
        {
            if(string.IsNullOrEmpty(roleName))
            {
                throw new ArgumentNullException("roleName");
            }
            using (var db = _sqlConnectionInfo.GetDB())
            {
                var count = db.ExecuteScalar<int>("SELECT COUNT(*) FROM SystemRole WHERE RoleName=@0", roleName);
                if (count != 0)
                {
                    throw new ArgumentException("RoleName already exists: " + roleName);
                }
            }
            var item = new SqlSystemRole
            {
                Id = Guid.NewGuid().ToString(),
                RoleName = roleName, 
                RoleType = roleType,
                PermissionsJson = (permissions ?? new SystemRolePermissions()).ToJson(),
                //AssignmentsJson = (assignments ?? new SystemRoleAssignments()).ToJson()
            };
            item.SetCreatedFields(_userIdentity.UserName);
            using(var db = _sqlConnectionInfo.GetDB())
            {
                db.Insert("SystemRole", "ID", false, item);
                
                if(assignments != null && assignments.UserNameList != null)
                {
                    foreach(var userName in assignments.UserNameList)
                    {
                        var userItem = new SqlSystemRoleUser
                        {
                            Id = Guid.NewGuid().ToString(),
                            SystemRoleId = item.Id,
                            UserName = userName
                        };
                        db.Insert("SystemRoleUser", "ID", false, userItem);
                    }
                }
            }
            return this.GetSystemRole(item.Id);
        }

        public SystemRole GetSystemRole(string systemRoleId)
        {
            return this.GetDBItem(systemRoleId).ToDto();
        }

        private SqlSystemRole GetDBItem(string systemRoleId)
        {
            if(string.IsNullOrEmpty(systemRoleId))
            {
                throw new ArgumentNullException("systemRoleId");
            }
            using(var db = _sqlConnectionInfo.GetDB())
            {
                var item = db.FirstOrDefault<SqlSystemRole>("FROM SystemRole WHERE ID=@0", systemRoleId);
                if(item == null)
                {
                    throw new RecordNotFoundException(typeof(SqlSystemRole), "Id", systemRoleId);
                }
                LoadDBItemChildren(db, item);
                return item;
            }
        }

        private static void LoadDBItemChildren(PetaPoco.Database db, SqlSystemRole item)
        {
            item.SystemRoleUserList = db.Query<SqlSystemRoleUser>("FROM SystemRoleUser WHERE SystemRoleID=@0", item.Id).ToList();
        }

        public SystemRole UpdateSystemRole(string systemRoleId, string roleName, EnumSystemRoleType roleType, SystemRolePermissions permissions, SystemRoleAssignments assignments)
        {
            if(string.IsNullOrEmpty(roleName))
            {
                throw new ArgumentNullException("roleName");
            }
            using(var db = _sqlConnectionInfo.GetDB())
            {
                var count = db.ExecuteScalar<int>("SELECT COUNT(*) FROM SystemRole WHERE RoleName=@0 AND ID<>@1", roleName, systemRoleId);
                if(count != 0)
                {
                    throw new ArgumentException("RoleName already exists: " + roleName);
                }
            }
            var item = this.GetDBItem(systemRoleId);
            item.RoleName = roleName;
            item.RoleType = roleType;
            item.PermissionsJson = (permissions ?? new SystemRolePermissions()).ToJson();
            item.SetUpdatedFields(_userIdentity.UserName);
            using(var db = _sqlConnectionInfo.GetDB())
            {
                db.Update("SystemRole", "ID", item, item.Id);

                if(assignments == null || assignments.UserNameList == null || assignments.UserNameList.Count == 0)
                {
                    db.Execute("DELETE FROM SystemRoleUser WHERE SystemRoleID=@0", systemRoleId);
                }
                else 
                {
                    var itemsToAdd = new List<string>();
                    var itemsToDelete = new List<SqlSystemRoleUser>();

                    foreach(var newUserName in assignments.UserNameList)
                    {
                        if(!item.SystemRoleUserList.Any(i=>i.UserName.Equals(newUserName, StringComparison.CurrentCultureIgnoreCase)))
                        {
                            itemsToAdd.Add(newUserName);
                        }
                    }
                    foreach(var oldUser in item.SystemRoleUserList)
                    {
                        if(!assignments.UserNameList.Contains(oldUser.UserName, StringComparer.CurrentCultureIgnoreCase))
                        {
                            itemsToDelete.Add(oldUser);
                        }
                    }
                    foreach(var newUserName in itemsToAdd)
                    {
                        var dbItem = new SqlSystemRoleUser
                        {
                            Id = Guid.NewGuid().ToString(),
                            SystemRoleId = systemRoleId, 
                            UserName = newUserName
                        };
                        db.Insert("SystemRoleUser", "ID", false, dbItem);
                    }
                    foreach(var oldUser in itemsToDelete)
                    {
                        db.Execute("DELETE FROM SystemRoleUser WHERE ID=@0", oldUser.Id);
                    }
                }
            }
            return this.GetSystemRole(item.Id);
        }

        public SystemRole DeleteSystemRole(string systemRoleId)
        {
            var itemToDelete = this.GetDBItem(systemRoleId);
            using(var db = _sqlConnectionInfo.GetDB())
            {
                var sql = PetaPoco.Sql.Builder
                            .Append("DELETE FROM SystemRoleUser WHERE SystemRoleID=@0;", systemRoleId)
                            .Append("DELETE FROM SystemRole WHERE ID=@0", systemRoleId);
                db.Execute(sql);
            }
            return itemToDelete.ToDto();
        }

        public List<SystemRole> GetSystemRoleListForUser(string userName)
        {
            using (var db = _sqlConnectionInfo.GetDB())
            {
                var list  = db.Query<SqlSystemRole>("FROM SystemRole WHERE ID IN (SELECT SystemRoleID FROM SystemRoleUser WHERE UserName=@0)", userName).ToList();
                foreach(var item in list)
                {
                    LoadDBItemChildren(db, item);
                }
                return list.Select(i=>i.ToDto()).ToList();
            }
        }

        public SystemRole TryGetSpecialSystemRole(EnumSystemRoleType roleType)
        {
            using(var db = _sqlConnectionInfo.GetDB())
            {
                var item = db.FirstOrDefault<SqlSystemRole>("FROM SystemRole WHERE EnumSystemRoleTypeID=@0", roleType);
                if (item == null)
                {
                    return null;
                }
                else 
                {
                    LoadDBItemChildren(db, item);
                    return item.ToDto();
                }
            }
        }
    }
}
