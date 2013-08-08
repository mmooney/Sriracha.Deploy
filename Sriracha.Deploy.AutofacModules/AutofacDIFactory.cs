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
		private IContainer _container;

		public AutofacDIFactory(IContainer container)
		{
			_container = DIHelper.VerifyParameter(container);
		}

		public object CreateInjectedObject(Type t)
		{
			return _container.Resolve(t);
		}

		public T CreateInjectedObject<T>()
		{
			return _container.Resolve<T>();
		}
	}
}
