using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMDB.Permissions
{
	public interface IPermissionRepository
	{
		PermissionDefinition CreatePermissionDefinition(string permissionName, string permissionDisplayValue);

		PermissionDefinition GetPermissionDefinition(string id);

		PermissionDefinition GetPermissionDefinitionByName(string permissionName);
		PermissionDefinition TryGetPermissionDefinitionByName(string permissionName);
		PermissionDefinition DeletePermissionDefinition(string id);

		List<PermissionDefinition> GetPermissionDefinitionList();

		List<PermissionGroup> GetGroupList();
		PermissionGroup GetGroup(string id);
		PermissionGroup CreateGroup(string groupName, string parentGroupId);
		PermissionGroup DeleteGroup(string groupId);

		List<PermissionGroup> GetUserGroupList(string userId, bool includeParents);
		UserGroupAssignment CreateUserGroupAssignment(string userId, string groupId);
		UserGroupAssignment TryGetUserGroupAssignment(string userId, string groupId);


		PermissionRole CreateRole(string roleName, List<PermissionDataAssignment> roleDataItems = null);
		PermissionRole GetRole(string roleId);
		RoleGroupAssignment AssignGroupToRole(string roleId, string groupId);
		RoleGroupAssignment TryGetRoleGroupAssignment(string roleId, string groupId);
	}
}
