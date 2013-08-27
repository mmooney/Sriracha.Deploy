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
	}
}
