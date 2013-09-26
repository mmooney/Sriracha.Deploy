using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMDB.Permissions
{
	public class PermissionGroup
	{
		public string Id { get; set; }
		public string GroupName { get; set; }
		public string ParentGroupId { get; set; }
	}
}
