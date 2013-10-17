using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMDB.Permissions
{
	public class PermissionRole
	{
		public string Id { get; set; }
		public List<PermissionDataAssignment> DataAssignmentList { get; set; }

		public PermissionRole()
		{
			this.DataAssignmentList = new List<PermissionDataAssignment>();
		}
	}
}
