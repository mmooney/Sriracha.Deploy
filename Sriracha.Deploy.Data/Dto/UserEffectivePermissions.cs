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
		public List<DeployProjectRolePermissions> ProjectPermissionList { get; set; }

		public UserEffectivePermissions()
		{
			this.ProjectPermissionList = new List<DeployProjectRolePermissions>();
		}
	}
}
