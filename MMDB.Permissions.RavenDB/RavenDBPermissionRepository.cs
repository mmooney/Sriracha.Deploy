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

		public PermissionItem CreatePermission(string permissionName, string permissionDisplayValue)
		{
			if(string.IsNullOrEmpty(permissionName))
			{
				throw new ArgumentNullException("Missing permissionName");
			}
			if(string.IsNullOrEmpty(permissionDisplayValue))
			{
				throw new ArgumentNullException("Missing permissionDisplayValue");
			}
			var existingItem = _session.Query<PermissionItem>().FirstOrDefault(i=>i.PermissionName == permissionName);
			if(existingItem != null)
			{
				throw new ArgumentException(string.Format("Permission with name \"{0}\" already exists", permissionName));
			}
			var item = new PermissionItem
			{
				Id = Guid.NewGuid().ToString(),
				PermissionName = permissionName,
				PermissionDisplayValue = permissionDisplayValue
			};
			_session.Store(item);
			_session.SaveChanges();
			return item;
		}

		public PermissionItem GetPermission(string id)
		{
			if(string.IsNullOrEmpty(id))
			{
				throw new ArgumentNullException("Missing id");
			}
			var item = _session.Load<PermissionItem>(id);
			if(item == null)
			{
				throw new RecordNotFoundException(typeof(PermissionItem), "Id", id);
			}
			return item;
		}

		public PermissionItem GetPermissionByName(string permissionName)
		{
			if(string.IsNullOrEmpty(permissionName))
			{
				throw new ArgumentNullException("Missing permissionName");
			}
			var returnValue = TryGetPermissionByName(permissionName);
			if(returnValue == null)
			{
				throw new RecordNotFoundException(typeof(PermissionItem), "PermissionName", permissionName);
			}
			return returnValue;
		}

		public PermissionItem TryGetPermissionByName(string permissionName)
		{
			if(string.IsNullOrEmpty(permissionName))
			{
				throw new ArgumentNullException("Missing permissionName");
			}
			return _session.Query<PermissionItem>().FirstOrDefault(i=>i.PermissionName == permissionName);
		}

		public UserPermissionAssignment TryGetUserPermissionAssignment(string permissionId, string userId)
		{
			return _session.Query<UserPermissionAssignment>().FirstOrDefault(i=>i.PermissionId == permissionId && i.UserId == userId);
		}

		public List<PermissionItem> GetPermissionList()
		{
			return this._session.Query<PermissionItem>().ToList();
		}

		public PermissionItem DeletePermission(string id)
		{
			var item = this.GetPermission(id);
			var userPermissionList = _session.Query<UserPermissionAssignment>().Where(i=>i.PermissionId == id).ToList();
			foreach(var userPermissionItem in userPermissionList)
			{
				_session.Delete(userPermissionItem);
			}
			var groupPermissionList = _session.Query<GroupPermissionAssignment>().Where(i=>i.PermissionId == id).ToList();
			foreach(var groupPermissionItem in groupPermissionList)
			{
				_session.Delete(groupPermissionItem);
			}
			_session.Delete(item);
			_session.SaveChanges();
			return item;
		}

		public UserPermissionAssignment CreateUserPermissionAssignment(string permissionId, string userId, EnumPermissionAccess access)
		{
			if(string.IsNullOrEmpty(permissionId))
			{
				throw new ArgumentNullException("Missing permission ID");
			}
			if(string.IsNullOrEmpty(userId))
			{
				throw new ArgumentNullException("Missing user ID");
			}
			var existingItem = _session.Query<UserPermissionAssignment>().Any(i=>i.PermissionId == permissionId && i.UserId == userId);
			if(existingItem)
			{
				throw new ArgumentException(string.Format("User Permission Assignment already exists for user {0} and permission {1}", userId, permissionId));
			}
			var item = new UserPermissionAssignment
			{
				Id = Guid.NewGuid().ToString(),
				PermissionId = permissionId,
				UserId = userId,
				Access = access
			};
			_session.Store(item);
			_session.SaveChanges();
			return item;
		}

		public UserPermissionAssignment UpdateUserPermissionAssignment(string userPermissionAssignmentId, EnumPermissionAccess access)
		{
			if(string.IsNullOrEmpty(userPermissionAssignmentId))
			{
				throw new ArgumentNullException("Missing userPermissionAssignmentId");
			}
			var item = _session.Load<UserPermissionAssignment>(userPermissionAssignmentId);
			if(item == null)
			{
				throw new RecordNotFoundException(typeof(UserPermissionAssignment), "Id", userPermissionAssignmentId);
			}
			item.Access = access;
			_session.SaveChanges();
			return item;
		}

		public UserPermissionAssignment DeleteUserPermissionAssignment(string userPermissionAssignmentId)
		{
			if(string.IsNullOrEmpty(userPermissionAssignmentId))
			{
				throw new ArgumentNullException("Missing userPermissionAssignmentId");
			}
			var item = _session.Load<UserPermissionAssignment>(userPermissionAssignmentId);
			if(item == null)
			{
				throw new RecordNotFoundException(typeof(UserPermissionAssignment), "Id", userPermissionAssignmentId);
			}
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

		public GroupPermissionAssignment CreateGroupPermissionAssignment(string permissionId, string groupId, EnumPermissionAccess access)
		{
			if(string.IsNullOrEmpty(permissionId))
			{
				throw new ArgumentNullException("Missing permission ID");
			}
			if(string.IsNullOrEmpty(groupId))
			{
				throw new ArgumentNullException("Missing group ID");
			}
			var existingItem = _session.Query<GroupPermissionAssignment>().Any(i=>i.PermissionId == permissionId && i.GroupId == groupId);
			if(existingItem)
			{
				throw new ArgumentException(string.Format("Group Permission Assignment already exists for group {0} and permission {1}", groupId, permissionId));
			}
			var item = new GroupPermissionAssignment
			{
				Id = Guid.NewGuid().ToString(),
				PermissionId = permissionId,
				GroupId = groupId,
				Access = access
			};
			_session.Store(item);
			_session.SaveChanges();
			return item;
		}

		public GroupPermissionAssignment TryGetGroupPermissionAssignment(string permissionId, string groupId)
		{
			return _session.Query<GroupPermissionAssignment>().FirstOrDefault(i=>i.PermissionId == permissionId && i.GroupId == groupId);
		}

		public GroupPermissionAssignment UpdateGroupPermissionAssignment(string groupPermissionAssignmentId, EnumPermissionAccess access)
		{
			if(string.IsNullOrEmpty(groupPermissionAssignmentId))
			{
				throw new ArgumentNullException("Missing groupPermissionAssignmentId");
			}
			var item = _session.Load<GroupPermissionAssignment>(groupPermissionAssignmentId);
			if(item == null)
			{
				throw new RecordNotFoundException(typeof(GroupPermissionAssignment), "Id", groupPermissionAssignmentId);
			}
			item.Access = access;
			_session.SaveChanges();
			return item;
		}

		public GroupPermissionAssignment DeleteGroupPermissionAssignment(string groupPermissionAssignmentId)
		{
			if(string.IsNullOrEmpty(groupPermissionAssignmentId))
			{
				throw new ArgumentNullException("Missing groupPermissionAssignmentId");
			}
			var item = _session.Load<GroupPermissionAssignment>(groupPermissionAssignmentId);
			if(item == null)
			{
				throw new RecordNotFoundException(typeof(GroupPermissionAssignment), "Id", groupPermissionAssignmentId);
			}
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
