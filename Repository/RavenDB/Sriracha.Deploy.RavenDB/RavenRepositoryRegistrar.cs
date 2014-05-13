using Autofac;
using MMDB.ConnectionSettings;
using Raven.Client;
using Sriracha.Deploy.AutofacModules;
using Sriracha.Deploy.Data.Build;
using Sriracha.Deploy.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.RavenDB
{
    public class RavenRepositoryRegistrar : ISrirachaRepositoryRegistar
    {
        public void RegisterRepositories(ContainerBuilder builder)
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
            builder.RegisterType<RavenDeployRepository>().As<IDeployRepository>();
            builder.RegisterType<RavenDeployStateRepository>().As<IDeployStateRepository>();
            builder.RegisterType<RavenSystemLogRepository>().As<ISystemLogRepository>();
            builder.RegisterType<RavenMembershipRepository>().As<IMembershipRepository>();
            builder.RegisterType<RavenEmailQueueRepository>().As<IEmailQueueRepository>();
            builder.RegisterType<RavenCredentialsRepository>().As<ICredentialsRepository>();
            builder.RegisterType<RavenSystemSettingsRepository>().As<ISystemSettingsRepository>();
            builder.RegisterType<RavenRazorTemplateRepository>().As<IRazorTemplateRepository>();
            builder.RegisterType<RavenCleanupRepository>().As<ICleanupRepository>();
            builder.RegisterType<RavenOfflineDeploymentRepository>().As<IOfflineDeploymentRepository>();
            builder.RegisterType<RavenSystemRoleRepository>().As<ISystemRoleRepository>();
            builder.RegisterType<RavenBuildPurgeRuleRepository>().As<IBuildPurgeRuleRepository>();

            builder.RegisterType<RavenConnectionSettingRepository>().As<IConnectionSettingRepository>();
            builder.RegisterType<RavenPermissionRepository>().As<IPermissionRepository>();
        }
    }
}
