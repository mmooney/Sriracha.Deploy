using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sriracha.Deploy.Web
{
	public enum DIContainer
	{
		Ninject,
		Autofac
	}

	public static class WebConfigOptions
	{
		public static DIContainer DIContainer
		{
			get 
			{
				return Web.DIContainer.Ninject;
			}
		}
	}
}