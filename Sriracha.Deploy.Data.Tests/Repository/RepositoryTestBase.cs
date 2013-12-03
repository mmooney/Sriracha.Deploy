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
    public abstract class RepositoryTestBase<T>
	{
        [SetUp]
        public void SetUp()
        {
            this.Fixture = new Fixture();
            this.UserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity = new Mock<IUserIdentity>();
            this.UserIdentity.Setup(i => i.UserName).Returns(this.UserName);
            this.Logger = new Mock<NLog.Logger>();
        }

        protected Mock<IUserIdentity> UserIdentity { get; private set; }
        protected Mock<NLog.Logger> Logger { get; private set; }
        protected string UserName { get; private set; }
        protected Fixture Fixture { get; private set; }

        protected abstract T GetRepository();

        protected void AssertIsRecent(DateTime dateTime)
        {
            AssertHelpers.AssertIsRecent(dateTime);
        }

        protected void AssertDateEqual(DateTime expected, DateTime actual)
        {
            AssertHelpers.AssertDateEqual(expected, actual);
        }

    }
}
