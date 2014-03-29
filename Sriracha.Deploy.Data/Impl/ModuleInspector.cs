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


        public Type GetType(string typeName)
        {
            var typeList = AppDomain.CurrentDomain.GetAssemblies().ToList()
                .SelectMany(s => s.GetTypes())
                .Where(p => p.FullName == typeName
                        && p.IsClass).ToList();
            if(typeList == null || typeList.Count == 0)
            {
                throw new ArgumentException("Failed to find any types matching " + typeName);
            }
            if(typeList.Count > 1)
            {
                throw new ArgumentNullException("Found multiple types matching " + typeName);
            }
            return typeList.Single();
        }
    }
}