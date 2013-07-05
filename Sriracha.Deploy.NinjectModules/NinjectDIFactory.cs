using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject;
using Sriracha.Deploy.Data;

namespace Sriracha.Deploy.NinjectModules
{
	public class NinjectDIFactory : IDIFactory
	{
		private readonly IKernel _kernel;

		public NinjectDIFactory(IKernel kernel)
		{
			_kernel = kernel;
		}

		public object CreateInjectedObject(Type t)
		{
			return _kernel.Get(t);
		}
	}
}
