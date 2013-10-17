using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Dto.Project.Roles
{
	public class DeployProjectRolePermissions
	{
		public List<DeployProjectRoleEnvironmentPermission> RequestDeployPermissionList { get; set; }

		public DeployProjectRolePermissions ()
		{
			this.RequestDeployPermissionList = new List<DeployProjectRoleEnvironmentPermission>();
		}
	}
}
