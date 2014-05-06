using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
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


        public Type GetType(string typeName, string directory=null)
        {
            try 
            {
                IEnumerable<Assembly> assemblyList;
                if(string.IsNullOrEmpty(directory))
                {
                    assemblyList = AppDomain.CurrentDomain.GetAssemblies().ToList();
                }
                else 
                {
                    if(!Directory.Exists(directory))
                    {
                        throw new Exception("Directory does not exist");
                    }
                    assemblyList = Directory.GetFiles(directory, "*.dll")
                                    .Select(i=>Assembly.LoadFrom(i)).ToList();
                }
                var typeList = assemblyList
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
            catch (ReflectionTypeLoadException err)
            {
                foreach (var loaderError in err.LoaderExceptions)
                {
                    Console.WriteLine("LOADER EXCEPTION: " + loaderError.ToString());
                }
                throw;
            }
        }
    }
}