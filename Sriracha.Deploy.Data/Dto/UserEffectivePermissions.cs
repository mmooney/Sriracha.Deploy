using Sriracha.Deploy.Data.Dto.Project.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Dto
{
	public class UserEffectivePermissions
	{
		public string UserName { get; set;  }
		public DeployProjectRolePermissions PropertyPermission { get; set; }
	}
}
