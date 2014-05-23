using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Dto.Account
{
    public class SystemRolePermissions
    {
        public EnumPermissionAccess EditSystemPermissionsAccess { get; set; }
        public EnumPermissionAccess EditUsersAccess { get; set; }
        public EnumPermissionAccess EditDeploymentCredentialsAccess { get; set; }
        public EnumPermissionAccess EditDeploymentToolsAccess { get; set; }
        public EnumPermissionAccess EditBuildPurgeRulesAccess { get; set; }
    }
}
