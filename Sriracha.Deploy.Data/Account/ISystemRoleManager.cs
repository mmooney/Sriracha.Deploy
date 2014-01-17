using Sriracha.Deploy.Data.Dto.Account;
using Sriracha.Deploy.Data.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Account
{
    public interface ISystemRoleManager
    {
        PagedSortedList<SystemRole> GetSystemRoleList(ListOptions listOptions);
        SystemRole CreateSystemRole(string roleName, SystemRolePermissions permissions, SystemRoleAssignments assignments);
        SystemRole GetSystemRole(string systemRoleId);
        SystemRole UpdateSystemRole(string systemRoleId, string roleName, SystemRolePermissions permissions, SystemRoleAssignments assignments);
        SystemRole DeleteSystemRole(string systemRoleId);

        List<SystemRole> GetSystemRoleListForUser(string userName);
        List<SystemRole> GetSystemRoleListForUserId(string userId);
    }
}
