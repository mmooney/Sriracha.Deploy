using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMDB.Permissions
{
	public interface IPermissionRepository
	{
		PermissionItem CreatePermission(string permissionName, string permissionDisplayValue);

		PermissionItem GetPermission(string id);

		PermissionItem GetPermissionByName(string permissionName);
		PermissionItem TryGetPermissionByName(string permissionName);

		UserPermissionAssignment TryGetUserPermissionAssignment(string permissionId, string userId);

		List<PermissionItem> GetPermissionList();

		UserPermissionAssignment CreateUserPermissionAssignment(string permissionId, string userId, EnumPermissionAccess enumPermissionAccess);
		UserPermissionAssignment UpdateUserPermissionAssignment(string userPermissionAssignmentId, EnumPermissionAccess access);
		UserPermissionAssignment DeleteUserPermissionAssignment(string userPermissionAssignmentId);

		List<PermissionGroup> GetGroupList();
		PermissionGroup CreateGroup(string groupName, string parentGroupId);
		PermissionGroup DeleteGroup(string groupId);
		GroupPermissionAssignment CreateGroupPermissionAssignment(string permissionId, string groupId, EnumPermissionAccess enumPermissionAccess);
		GroupPermissionAssignment TryGetGroupPermissionAssignment(string permissionId, string groupId);
		GroupPermissionAssignment UpdateGroupPermissionAssignment(string groupPermissionAssignmentId, EnumPermissionAccess access);

		GroupPermissionAssignment DeleteGroupPermissionAssignment(string groupPermissionAssignmentId);
	}
}
