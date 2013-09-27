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
			return _session.Load<PermissionItem>(id);
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
			throw new NotImplementedException();
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

		public PermissionGroup CreateGroup(string groupName, string parentGroupId)
		{
			throw new NotImplementedException();
		}

		public PermissionGroup DeleteGroup(string groupId)
		{
			throw new NotImplementedException();
		}

		public GroupPermissionAssignment CreateGroupPermissionAssignment(string permissionId, string groupId, EnumPermissionAccess enumPermissionAccess)
		{
			throw new NotImplementedException();
		}

		public GroupPermissionAssignment TryGetGroupPermissionAssignment(string permissionId, string groupId)
		{
			throw new NotImplementedException();
		}

		public GroupPermissionAssignment UpdateGroupPermissionAssignment(string groupPermissionAssignmentId, EnumPermissionAccess access)
		{
			throw new NotImplementedException();
		}

		public GroupPermissionAssignment DeleteGroupPermissionAssignment(string groupPermissionAssignmentId)
		{
			throw new NotImplementedException();
		}
	}
}
