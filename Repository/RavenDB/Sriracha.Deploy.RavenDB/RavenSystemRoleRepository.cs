using Raven.Client;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Dto.Account;
using Sriracha.Deploy.Data.Exceptions;
using Sriracha.Deploy.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.RavenDB
{
    public class RavenSystemRoleRepository : ISystemRoleRepository
    {
        private readonly IDocumentSession _documentSession;
        private readonly IUserIdentity _userIdentity;

        public RavenSystemRoleRepository(IDocumentSession documentSession, IUserIdentity userIdentity)
        {
            _documentSession = DIHelper.VerifyParameter(documentSession);
            _userIdentity = DIHelper.VerifyParameter(userIdentity);
        }

        public PagedSortedList<SystemRole> GetSystemRoleList(ListOptions listOptions)
        {
            listOptions = ListOptions.SetDefaults(listOptions, 20, 1, "RoleName", true);
            var query = _documentSession.QueryNoCache<SystemRole>();
            switch(listOptions.SortField)
            {
                case "RoleName":
                    return query.PageAndSort(listOptions, i=>i.RoleName);
                default:
                    throw new UnrecognizedSortFieldException<SystemRole>(listOptions);
            }
        }

        public SystemRole CreateSystemRole(string roleName, EnumSystemRoleType roleType, SystemRolePermissions permissions, SystemRoleAssignments assignments)
        {
            if(string.IsNullOrEmpty(roleName))
            {
                throw new ArgumentNullException("Missing role name");
            }

            permissions = permissions ?? new SystemRolePermissions();
            assignments = assignments ?? new SystemRoleAssignments();

            if(_documentSession.QueryNoCache<SystemRole>().Any(i=>i.RoleName == roleName))
            {
                throw new ArgumentException(string.Format("Role with role name {0} already exists", roleName));
            }
            string systemRoleId = Guid.NewGuid().ToString();
            var role = new SystemRole
            {
                Id = systemRoleId,
                RoleName = roleName,
                Assignments = assignments,
                Permissions = permissions,  
                RoleType = roleType
            };
            role.SetCreatedFields(_userIdentity.UserName);
            
            return _documentSession.StoreSaveEvict(role);
        }

        public SystemRole GetSystemRole(string systemRoleId)
        {
            return _documentSession.LoadEnsureNoCache<SystemRole>(systemRoleId);
        }

        public SystemRole UpdateSystemRole(string systemRoleId, string roleName, EnumSystemRoleType roleType, SystemRolePermissions permissions, SystemRoleAssignments assignments)
        {
            if(string.IsNullOrEmpty(roleName))
            {
                throw new ArgumentNullException("Missing role name");
            }
            var existingItem = _documentSession.QueryNoCacheNotStale<SystemRole>().Any(i=>i.Id != systemRoleId && i.RoleName == roleName);
            if(existingItem)
            {
                throw new ArgumentException("RoleName already exists: " + roleName);
            }
            var role = _documentSession.LoadEnsure<SystemRole>(systemRoleId);
            role.RoleName = roleName;
            role.RoleType = roleType;
            role.SetUpdatedFields(_userIdentity.UserName);
            role.Permissions = permissions ?? new SystemRolePermissions();
            role.Assignments = assignments ?? new SystemRoleAssignments();
            return _documentSession.SaveEvict(role);
        }

        public SystemRole TryGetSpecialSystemRole(EnumSystemRoleType roleType)
        {
            return _documentSession.QueryNoCacheNotStale<SystemRole>().FirstOrDefault(i=>i.RoleType == roleType);
        }

        public List<SystemRole> GetSystemRoleListForUser(string userName)
        {
            return _documentSession.QueryNoCacheNotStale<SystemRole>().Where(i => i.Assignments.UserNameList.Contains(userName)).ToList();
        }

        public SystemRole DeleteSystemRole(string systemRoleId)
        {
            var item = _documentSession.LoadEnsure<SystemRole>(systemRoleId);
            return _documentSession.DeleteSaveEvict(item);
        }
    }
}
