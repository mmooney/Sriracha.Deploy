using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;

namespace MMDB.Permissions.Tests
{
    public class PermissionManagerTests
    {
		private class TestData
		{
			public Fixture Fixture {  get; set; }
			public Mock<IPermissionRepository> Repository { get; set; }
			public IPermissionManager Sut { get; set; }
			public List<PermissionItem> PermissionList { get; set; }
			public List<UserGroupAssignment> UserGroupAssignmentList { get; set; }
			public List<PermissionGroup> GroupList { get; set; }
			public static TestData Create(int numberOfExistingPermissions)
			{
				var testData = new TestData
				{
					Fixture = new Fixture(),
					Repository = new Mock<IPermissionRepository>(),
					PermissionList = new List<PermissionItem>(),
					UserGroupAssignmentList = new List<UserGroupAssignment>(),
					GroupList = new List<PermissionGroup>()
				};
				testData.Sut = new PermissionManager(testData.Repository.Object);
				for(var x = 0; x < numberOfExistingPermissions; x++)
				{
					var item = testData.Fixture.Create<PermissionItem>();
					testData.Repository.Setup(i=>i.GetPermission(item.Id)).Returns(item);
					testData.Repository.Setup(i=>i.GetPermissionByName(item.PermissionName)).Returns(item);
					testData.PermissionList.Add(item);
				}
				testData.Repository.Setup(i=>i.GetPermissionList()).Returns(testData.PermissionList);
				testData.Repository.Setup(i=>i.GetUserGroupList(It.IsAny<string>(), It.IsAny<bool>())).Returns(new List<PermissionGroup>());
				return testData;
			}

			public UserPermissionAssignment PermissionAssignedToUser(PermissionItem permissionItem, string userId, EnumPermissionAccess access)
			{
				var returnValue = new UserPermissionAssignment
				{
					Id = this.Fixture.Create<string>(),
					PermissionId = permissionItem.Id,
					UserId = userId,
					Access = access
				};
				this.Repository.Setup(i=>i.TryGetUserPermissionAssignment(permissionItem.Id, userId)).Returns(returnValue);
				this.Repository.Setup(i=>i.DeleteUserPermissionAssignment(returnValue.Id)).Returns(returnValue);
				this.Repository.Setup(i=>i.UpdateUserPermissionAssignment(returnValue.Id, It.IsAny<EnumPermissionAccess>()))
								.Returns(
									(string id, EnumPermissionAccess innerAccess) 
									=> 
									{ 
										returnValue.Access = innerAccess; 
										return returnValue; 
									});
				return returnValue;
			}

			public PermissionGroup CreateGroup()
			{
				var returnValue = this.Fixture.Create<PermissionGroup>();
				this.Repository.Setup(i=>i.DeleteGroup(returnValue.Id)).Returns(returnValue);
				this.Repository.Setup(i=>i.GetGroup(returnValue.Id)).Returns(returnValue);
				this.GroupList.Add(returnValue);
				this.Repository.Setup(i=>i.GetGroupList()).Returns(this.GroupList);
				return returnValue;
			}

			public GroupPermissionAssignment PermissionAssignedToGroup(PermissionItem permissionItem, string groupId, EnumPermissionAccess access)
			{
				var returnValue = new GroupPermissionAssignment
				{
					Id = this.Fixture.Create<string>(),
					PermissionId = permissionItem.Id,
					GroupId = groupId,
					Access = access
				};
				this.Repository.Setup(i=>i.TryGetGroupPermissionAssignment(permissionItem.Id, groupId)).Returns(returnValue);
				this.Repository.Setup(i=>i.DeleteGroupPermissionAssignment(returnValue.Id)).Returns(returnValue);
				this.Repository.Setup(i=>i.UpdateGroupPermissionAssignment(returnValue.Id, It.IsAny<EnumPermissionAccess>()))
								.Returns(
									(string id, EnumPermissionAccess innerAccess) 
									=> 
									{ 
										returnValue.Access = innerAccess; 
										return returnValue; 
									});
				return returnValue;
			}

