using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;

namespace Sriracha.Deploy.Data.Dto
{
	public class PagedSortedList<T> : IPagedList
	{
		private IPagedList<T> _list;

		public PagedSortedList(IPagedList<T> list, string sortField, bool sortAscending)
		{
			_list = list;
			this.SortAscending = sortAscending;
			this.SortField = sortField;
		}

		public string SortField { get; private set; }
		public bool SortAscending { get; private set; }

		public List<T> Items
		{
			get { return _list.ToList(); } 
		}

		public int FirstItemOnPage
		{
			get { return _list.FirstItemOnPage; }
		}

		public bool HasNextPage
		{
			get { return _list.HasNextPage; }
		}

		public bool HasPreviousPage
		{
			get { return _list.HasPreviousPage; }
		}

		public bool IsFirstPage
		{
			get { return _list.IsFirstPage;  }
		}

		public bool IsLastPage
		{
			get { return _list.IsLastPage; }
		}

		public int LastItemOnPage
		{
			get { return _list.LastItemOnPage; }
		}

		public int PageCount
		{
			get { return _list.PageCount; }
		}

		public int PageNumber
		{
			get { return _list.PageNumber; }
		}

		public int PageSize
		{
			get { return _list.PageSize; }
		}

		public int TotalItemCount
		{
			get { return _list.TotalItemCount; }
		}
	}
}