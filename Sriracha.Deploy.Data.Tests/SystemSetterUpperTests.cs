using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Sriracha.Deploy.Data.Account;
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
            private Fixture Fixture { get; set; }
            private Mock<IUserManager> UserManager { get; set; }
            public Mock<ISystemRoleManager> SystemRoleManager { get; set; }
            public string AdministratorUserName { get; set; }
            public string AdministratorPassword { get; set; }
            public string AdministratorEmailAddress { get; set; }
            public ISystemSetterUpper Sut { get; set; }

            public static TestData Create()
            {
                var fixture = new Fixture();
                var testData = new TestData
                {
                    Fixture = fixture,
                    UserManager = new Mock<IUserManager>(),
                    AdministratorUserName = fixture.Create<string>(),
                    AdministratorPassword = fixture.Create<string>(),
                    AdministratorEmailAddress = fixture.Create<string>(),
                    SystemRoleManager = new Mock<ISystemRoleManager>()
                };
                testData.Sut = new SystemSetterUpper(testData.UserManager.Object, testData.SystemRoleManager.Object);

                return testData;
            }
        }

        [Test]
        public void NullUserName_ThrowsException()
        {
            var testData = TestData.Create();

            Assert.Throws<ArgumentNullException>(()=>testData.Sut.SetupSystem(null, Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));
        }

        [Test]
        public void NullPassword_ThrowsException()
        {
            var testData = TestData.Create();

            Assert.Throws<ArgumentNullException>(()=>testData.Sut.SetupSystem(Guid.NewGuid().ToString(), null, Guid.NewGuid().ToString()));
        }

        [Test]
        public void NullEmailAddress_ThrowsException()
        {
            var testData = TestData.Create();

            Assert.Throws<ArgumentNullException>(()=>testData.Sut.SetupSystem(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), null));
        }

        [Test]
        public void NewUser_CreatesUser() 
        {
            var testData = TestData.Create();

            testData.Sut.SetupSystem(testData.AdministratorUserName, testData.AdministratorPassword, testData.AdministratorEmailAddress);
        }
    }
}
