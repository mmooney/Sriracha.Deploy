using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data
{
	public interface IDIFactory
	{
        object CreateInjectedObject(Type t, Dictionary<Type, object> parameters=null);
		T CreateInjectedObject<T>(Dictionary<Type, object> parameters=null);
	}
}
