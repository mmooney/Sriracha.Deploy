using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MMDB.Shared;
using NUnit.Framework;
using Ploeh.AutoFixture;

//namespace MMDB.Permissions.Tests
//{
//	[TestFixture]
//	public abstract class PermissionRepositoryBaseTests
//	{
//		protected abstract IPermissionRepository GetRepository();

//		[Test]
//		public void CreatePermissionDefinition()
//		{
//			var sut = this.GetRepository();
//			var existingPermissionList = sut.GetPermissionDefinitionList();

//			var fixture = new Fixture();
//			string permissionName = fixture.Create<string>();
//			string permissionDisplayValue = fixture.Create<string>();
//			var newPermission = sut.CreatePermissionDefinition(permissionName, permissionDisplayValue, null);

//			Assert.IsNotNull(newPermission);
//			Assert.AreEqual(permissionName, newPermission.PermissionName);
//			Assert.AreEqual(permissionDisplayValue, newPermission.PermissionDisplayValue);

//			var newPermissionList = sut.GetPermissionDefinitionList();
//			Assert.AreEqual(existingPermissionList.Count+1, newPermissionList.Count);
//			var newDbItem = newPermissionList.SingleOrDefault(i=>i.Id == newPermission.Id);
//			Assert.IsNotNull(newDbItem);
//			Assert.AreEqual(permissionName, newDbItem.PermissionName);
//			Assert.AreEqual(permissionDisplayValue, newDbItem.PermissionDisplayValue);
//		}

//		[Test]
//		public void CreatePermission_DuplicateName_ThrowsArgumentException()
//		{
//			var sut = this.GetRepository();
//			var existingPermissionList = sut.GetPermissionDefinitionList();

//			var fixture = new Fixture();
//			string permissionName = fixture.Create<string>();
//			string permissionDisplayValue = fixture.Create<string>();
//			var newPermission = sut.CreatePermissionDefinition(permissionName, permissionDisplayValue, null);

//			string secondPermissionDisplayValue = fixture.Create<string>();
//			Assert.Throws<ArgumentException>(()=> sut.CreatePermissionDefinition(permissionName, secondPermissionDisplayValue, null));
//		}

//		[Test]
//		public void GetPermission()
//		{
//			var sut = this.GetRepository();

//			var fixture = new Fixture();
//			string permissionName = fixture.Create<string>();
//			string permissionDisplayValue = fixture.Create<string>();
//			var newPermission = sut.CreatePermissionDefinition(permissionName, permissionDisplayValue, null);

//			var dbItem = sut.GetPermissionDefinition(newPermission.Id);

//			Assert.IsNotNull(dbItem);
//			Assert.AreEqual(permissionName, dbItem.PermissionName);
//			Assert.AreEqual(permissionDisplayValue, dbItem.PermissionDisplayValue);
//		}

//		[Test]
//		public void GetPermission_NoRecord_ThrowsRecordNotFoundException()
//		{
//			var sut = this.GetRepository();

//			Assert.Throws<RecordNotFoundException>(()=>sut.GetPermissionDefinition(Guid.NewGuid().ToString()));
//		}

//		[Test]
//		public void GetPermissionByName()
//		{
//			var sut = this.GetRepository();

//			var fixture = new Fixture();
//			string permissionName = fixture.Create<string>();
//			string permissionDisplayValue = fixture.Create<string>();
//			var newPermission = sut.CreatePermissionDefinition(permissionName, permissionDisplayValue, null);

//			var dbItem = sut.GetPermissionDefinitionByName(permissionName);

//			Assert.IsNotNull(dbItem);
//			Assert.AreEqual(permissionName, dbItem.PermissionName);
//			Assert.AreEqual(permissionDisplayValue, dbItem.PermissionDisplayValue);
//		}

//		[Test]
//		public void GetPermissionByName_NoRecord_ThrowsRecordNotFoundException()
//		{
//			var sut = this.GetRepository();

//			var fixture = new Fixture();
//			string permissionName = fixture.Create<string>();
//			string permissionDisplayValue = fixture.Create<string>();

//			Assert.Throws<RecordNotFoundException>(()=>sut.GetPermissionDefinitionByName(permissionName));
//		}

