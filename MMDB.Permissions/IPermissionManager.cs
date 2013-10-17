using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMDB.Permissions
{
	public interface IPermissionManager
	{
		List<PermissionDefinition> GetPermissionDefinitionList();
		PermissionDefinition CreatePermissionDefinition(string permissionName, string permissionDisplayValue, List<PermissionFilterDefinition> filterDefinitionList);

		PermissionGroup CreateGroup(string groupName, string parentGroupId);
		PermissionGroup DeleteGroup(string groupId);
	}
}
