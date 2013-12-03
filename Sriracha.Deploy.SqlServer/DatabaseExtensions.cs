using PagedList;
using Sriracha.Deploy.Data.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.SqlServer
{
    public static class DatabaseExtensions
    {
        public static PagedSortedList<T> PageAndSort<T>(this PetaPoco.Database db, ListOptions listOptions, PetaPoco.Sql sql)
        {
            int pageNumber = listOptions.PageNumber.GetValueOrDefault(1);
            int pageSize = listOptions.PageSize.GetValueOrDefault(20);

            sql = sql.Append("ORDER BY " + listOptions.SortField);
            if(listOptions.SortAscending.GetValueOrDefault())
            {
                sql = sql.Append("ASC");
            }
            else
            {
                sql = sql.Append("DESC");
            }
            var dbPagedList = db.Page<T>(pageNumber, pageSize, sql);
            var staticPagedList = new StaticPagedList<T>(dbPagedList.Items, pageNumber, pageSize, (int)dbPagedList.TotalItems);
            return new PagedSortedList<T>(staticPagedList, listOptions.SortField, listOptions.SortAscending.GetValueOrDefault());
        }
    }
}