//		[Test]
//		public void TryGetPermissionByName()
//		{ 
//			var sut = this.GetRepository();

//			var fixture = new Fixture();
//			string permissionName = fixture.Create<string>();
//			string permissionDisplayValue = fixture.Create<string>();
//			var newPermission = sut.CreatePermissionDefinition(permissionName, permissionDisplayValue, null);

//			var dbItem = sut.TryGetPermissionDefinitionByName(permissionName);

//			Assert.IsNotNull(dbItem);
//			Assert.AreEqual(permissionName, dbItem.PermissionName);
//			Assert.AreEqual(permissionDisplayValue, dbItem.PermissionDisplayValue);
//		}

//		[Test]
//		public void TryGetPermissionName_NoPermission_ReturnsNull()
//		{
//			var sut = this.GetRepository();

//			var fixture = new Fixture();
//			string permissionName = fixture.Create<string>();
//			string permissionDisplayValue = fixture.Create<string>();

//			var result = sut.TryGetPermissionDefinitionByName(permissionName);

//			Assert.IsNull(result);
//		}

//		[Test]
//		public void DeletePermission()
//		{
//			var sut = this.GetRepository();
//			var fixture = new Fixture();
//			string permissionName = fixture.Create<string>();
//			string permissionDisplayValue = fixture.Create<string>();
//			var existingPermission = sut.CreatePermissionDefinition(permissionName, permissionDisplayValue, null);
//			var existingList = sut.GetPermissionDefinitionList();

//			var deletedPermission = sut.DeletePermissionDefinition(existingPermission.Id);

//			Assert.IsNotNull(deletedPermission);
//			Assert.AreEqual(existingPermission.Id, deletedPermission.Id);
//			Assert.AreEqual(existingPermission.PermissionName, deletedPermission.PermissionName);
//			Assert.AreEqual(existingPermission.PermissionDisplayValue, deletedPermission.PermissionDisplayValue);
//			Assert.Throws<RecordNotFoundException>(()=>sut.GetPermissionDefinition(existingPermission.Id));
//			var newList = sut.GetPermissionDefinitionList();
//			Assert.AreEqual(existingList.Count-1, newList.Count);
//			Assert.IsFalse(newList.Any(i=>i.Id == existingPermission.Id));
//		}

//		[Test]
//		public void DeletePermission_NoRecord_ThrowsRecordNotFoundException()
//		{
//			var sut = this.GetRepository();

//			Assert.Throws<RecordNotFoundException>(()=>sut.DeletePermissionDefinition(Guid.NewGuid().ToString()));
//		}

//		[Test]
//		public void CreateGroup()
//		{
//			var sut = this.GetRepository();
//			var fixture = new Fixture();
//			var group = fixture.Create<PermissionGroup>();
//			group.ParentGroupId = null;
//			var oldGroupList = sut.GetGroupList();

//			var result = sut.CreateGroup(group.GroupName, group.ParentGroupId);
			
//			Assert.IsNotNull(result);
//			Assert.AreEqual(group.GroupName, result.GroupName);
//			Assert.AreEqual(group.ParentGroupId, result.ParentGroupId);

//			var newGroupList = sut.GetGroupList();
//			Assert.AreEqual(oldGroupList.Count()+1, newGroupList.Count());
//			var newGroupListItem = newGroupList.SingleOrDefault(i=>i.Id == result.Id);
//			Assert.IsNotNull(newGroupListItem);
//			Assert.AreEqual(group.GroupName, newGroupListItem.GroupName);
//			Assert.AreEqual(group.ParentGroupId, newGroupListItem.ParentGroupId);
//		}

//		[Test]
//		public void CreateGroup_WithParent()
//		{
//			var sut = this.GetRepository();
//			var fixture = new Fixture();
//			var parentGroup = sut.CreateGroup(fixture.Create<string>(), null);
//			var group = fixture.Create<PermissionGroup>();
//			group.ParentGroupId = parentGroup.Id;
//			var oldGroupList = sut.GetGroupList();

//			var result = sut.CreateGroup(group.GroupName, group.ParentGroupId);
			
