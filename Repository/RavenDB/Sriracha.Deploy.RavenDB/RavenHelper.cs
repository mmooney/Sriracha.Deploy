using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using MMDB.Shared;
using PagedList;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Indexes;
using Sriracha.Deploy.Data.Dto;

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
						DefaultQueryingConsistency = ConsistencyOptions.AlwaysWaitForNonStaleResultsAsOfLastWrite,
						DisableProfiling = true,
						ShouldCacheRequest = url => false,
						MaxNumberOfRequestsPerSession = 3000
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

		public static PagedList.IPagedList<T> QueryPageAndSort<T>(IDocumentSession documentSession, ListOptions listOptions, string defaultSortField, bool defaultSortAscending=true)
		{
			listOptions = listOptions ?? new ListOptions();
			RavenQueryStatistics stats;
			var documentQuery = documentSession.Advanced.LuceneQuery<T>()
							.Statistics(out stats);

			string sortField = StringHelper.IsNullOrEmpty(listOptions.SortField, defaultSortField);
			if(listOptions.SortAscending.GetValueOrDefault(defaultSortAscending))
			{
				documentQuery = documentQuery.OrderBy(sortField);
			}
			else 
			{
				documentQuery = documentQuery.OrderByDescending(sortField);
			}

			int pageNumber = listOptions.PageNumber.GetValueOrDefault(1);
			int pageSize = listOptions.PageSize.GetValueOrDefault(20);
			var resultQuery = documentQuery
						.Skip((pageNumber - 1) * pageSize)
						.Take(pageSize);
			return new StaticPagedList<T>(resultQuery.ToList(), pageSize, pageNumber, stats.TotalResults);
		}
	}
}
