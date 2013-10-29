using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Dto.Project.Roles
{
	public class DeployProjectRolePermissions
	{
		public string Id { get; set; }
		public string ProjectId { get; set; }
		public string ProjectRoleId { get; set; }

		public List<DeployProjectRoleEnvironmentPermission> RequestDeployPermissionList { get; set; }
		public List<DeployProjectRoleEnvironmentPermission> ApproveRejectDeployPermissionList { get; set; }
		public List<DeployProjectRoleEnvironmentPermission> RunDeploymentPermissionList { get; set; }
		public List<DeployProjectRoleEnvironmentPermission> EditEnvironmentPermissionList { get; set; }
		public List<DeployProjectRoleEnvironmentPermission> ManagePermissionsPermissionList { get; set; }
		public EnumPermissionAccess EditComponentConfigurationAccess { get; set; }
		public EnumPermissionAccess CreateEnvironmentAccess { get; set; }

		public string CreatedByUserName { get; set; }
		public DateTime CreatedDateTimeUtc { get; set; }
		public string UpdatedByUserName { get; set; }
		public DateTime UpdateDateTimeUtc { get; set; }

		public DeployProjectRolePermissions ()
		{
			this.RequestDeployPermissionList = new List<DeployProjectRoleEnvironmentPermission>();
			this.ApproveRejectDeployPermissionList = new List<DeployProjectRoleEnvironmentPermission>();
			this.RunDeploymentPermissionList = new List<DeployProjectRoleEnvironmentPermission>();
			this.EditEnvironmentPermissionList = new List<DeployProjectRoleEnvironmentPermission>();
			this.ManagePermissionsPermissionList = new List<DeployProjectRoleEnvironmentPermission>();
		}

		public string ProjectName { get; set; }
	}
}
