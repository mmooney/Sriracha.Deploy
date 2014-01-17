using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Dto.Account;
using Sriracha.Deploy.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Account.AccountImpl
{
    public class SystemRoleManager : ISystemRoleManager
    {
        private readonly ISystemRoleRepository _systemRoleRepository;
        private readonly IMembershipRepository _membershipRepository;

        public SystemRoleManager(ISystemRoleRepository systemRoleRepository, IMembershipRepository membershipRepository)
        {
            _systemRoleRepository = DIHelper.VerifyParameter(systemRoleRepository);
            _membershipRepository = DIHelper.VerifyParameter(membershipRepository);
        }

        public PagedSortedList<SystemRole> GetSystemRoleList(ListOptions listOptions)
        {
            this.EnsureEveryoneRoleExists();
            return _systemRoleRepository.GetSystemRoleList(listOptions);
        }

        public SystemRole CreateSystemRole(string roleName, SystemRolePermissions permissions, SystemRoleAssignments assignments)
        {
            return _systemRoleRepository.CreateSystemRole(roleName, false, permissions, assignments);
        }

        public SystemRole UpdateSystemRole(string systemRoleId, string roleName, SystemRolePermissions permissions, SystemRoleAssignments assignments)
        {
            var role = _systemRoleRepository.GetSystemRole(systemRoleId);
            return _systemRoleRepository.UpdateSystemRole(systemRoleId, roleName, role.EveryoneRoleIndicator, permissions, assignments);
        }

        public SystemRole GetSystemRole(string systemRoleId)
        {
            return _systemRoleRepository.GetSystemRole(systemRoleId);
        }

        public List<SystemRole> GetSystemRoleListForUser(string userName)
        {
            var roleList = _systemRoleRepository.GetSystemRoleListForUser(userName);
            var everyoneRole = this.EnsureEveryoneRoleExists();
            if (everyoneRole == null)
            {
                everyoneRole = CreateEveryoneRole();
            }
            roleList.Add(everyoneRole);

            return roleList;
        }

        public List<SystemRole> GetSystemRoleListForUserId(string userId)
        {
            var user = _membershipRepository.GetUser(userId);
            var roleList = _systemRoleRepository.GetSystemRoleListForUser(user.UserName);
            var everyoneRole = this.EnsureEveryoneRoleExists();
            if (everyoneRole == null)
            {
                everyoneRole = CreateEveryoneRole();
            }
            roleList.Add(everyoneRole);

            return roleList;
        }

        private SystemRole EnsureEveryoneRoleExists()
        {
            var everyoneRole = _systemRoleRepository.TryGetSystemEveryoneRole();
            if(everyoneRole == null)
            {
                everyoneRole = CreateEveryoneRole();
                return _systemRoleRepository.CreateSystemRole(everyoneRole.RoleName, everyoneRole.EveryoneRoleIndicator, everyoneRole.Permissions, everyoneRole.Assignments);
            }
            else 
            {
                return everyoneRole;
            }
        }

        private SystemRole CreateEveryoneRole()
        {
            var role = new SystemRole
            {
                Id = "EveryoneSystemRole",
                RoleName = "Everyone",
                EveryoneRoleIndicator = true
            };
            role.Permissions.EditSystemPermissionsAccess = EnumPermissionAccess.Grant;
            role.Permissions.EditUsersAccess = EnumPermissionAccess.Grant;
            role.Permissions.EditDeploymentCredentialsAccess = EnumPermissionAccess.Grant;
            return role;
        }

        public SystemRole DeleteSystemRole(string systemRoleId)
        {
            return _systemRoleRepository.DeleteSystemRole(systemRoleId);
        }
    }
}