			public object UserAssignedToGroup(string userId, PermissionGroup group)
			{
				var returnValue = new UserGroupAssignment
				{
					UserId = userId,
					GroupId = group.Id
				};
				this.UserGroupAssignmentList.Add(returnValue);
				var userGroupList = (from g in this.GroupList
										join uga in this.UserGroupAssignmentList on g.Id equals uga.GroupId
										where uga.UserId == userId
										select g).Distinct().ToList();
				this.Repository.Setup(i=>i.GetUserGroupList(userId, It.IsAny<bool>())).Returns(userGroupList);
				this.Repository.Setup(i=>i.TryGetUserGroupAssignment(userId, group.Id)).Returns(returnValue);
				return returnValue;
			}
		}

		[Test]
		public void GetPermissionList()
		{
			var testData = TestData.Create(2);

			var result = testData.Sut.GetPermissionList();
			
			Assert.IsNotNull(result);
			Assert.AreEqual(2, result.Count);
			Assert.AreEqual(testData.PermissionList[0].Id, result[0].Id);
			Assert.AreEqual(testData.PermissionList[1].Id, result[1].Id);
		}

		[Test]
		public void CanCreatePermission()
		{
			var testData = TestData.Create(0);
			var item = testData.Fixture.Create<PermissionItem>();
			testData.Repository.Setup(i=>i.CreatePermission(item.PermissionName, item.PermissionDisplayValue)).Returns(item);

			var result = testData.Sut.CreatePermission(item.PermissionName, item.PermissionDisplayValue);

			Assert.IsNotNull(result);
			testData.Repository.Verify(i=>i.CreatePermission(item.PermissionName, item.PermissionDisplayValue), Times.Once());
		}

		[Test]
		public void CanAssignPermissionToUser()
		{
			var testData = TestData.Create(1);
			string userId = testData.Fixture.Create<string>();
			
			var result = testData.Sut.AssignPermissionToUser(testData.PermissionList[0].Id, userId, EnumPermissionAccess.Grant);

			testData.Repository.Verify(i=>i.CreateUserPermissionAssignment(testData.PermissionList[0].Id, userId, EnumPermissionAccess.Grant), Times.Once());
		}

		[Test]
		public void CanUpdatePermissionForUser()
		{
			var testData = TestData.Create(1);
			string userId = testData.Fixture.Create<string>();
			var assignment = testData.PermissionAssignedToUser(testData.PermissionList[0], userId, EnumPermissionAccess.Grant);
			
			var result = testData.Sut.AssignPermissionToUser(testData.PermissionList[0].Id, userId, EnumPermissionAccess.Deny);

			Assert.AreEqual(assignment, result);
			Assert.AreEqual(EnumPermissionAccess.Deny, result.Access);
			testData.Repository.Verify(i=>i.UpdateUserPermissionAssignment(assignment.Id, EnumPermissionAccess.Deny), Times.Once());
		}

		[Test]
		public void CanDeletePermissionForUser()
		{
			var testData = TestData.Create(1);
			string userId = testData.Fixture.Create<string>();
			var assignment = testData.PermissionAssignedToUser(testData.PermissionList[0], userId, EnumPermissionAccess.Grant);
			
			var result = testData.Sut.DeletePermissionForUser(testData.PermissionList[0].Id, userId);

			Assert.AreEqual(assignment, result);
			testData.Repository.Verify(i=>i.DeleteUserPermissionAssignment(assignment.Id), Times.Once());
		}

		[Test]
		public void CanCreateGroup()
		{
			var testData = TestData.Create(0);
			var group = testData.Fixture.Create<PermissionGroup>();
			testData.Repository.Setup(i=>i.CreateGroup(group.GroupName, group.ParentGroupId)).Returns(group);

			var result = testData.Sut.CreateGroup(group.GroupName, group.ParentGroupId);

			Assert.IsNotNull(result);
			Assert.AreEqual(group, result);
			testData.Repository.Verify(i=>i.CreateGroup(group.GroupName, group.ParentGroupId), Times.Once());
		}

