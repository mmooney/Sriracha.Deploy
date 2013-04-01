using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client;
using Raven.Client.Document;

namespace Sriracha.Deploy.RavenDB
{
	public static class RavenHelper
	{
		public static IDocumentStore CreateDocumentStore()
		{
			var documentStore = new DocumentStore()
			{
				ConnectionStringName = "RavenDB",
				Conventions = new DocumentConvention
				{
					DefaultQueryingConsistency = ConsistencyOptions.QueryYourWrites
				}
			};
			return documentStore.Initialize();
		}
	}
}
