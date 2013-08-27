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
		public static IPagedList<T> QueryPageAndSort<T>(this IDocumentSession documentSession, ListOptions listOptions, string defaultSortField, bool defaultSortAscending = true)
		{
			listOptions = listOptions ?? new ListOptions();
			RavenQueryStatistics stats;
			var documentQuery = documentSession.Advanced.LuceneQuery<T>()
							.Statistics(out stats);

			string sortField = StringHelper.IsNullOrEmpty(listOptions.SortField, defaultSortField);
			if (listOptions.SortAscending.GetValueOrDefault(defaultSortAscending))
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
