using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using PagedList;
using Raven.Client;
using Raven.Client.Linq;
using Sriracha.Deploy.Data.Dto;

namespace Sriracha.Deploy.RavenDB
{
	public static class RavenQueryExtensions
	{
		public static IPagedList<T> PageAndSort<T, SortKey>(this IRavenQueryable<T> query, ListOptions listOptions, Expression<Func<T, SortKey>> sortSelector)
		{
			RavenQueryStatistics stats;
			int pageNumber = listOptions.PageNumber.GetValueOrDefault(1);
			int pageSize = listOptions.PageSize.GetValueOrDefault(20);
			var resultQuery = query
						.Statistics(out stats)
						.Skip((pageNumber- 1) * pageSize)
						.OrderBy(sortSelector)
						.Take(pageSize);
			return new StaticPagedList<T>(query.ToList(), pageSize, pageNumber, stats.TotalResults);
		}
	}
}
