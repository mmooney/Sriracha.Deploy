using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMDB.Permissions
{
	public interface IPermissionManager
	{
		PermissionItem CreatePermission(string permissionName, string permissionDisplayValue);

		UserPermissionAssignment AssignPermissionToUser(string permissionId, string userId, EnumPermissionAccess access);
		UserPermissionAssignment AssignPermissionToUser(PermissionItem permissionItem, string userId, EnumPermissionAccess access);

		UserPermissionAssignment DeletePermissionForUser(string permissionId, string userId);
	}
}
