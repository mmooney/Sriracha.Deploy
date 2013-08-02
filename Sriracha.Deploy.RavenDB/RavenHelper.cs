using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Indexes;

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
					DefaultQueryingConsistency = ConsistencyOptions.QueryYourWrites,
					ShouldCacheRequest = url => false 
				}
			};
			documentStore.Initialize();
			documentStore.DisableAggressiveCaching();
			documentStore.DatabaseCommands.DisableAllCaching();
			IndexCreation.CreateIndexes(typeof(RavenHelper).Assembly, documentStore);
			return documentStore;
		}
	}
}
