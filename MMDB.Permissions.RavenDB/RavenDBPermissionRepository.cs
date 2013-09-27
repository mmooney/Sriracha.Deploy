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
			throw new NotImplementedException();
		}

		public List<PermissionItem> GetPermissionList()
		{
			return this._session.Query<PermissionItem>().ToList();
		}

		public UserPermissionAssignment CreateUserPermissionAssignment(string permissionId, string userId, EnumPermissionAccess enumPermissionAccess)
		{
			throw new NotImplementedException();
		}

		public UserPermissionAssignment UpdateUserPermissionAssignment(string userPermissionAssignmentId, EnumPermissionAccess access)
		{
			throw new NotImplementedException();
		}

		public UserPermissionAssignment DeleteUserPermissionAssignment(string userPermissionAssignmentId)
		{
			throw new NotImplementedException();
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
