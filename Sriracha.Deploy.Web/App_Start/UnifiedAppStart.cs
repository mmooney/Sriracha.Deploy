[assembly: WebActivator.PreApplicationStartMethod(typeof(Sriracha.Deploy.Web.App_Start.UnifiedAppStart), "Start")]
[assembly: WebActivator.ApplicationShutdownMethodAttribute(typeof(Sriracha.Deploy.Web.App_Start.UnifiedAppStart), "Stop")]

namespace Sriracha.Deploy.Web.App_Start
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Web;
	using MMDB.Shared;

	public class UnifiedAppStart
	{
		public static void Start()
		{
			ServiceStack.Configuration.IContainerAdapter  containerAdapter;
			switch(WebConfigOptions.DIContainer)
			{
				case DIContainer.Ninject:
					NinjectWebCommon.Start();
					containerAdapter = NinjectWebCommon.CreateServiceStackAdapter();
					break;
				case DIContainer.Autofac:
					AutofacWebCommon.Start();
					containerAdapter = AutofacWebCommon.CreateServiceStackAdapter();
					break;
				default:
					throw new UnknownEnumValueException(WebConfigOptions.DIContainer);
			}
			//Setup ServiceStack
			var appHost = new AppHost();
			appHost.Container.Adapter = containerAdapter;
			appHost.Init();

		}

		private static void SetupServiceStack()
		{
			new AppHost().Init();
		}

		public static void Stop()
		{
		}
	}
}