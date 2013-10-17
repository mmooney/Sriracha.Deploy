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
		private readonly IDocumentSession _session;

		public RavenDBPermissionRepository(IDocumentSession session)
		{
			_session = session;
		}

		public PermissionDefinition CreatePermissionDefinition(string permissionName, string permissionDisplayValue, List<PermissionFilterDefinition> filterDefinitionList)
		{
			if(string.IsNullOrEmpty(permissionName))
			{
				throw new ArgumentNullException("Missing permissionName");
			}
			if(string.IsNullOrEmpty(permissionDisplayValue))
			{
				throw new ArgumentNullException("Missing permissionDisplayValue");
			}
			var existingItem = _session.Query<PermissionDefinition>().FirstOrDefault(i=>i.PermissionName == permissionName);
			if(existingItem != null)
			{
				throw new ArgumentException(string.Format("Permission with name \"{0}\" already exists", permissionName));
			}
			var item = new PermissionDefinition
			{
				Id = Guid.NewGuid().ToString(),
				PermissionName = permissionName,
				PermissionDisplayValue = permissionDisplayValue,
				FilterDefinitionList = filterDefinitionList
			};
			if(filterDefinitionList != null)
			{
				foreach(var filter in filterDefinitionList)
				{
					filter.Id = Guid.NewGuid().ToString();
				}
			}
			_session.Store(item);
			_session.SaveChanges();
			return item;
		}

		public PermissionDefinition GetPermissionDefinition(string id)
		{
			if(string.IsNullOrEmpty(id))
			{
				throw new ArgumentNullException("Missing id");
			}
			var item = _session.Load<PermissionDefinition>(id);
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
			return _session.Query<PermissionDefinition>().FirstOrDefault(i=>i.PermissionName == permissionName);
		}

		public List<PermissionDefinition> GetPermissionDefinitionList()
		{
			return this._session.Query<PermissionDefinition>().ToList();
		}

		public PermissionDefinition DeletePermissionDefinition(string id)
		{
			var item = this.GetPermissionDefinition(id);
			_session.Delete(item);
			_session.SaveChanges();
			return item;
		}

		public List<PermissionGroup> GetGroupList()
		{
			return _session.Query<PermissionGroup>().ToList();
		}

		public PermissionGroup CreateGroup(string groupName, string parentGroupId)
		{
			if(string.IsNullOrEmpty(groupName))
			{
				throw new ArgumentNullException("Missing groupName");
			}
			var existingItem = _session.Query<PermissionGroup>().FirstOrDefault(i=>i.GroupName == groupName);
			if(existingItem != null)
			{
				throw new ArgumentException(string.Format("Permission Group with name \"{0}\" already exists", groupName));
			}
			if(!string.IsNullOrEmpty(parentGroupId))
			{
				var parent = _session.Load<PermissionGroup>(parentGroupId);
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
			_session.Store(item);
			_session.SaveChanges();
			return item;
		}
		public PermissionGroup GetGroup(string id)
		{
			var item = _session.Load<PermissionGroup>(id);
			if(item == null)
			{
				throw new RecordNotFoundException(typeof(PermissionGroup), "Id", id);
			}
			return item;
		}

		public PermissionGroup DeleteGroup(string groupId)
		{
			var item = this.GetGroup(groupId);
			_session.Delete(item);
			_session.SaveChanges();
			return item;
		}

	
		public List<PermissionGroup> GetUserGroupList(string userId, bool includeParents)
		{
			var returnList = new List<PermissionGroup>();
			var userGroupAssignmentList = _session.Query<UserGroupAssignment>().Where(i=>i.UserId == userId).ToList();
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
			_session.Store(item);
			_session.SaveChanges();
			return item;
		}

		private void VerifyGroupExists(string groupId)
		{
			GetGroup(groupId);
		}


		public UserGroupAssignment TryGetUserGroupAssignment(string userId, string groupId)
		{
			return _session.Query<UserGroupAssignment>().FirstOrDefault(i=>i.UserId == userId && i.GroupId == groupId);
		}
	}
}
