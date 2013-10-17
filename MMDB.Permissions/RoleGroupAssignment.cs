using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMDB.Permissions
{
	public class RoleGroupAssignment
	{
		public string Id { get; set; }
		public string RoleId { get; set; }
		public string GroupId { get; set; }
	}
}
