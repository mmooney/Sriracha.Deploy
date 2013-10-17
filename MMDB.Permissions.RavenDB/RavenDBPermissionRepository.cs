using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MMDB.Shared;
using Raven.Client;

namespace MMDB.Permissions.RavenDB
{
	public class RavenDBPermissionRepository : IPermissionRepository
	{
		private readonly IDocumentSession _documentSession;

		public RavenDBPermissionRepository(IDocumentSession documentSession)
		{
			_documentSession = documentSession;
		}

		public PermissionDefinition CreatePermissionDefinition(string permissionName, string permissionDisplayValue)
		{
			if(string.IsNullOrEmpty(permissionName))
			{
				throw new ArgumentNullException("Missing permissionName");
			}
			if(string.IsNullOrEmpty(permissionDisplayValue))
			{
				throw new ArgumentNullException("Missing permissionDisplayValue");
			}
			var existingItem = _documentSession.Query<PermissionDefinition>().FirstOrDefault(i=>i.PermissionName == permissionName);
			if(existingItem != null)
			{
				throw new ArgumentException(string.Format("Permission with name \"{0}\" already exists", permissionName));
			}
			var item = new PermissionDefinition
			{
				Id = Guid.NewGuid().ToString(),
				PermissionName = permissionName,
				PermissionDisplayValue = permissionDisplayValue
			};
			_documentSession.Store(item);
			_documentSession.SaveChanges();
			return item;
		}

		public PermissionDefinition GetPermissionDefinition(string id)
		{
			if(string.IsNullOrEmpty(id))
			{
				throw new ArgumentNullException("Missing id");
			}
			var item = _documentSession.Load<PermissionDefinition>(id);
			if(item == null)
			{
				throw new RecordNotFoundException(typeof(PermissionDefinition), "Id", id);
			}
			return item;
		}

		public PermissionDefinition GetPermissionDefinitionByName(string permissionName)
		{
			if(string.IsNullOrEmpty(permissionName))
			{
				throw new ArgumentNullException("Missing permissionName");
			}
			var returnValue = TryGetPermissionDefinitionByName(permissionName);
			if(returnValue == null)
			{
				throw new RecordNotFoundException(typeof(PermissionDefinition), "PermissionName", permissionName);
			}
			return returnValue;
		}

		public PermissionDefinition TryGetPermissionDefinitionByName(string permissionName)
		{
			if(string.IsNullOrEmpty(permissionName))
			{
				throw new ArgumentNullException("Missing permissionName");
			}
			return _documentSession.Query<PermissionDefinition>().FirstOrDefault(i=>i.PermissionName == permissionName);
		}

		public List<PermissionDefinition> GetPermissionDefinitionList()
		{
			return this._documentSession.Query<PermissionDefinition>().ToList();
		}

		public PermissionDefinition DeletePermissionDefinition(string id)
		{
			var item = this.GetPermissionDefinition(id);
			_documentSession.Delete(item);
			_documentSession.SaveChanges();
			return item;
		}

		public List<PermissionGroup> GetGroupList()
		{
			return _documentSession.Query<PermissionGroup>().ToList();
		}

		public PermissionGroup CreateGroup(string groupName, string parentGroupId)
		{
			if(string.IsNullOrEmpty(groupName))
			{
				throw new ArgumentNullException("Missing groupName");
			}
			var existingItem = _documentSession.Query<PermissionGroup>().FirstOrDefault(i=>i.GroupName == groupName);
			if(existingItem != null)
			{
				throw new ArgumentException(string.Format("Permission Group with name \"{0}\" already exists", groupName));
			}
			if(!string.IsNullOrEmpty(parentGroupId))
			{
				var parent = _documentSession.Load<PermissionGroup>(parentGroupId);
				if(parent == null)
				{
					throw new ArgumentException("No parent group found for parentGroupId " + parentGroupId);
				}
			}
			var item = new PermissionGroup
			{
				Id = Guid.NewGuid().ToString(),
				GroupName = groupName,
				ParentGroupId = parentGroupId,
			};
			_documentSession.Store(item);
			_documentSession.SaveChanges();
			return item;
		}

