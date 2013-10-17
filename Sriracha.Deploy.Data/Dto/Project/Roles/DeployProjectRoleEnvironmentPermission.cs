using MMDB.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Dto.Project.Roles
{
	public class DeployProjectRoleEnvironmentPermission
	{
		public string Id { get; set; }
		public string ProjectId { get; set; }
		public string ProjectRoleId { get; set; }
		public string EnvironmentId { get; set; }
		public string EnvironmentName { get; set; }
		public EnumPermissionAccess Access { get; set; }
	}
}
