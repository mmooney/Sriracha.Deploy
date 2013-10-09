using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.Permissions
{
	public class EffectivePermissionAssignment
	{
		public string UserId { get; set ; }
		public string PermissionId { get; set; }
		public UserPermissionAssignment UserPermissionAssignment { get; set; }
		public List<GroupPermissionAssignment> GroupPermissionAssignmentList { get; set; }
		public bool HasPermission { get; set; }

		public EffectivePermissionAssignment()
		{
			this.GroupPermissionAssignmentList = new List<GroupPermissionAssignment>();
		}
	}
}
