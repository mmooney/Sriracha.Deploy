using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMDB.Permissions
{
    public class PermissionManager : IPermissionManager
    {
		private readonly IPermissionRepository _repository;

		public PermissionManager(IPermissionRepository repository)
		{
			_repository = repository;
		}

		public PermissionItem CreatePermission(string permissionName, string permissionDisplayValue)
		{
			if(String.IsNullOrEmpty(permissionName))
			{ 
				throw new ArgumentNullException("Missing permissionName");
			}
			if(string.IsNullOrEmpty(permissionDisplayValue))
			{
				throw new ArgumentNullException("Missing permissionDisplayValue");
			}
			return _repository.CreatePermission(permissionName, permissionDisplayValue);
		}


		public UserPermissionAssignment AssignPermissionToUser(string permissionId, string userId, EnumPermissionAccess access)
		{
			if(string.IsNullOrEmpty(permissionId))
			{
				throw new ArgumentNullException("Missing permissionId");
			}
			if(string.IsNullOrEmpty(userId))
			{
				throw new ArgumentNullException("Missing userId");
			}
			var permission = _repository.GetPermission(permissionId);
			return this.AssignPermissionToUser(permission, userId, access);
		}

		public UserPermissionAssignment AssignPermissionToUser(PermissionItem permission, string userId, EnumPermissionAccess access)
		{
			if(permission == null)
			{
				throw new ArgumentNullException("Missing permission");
			}
			if(string.IsNullOrEmpty(userId))
			{
				throw new ArgumentNullException("Missing userId");
			}
			var existingAssignment = _repository.TryGetUserPermissionAssignment(permission.Id, userId);
			if(existingAssignment != null)
			{
				return _repository.UpdateUserPermissionAssignment(existingAssignment.Id, access);
			}
			else
			{
				return _repository.CreateUserPermissionAssignment(permission.Id, userId, access);
			}
		}


		public UserPermissionAssignment DeletePermissionForUser(string permissionId, string userId)
		{
			if(string.IsNullOrEmpty(permissionId))
			{
				throw new ArgumentNullException("Missing permissionId");
			}
			if(string.IsNullOrEmpty(userId))
			{
				throw new ArgumentNullException("Missing userId");
			}
			var assignment = _repository.TryGetUserPermissionAssignment(permissionId, userId);
			return _repository.DeleteUserPermissionAssignment(assignment.Id);
		}
	}
}
