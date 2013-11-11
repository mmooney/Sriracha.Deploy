using MMDB.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Dto
{
	public class ListOptions
	{
		public int? PageSize { get; set; }
		public string SortField { get; set; }
		public int? PageNumber { get; set; }	//1-BASED FOOL!
		public bool? SortAscending { get; set; }

        public static ListOptions SetDefaults(ListOptions listOptions, int pageSize, int pageNumber, string sortField, bool sortAscending)
        {
            if(listOptions == null)
            {
                listOptions = new ListOptions();
            }
            listOptions.PageSize = listOptions.PageSize.GetValueOrDefault(pageSize);
            listOptions.PageNumber = listOptions.PageNumber.GetValueOrDefault(pageNumber);
            listOptions.SortField = StringHelper.IsNullOrEmpty(listOptions.SortField, sortField);
            listOptions.SortAscending = listOptions.SortAscending.GetValueOrDefault();
            return listOptions;
        }
    }
}
