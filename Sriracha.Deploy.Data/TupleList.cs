using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data
{
	[Obsolete("Move to MMDB.Shared")]
	//http://whathecode.wordpress.com/2011/11/03/list-of-tuples/
	public class TupleList<T1, T2> : List<Tuple<T1, T2>>
	{
		public void Add(T1 item, T2 item2)
		{
			this.Add(new Tuple<T1, T2>(item, item2));
		}
	}

	public class TupleList<T1, T2, T3> : List<Tuple<T1, T2, T3>>
	{
		public void Add(T1 item, T2 item2, T3 item3)
		{
			this.Add(new Tuple<T1, T2, T3>(item, item2, item3));
		}
	}
}
