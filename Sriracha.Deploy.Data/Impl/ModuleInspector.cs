using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Impl
{
	public class ModuleInspector : IModuleInspector
	{
		public List<Type> FindTypesImplementingInterfaces(Type interfaceType)
		{
			var typeList = AppDomain.CurrentDomain.GetAssemblies().ToList()
				.SelectMany(s => s.GetTypes())
				.Where(p => interfaceType.IsAssignableFrom(p)
						&& p.IsClass).ToList();
			return typeList;
		}
	}
}