using System;
using System.Collections.Generic;
using System.Configuration;
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
			try 
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
			catch(Exception err)
			{
				var connStrObject = ConfigurationManager.ConnectionStrings["RavenDB"];
				if(connStrObject == null || string.IsNullOrWhiteSpace(connStrObject.ConnectionString))
				{
					throw new Exception("Error initializing RavenDB DocumentStore, ConnectionString RavenDB is missing", err);
				}
				else 
				{
					throw new Exception(string.Format("Error initializing RavenDB DocumentStore with ConnectionString \"{0}\"", connStrObject.ConnectionString), err);					
				}
			}
		}
	}
}
