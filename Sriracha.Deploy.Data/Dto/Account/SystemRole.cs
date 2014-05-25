using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Dto.Account
{
    public enum EnumSystemRoleType
    {
        Normal = 0,
        Everyone = 1,
        Administrator = 2
    }

    public class SystemRole : BaseDto
    {
        public string RoleName { get; set; }
        public EnumSystemRoleType RoleType { get; set; }

        public SystemRolePermissions Permissions { get; set; }
        public SystemRoleAssignments Assignments { get; set; }

        public SystemRole()
        {
            this.Permissions = new SystemRolePermissions();
            this.Assignments = new SystemRoleAssignments();
        }

    }
}
