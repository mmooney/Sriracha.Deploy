using MMDB.ConnectionSettings;
using NUnit.Framework;
using Raven.Client;
using Sriracha.Deploy.Data.Tests.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.RavenDB.Tests
{
    public class RavenConnectionSettingRepositoryTests : ConnectionSettingRepositoryTests
    {
        protected IDocumentSession DocumentSession { get; private set; }

        [SetUp]
        public void RavenSetUp()
        {
            this.DocumentSession = EmbeddedRavenProvider.DocumentStore.OpenSession();
        }

        protected override IConnectionSettingRepository GetRepository()
        {
            return new RavenConnectionSettingRepository(this.DocumentSession);
        }

        [TearDown]
        public void RavenTearDown()
        {
            using (this.DocumentSession) { };
            this.DocumentSession = null;
        }
    }
}
