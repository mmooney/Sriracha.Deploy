[assembly: WebActivator.PreApplicationStartMethod(typeof(Sriracha.Deploy.Web.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivator.ApplicationShutdownMethodAttribute(typeof(Sriracha.Deploy.Web.App_Start.NinjectWebCommon), "Stop")]

namespace Sriracha.Deploy.Web.App_Start
{
	using System;
	using System.Web;
	using System.Web.Http;
	using Microsoft.Web.Infrastructure.DynamicModuleHelper;
	using Ninject;
	using Ninject.Planning.Bindings;
	using Ninject.Web.Common;
	using ServiceStack.Configuration;
	using ServiceStack.ContainerAdapter.Ninject;
	using Sriracha.Deploy.Data;
	using Sriracha.Deploy.Data.Impl;
	using Sriracha.Deploy.Web.Helpers;

    public static class NinjectWebCommon 
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start() 
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }
        
        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }

		public static IContainerAdapter CreateServiceStackAdapter()
		{
			if (bootstrapper.Kernel == null)
			{
				Start();
			}
			return new NinjectContainerAdapter(bootstrapper.Kernel);
		}
		        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
			
			kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
            kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();
            
            RegisterServices(kernel);

			return kernel;
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
			kernel.Bind<IProjectManager>().To<ProjectManager>();
			kernel.Bind<IBuildManager>().To<BuildManager>();
			kernel.Bind<IFileManager>().To<FileManager>();
			kernel.Bind<ITaskManager>().To<TaskManager>();
			kernel.Bind<IDeployHistoryManager>().To<DeployHistoryManager>();
			kernel.Bind<IModuleInspector>().To<ModuleInspector>();
			kernel.Load(new RavenDBNinjectModule());
        }        
    }
}
