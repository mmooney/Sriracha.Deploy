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

        public SystemRoleManager(ISystemRoleRepository systemRoleRepository)
        {
            _systemRoleRepository = DIHelper.VerifyParameter(systemRoleRepository);
        }

        public PagedSortedList<SystemRole> GetSystemRoleList(ListOptions listOptions)
        {
            this.EnsureEveryoneRoleExists();
            return _systemRoleRepository.GetSystemRoleList(listOptions);
        }

        public SystemRole CreateSystemRole(string roleName, bool everyoneRoleIndicator, SystemRolePermissions permissions, SystemRoleAssignments assignments)
        {
            if(everyoneRoleIndicator)
            {
                var existingEveryoneRole = _systemRoleRepository.TryGetSystemEveryoneRole();
                if (existingEveryoneRole != null)
                {
                    throw new ArgumentException("Everyone role already exists");
                }
            }
            return _systemRoleRepository.CreateSystemRole(roleName, everyoneRoleIndicator, permissions, assignments);
        }

        public SystemRole UpdateSystemRole(string systemRoleId, string roleName, bool everyoneRoleIndicator, SystemRolePermissions permissions, SystemRoleAssignments assignments)
        {
            if(everyoneRoleIndicator)
            {
                var existingEveryoneRole = _systemRoleRepository.TryGetSystemEveryoneRole();
                if(existingEveryoneRole != null && existingEveryoneRole.Id != systemRoleId)
                {
                    throw new ArgumentException("Everyone role already exists");
                }
            }
            return _systemRoleRepository.UpdateSystemRole(systemRoleId, roleName, everyoneRoleIndicator, permissions, assignments);
        }

        public SystemRole GetSystemRole(string systemRoleId)
        {
            return _systemRoleRepository.GetSystemRole(systemRoleId);
        }

        public List<SystemRole> GetSystemRoleListForUser(string userName)
        {
            var roleList = _systemRoleRepository.GetSystemRoleListForUser(userName);
            foreach (var role in roleList)
            {
                //this.ValidateRole(role);
            }
            var everyoneRole = this.EnsureEveryoneRoleExists();
            if (everyoneRole == null)
            {
                everyoneRole = CreateEveryoneRole();
            }
            //this.ValidateRole(everyoneRole);
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
            role.Permissions.ManageDeploymentCredentialsAccess = EnumPermissionAccess.Grant;
            return role;
        }

    }
}
