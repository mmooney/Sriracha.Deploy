using MMDB.Shared;
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
            this.EnsureAdministratorRoleExists();
            return _systemRoleRepository.GetSystemRoleList(listOptions);
        }

        public SystemRole CreateSystemRole(string roleName, SystemRolePermissions permissions, SystemRoleAssignments assignments)
        {
            return _systemRoleRepository.CreateSystemRole(roleName, EnumSystemRoleType.Normal, permissions, assignments);
        }

        public SystemRole UpdateSystemRole(string systemRoleId, string roleName, SystemRolePermissions permissions, SystemRoleAssignments assignments)
        {
            var role = _systemRoleRepository.GetSystemRole(systemRoleId);
            return _systemRoleRepository.UpdateSystemRole(systemRoleId, roleName, role.RoleType, permissions, assignments);
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
            var everyoneRole = _systemRoleRepository.TryGetSpecialSystemRole(EnumSystemRoleType.Everyone);
            if (everyoneRole == null)
            {
                everyoneRole = CreateEveryoneRole();
                return _systemRoleRepository.CreateSystemRole(everyoneRole.RoleName, EnumSystemRoleType.Everyone, everyoneRole.Permissions, everyoneRole.Assignments);
            }
            else
            {
                return everyoneRole;
            }
        }

        private SystemRole EnsureAdministratorRoleExists()
        {
            var adminRole = _systemRoleRepository.TryGetSpecialSystemRole(EnumSystemRoleType.Administrator);
            if (adminRole == null)
            {
                adminRole = CreateAdministratorRole();
                return _systemRoleRepository.CreateSystemRole(adminRole.RoleName, EnumSystemRoleType.Administrator, adminRole.Permissions, adminRole.Assignments);
            }
            else
            {
                return adminRole;
            }
        }

        private SystemRole CreateAdministratorRole()
        {
            var role = new SystemRole
            {
                Id = "AdministratorSystemRole",
                RoleName = "Administrators",
                RoleType = EnumSystemRoleType.Administrator
            };
            role.Permissions.EditSystemPermissionsAccess = EnumPermissionAccess.Grant;
            role.Permissions.EditUsersAccess = EnumPermissionAccess.Grant;
            role.Permissions.EditDeploymentCredentialsAccess = EnumPermissionAccess.Grant;
            return role;
        }

        private SystemRole CreateEveryoneRole()
        {
            var role = new SystemRole
            {
                Id = "EveryoneSystemRole",
                RoleName = "Everyone",
                RoleType = EnumSystemRoleType.Everyone
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


        public SystemRole GetBuiltInRole(EnumSystemRoleType roleType)
        {
            switch(roleType)
            {
                case EnumSystemRoleType.Administrator:
                    return this.EnsureAdministratorRoleExists();
                case EnumSystemRoleType.Everyone:
                    return this.EnsureEveryoneRoleExists();
                default:
                    throw new UnknownEnumValueException(roleType);
            }
        }
    }
}
