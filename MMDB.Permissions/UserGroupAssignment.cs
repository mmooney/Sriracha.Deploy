using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.Permissions
{
	public class UserGroupAssignment
	{
		public string Id { get; set; }
		public string UserId { get; set; }
		public string GroupId { get; set; }
	}
}
