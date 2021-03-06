﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Embedded;
using Raven.Client.Indexes;

namespace Sriracha.Deploy.RavenDB.Tests
{
	public class EmbeddedRavenProvider
	{
		private static IDocumentStore _documentStore;
		public static IDocumentStore DocumentStore
		{
			get
			{
				lock (typeof(IDocumentStore))
				{
					if (_documentStore == null)
					{
						_documentStore = new EmbeddableDocumentStore()
						{
							RunInMemory = true,
							Conventions = new DocumentConvention
							{
								DefaultQueryingConsistency = ConsistencyOptions.AlwaysWaitForNonStaleResultsAsOfLastWrite,
                                DisableProfiling = true,
                                ShouldCacheRequest = url => false,
                                MaxNumberOfRequestsPerSession = 3000
                            },
						};
                        _documentStore.Initialize();
                        _documentStore.DisableAggressiveCaching();
                        _documentStore.DatabaseCommands.DisableAllCaching();
                        IndexCreation.CreateIndexes(typeof(RavenHelper).Assembly, _documentStore);
					}
				}
				return _documentStore;
			}
		}

		public EmbeddedRavenProvider()
		{

		}

		public IDocumentSession CreateSession()
		{
			return EmbeddedRavenProvider.DocumentStore.OpenSession();
		}
	}
}
