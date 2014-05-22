using Common.Logging;
using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Tests.Repository
{
    [TestFixture]
    [Category("DBIntegration")]
    public abstract class RepositoryTestBase<T>
	{
        [SetUp]
        public void SetUp()
        {
            this.Fixture = new Fixture();
            this.UserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity = new Mock<IUserIdentity>();
            this.UserIdentity.Setup(i => i.UserName).Returns(this.UserName);
            this.Logger = new Mock<ILog>();
        }

        protected Mock<IUserIdentity> UserIdentity { get; private set; }
        protected Mock<ILog> Logger { get; private set; }
        protected string UserName { get; private set; }
        protected Fixture Fixture { get; private set; }

        protected abstract T GetRepository();

        protected string CreateNewUserName()
        {
            this.UserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(this.UserName);
            return this.UserName;
        }

        protected void AssertIsRecent(DateTime? dateTime)
        {
            AssertHelpers.AssertIsRecent(dateTime);
        }

        protected void AssertDateEqual(DateTime? expected, DateTime? actual)
        {
            AssertHelpers.AssertDateEqual(expected, actual);
        }

    }
}
