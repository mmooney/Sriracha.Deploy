using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using Sriracha.Deploy.Data;

namespace Sriracha.Deploy.AutofacModules
{
	public class AutofacDIFactory : IDIFactory
	{
		private IComponentContext _container;

		public AutofacDIFactory(IComponentContext container)
		{
			_container = DIHelper.VerifyParameter(container);
		}

        public object CreateInjectedObject(Type t, Dictionary<Type, object> parameters=null)
		{
            if(parameters != null && parameters.Count > 0)
            {
                var parameterArray = parameters.Select(i=>new TypedParameter(i.Key, i.Value));
	    		return _container.Resolve(t, parameterArray);
            }
            else 
            {
                return _container.Resolve(t);
            }
		}

        public T CreateInjectedObject<T>(Dictionary<Type, object> parameters=null)
		{
            if (parameters != null && parameters.Count > 0)
            {
                var parameterArray = parameters.Select(i => new TypedParameter(i.Key, i.Value)).ToArray();
                return _container.Resolve<T>(parameterArray);
            }
            else
            {
                return _container.Resolve<T>();
            }
		}
	}
}
