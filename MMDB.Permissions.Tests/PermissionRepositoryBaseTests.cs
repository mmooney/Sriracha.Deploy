using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MMDB.Shared;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace MMDB.Permissions.Tests
{
	[TestFixture]
	public abstract class PermissionRepositoryBaseTests
	{
		protected abstract IPermissionRepository GetRepository();

		[Test]
		public void CreatePermission()
		{
			var sut = this.GetRepository();
			var existingPermissionList = sut.GetPermissionList();

			var fixture = new Fixture();
			string permissionName = fixture.Create<string>();
			string permissionDisplayValue = fixture.Create<string>();
			var newPermission = sut.CreatePermission(permissionName, permissionDisplayValue);

			Assert.IsNotNull(newPermission);
			Assert.AreEqual(permissionName, newPermission.PermissionName);
			Assert.AreEqual(permissionDisplayValue, newPermission.PermissionDisplayValue);

			var newPermissionList = sut.GetPermissionList();
			Assert.AreEqual(existingPermissionList.Count+1, newPermissionList.Count);
			var newDbItem = newPermissionList.SingleOrDefault(i=>i.Id == newPermission.Id);
			Assert.IsNotNull(newDbItem);
			Assert.AreEqual(permissionName, newDbItem.PermissionName);
			Assert.AreEqual(permissionDisplayValue, newDbItem.PermissionDisplayValue);
		}

		[Test]
		public void CreatePermission_DuplicateName_ThrowsArgumentException()
		{
			var sut = this.GetRepository();
			var existingPermissionList = sut.GetPermissionList();

			var fixture = new Fixture();
			string permissionName = fixture.Create<string>();
			string permissionDisplayValue = fixture.Create<string>();
			var newPermission = sut.CreatePermission(permissionName, permissionDisplayValue);

			string secondPermissionDisplayValue = fixture.Create<string>();
			Assert.Throws<ArgumentException>(()=> sut.CreatePermission(permissionName, secondPermissionDisplayValue));
		}

		[Test]
		public void GetPermission()
		{
			var sut = this.GetRepository();

			var fixture = new Fixture();
			string permissionName = fixture.Create<string>();
			string permissionDisplayValue = fixture.Create<string>();
			var newPermission = sut.CreatePermission(permissionName, permissionDisplayValue);

			var dbItem = sut.GetPermission(newPermission.Id);

			Assert.IsNotNull(dbItem);
			Assert.AreEqual(permissionName, dbItem.PermissionName);
			Assert.AreEqual(permissionDisplayValue, dbItem.PermissionDisplayValue);
		}

		[Test]
		public void GetPermissionByName()
		{
			var sut = this.GetRepository();

			var fixture = new Fixture();
			string permissionName = fixture.Create<string>();
			string permissionDisplayValue = fixture.Create<string>();
			var newPermission = sut.CreatePermission(permissionName, permissionDisplayValue);

			var dbItem = sut.GetPermissionByName(permissionName);

			Assert.IsNotNull(dbItem);
			Assert.AreEqual(permissionName, dbItem.PermissionName);
			Assert.AreEqual(permissionDisplayValue, dbItem.PermissionDisplayValue);
		}

		[Test]
		public void GetPermissionByName_NoRecord_ThrowsRecordNotFoundException()
		{
			var sut = this.GetRepository();

			var fixture = new Fixture();
			string permissionName = fixture.Create<string>();
			string permissionDisplayValue = fixture.Create<string>();

			Assert.Throws<RecordNotFoundException>(()=>sut.GetPermissionByName(permissionName));
		}

		[Test]
		public void TryGetPermissionByName()
		{ 
			var sut = this.GetRepository();

			var fixture = new Fixture();
			string permissionName = fixture.Create<string>();
			string permissionDisplayValue = fixture.Create<string>();
			var newPermission = sut.CreatePermission(permissionName, permissionDisplayValue);

			var dbItem = sut.TryGetPermissionByName(permissionName);

			Assert.IsNotNull(dbItem);
			Assert.AreEqual(permissionName, dbItem.PermissionName);
			Assert.AreEqual(permissionDisplayValue, dbItem.PermissionDisplayValue);
		}

		[Test]
		public void TryGetPermissionName_NoPermission_ReturnsNull()
		{
			var sut = this.GetRepository();

			var fixture = new Fixture();
			string permissionName = fixture.Create<string>();
			string permissionDisplayValue = fixture.Create<string>();

			var result = sut.TryGetPermissionByName(permissionName);

			Assert.IsNull(result);
		}

		[Test]
		public void TryGetUserPermissionAssignment()
		{
			var sut = this.GetRepository();

			var fixture = new Fixture();
			string permissionName = fixture.Create<string>();
			string permissionDisplayValue = fixture.Create<string>();
			string userId = fixture.Create<string>();
			var permission = sut.CreatePermission(permissionName, permissionDisplayValue);
			var assignment = sut.CreateUserPermissionAssignment(permission.Id, userId, EnumPermissionAccess.Grant);

			var result = sut.TryGetUserPermissionAssignment(permission.Id, userId);

			Assert.IsNotNull(result);
			Assert.AreEqual(assignment.Id, result.Id);
			Assert.AreEqual(permission.Id, result.PermissionId);
			Assert.AreEqual(userId, result.UserId);
			Assert.AreEqual(EnumPermissionAccess.Grant, result.Access);
		}

		[Test]
		public void TryGetUserPermissionAssignment_NoRecord_ReturnsNull()
		{
			var sut = this.GetRepository();

			var fixture = new Fixture();
			string permissionName = fixture.Create<string>();
			string permissionDisplayValue = fixture.Create<string>();
			string userId = fixture.Create<string>();
			var permission = sut.CreatePermission(permissionName, permissionDisplayValue);

			var result = sut.TryGetUserPermissionAssignment(permission.Id, userId);

			Assert.IsNull(result);
		}

		[Test]
		public void UpdateUserPermissionAssignment()
		{ 
			var sut = this.GetRepository();

			var fixture = new Fixture();
			string permissionName = fixture.Create<string>();
			string permissionDisplayValue = fixture.Create<string>();
			string userId = fixture.Create<string>();
			var permission = sut.CreatePermission(permissionName, permissionDisplayValue);
			var assignment = sut.CreateUserPermissionAssignment(permission.Id, userId, EnumPermissionAccess.Grant);

			var result = sut.UpdateUserPermissionAssignment(assignment.Id, EnumPermissionAccess.Deny);
			var dbItem = sut.TryGetUserPermissionAssignment(permission.Id, userId);

			Assert.IsNotNull(result);
			Assert.AreEqual(assignment.Id, result.Id);
			Assert.AreEqual(permission.Id, result.PermissionId);
			Assert.AreEqual(userId, result.UserId);
			Assert.AreEqual(EnumPermissionAccess.Deny, result.Access);	
			
			Assert.IsNotNull(dbItem);
			Assert.AreEqual(assignment.Id, dbItem.Id);
			Assert.AreEqual(permission.Id, dbItem.PermissionId);
			Assert.AreEqual(userId, dbItem.UserId);
			Assert.AreEqual(EnumPermissionAccess.Deny, dbItem.Access);	
		}

		[Test]
		public void UpdateUserPermissionAssignment_NoRecord_ThrowRecordNotFoundException()
		{ 
			var sut = this.GetRepository();

			Assert.Throws<RecordNotFoundException>(()=>sut.UpdateUserPermissionAssignment(Guid.NewGuid().ToString(), EnumPermissionAccess.Deny));
		}

		[Test]
		public void DeleteUserPermissionAssignment()
		{
			var sut = this.GetRepository();

			var fixture = new Fixture();
			string permissionName = fixture.Create<string>();
			string permissionDisplayValue = fixture.Create<string>();
			string userId = fixture.Create<string>();
			var permission = sut.CreatePermission(permissionName, permissionDisplayValue);
			var assignment = sut.CreateUserPermissionAssignment(permission.Id, userId, EnumPermissionAccess.Grant);

			var result = sut.DeleteUserPermissionAssignment(assignment.Id);
			var dbItem = sut.TryGetUserPermissionAssignment(permission.Id, userId);

			Assert.IsNotNull(result);
			Assert.AreEqual(assignment.Id, result.Id);
			Assert.AreEqual(permission.Id, result.PermissionId);
			Assert.AreEqual(userId, result.UserId);
			Assert.AreEqual(EnumPermissionAccess.Grant, result.Access);	
			
			Assert.IsNull(dbItem);
		}

		[Test]
		public void DeleteUserPermissionAssignment_NoRecord_ThrowRecordNotFoundException()
		{
			var sut = this.GetRepository();

			Assert.Throws<RecordNotFoundException>(()=>sut.DeleteUserPermissionAssignment(Guid.NewGuid().ToString()));
		}

		[Test]
		public void CreateGroup()
		{
			var sut = this.GetRepository();
			var fixture = new Fixture();
			var group = fixture.Create<PermissionGroup>();
			group.ParentGroupId = null;
			var oldGroupList = sut.GetGroupList();

			var result = sut.CreateGroup(group.GroupName, group.ParentGroupId);
			
			Assert.IsNotNull(result);
			Assert.AreEqual(group.GroupName, result.GroupName);
			Assert.AreEqual(group.ParentGroupId, result.ParentGroupId);

			var newGroupList = sut.GetGroupList();
			Assert.AreEqual(oldGroupList.Count()+1, newGroupList.Count());
			var newGroupListItem = newGroupList.SingleOrDefault(i=>i.Id == result.Id);
			Assert.IsNotNull(newGroupListItem);
			Assert.AreEqual(group.GroupName, newGroupListItem.GroupName);
			Assert.AreEqual(group.ParentGroupId, newGroupListItem.ParentGroupId);
		}

		[Test]
		public void CreateGroup_WithParent()
		{
			var sut = this.GetRepository();
			var fixture = new Fixture();
			var parentGroup = sut.CreateGroup(fixture.Create<string>(), null);
			var group = fixture.Create<PermissionGroup>();
			group.ParentGroupId = parentGroup.Id;
			var oldGroupList = sut.GetGroupList();

			var result = sut.CreateGroup(group.GroupName, group.ParentGroupId);
			
			Assert.IsNotNull(result);
			Assert.AreEqual(group.GroupName, result.GroupName);
			Assert.AreEqual(group.ParentGroupId, result.ParentGroupId);

			var newGroupList = sut.GetGroupList();
			Assert.AreEqual(oldGroupList.Count()+1, newGroupList.Count());
			var newGroupListItem = newGroupList.SingleOrDefault(i=>i.Id == result.Id);
			Assert.IsNotNull(newGroupListItem);
			Assert.AreEqual(group.GroupName, newGroupListItem.GroupName);
			Assert.AreEqual(group.ParentGroupId, newGroupListItem.ParentGroupId);
		}

		[Test]
		public void CreateGroup_InvalidParentGroupId_ThrowsArgumentException()
		{
			var sut = this.GetRepository();
			var fixture = new Fixture();
			var group = fixture.Create<PermissionGroup>();

			Assert.Throws<ArgumentException>(()=>sut.CreateGroup(group.GroupName, group.ParentGroupId));
		}

		[Test]
		public void CreateGroup_DuplicateName_ThrowsArgumentException()
		{
			var sut = this.GetRepository();
			var fixture = new Fixture();
			var existingGroup = sut.CreateGroup(fixture.Create<string>(), null);

			Assert.Throws<ArgumentException>(()=>sut.CreateGroup(existingGroup.GroupName, null));
		}
	}
}
