using System.Web;
using Ninject;
using Ninject.Activation;
using Ninject.Modules;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Impl;
using Sriracha.Deploy.Data.ServiceJobs;
using Sriracha.Deploy.Data.ServiceJobs.ServiceJobImpl;
using Sriracha.Deploy.Data.Tasks;
using Sriracha.Deploy.Data.Tasks.TaskImpl;

namespace Sriracha.Deploy.NinjectModules
{
    public class SrirachaNinjectorator : NinjectModule
    {
		private static NLog.Logger _logger;

		public override void Load()
		{
			this.Bind<IDIFactory>().To<NinjectDIFactory>().InSingletonScope();
			if (HttpContext.Current != null)
			{
				this.Bind<IUserIdentity>().To<WebUserIdentity>();
			}
			else
			{
				this.Bind<IUserIdentity>().To<ConsoleUserIdentity>();
			}
			this.SetupLogging();
			_logger.Info("Binding Ninject References");

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
			this.Bind<IProcessRunner>().To<ProcessRunner>().InSingletonScope();
			this.Bind<IParameterParser>().To<ParameterParser>().InSingletonScope();
			this.Bind<IFileWriter>().To<FileWriter>().InSingletonScope();
			this.Bind<IBuildPublisher>().To<BuildPublisher>();
			this.Bind<IDeployStateManager>().To<DeployStateManager>();
			this.Bind<IJobScheduler>().To<JobScheduler>();
			this.Bind<IZipper>().To<Zipper>();
			this.Bind<ISystemSettings>().To<DefaultSystemSettings>();

			this.Bind<IRunDeploymentJob>().To<RunDeploymentJob>();
			this.Bind<IJobFactory>().To<JobFactory>();
			this.Bind<ISchedulerFactory>().To<StdSchedulerFactory>();
			this.Bind<IScheduler>().ToMethod(CreateScheduler).InSingletonScope();

			this.Kernel.Load(new RavenDBNinjectModule());
			_logger.Info("Binding Ninject References Complete");
		}

		private void SetupLogging()
		{
			var dbTarget = this.Kernel.Get<NLogDBLogTarget>();
			dbTarget.Layout = "${message} ${exception:format=message,stacktrace:separator=\r\n}";
			var loggingConfig = NLog.LogManager.Configuration;
			if (loggingConfig == null)
			{
				loggingConfig = new NLog.Config.LoggingConfiguration();
			}
			loggingConfig.AddTarget("dbTarget", dbTarget);
			var rule = new NLog.Config.LoggingRule("*", NLog.LogLevel.Trace, dbTarget);
			loggingConfig.LoggingRules.Add(rule);
			NLog.LogManager.Configuration = loggingConfig;

			_logger = NLog.LogManager.GetCurrentClassLogger();
			this.Bind<NLog.Logger>().ToMethod(ctx=>_logger);	 
		}

		public static IScheduler CreateScheduler(IContext context)
		{
			var schedulerFactory = context.Kernel.Get<ISchedulerFactory>();
			var scheduler = schedulerFactory.GetScheduler();
			scheduler.JobFactory = context.Kernel.Get<IJobFactory>();
			return scheduler;
		}
	}
}
