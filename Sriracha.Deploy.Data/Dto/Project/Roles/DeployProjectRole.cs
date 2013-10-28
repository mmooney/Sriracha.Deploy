using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Dto.Project.Roles
{
	public class DeployProjectRole
	{
		public string Id { get; set; }
		public string ProjectId { get; set; }
		public string RoleName { get; set; }
		public bool EveryoneRoleIndicator { get; set; }

		public string CreatedByUserName { get; set; }
		public DateTime CreatedDateTimeUtc { get; set; }
		public string UpdatedByUserName { get; set; }
		public DateTime UpdateDateTimeUtc { get; set; }

		public DeployProjectRolePermissions Permissions { get; set; }
		public DeployProjectRoleAssignments Assignments { get; set; }

		public DeployProjectRole()
		{
			this.Permissions = new DeployProjectRolePermissions();
			this.Assignments = new DeployProjectRoleAssignments();
		}

	}
}
