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
			public static TestData Create(int numberOfExistingPermissions)
			{
				var testData = new TestData
				{
					Fixture = new Fixture(),
					Repository = new Mock<IPermissionRepository>(),
					PermissionList = new List<PermissionItem>()
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
    }
}
