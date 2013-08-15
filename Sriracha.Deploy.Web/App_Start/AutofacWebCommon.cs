//[assembly: WebActivator.PreApplicationStartMethod(typeof(Sriracha.Deploy.Web.App_Start.AutofacWebCommon), "Start")]
//[assembly: WebActivator.ApplicationShutdownMethodAttribute(typeof(Sriracha.Deploy.Web.App_Start.AutofacWebCommon), "Stop")]

namespace Sriracha.Deploy.Web.App_Start
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Web;
	using System.Web.Mvc;
	using Autofac;
	using Autofac.Integration.Mvc;
	using MMDB.Shared;
	using ServiceStack.Configuration;
	using Sriracha.Deploy.AutofacModules;

	public static class AutofacWebCommon
	{
		private static IContainer _container;

		public static void Start()
		{
			if(_container == null)
			{
				var builder = new ContainerBuilder();
				builder.RegisterControllers(typeof(MvcApplication).Assembly);
				builder.RegisterModule(new SrirachaAutofacorator(EnumDIMode.Web));
				_container = builder.Build();
				DependencyResolver.SetResolver(new AutofacDependencyResolver(_container));
			}
		}

		public static void Stop()
		{
		}

		public static IContainerAdapter CreateServiceStackAdapter()
		{
			Start();
			return new Helpers.AutofacIocAdapter(_container);
		}
	}
}