//			Assert.IsNotNull(result);
//			Assert.AreEqual(group.GroupName, result.GroupName);
//			Assert.AreEqual(group.ParentGroupId, result.ParentGroupId);

//			var newGroupList = sut.GetGroupList();
//			Assert.AreEqual(oldGroupList.Count()+1, newGroupList.Count());
//			var newGroupListItem = newGroupList.SingleOrDefault(i=>i.Id == result.Id);
//			Assert.IsNotNull(newGroupListItem);
//			Assert.AreEqual(group.GroupName, newGroupListItem.GroupName);
//			Assert.AreEqual(group.ParentGroupId, newGroupListItem.ParentGroupId);
//		}

//		[Test]
//		public void CreateGroup_InvalidParentGroupId_ThrowsArgumentException()
//		{
//			var sut = this.GetRepository();
//			var fixture = new Fixture();
//			var group = fixture.Create<PermissionGroup>();

//			Assert.Throws<ArgumentException>(()=>sut.CreateGroup(group.GroupName, group.ParentGroupId));
//		}

//		[Test]
//		public void CreateGroup_DuplicateName_ThrowsArgumentException()
//		{
//			var sut = this.GetRepository();
//			var fixture = new Fixture();
//			var existingGroup = sut.CreateGroup(fixture.Create<string>(), null);

//			Assert.Throws<ArgumentException>(()=>sut.CreateGroup(existingGroup.GroupName, null));
//		}

//		[Test]
//		public void DeleteGroup()
//		{
//			var sut = this.GetRepository();
//			var fixture = new Fixture();
//			var existingGroup = sut.CreateGroup(fixture.Create<string>(), null);
//			var existingList = sut.GetGroupList();

//			var deletedGroup = sut.DeleteGroup(existingGroup.Id);

//			Assert.IsNotNull(deletedGroup);
//			Assert.AreEqual(existingGroup.Id, deletedGroup.Id);
//			Assert.AreEqual(existingGroup.GroupName, deletedGroup.GroupName);
//			Assert.Throws<RecordNotFoundException>(()=>sut.GetGroup(existingGroup.Id));
//			var newList = sut.GetGroupList();
//			Assert.AreEqual(existingList.Count-1, newList.Count);
//			Assert.IsFalse(newList.Any(i=>i.Id == existingGroup.Id));
//		}

//		[Test]
//		public void DeleteGroup_NoRecord_ThrowsRecordNotFoundException()
//		{
//			var sut = this.GetRepository();

//			Assert.Throws<RecordNotFoundException>(()=>sut.DeleteGroup(Guid.NewGuid().ToString()));
//		}

//		[Test]
//		public void GetUserGroupList_WithoutParents_NoGroups_ReturnsEmptyList()
//		{
//			var sut = this.GetRepository();
			
//			var result = sut.GetUserGroupList(Guid.NewGuid().ToString(), false);

//			Assert.IsNotNull(result);
//			Assert.AreEqual(0, result.Count);
//		}

//		[Test]
//		public void GetUserGroupList_WitParents_NoGroups_ReturnsEmptyList()
//		{
//			var sut = this.GetRepository();

//			var result = sut.GetUserGroupList(Guid.NewGuid().ToString(), false);

//			Assert.IsNotNull(result);
//			Assert.AreEqual(0, result.Count);
//		}

//		[Test]
//		public void GetUserGroupList_ReturnsSingleGroup()
//		{
//			var sut = this.GetRepository();
//			var fixture = new Fixture();
//			var userId = fixture.Create<string>();
//			var group = sut.CreateGroup(fixture.Create<string>(),null);
//			var assignment = sut.CreateUserGroupAssignment(userId, group.Id);

//			var result = sut.GetUserGroupList(userId, false);

//			Assert.IsNotNull(result);
//			Assert.AreEqual(1, result.Count);
//			Assert.AreEqual(group.Id, result[0].Id);
//			Assert.AreEqual(group.GroupName, result[0].GroupName);
//			Assert.AreEqual(group.ParentGroupId, result[0].ParentGroupId);
//		}


//		[Test, Ignore]
//		public void HandlesGroupParents()
//		{
//			Assert.Inconclusive();
//		}

//	}
//}
