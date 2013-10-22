using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.Shared;
using PagedList;
using Raven.Client;
using Sriracha.Deploy.Data.Dto;

namespace Sriracha.Deploy.RavenDB
{
	public static class DocumentSessionExtensions
	{
		public static IPagedList<T> QueryPageAndSort<T>(this IDocumentSession documentSession, ListOptions listOptions, string defaultSortField, bool defaultSortAscending = true, Func<IDocumentQuery<T>, IDocumentQuery<T>> filterFunc=null)
		{
			listOptions = listOptions ?? new ListOptions();
			RavenQueryStatistics stats;
			var documentQuery = documentSession.Advanced.LuceneQuery<T>()
							.Statistics(out stats);

			
			if(filterFunc != null)
			{
				documentQuery = filterFunc(documentQuery);
			}
			listOptions.SortField = StringHelper.IsNullOrEmpty(listOptions.SortField, defaultSortField);
			listOptions.SortAscending = listOptions.SortAscending.GetValueOrDefault(defaultSortAscending);
			if (listOptions.SortAscending.Value)
			{
				documentQuery = documentQuery.OrderBy(listOptions.SortField);
			}
			else
			{
				documentQuery = documentQuery.OrderByDescending(listOptions.SortField);
			}

			int pageNumber = listOptions.PageNumber.GetValueOrDefault(1);
			int pageSize = listOptions.PageSize.GetValueOrDefault(20);

			var resultQuery = documentQuery
						.Skip((pageNumber - 1) * pageSize)
						.Take(pageSize);
			var resultList = resultQuery.ToList();
			return new StaticPagedList<T>(resultList, pageNumber, pageSize, stats.TotalResults);
		}
	}
}
