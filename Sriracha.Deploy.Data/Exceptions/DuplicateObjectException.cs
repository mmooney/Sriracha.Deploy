using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Exceptions
{
	public class DuplicateObjectException<T> : ApplicationException
	{
		public T DataObject { get; private set; }

		public DuplicateObjectException(T dataObject)
		{
			this.DataObject = dataObject;
		}
	}
}
