using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Dto.Project.Roles
{
	public class DeployProjectRoleAssignments
	{
		public List<string> UserNameList { get; set; }

		public DeployProjectRoleAssignments()
		{
			this.UserNameList = new List<string>();
		}
	}
}
