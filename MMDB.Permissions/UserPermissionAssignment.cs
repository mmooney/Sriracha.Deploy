using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMDB.Permissions
{
	public class UserPermissionAssignment
	{
		public string Id { get; set; }
		public string PermissionId { get; set; }
		public string UserId { get; set; }
		public EnumPermissionAccess Access { get; set; }
	}
}
