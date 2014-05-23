using Autofac;
using MMDB.ConnectionSettings;
using Sriracha.Deploy.AutofacModules;
using Sriracha.Deploy.Data.Build;
using Sriracha.Deploy.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.SqlServer
{
    public class SqlServerRepositoryRegistar : ISrirachaRepositoryRegistar
    {
        public void RegisterRepositories(ContainerBuilder builder)
        {
            builder.RegisterType<SqlConnectionInfo>().As<ISqlConnectionInfo>().SingleInstance();

            builder.RegisterType<SqlServerBuildRepository>().As<IBuildRepository>();
            builder.RegisterType<SqlServerProjectRepository>().As<IProjectRepository>();
            builder.RegisterType<SqlServerFileRepository>().As<IFileRepository>();
            builder.RegisterType<SqlServerFileStorage>().As<IFileStorage>();
            builder.RegisterType<SqlServerDeployRepository>().As<IDeployRepository>();
            builder.RegisterType<SqlServerDeployStateRepository>().As<IDeployStateRepository>();
            builder.RegisterType<SqlServerSystemLogRepository>().As<ISystemLogRepository>();
            builder.RegisterType<SqlServerMembershipRepository>().As<IMembershipRepository>();
            builder.RegisterType<SqlServerEmailQueueRepository>().As<IEmailQueueRepository>();
            builder.RegisterType<SqlServerCredentialsRepository>().As<ICredentialsRepository>();
            builder.RegisterType<SqlServerSystemSettingsRepository>().As<ISystemSettingsRepository>();
            builder.RegisterType<SqlServerRazorTemplateRepository>().As<IRazorTemplateRepository>();
            builder.RegisterType<SqlServerCleanupRepository>().As<ICleanupRepository>();
            throw new NotImplementedException();
            //builder.RegisterType<SqlServerOfflineDeploymentRepository>().As<IOfflineDeploymentRepository>();
            //builder.RegisterType<SqlServerSystemRoleRepository>().As<ISystemRoleRepository>();
            builder.RegisterType<SqlServerBuildPurgeRuleRepository>().As<IBuildPurgeRuleRepository>();

            //builder.RegisterType<SqlServerConnectionSettingRepository>().As<IConnectionSettingRepository>();
            //builder.RegisterType<SqlServerPermissionRepository>().As<IPermissionRepository>();
        }
    }
}
