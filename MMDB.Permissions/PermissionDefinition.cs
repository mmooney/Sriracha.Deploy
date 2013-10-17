using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.Permissions
{
	public class PermissionDefinition
	{
		public string Id { get; set; }
		public string PermissionName { get; set; }
		public string PermissionDisplayValue { get; set; }
		public List<PermissionFilterDefinition> FilterDefinitionList { get; set; }
	}
}
