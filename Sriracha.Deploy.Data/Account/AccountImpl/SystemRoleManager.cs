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

        public List<SystemRole> GetSystemRoleListForUser(string userName)
        {
            var roleList = _systemRoleRepository.GetSystemRoleListForUser(userName);
            foreach (var role in roleList)
            {
                //this.ValidateRole(role);
            }
            var everyoneRole = _systemRoleRepository.TryGetSystemEveryoneRole();
            if (everyoneRole == null)
            {
                everyoneRole = CreateEveryoneRole();
            }
            //this.ValidateRole(everyoneRole);
            roleList.Add(everyoneRole);

            return roleList;
        }

        private SystemRole CreateEveryoneRole()
        {
            var role = new SystemRole
            {
                Id = "Everyone",
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
