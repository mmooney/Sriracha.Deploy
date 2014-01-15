using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Dto.Account
{
    public class SystemRolePermissions
    {
        public string Id { get; set; }
        public string SystemRoleId { get; set; }

        public EnumPermissionAccess EditSystemPermissionsAccess { get; set; }
        public EnumPermissionAccess EditUsersAccess { get; set; }
        public EnumPermissionAccess EditDeploymentCredentialsAccess { get; set; }

        public string CreatedByUserName { get; set; }
        public DateTime CreatedDateTimeUtc { get; set; }
        public string UpdatedByUserName { get; set; }
        public DateTime UpdateDateTimeUtc { get; set; }
    }
}