		public PermissionGroup GetGroup(string id)
		{
			var item = _documentSession.Load<PermissionGroup>(id);
			if(item == null)
			{
				throw new RecordNotFoundException(typeof(PermissionGroup), "Id", id);
			}
			return item;
		}

		public PermissionGroup DeleteGroup(string groupId)
		{
			var item = this.GetGroup(groupId);
			_documentSession.Delete(item);
			_documentSession.SaveChanges();
			return item;
		}

	
		public List<PermissionGroup> GetUserGroupList(string userId, bool includeParents)
		{
			var returnList = new List<PermissionGroup>();
			var userGroupAssignmentList = _documentSession.Query<UserGroupAssignment>().Where(i=>i.UserId == userId).ToList();
			foreach(var userGroupAssignment in userGroupAssignmentList)
			{
				var group = this.GetGroup(userGroupAssignment.GroupId);
				returnList.Add(group);
			}
			return returnList;
		}


		public UserGroupAssignment CreateUserGroupAssignment(string userId, string groupId)
		{
			if(string.IsNullOrEmpty(userId))
			{
				throw new ArgumentNullException("Missing user ID");
			}
			if(string.IsNullOrEmpty(groupId))
			{
				throw new ArgumentNullException("Missing group ID");
			}
			var existingItem = this.TryGetUserGroupAssignment(userId, groupId);
			if(existingItem != null)
			{
				throw new ArgumentException(string.Format("UserGroupAssignment already exists for userID {0} and groupID {1}", userId, groupId));
			}
			VerifyGroupExists(groupId);
			var item = new UserGroupAssignment
			{
				Id = Guid.NewGuid().ToString(),
				GroupId = groupId,
				UserId = userId
			};
			_documentSession.Store(item);
			_documentSession.SaveChanges();
			return item;
		}

		private void VerifyGroupExists(string groupId)
		{
			GetGroup(groupId);
		}


		public UserGroupAssignment TryGetUserGroupAssignment(string userId, string groupId)
		{
			return _documentSession.Query<UserGroupAssignment>().FirstOrDefault(i=>i.UserId == userId && i.GroupId == groupId);
		}


		public List<PermissionRole> GetRoleList(PermissionDataAssignment assignment)
		{
			var query = _documentSession.Query<PermissionRole>().AsQueryable();
			if (assignment != null)
			{
				query = query.Where(i => i.DataAssignmentList.Any(j => j.DataPropertyValue == assignment.DataPropertyValue
																	&& j.DataPropertyName == assignment.DataPropertyName));
			}
			return query.ToList();
		}

		public PermissionRole CreateRole(string roleName, List<PermissionDataAssignment> roleDataItems = null)
		{
			var item = new PermissionRole
			{
				Id = Guid.NewGuid().ToString(),
				RoleName = roleName,
				DataAssignmentList = roleDataItems
			};
			_documentSession.Store(item);
			_documentSession.SaveChanges();
			return item;
		}

		public PermissionRole GetRole(string roleId)
		{
			if(string.IsNullOrEmpty(roleId))
			{
				throw new ArgumentNullException("roleId is missing");
			}
			var role = _documentSession.Load<PermissionRole>(roleId);
			if(role == null)
			{
				throw new RecordNotFoundException(typeof(PermissionRole), "Id", roleId);
			}
			return role;
		}

		public List<RolePermission> GetRolePermissionList(string roleId)
		{
			return _documentSession.Query<RolePermission>().Where(i=>i.RoleId == roleId).ToList();
		}

		public PermissionRole UpdateRole(string roleId, string roleName, List<PermissionDataAssignment> list)
		{
			if(string.IsNullOrEmpty(roleId))
			{
				throw new ArgumentNullException("Missing roleId");
			}
			if (string.IsNullOrEmpty(roleName))
			{
				throw new ArgumentNullException("Missing roleName");
			}
			var role = this.GetRole(roleId);
			role.RoleName = roleName;
			role.DataAssignmentList = list;
			_documentSession.SaveChanges();
			return role;
		}

		public RoleGroupAssignment AssignGroupToRole(string roleId, string groupId)
		{
			throw new NotImplementedException();
		}

		public RoleGroupAssignment TryGetRoleGroupAssignment(string roleId, string groupId)
		{
			throw new NotImplementedException();
		}


	}
}
