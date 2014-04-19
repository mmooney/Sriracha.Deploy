using NLog;
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
                Assignments = UpdateAssignments(assignments, systemRoleId),
                Permissions = UpdatePermissions(permissions, systemRoleId),
                RoleType = roleType,
                CreatedByUserName = _userIdentity.UserName,
                CreatedDateTimeUtc = DateTime.UtcNow,
                UpdatedByUserName = _userIdentity.UserName,
                UpdateDateTimeUtc = DateTime.UtcNow
            };
            
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
            var role = _documentSession.LoadEnsure<SystemRole>(systemRoleId);
            role.RoleName = roleName;
            role.RoleType = roleType;
            role.UpdateDateTimeUtc = DateTime.UtcNow;
            role.UpdatedByUserName = _userIdentity.UserName;
            role.Permissions = UpdatePermissions(permissions, systemRoleId);
            role.Assignments = UpdateAssignments(assignments, systemRoleId);
            return _documentSession.SaveEvict(role);
        }

        public SystemRole TryGetSpecialSystemRole(EnumSystemRoleType roleType)
        {
            return _documentSession.QueryNoCache<SystemRole>().FirstOrDefault(i=>i.RoleType == roleType);
        }

        public List<SystemRole> GetSystemRoleListForUser(string userName)
        {
            return _documentSession.QueryNoCache<SystemRole>().Where(i=>i.Assignments.UserNameList.Contains(userName)).ToList();
        }

        private SystemRoleAssignments UpdateAssignments(SystemRoleAssignments assignments, string roleId)
        {
            assignments = assignments ?? new SystemRoleAssignments();
            if (string.IsNullOrEmpty(assignments.Id))
            {
                assignments.Id = Guid.NewGuid().ToString();
                assignments.CreatedByUserName = _userIdentity.UserName;
                assignments.CreatedDateTimeUtc = DateTime.UtcNow;
            }
            assignments.SystemRoleId = roleId;
            assignments.UpdatedByUserName = _userIdentity.UserName;
            assignments.UpdateDateTimeUtc = DateTime.UtcNow;
            return assignments;
        }

        private SystemRolePermissions UpdatePermissions(SystemRolePermissions permissions, string roleId)
        {
            permissions = permissions ?? new SystemRolePermissions();
            if (string.IsNullOrEmpty(permissions.Id))
            {
                permissions.Id = Guid.NewGuid().ToString();
                permissions.CreatedByUserName = _userIdentity.UserName;
                permissions.CreatedDateTimeUtc = DateTime.UtcNow;
            }
            permissions.SystemRoleId = roleId;
            permissions.UpdatedByUserName = _userIdentity.UserName;
            permissions.UpdateDateTimeUtc = DateTime.UtcNow;
            return permissions;
        }


        public SystemRole DeleteSystemRole(string systemRoleId)
        {
            var item = _documentSession.LoadEnsure<SystemRole>(systemRoleId);
            return _documentSession.DeleteSaveEvict(item);
        }
    }
}
