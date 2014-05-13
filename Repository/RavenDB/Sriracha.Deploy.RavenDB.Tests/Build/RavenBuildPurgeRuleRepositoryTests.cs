using NUnit.Framework;
using Raven.Client;
using Sriracha.Deploy.Data.Repository;
using Sriracha.Deploy.Data.Tests.Repository.Build;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.RavenDB.Tests.Build
{
    public class RavenBuildPurgeRuleRepositoryTests : BuildPurgeRuleRepositoryTests
    {
        protected IDocumentSession DocumentSession { get; private set; }

        [SetUp]
        public void RavenSetUp()
        {
            this.DocumentSession = EmbeddedRavenProvider.DocumentStore.OpenSession();
        }

        protected override IBuildPurgeRuleRepository GetRepository()
        {
            return new RavenBuildPurgeRuleRepository(this.DocumentSession, this.UserIdentity.Object);
        }

        [TearDown]
        public void RavenTearDown()
        {
            using (this.DocumentSession) { };
            this.DocumentSession = null;
        }
    }
}