		[Test]
		public void CanDeleteGroup()
		{ 
			var testData = TestData.Create(1);
			var group = testData.CreateGroup();
			
			var result = testData.Sut.DeleteGroup(group.Id);

			Assert.IsNotNull(result);
			Assert.AreEqual(group, result);
			testData.Repository.Verify(i=>i.DeleteGroup(group.Id), Times.Once());
		}

		[Test]
		public void CanAssignPermissionToGroup()
		{
			var testData = TestData.Create(1);
			var group = testData.CreateGroup();
			
			var result = testData.Sut.AssignPermissionToGroup(testData.PermissionList[0].Id, group.Id, EnumPermissionAccess.Grant);

			testData.Repository.Verify(i=>i.CreateGroupPermissionAssignment(testData.PermissionList[0].Id, group.Id, EnumPermissionAccess.Grant), Times.Once());
		}

		[Test]
		public void CanUpdatePermissionForGroup()
		{
			var testData = TestData.Create(1);
			var group = testData.CreateGroup();
			var assignment = testData.PermissionAssignedToGroup(testData.PermissionList[0], group.Id, EnumPermissionAccess.Grant);
			
			var result = testData.Sut.AssignPermissionToGroup(testData.PermissionList[0].Id, group.Id, EnumPermissionAccess.Deny);

			Assert.AreEqual(assignment, result);
			Assert.AreEqual(EnumPermissionAccess.Deny, result.Access);
			testData.Repository.Verify(i=>i.UpdateGroupPermissionAssignment(assignment.Id, EnumPermissionAccess.Deny), Times.Once());
		}

		[Test]
		public void CanDeletePermissionForGroup()
		{ 
			var testData = TestData.Create(1);
			var group = testData.CreateGroup();
			var assignment = testData.PermissionAssignedToGroup(testData.PermissionList[0], group.Id, EnumPermissionAccess.Grant);
			
			var result = testData.Sut.DeletePermissionForGroup(testData.PermissionList[0].Id, group.Id);

			Assert.AreEqual(assignment, result);
			testData.Repository.Verify(i=>i.DeleteGroupPermissionAssignment(assignment.Id), Times.Once());
		}

		[Test]
		public void CanVerifyUserHasUserPermission()
		{
			var testData = TestData.Create(1);
			string userId = testData.Fixture.Create<string>();
			var permissionAssignment = testData.PermissionAssignedToUser(testData.PermissionList[0], userId, EnumPermissionAccess.Grant);

			bool result = testData.Sut.HasPermission(userId, testData.PermissionList[0].Id);

			Assert.IsTrue(result);
		}

		[Test]
		public void CanVerifyUserHasGroupPermission()
		{
			var testData = TestData.Create(1);
			string userId = testData.Fixture.Create<string>();
			var group = testData.CreateGroup();
			var userGroupAssignment = testData.UserAssignedToGroup(userId, group);
			var permissionAssignment = testData.PermissionAssignedToGroup(testData.PermissionList[0], group.Id, EnumPermissionAccess.Grant);

			bool result = testData.Sut.HasPermission(userId, testData.PermissionList[0].Id);

			Assert.IsTrue(result);
		}

		[Test]
		public void CanVerifyUserDoesNotHavePermission()
		{
			var testData = TestData.Create(1);
			string userId = testData.Fixture.Create<string>();

			bool result = testData.Sut.HasPermission(userId, testData.PermissionList[0].Id);

			Assert.IsFalse(result);
		}

		[Test]
		public void CanVerifyUserHasDeniedUserPermission()
		{
			var testData = TestData.Create(1);
			string userId = testData.Fixture.Create<string>();
			var permissionAssignment = testData.PermissionAssignedToUser(testData.PermissionList[0], userId, EnumPermissionAccess.Deny);

			bool result = testData.Sut.HasPermission(userId, testData.PermissionList[0].Id);

			Assert.IsFalse(result);
		}

