using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Sriracha.Deploy.Data.Account;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Dto.Account;
using Sriracha.Deploy.Data.SystemSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Tests
{
    public class SystemSetterUpperTests
    {
        private class TestData
        {
            public Fixture Fixture { get; set; }
            public Mock<IUserManager> UserManager { get; set; }
            public Mock<ISystemRoleManager> SystemRoleManager { get; set; }
            public Mock<ISystemSettings> SystemSettings { get; set; }
            public string AdministratorUserName { get; set; }
            public string AdministratorPassword { get; set; }
            public string AdministratorEmailAddress { get; set; }
            public SrirachaUser ExistingUser { get; set; }
            public SystemRole AdministratorRole { get; set; }
            public ISystemSetterUpper Sut { get; set; }

            public static TestData Create(bool existingUser = false)
            {
                var fixture = new Fixture();
                var testData = new TestData
                {
                    Fixture = fixture,
                    UserManager = new Mock<IUserManager>(),
                    SystemRoleManager = new Mock<ISystemRoleManager>(),
                    SystemSettings = new Mock<ISystemSettings>(),
                    AdministratorUserName = fixture.Create<string>("UserName"),
                    AdministratorPassword = fixture.Create<string>("Password"),
                    AdministratorEmailAddress = fixture.Create<string>("EmailAddress"),
                    AdministratorRole = fixture.Build<SystemRole>()
                                                .With(i => i.RoleType, EnumSystemRoleType.Administrator)
                                                .Create()
                };
                if (existingUser)
                {
                    testData.ExistingUser = fixture.Build<SrirachaUser>()
                                                    .With(i => i.UserName, testData.AdministratorUserName)
                                                    .Create();
                    testData.UserManager.Setup(i => i.GetUser(testData.ExistingUser.Id)).Returns(testData.ExistingUser);
                    testData.UserManager.Setup(i => i.TryGetUserByUserName(testData.ExistingUser.UserName)).Returns(testData.ExistingUser);
                    testData.UserManager.Setup(i => i.GetUserByUserName(testData.ExistingUser.UserName)).Returns(testData.ExistingUser);
                }
                testData.SystemRoleManager.Setup(i => i.GetBuiltInRole(EnumSystemRoleType.Administrator)).Returns(testData.AdministratorRole);

                testData.Sut = new SystemSetterUpper(testData.UserManager.Object, testData.SystemRoleManager.Object, testData.SystemSettings.Object);

                return testData;
            }
        }

        public class SetupAdministratorUser
        {
            [Test]
            public void NullUserName_ThrowsException()
            {
                var testData = TestData.Create();

                Assert.Throws<ArgumentNullException>(()=>testData.Sut.SetupAdministratorUser(null, Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));
            }

            [Test]
            public void NullPassword_ThrowsException()
            {
                var testData = TestData.Create();

                Assert.Throws<ArgumentNullException>(()=>testData.Sut.SetupAdministratorUser(Guid.NewGuid().ToString(), null, Guid.NewGuid().ToString()));
            }

            [Test]
            public void NullEmailAddress_ThrowsException()
            {
                var testData = TestData.Create();

                Assert.Throws<ArgumentNullException>(()=>testData.Sut.SetupAdministratorUser(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), null));
            }

            [Test]
            public void NewUser_CreatesUser() 
            {
                var testData = TestData.Create(false);

                testData.Sut.SetupAdministratorUser(testData.AdministratorUserName, testData.AdministratorPassword, testData.AdministratorEmailAddress);

                testData.UserManager.Verify(i=>i.CreateUser(testData.AdministratorUserName, testData.AdministratorEmailAddress, testData.AdministratorPassword), Times.Once());
            }

            [Test]
            public void ExistingUser_UpdatesUser()
            {
                var testData = TestData.Create(true);
                var user = testData.Fixture.Build<SrirachaUser>()
                                    .With(i=>i.UserName, testData.AdministratorUserName)
                                    .Create();
                testData.UserManager.Setup(i=>i.TryGetUserByUserName(user.UserName)).Returns(user);

                testData.Sut.SetupAdministratorUser(testData.AdministratorUserName, testData.AdministratorPassword, testData.AdministratorEmailAddress);

                testData.UserManager.Verify(i => i.CreateUser(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never());
                testData.UserManager.Verify(i=>i.UpdateUser(user.Id, testData.AdministratorUserName, testData.AdministratorEmailAddress, testData.AdministratorPassword), Times.Once());
            }

            [Test]
            public void NewUser_AssignsToSystemRole()
            {
                var testData = TestData.Create(false);

                testData.Sut.SetupAdministratorUser(testData.AdministratorUserName, testData.AdministratorPassword, testData.AdministratorEmailAddress);

                testData.SystemRoleManager.Verify(i => i.UpdateSystemRole(testData.AdministratorRole.Id, testData.AdministratorRole.RoleName, testData.AdministratorRole.Permissions, 
                                                            It.Is<SystemRoleAssignments>(j=>j.UserNameList.Contains(testData.AdministratorUserName))), Times.Once());
            }

            [Test]
            public void ExistingUser_NotYetAssigned_AssignsToSystemRole()
            {
                var testData = TestData.Create(true);

                testData.Sut.SetupAdministratorUser(testData.AdministratorUserName, testData.AdministratorPassword, testData.AdministratorEmailAddress);

                testData.SystemRoleManager.Verify(i => i.UpdateSystemRole(testData.AdministratorRole.Id, testData.AdministratorRole.RoleName, testData.AdministratorRole.Permissions,
                                                            It.Is<SystemRoleAssignments>(j => j.UserNameList.Contains(testData.AdministratorUserName))), Times.Once());
            }

            [Test]
            public void ExistingUser_AlreadyAssigned_MaintainsAssginmentToSystemRole()
            {
                var testData = TestData.Create(true);
                testData.AdministratorRole.Assignments.UserNameList.Add(testData.AdministratorUserName);

                testData.Sut.SetupAdministratorUser(testData.AdministratorUserName, testData.AdministratorPassword, testData.AdministratorEmailAddress);

                testData.SystemRoleManager.Verify(i => i.UpdateSystemRole(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<SystemRolePermissions>(),It.IsAny<SystemRoleAssignments>()), Times.Never());
            }
        }

        public class GetAdministratorUser
        {
            [Test]
            public void NoUser_ReturnsNull()
            {
                var testData = TestData.Create(false);

                var result = testData.Sut.GetAdministratorUser();

                Assert.IsNull(result);
            }

            [Test]
            public void ExistingUser_ReturnsUser()
            {
                var testData = TestData.Create(true);
                testData.AdministratorRole.Assignments.UserNameList.Clear();
                testData.AdministratorRole.Assignments.UserNameList.Add(testData.AdministratorUserName);

                var result = testData.Sut.GetAdministratorUser();

                Assert.IsNotNull(result);
                Assert.AreEqual(testData.ExistingUser, result);
            }
        }

        public class SetupSystemSettings
        {
            [Test]
            public void SetsSelfRegistration()
            {
                var testData = TestData.Create();

                testData.Sut.SetupAdministratorUser(true, EnumPermissionAccess.Grant);

                testData.SystemSettings.VerifySet(i=>i.AllowSelfRegistration = true);
            }

            [Test]
            public void SetsDefaultAccess()
            {
                var testData = TestData.Create();

                testData.Sut.SetupAdministratorUser(true, EnumPermissionAccess.Grant);

                testData.SystemSettings.VerifySet(i => i.DefaultAccess = EnumPermissionAccess.Grant);
            }
        }
    }
}
