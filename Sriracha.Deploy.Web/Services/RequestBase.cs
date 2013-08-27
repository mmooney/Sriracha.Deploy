using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sriracha.Deploy.Data.Dto;

namespace Sriracha.Deploy.Web.Services
{
	public abstract class RequestBase<T>
	{
		//If requesting single item
		public string Id { get; set; }	

		//If requesting a list
		public int? PageSize { get; set; }
		public string SortField { get; set; }
		public int? PageNumber { get; set; }	//1-BASED FOOL!
		public bool? SortAscending { get; set; }

		internal ListOptions BuildListOptions()
		{
			return new ListOptions
			{
				PageSize = this.PageSize,
				SortField = this.SortField,
				SortAscending = this.SortAscending,
				PageNumber = this.PageNumber
			};
		}
	}
}