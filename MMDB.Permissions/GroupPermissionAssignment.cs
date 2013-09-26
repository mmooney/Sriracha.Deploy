using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.Permissions
{
	public class GroupPermissionAssignment
	{
		public string Id { get; set; }
		public string PermissionId { get; set; }
		public string GroupId { get; set; }
		public EnumPermissionAccess Access { get; set; }
	}
}
