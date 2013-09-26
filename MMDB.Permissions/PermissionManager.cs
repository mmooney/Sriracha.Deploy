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
			if(assignment != null)
			{ 
				return _repository.DeleteUserPermissionAssignment(assignment.Id);
			}
			else
			{
				return null;
			}
		}

		public PermissionGroup CreateGroup(string groupName, string parentGroupId)
		{
			if(string.IsNullOrEmpty(groupName))
			{
				throw new ArgumentNullException("Missing groupName");
			}
			return _repository.CreateGroup(groupName, parentGroupId);
		}


		public GroupPermissionAssignment AssignPermissionToGroup(string permissionId, string groupId, EnumPermissionAccess access)
		{
			if(string.IsNullOrEmpty(permissionId))
			{
				throw new Exception("Missing permissionId");
			}
			if(string.IsNullOrEmpty(groupId))
			{
				throw new Exception("Missing groupId");
			}
			var existingAssignment = _repository.TryGetGroupPermissionAssignment(permissionId, groupId);
			if(existingAssignment != null)
			{
				return _repository.UpdateGroupPermissionAssignment(existingAssignment.Id, access);
			}
			else
			{
				return _repository.CreateGroupPermissionAssignment(permissionId, groupId, access);
			}
		}


		public GroupPermissionAssignment DeletePermissionForGroup(string permissionId, string groupId)
		{
			if(string.IsNullOrEmpty(permissionId))
			{
				throw new ArgumentNullException("Missing permissionId");
			}
			if(string.IsNullOrEmpty(groupId))
			{
				throw new ArgumentNullException("Missing groupId");
			}
			var assignment = _repository.TryGetGroupPermissionAssignment(permissionId, groupId);
			if(assignment != null)
			{ 
				return _repository.DeleteGroupPermissionAssignment(assignment.Id);
			}
			else
			{
				return null;
			}
		}
	}
}
