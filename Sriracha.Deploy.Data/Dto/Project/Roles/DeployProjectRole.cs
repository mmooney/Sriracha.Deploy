using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Dto.Project.Roles
{
	public class DeployProjectRole : BaseDto
	{
		public string ProjectId { get; set; }
		public string ProjectName { get; set; }
		public string RoleName { get; set; }
		public bool EveryoneRoleIndicator { get; set; }

		public DeployProjectRolePermissions Permissions { get; set; }
		public DeployProjectRoleAssignments Assignments { get; set; }

		public DeployProjectRole()
		{
			this.Permissions = new DeployProjectRolePermissions();
			this.Assignments = new DeployProjectRoleAssignments();
		}
	}
}
