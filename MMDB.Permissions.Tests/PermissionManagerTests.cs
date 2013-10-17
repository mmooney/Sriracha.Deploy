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
			public List<UserGroupAssignment> UserGroupAssignmentList { get; set; }
			public List<PermissionDefinition> PermissionList { get; set; }
			public List<PermissionGroup> GroupList { get; set; }
			public static TestData Create(int numberOfExistingPermissions)
			{
				var testData = new TestData
				{
					Fixture = new Fixture(),
					Repository = new Mock<IPermissionRepository>(),
					UserGroupAssignmentList = new List<UserGroupAssignment>(),
					PermissionList = new List<PermissionDefinition>(),
					GroupList = new List<PermissionGroup>()
				};
				testData.Sut = new PermissionManager(testData.Repository.Object);
				for(var x = 0; x < numberOfExistingPermissions; x++)
				{
					var item = testData.Fixture.Create<PermissionDefinition>();
					testData.Repository.Setup(i=>i.GetPermissionDefinition(item.Id)).Returns(item);
					testData.Repository.Setup(i=>i.GetPermissionDefinitionByName(item.PermissionName)).Returns(item);
					testData.PermissionList.Add(item);
				}
				testData.Repository.Setup(i=>i.GetPermissionDefinitionList()).Returns(testData.PermissionList);
				testData.Repository.Setup(i=>i.GetUserGroupList(It.IsAny<string>(), It.IsAny<bool>())).Returns(new List<PermissionGroup>());
				return testData;
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

			var result = testData.Sut.GetPermissionDefinitionList();
			
			Assert.IsNotNull(result);
			Assert.AreEqual(2, result.Count);
			Assert.AreEqual(testData.PermissionList[0].Id, result[0].Id);
			Assert.AreEqual(testData.PermissionList[1].Id, result[1].Id);
		}

		[Test]
		public void CanCreatePermission()
		{
			var testData = TestData.Create(0);
			var item = testData.Fixture.Create<PermissionDefinition>();
			testData.Repository.Setup(i=>i.CreatePermissionDefinition(item.PermissionName, item.PermissionDisplayValue, null)).Returns(item);

			var result = testData.Sut.CreatePermissionDefinition(item.PermissionName, item.PermissionDisplayValue, null);

			Assert.IsNotNull(result);
			testData.Repository.Verify(i => i.CreatePermissionDefinition(item.PermissionName, item.PermissionDisplayValue, null), Times.Once());
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
    }
}
