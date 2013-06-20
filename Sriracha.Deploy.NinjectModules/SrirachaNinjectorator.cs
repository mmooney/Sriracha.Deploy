using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ninject;
using Ninject.Modules;
using Ninject.Planning.Bindings;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Impl;

namespace Sriracha.Deploy.NinjectModules
{
    public class SrirachaNinjectorator : NinjectModule
    {
		public override void Load()
		{
			this.Bind<IProjectManager>().To<ProjectManager>();
			this.Bind<IBuildManager>().To<BuildManager>();
			this.Bind<IFileManager>().To<FileManager>();
			this.Bind<ITaskManager>().To<TaskManager>();
			this.Bind<IDeployHistoryManager>().To<DeployHistoryManager>();
			this.Bind<IModuleInspector>().To<ModuleInspector>();
			this.Kernel.Load(new RavenDBNinjectModule());
		}
	}
}
