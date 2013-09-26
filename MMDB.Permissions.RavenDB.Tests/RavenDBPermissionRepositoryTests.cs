using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MMDB.Permissions.Tests;
using NUnit.Framework;
using Raven.Client;

namespace MMDB.Permissions.RavenDB.Tests
{
	[TestFixture]
	public class RavenDBPermissionRepositoryTests : PermissionRepositoryBaseTests
	{
		private IDocumentSession DocumentSession { get; set; }

		[SetUp]
		public void SetUp()
		{
			this.DocumentSession = EmbeddedRavenProvider.DocumentStore.OpenSession();
		}

		[TearDown]
		public void TearDown()
		{
			using (this.DocumentSession) {};
			this.DocumentSession = null;
		}

		protected override IPermissionRepository GetRepository()
		{
			return new RavenDBPermissionRepository(this.DocumentSession);
		}
	}
}
