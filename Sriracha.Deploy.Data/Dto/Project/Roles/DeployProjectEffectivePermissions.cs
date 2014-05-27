using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Dto.Project.Roles
{
    public class DeployProjectEffectivePermissions : DeployProjectRolePermissions
    {
        public string ProjectId { get; set; }
        public string ProjectName { get; set; }
    }
}
