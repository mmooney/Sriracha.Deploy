using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using PagedList;
using Raven.Client;
using Raven.Client.Linq;
using Sriracha.Deploy.Data.Dto;
using System.ComponentModel;

namespace Sriracha.Deploy.RavenDB
{
    public static class RavenQueryExtensions
	{
        public static PagedSortedList<T> PageAndSort<T, SortKey>(this IRavenQueryable<T> query, ListOptions listOptions, Expression<Func<T, SortKey>> sortSelector)
        {
            RavenQueryStatistics stats;
            int pageNumber = listOptions.PageNumber.GetValueOrDefault(1);
            int pageSize = listOptions.PageSize.GetValueOrDefault(20);
            var startingQuery = query
                        .Statistics(out stats);
            IRavenQueryable<T> sortedQuery;
            if (listOptions.SortAscending.GetValueOrDefault())
            {
                sortedQuery = startingQuery.OrderBy(sortSelector);
            }
            else
            {
                sortedQuery = startingQuery.OrderByDescending(sortSelector);
            }
            var resultQuery = sortedQuery.Skip((pageNumber - 1) * pageSize)
                                            .Take(pageSize);
            var pagedList = new StaticPagedList<T>(resultQuery.ToList(), pageNumber, pageSize, stats.TotalResults);
            return new PagedSortedList<T>(pagedList, listOptions.SortField, listOptions.SortAscending.GetValueOrDefault());
        }
    }
}
