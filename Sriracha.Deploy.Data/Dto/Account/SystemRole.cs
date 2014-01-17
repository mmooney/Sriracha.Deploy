using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Dto.Account
{
    public enum EnumSystemRoleType
    {
        Normal,
        Everyone,
        Administrator
    }

    public class SystemRole
    {
        public string Id { get; set; }
        public string RoleName { get; set; }
        public EnumSystemRoleType RoleType { get; set; }

        public SystemRolePermissions Permissions { get; set; }
        public SystemRoleAssignments Assignments { get; set; }

        public string CreatedByUserName { get; set; }
        public DateTime CreatedDateTimeUtc { get; set; }
        public string UpdatedByUserName { get; set; }
        public DateTime UpdateDateTimeUtc { get; set; }

        public SystemRole()
        {
            this.Permissions = new SystemRolePermissions();
            this.Assignments = new SystemRoleAssignments();
        }

    }
}
