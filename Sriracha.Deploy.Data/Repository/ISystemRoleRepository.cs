using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Dto.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Repository
{
    public interface ISystemRoleRepository
    {
        PagedSortedList<SystemRole> GetSystemRoleList(ListOptions listOptions);
        SystemRole CreateSystemRole(string roleName, bool everyoneRoleIndicator, SystemRolePermissions permissions, SystemRoleAssignments assignments);
        SystemRole GetSystemRole(string systemRoleId);
        SystemRole UpdateSystemRole(string systemRoleId, string roleName, bool everyoneRoleIndicator, SystemRolePermissions permissions, SystemRoleAssignments assignments);
        SystemRole DeleteSystemRole(string systemRoleId);

        List<SystemRole> GetSystemRoleListForUser(string userName);
        SystemRole TryGetSystemEveryoneRole();

    }
}
