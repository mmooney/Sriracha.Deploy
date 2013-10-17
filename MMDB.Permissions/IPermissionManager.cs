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
		PermissionDefinition CreatePermissionDefinition(string permissionName, string permissionDisplayValue);

		PermissionGroup CreateGroup(string groupName, string parentGroupId);
		PermissionGroup DeleteGroup(string groupId);

		PermissionRole CreateRole(string roleName, List<PermissionDataAssignment> roleDataItems=null);
		List<PermissionRole> GetRoleList(PermissionDataAssignment assignment);
		PermissionRole GetRole(string roleId);
		PermissionRole UpdateRole(string roleId, string roleName, List<PermissionDataAssignment> list);
		RoleGroupAssignment AssignGroupToRole(string roleId, string groupId);
		RoleGroupAssignment TryGetRoleGroupAssignment(string roleId, string groupId);
	}
}
