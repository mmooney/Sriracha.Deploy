using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Dto.Project.Roles
{
	public class DeployProjectRoleAssignments
	{
		public string Id { get; set; }
		public string ProjectId { get; set; }
		public string ProjectRoleId { get; set; }
		
		public List<string> UserNameList { get; set; }

		public string CreatedByUserName { get; set; }
		public DateTime CreatedDateTimeUtc { get; set; }
		public string UpdatedByUserName { get; set; }
		public DateTime UpdateDateTimeUtc { get; set; }

		public DeployProjectRoleAssignments()
		{
			this.UserNameList = new List<string>();
		}
	}
}
