using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject;
using Ninject.Modules;
using Ninject.Planning.Bindings;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Impl;
using Sriracha.Deploy.Data.Tasks;
using Sriracha.Deploy.Data.Tasks.TaskImpl;

namespace Sriracha.Deploy.NinjectModules
{
    public class SrirachaNinjectorator : NinjectModule
    {
		private static NLog.Logger _logger;

		public override void Load()
		{
			this.SetupLogging();
			this.Bind<IProjectManager>().To<ProjectManager>();
			this.Bind<IBuildManager>().To<BuildManager>();
			this.Bind<IFileManager>().To<FileManager>();
			this.Bind<ITaskManager>().To<TaskManager>();
			this.Bind<IDeployHistoryManager>().To<DeployHistoryManager>();
			this.Bind<IModuleInspector>().To<ModuleInspector>();
			this.Bind<IDeployRunner>().To<DeployRunner>();
			this.Bind<IDeployTaskStatusManager>().To<DeployTaskStatusManager>();
			this.Bind<IDeployComponentRunner>().To<DeployComponentRunner>();
			this.Bind<IDeployTaskFactory>().To<DeployTaskFactory>().InSingletonScope();
			this.Bind<IDeployRequestManager>().To<DeployRequestManager>();
			this.Bind<IDeploymentValidator>().To<DeploymentValidator>().InSingletonScope();
			this.Bind<IDIFactory>().To<NinjectDIFactory>().InSingletonScope();
			this.Bind<IParameterParser>().To<ParameterParser>().InSingletonScope();
			this.Bind<IFileWriter>().To<FileWriter>().InSingletonScope();
			this.Bind<IBuildPublisher>().To<BuildPublisher>();
			this.Bind<IZipper>().To<Zipper>();
			this.Kernel.Load(new RavenDBNinjectModule());
		}

		private void SetupLogging()
		{
			_logger = NLog.LogManager.GetCurrentClassLogger();
			this.Bind<NLog.Logger>().ToMethod(ctx=>_logger);	 
		}
	}
}