		[Test]
		public void CanVerifyUserHasDeniedGroupPermission()
		{
			var testData = TestData.Create(1);
			string userId = testData.Fixture.Create<string>();
			var group = testData.CreateGroup();
			var userGroupAssignment = testData.UserAssignedToGroup(userId, group);
			var permissionAssignment = testData.PermissionAssignedToGroup(testData.PermissionList[0], group.Id, EnumPermissionAccess.Deny);

			bool result = testData.Sut.HasPermission(userId, testData.PermissionList[0].Id);

			Assert.IsFalse(result);
		}


		[Test]
		public void GetEffectiveUserPermissionList_IncludesUnassignedPermissions()
		{
			var testData = TestData.Create(1);
			string userId = testData.Fixture.Create<string>();

			var result = testData.Sut.GetEffectiveUserPermissionList(userId);

			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.Count);
			Assert.IsFalse(result[0].HasPermission);
			Assert.AreEqual(testData.PermissionList[0].Id, result[0].PermissionId);
		}

		[Test]
		public void GetEffectiveUserPermissionList_IncludesUserPermission()
		{
			var testData = TestData.Create(1);
			string userId = testData.Fixture.Create<string>();
			var permissionAssignment = testData.PermissionAssignedToUser(testData.PermissionList[0], userId, EnumPermissionAccess.Grant);

			var result = testData.Sut.GetEffectiveUserPermissionList(userId);

			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.Count);
			Assert.IsTrue(result[0].HasPermission);
			Assert.IsNotNull(result[0].UserPermissionAssignment);
			Assert.AreEqual(testData.PermissionList[0].Id, result[0].UserPermissionAssignment.PermissionId);
			Assert.AreEqual(EnumPermissionAccess.Grant, result[0].UserPermissionAssignment.Access);
		}

		[Test]
		public void GetEffectiveUserPermissionList_IncludesSingleGroupPermission()
		{
			var testData = TestData.Create(1);
			string userId = testData.Fixture.Create<string>();
			var group = testData.CreateGroup();
			var userGroupAssignment = testData.UserAssignedToGroup(userId, group);
			var permissionAssignment = testData.PermissionAssignedToGroup(testData.PermissionList[0], group.Id, EnumPermissionAccess.Grant);

			var result = testData.Sut.GetEffectiveUserPermissionList(userId);

			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.Count);
			Assert.IsTrue(result[0].HasPermission);
			Assert.IsNotNull(result[0].GroupPermissionAssignmentList);
			Assert.AreEqual(permissionAssignment.PermissionId, result[0].PermissionId);
			Assert.AreEqual(1, result[0].GroupPermissionAssignmentList.Count);
			Assert.AreEqual(group.Id, result[0].GroupPermissionAssignmentList[0].GroupId);
			Assert.AreEqual(EnumPermissionAccess.Grant, result[0].GroupPermissionAssignmentList[0].Access);
		}

		[Test]
		public void GetEffectiveUserPermissionList_IncludesMultipleGroupPermissions()
		{
			var testData = TestData.Create(1);
			string userId = testData.Fixture.Create<string>();
			var group1 = testData.CreateGroup();
			var userGroupAssignment1 = testData.UserAssignedToGroup(userId, group1);
			var permissionAssignment1 = testData.PermissionAssignedToGroup(testData.PermissionList[0], group1.Id, EnumPermissionAccess.Grant);
			var group2 = testData.CreateGroup();
			var userGroupAssignment2 = testData.UserAssignedToGroup(userId, group2);
			var permissionAssignment2 = testData.PermissionAssignedToGroup(testData.PermissionList[0], group2.Id, EnumPermissionAccess.Grant);

			var result = testData.Sut.GetEffectiveUserPermissionList(userId);

			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.Count);
			Assert.IsTrue(result[0].HasPermission);
			Assert.IsNotNull(result[0].GroupPermissionAssignmentList);
			Assert.AreEqual(permissionAssignment1.PermissionId, result[0].PermissionId);
			Assert.AreEqual(2, result[0].GroupPermissionAssignmentList.Count);
			Assert.AreEqual(group1.Id, result[0].GroupPermissionAssignmentList[0].GroupId);
			Assert.AreEqual(EnumPermissionAccess.Grant, result[0].GroupPermissionAssignmentList[0].Access);
			Assert.AreEqual(group2.Id, result[0].GroupPermissionAssignmentList[1].GroupId);
			Assert.AreEqual(EnumPermissionAccess.Grant, result[0].GroupPermissionAssignmentList[1].Access);
		}

		[Test]
		public void GetEffectiveUserPermissionList_MultipleGroupPermissions_DenyFirstWins()
		{
			var testData = TestData.Create(1);
			string userId = testData.Fixture.Create<string>();
			var group1 = testData.CreateGroup();
			var userGroupAssignment1 = testData.UserAssignedToGroup(userId, group1);
			var permissionAssignment1 = testData.PermissionAssignedToGroup(testData.PermissionList[0], group1.Id, EnumPermissionAccess.Grant);
			var group2 = testData.CreateGroup();
			var userGroupAssignment2 = testData.UserAssignedToGroup(userId, group2);
			var permissionAssignment2 = testData.PermissionAssignedToGroup(testData.PermissionList[0], group2.Id, EnumPermissionAccess.Deny);

			var result = testData.Sut.GetEffectiveUserPermissionList(userId);

			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.Count);
			Assert.IsFalse(result[0].HasPermission);
			Assert.IsNotNull(result[0].GroupPermissionAssignmentList);
			Assert.AreEqual(permissionAssignment1.PermissionId, result[0].PermissionId);
			Assert.AreEqual(2, result[0].GroupPermissionAssignmentList.Count);
			Assert.AreEqual(group1.Id, result[0].GroupPermissionAssignmentList[0].GroupId);
			Assert.AreEqual(EnumPermissionAccess.Grant, result[0].GroupPermissionAssignmentList[0].Access);
			Assert.AreEqual(group2.Id, result[0].GroupPermissionAssignmentList[1].GroupId);
			Assert.AreEqual(EnumPermissionAccess.Deny, result[0].GroupPermissionAssignmentList[1].Access);
		}

		[Test]
		public void GetEffectiveUserPermissionList_MultipleGroupPermissions_DenySecondWins()
		{
			var testData = TestData.Create(1);
			string userId = testData.Fixture.Create<string>();
			var group1 = testData.CreateGroup();
			var userGroupAssignment1 = testData.UserAssignedToGroup(userId, group1);
			var permissionAssignment1 = testData.PermissionAssignedToGroup(testData.PermissionList[0], group1.Id, EnumPermissionAccess.Deny);
			var group2 = testData.CreateGroup();
			var userGroupAssignment2 = testData.UserAssignedToGroup(userId, group2);
			var permissionAssignment2 = testData.PermissionAssignedToGroup(testData.PermissionList[0], group2.Id, EnumPermissionAccess.Grant);

			var result = testData.Sut.GetEffectiveUserPermissionList(userId);

			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.Count);
			Assert.IsFalse(result[0].HasPermission);
			Assert.IsNotNull(result[0].GroupPermissionAssignmentList);
			Assert.AreEqual(permissionAssignment1.PermissionId, result[0].PermissionId);
			Assert.AreEqual(2, result[0].GroupPermissionAssignmentList.Count);
			Assert.AreEqual(group1.Id, result[0].GroupPermissionAssignmentList[0].GroupId);
			Assert.AreEqual(EnumPermissionAccess.Deny, result[0].GroupPermissionAssignmentList[0].Access);
			Assert.AreEqual(group2.Id, result[0].GroupPermissionAssignmentList[1].GroupId);
			Assert.AreEqual(EnumPermissionAccess.Grant, result[0].GroupPermissionAssignmentList[1].Access);
		}

		[Test]
		public void GetEffectiveUserPermissionList_MultipleGroupAndUserPermissions_DenyGroupWins()
		{
			var testData = TestData.Create(1);
			string userId = testData.Fixture.Create<string>();
			var userPermissionAssignment = testData.PermissionAssignedToUser(testData.PermissionList[0], userId, EnumPermissionAccess.Grant);
			var group1 = testData.CreateGroup();
			var userGroupAssignment1 = testData.UserAssignedToGroup(userId, group1);
			var groupPermissionAssignment1 = testData.PermissionAssignedToGroup(testData.PermissionList[0], group1.Id, EnumPermissionAccess.Deny);
			var group2 = testData.CreateGroup();
			var userGroupAssignment2 = testData.UserAssignedToGroup(userId, group2);
			var groupPermissionAssignment2 = testData.PermissionAssignedToGroup(testData.PermissionList[0], group2.Id, EnumPermissionAccess.Grant);

			var result = testData.Sut.GetEffectiveUserPermissionList(userId);

			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.Count);
			Assert.IsFalse(result[0].HasPermission);
			Assert.IsNotNull(result[0].UserPermissionAssignment);
			Assert.AreEqual(testData.PermissionList[0].Id, result[0].UserPermissionAssignment.PermissionId);
			Assert.AreEqual(EnumPermissionAccess.Grant, result[0].UserPermissionAssignment.Access);
			Assert.IsNotNull(result[0].GroupPermissionAssignmentList);
			Assert.AreEqual(groupPermissionAssignment1.PermissionId, result[0].PermissionId);
			Assert.AreEqual(2, result[0].GroupPermissionAssignmentList.Count);
			Assert.AreEqual(group1.Id, result[0].GroupPermissionAssignmentList[0].GroupId);
			Assert.AreEqual(EnumPermissionAccess.Deny, result[0].GroupPermissionAssignmentList[0].Access);
			Assert.AreEqual(group2.Id, result[0].GroupPermissionAssignmentList[1].GroupId);
			Assert.AreEqual(EnumPermissionAccess.Grant, result[0].GroupPermissionAssignmentList[1].Access);
		}

		[Test]
		public void GetEffectiveUserPermissionList_MultipleGroupAndUserPermissions_DenyUserWins()
		{
			var testData = TestData.Create(1);
			string userId = testData.Fixture.Create<string>();
			var userPermissionAssignment = testData.PermissionAssignedToUser(testData.PermissionList[0], userId, EnumPermissionAccess.Deny);
			var group1 = testData.CreateGroup();
			var userGroupAssignment1 = testData.UserAssignedToGroup(userId, group1);
			var groupPermissionAssignment1 = testData.PermissionAssignedToGroup(testData.PermissionList[0], group1.Id, EnumPermissionAccess.Grant);
			var group2 = testData.CreateGroup();
			var userGroupAssignment2 = testData.UserAssignedToGroup(userId, group2);
			var groupPermissionAssignment2 = testData.PermissionAssignedToGroup(testData.PermissionList[0], group2.Id, EnumPermissionAccess.Grant);

			var result = testData.Sut.GetEffectiveUserPermissionList(userId);

			Assert.IsNotNull(result);
			Assert.AreEqual(1, result.Count);
			Assert.IsFalse(result[0].HasPermission);
			Assert.IsNotNull(result[0].UserPermissionAssignment);
			Assert.AreEqual(testData.PermissionList[0].Id, result[0].UserPermissionAssignment.PermissionId);
			Assert.AreEqual(EnumPermissionAccess.Deny, result[0].UserPermissionAssignment.Access);
			Assert.IsNotNull(result[0].GroupPermissionAssignmentList);
			Assert.AreEqual(groupPermissionAssignment1.PermissionId, result[0].PermissionId);
			Assert.AreEqual(2, result[0].GroupPermissionAssignmentList.Count);
			Assert.AreEqual(group1.Id, result[0].GroupPermissionAssignmentList[0].GroupId);
			Assert.AreEqual(EnumPermissionAccess.Grant, result[0].GroupPermissionAssignmentList[0].Access);
			Assert.AreEqual(group2.Id, result[0].GroupPermissionAssignmentList[1].GroupId);
			Assert.AreEqual(EnumPermissionAccess.Grant, result[0].GroupPermissionAssignmentList[1].Access);
		}

		[Test]
		public void UsesPermissionCacheObject()
		{
			Assert.Inconclusive();
		}
    }
}
