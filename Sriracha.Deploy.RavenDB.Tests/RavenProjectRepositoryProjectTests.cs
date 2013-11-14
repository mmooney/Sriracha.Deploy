using System;
using System.Collections.Generic;
using System.Collections;
using NUnit.Framework;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Repository;
using System.Linq;
using MMDB.Shared;
using Moq;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Dto.Project;
using Sriracha.Deploy.Data.Tests.Repository;
using Raven.Client;

namespace Sriracha.Deploy.RavenDB.Tests
{
	public class RavenProjectRepositoryProjectTests : ProjectRepositoryBaseProjectTests
	{
        protected IDocumentSession DocumentSession { get; private set; }

        [SetUp]
        public void RavenSetUp()
        {
            this.DocumentSession = EmbeddedRavenProvider.DocumentStore.OpenSession();
        }
        
        protected override IProjectRepository GetRepository()
        {
            return new RavenProjectRepository(this.DocumentSession, this.UserIdentity.Object, this.Logger.Object);
        }

        [TearDown]
        public void RavenTearDown()
        {
            using (this.DocumentSession) { };
            this.DocumentSession = null;
        }
    }
}
