using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sriracha.Deploy.Data.Tasks;
using Sriracha.Deploy.Data.Deployment;
using MMDB.Shared;
using roundhouse.runners;
using roundhouse;
using roundhouse.infrastructure.app;
using roundhouse.consoles;
using roundhouse.infrastructure.containers;

namespace Sriracha.Deploy.Tasks.RoundhousE.DeployDatabase
{
    public class DeployRoundhouseDatabaseExecutor : BaseDeployTaskExecutor<DeployRoundhouseDatabaseTaskDefinition, DeployRoundhouseDatabaseTaskOptions>
    {
        public DeployRoundhouseDatabaseExecutor(IParameterEvaluator parameterEvaluator, IDeploymentValidator validator) : base(parameterEvaluator, validator)
        {

        }

        protected override DeployTaskExecutionResult InternalExecute(TaskExecutionContext<DeployRoundhouseDatabaseTaskDefinition, DeployRoundhouseDatabaseTaskOptions> context)
        {
            roundhouse.databases.Database database;
            switch(context.FormattedOptions.DatabaseType)
            {
                //case EnumRoundhouseDatabaseType.Access:
                //    database = new roundhouse.databases.access.AccessDatabase();
                //    break;
                case EnumRoundhouseDatabaseType.MySql:
                    database = new roundhouse.databases.mysql.MySqlDatabase();
                    break;
                case EnumRoundhouseDatabaseType.Oracle:
                    database = new roundhouse.databases.oracle.OracleDatabase();
                    break;
                case EnumRoundhouseDatabaseType.Postgres:
                    database = new roundhouse.databases.postgresql.PostgreSQLDatabase();
                    break;
                case EnumRoundhouseDatabaseType.SqlLite:
                    database = new roundhouse.databases.sqlite.SqliteDatabase();
                    break;
                case EnumRoundhouseDatabaseType.SqlServer:
                    database = new roundhouse.databases.sqlserver.SqlServerDatabase();
                    break;
                default:
                    throw new UnknownEnumValueException(context.FormattedOptions.DatabaseType);
            }

            var configuration = set_up_configuration_and_build_the_container(context);


            RoundhouseMigrationRunner migration_runner = get_migration_runner(configuration);
            migration_runner.run();

            return context.BuildResult();
        }

        //Adapted from roundhouse.console.Program.cs: https://github.com/chucknorris/roundhouse/blob/master/product/roundhouse.console/Program.cs
        private static ConfigurationPropertyHolder set_up_configuration_and_build_the_container(TaskExecutionContext<DeployRoundhouseDatabaseTaskDefinition,DeployRoundhouseDatabaseTaskOptions> context)
        {
            ConfigurationPropertyHolder configuration = new DefaultConfiguration()
            {
                EnvironmentName = context.FormattedOptions.EnvironmentName,
                SqlFilesDirectory = context.FormattedOptions.SqlFilesDirectory,
                Silent = true
            };

            configuration.Logger = new RoundhouseLogger<DeployRoundhouseDatabaseTaskDefinition, DeployRoundhouseDatabaseTaskOptions>(context);

            ApplicationConfiguraton.set_defaults_if_properties_are_not_set(configuration);
            ApplicationConfiguraton.build_the_container(configuration);

            return configuration;
        }
        private static RoundhouseMigrationRunner get_migration_runner(ConfigurationPropertyHolder configuration)
        {
            return new RoundhouseMigrationRunner(
                configuration.RepositoryPath,
                Container.get_an_instance_of<roundhouse.environments.Environment>(),
                Container.get_an_instance_of<roundhouse.folders.KnownFolders>(),
                Container.get_an_instance_of<roundhouse.infrastructure.filesystem.FileSystemAccess>(),
                Container.get_an_instance_of<roundhouse.migrators.DatabaseMigrator>(),
                Container.get_an_instance_of<roundhouse.resolvers.VersionResolver>(),
                configuration.Silent,
                configuration.Drop,
                configuration.DoNotCreateDatabase,
                configuration.WithTransaction,
                configuration.RecoveryModeSimple,
                configuration);
        }
    }
}
