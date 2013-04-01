using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using NUnit.Framework;
using Raven.Client;

namespace Sriracha.Deploy.RavenDB.Tests
{
	public class RavenTestBase
	{
		protected IDocumentSession DocumentSession { get; private set; }

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

	}
}
