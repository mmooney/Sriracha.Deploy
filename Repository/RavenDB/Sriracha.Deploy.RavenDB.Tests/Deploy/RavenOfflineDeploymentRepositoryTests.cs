using NUnit.Framework;
using Raven.Client;
using Sriracha.Deploy.Data.Repository;
using Sriracha.Deploy.Data.Tests.Repository.Deploy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.RavenDB.Tests.Deploy
{
    public class RavenOfflineDeploymentRepositoryTests : OfflineDeploymentRepositoryTests
    {
        protected IDocumentSession DocumentSession { get; private set; }

        [SetUp]
        public void RavenSetUp()
        {
            this.DocumentSession = EmbeddedRavenProvider.DocumentStore.OpenSession();
        }

        protected override IOfflineDeploymentRepository GetRepository()
        {
            return new RavenOfflineDeploymentRepository(this.DocumentSession, this.UserIdentity.Object);
        }

        [TearDown]
        public void RavenTearDown()
        {
            using (this.DocumentSession) { };
            this.DocumentSession = null;
        }
    }
}
