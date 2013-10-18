using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using MMDB.ConnectionSettings;
using Raven.Client;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Repository;
using Sriracha.Deploy.RavenDB;

namespace Sriracha.Deploy.AutofacModules
{
	public class RavenDBAutofacModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.Register(context =>
					{
						var documentStore = RavenHelper.CreateDocumentStore();
						return documentStore;
					})
					.As<IDocumentStore>()
					.SingleInstance();
			builder.Register(context => context.Resolve<IDocumentStore>().OpenSession()).As<IDocumentSession>();

			builder.RegisterType<RavenBuildRepository>().As<IBuildRepository>();
			builder.RegisterType<RavenProjectRepository>().As<IProjectRepository>();
			builder.RegisterType<RavenFileRepository>().As<IFileRepository>();
			//Bind<IFileStorage>().To<RavenFileStorage_Collection>();
			builder.RegisterType<RavenFileStorage_Attachment>().As<IFileStorage>();
			builder.RegisterType<RavenAttachmentManager>().As<IRavenAttachmentManager>();
			builder.RegisterType<RavenDeployHistoryRepository>().As<IDeployHistoryRepository>();
			builder.RegisterType<RavenDeployRepository>().As<IDeployRepository>();
			builder.RegisterType<RavenSystemLogRepository>().As<ISystemLogRepository>();
			builder.RegisterType<RavenMembershipRepository>().As<IMembershipRepository>();
			builder.RegisterType<RavenEmailQueueRepository>().As<IEmailQueueRepository>();
			builder.RegisterType<RavenRazorTemplateRepository>().As<IRazorTemplateRepository>();

			builder.RegisterType<RavenConnectionSettingRepository>().As<IConnectionSettingRepository>();
			builder.RegisterType<RavenDBPermissionRepository>().As<IPermissionRepository>();
		}
	}
}
