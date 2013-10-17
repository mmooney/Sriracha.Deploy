using System;
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

		public List<PermissionDefinition> GetPermissionDefinitionList()
		{
			return _repository.GetPermissionDefinitionList();
		}

		public PermissionDefinition CreatePermissionDefinition(string permissionName, string permissionDisplayValue)
		{
			if(String.IsNullOrEmpty(permissionName))
			{ 
				throw new ArgumentNullException("Missing permissionName");
			}
			if(string.IsNullOrEmpty(permissionDisplayValue))
			{
				throw new ArgumentNullException("Missing permissionDisplayValue");
			}
			return _repository.CreatePermissionDefinition(permissionName, permissionDisplayValue);
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

		public List<PermissionRole> GetRoleList(PermissionDataAssignment assignment)
		{
			return _repository.GetRoleList(assignment);
		}

		public PermissionRole CreateRole(string roleName, List<PermissionDataAssignment> roleDataItems=null)
		{
			return _repository.CreateRole(roleName, roleDataItems);
		}

		public PermissionRole UpdateRole(string roleId, string roleName, List<PermissionDataAssignment> list)
		{
			return _repository.UpdateRole(roleId, roleName, list);
		}

		public PermissionRole GetRole(string roleId)
		{
			return _repository.GetRole(roleId);
		}

		public List<RolePermission> GetRolePermissionList(string roleId)
		{
			return _repository.GetRolePermissionList(roleId);
		}

		public RoleGroupAssignment AssignGroupToRole(string roleId, string groupId)
		{
			return _repository.AssignGroupToRole(roleId, groupId);
		}

		public RoleGroupAssignment TryGetRoleGroupAssignment(string roleId, string groupId)
		{
			return _repository.TryGetRoleGroupAssignment(roleId, groupId);
		}
	}
}
