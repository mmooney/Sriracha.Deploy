using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Dto.Project.Roles
{
	public class DeployProjectRolePermissions
	{
		public List<DeployProjectRoleEnvironmentPermission> RequestDeployPermissionList { get; set; }
		public List<DeployProjectRoleEnvironmentPermission> ApproveRejectDeployPermissionList { get; set; }
		public List<DeployProjectRoleEnvironmentPermission> RunDeploymentPermissionList { get; set; }
		public List<DeployProjectRoleEnvironmentPermission> EditEnvironmentPermissionList { get; set; }
		public List<DeployProjectRoleEnvironmentPermission> EditEnvironmentPermissionsPermissionList { get; set; }
		public EnumPermissionAccess EditComponentConfigurationAccess { get; set; }
		public EnumPermissionAccess CreateEnvironmentAccess { get; set; }
		public EnumPermissionAccess EditProjectPermissionsAccess { get; set; }

		public DeployProjectRolePermissions ()
		{
			this.RequestDeployPermissionList = new List<DeployProjectRoleEnvironmentPermission>();
			this.ApproveRejectDeployPermissionList = new List<DeployProjectRoleEnvironmentPermission>();
			this.RunDeploymentPermissionList = new List<DeployProjectRoleEnvironmentPermission>();
			this.EditEnvironmentPermissionList = new List<DeployProjectRoleEnvironmentPermission>();
			this.EditEnvironmentPermissionsPermissionList = new List<DeployProjectRoleEnvironmentPermission>();
		}
	}
}
