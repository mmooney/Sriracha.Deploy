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

		public PermissionDefinition CreatePermissionDefinition(string permissionName, string permissionDisplayValue, List<PermissionFilterDefinition> filterDefinitionList)
		{
			if(String.IsNullOrEmpty(permissionName))
			{ 
				throw new ArgumentNullException("Missing permissionName");
			}
			if(string.IsNullOrEmpty(permissionDisplayValue))
			{
				throw new ArgumentNullException("Missing permissionDisplayValue");
			}
			return _repository.CreatePermissionDefinition(permissionName, permissionDisplayValue, filterDefinitionList);
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
	}
}
