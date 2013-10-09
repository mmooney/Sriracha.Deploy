﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MMDB.Shared;

namespace MMDB.Permissions
{
    public class PermissionManager : IPermissionManager
    {
		private readonly IPermissionRepository _repository;

		public PermissionManager(IPermissionRepository repository)
		{
			_repository = repository;
		}

		public List<PermissionItem> GetPermissionList()
		{
			return _repository.GetPermissionList();
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

		public PermissionGroup DeleteGroup(string groupId)
		{
			if(string.IsNullOrEmpty(groupId))
			{
				throw new ArgumentNullException("Missing groupId");
			}
			return _repository.DeleteGroup(groupId);
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


		public bool HasPermission(string userId, string permissionId)
		{
			if(string.IsNullOrEmpty(userId))
			{
				throw new ArgumentNullException("Missing user ID");
			}
			if(string.IsNullOrEmpty(permissionId))
			{
				throw new ArgumentNullException("Missing permission ID");
			}
			var effectivePermissions = this.GetEffectiveUserPermissionList(userId);
			var effectivePermission = effectivePermissions.FirstOrDefault(i=>i.PermissionId == permissionId);
			if(effectivePermission == null)
			{
				throw new RecordNotFoundException(typeof(EffectivePermissionAssignment), "PermissionId", permissionId);
			}
			return effectivePermission.HasPermission;
		}

		public List<EffectivePermissionAssignment> GetEffectiveUserPermissionList(string userId)
		{
			var returnList = new List<EffectivePermissionAssignment>();
			var permissionList = this.GetPermissionList();
			foreach(var permission in permissionList)
			{ 
				var effectiveItem = new EffectivePermissionAssignment
				{
					UserId = userId,
					PermissionId = permission.Id
				};
				effectiveItem.UserPermissionAssignment = _repository.TryGetUserPermissionAssignment(permission.Id, userId);
				var groupList = _repository.GetUserGroupList(userId, true);
				foreach(var group in groupList)
				{
					var groupPermissionAssignment = _repository.TryGetGroupPermissionAssignment(permission.Id, group.Id);
					if(groupPermissionAssignment != null)
					{
						effectiveItem.GroupPermissionAssignmentList.Add(groupPermissionAssignment);
					}
				}
				if((effectiveItem.UserPermissionAssignment != null && effectiveItem.UserPermissionAssignment.Access == EnumPermissionAccess.Deny)
						|| effectiveItem.GroupPermissionAssignmentList.Any(i=>i.Access == EnumPermissionAccess.Deny))
				{
					effectiveItem.HasPermission = false;
				}
				else if((effectiveItem.UserPermissionAssignment != null && effectiveItem.UserPermissionAssignment.Access == EnumPermissionAccess.Grant) 
						|| effectiveItem.GroupPermissionAssignmentList.Any(i=>i.Access == EnumPermissionAccess.Grant))
				{
					effectiveItem.HasPermission = true;
				}
				else
				{
					effectiveItem.HasPermission = false;
				}
				returnList.Add(effectiveItem);
			}
			return returnList;
		}
	}
}
