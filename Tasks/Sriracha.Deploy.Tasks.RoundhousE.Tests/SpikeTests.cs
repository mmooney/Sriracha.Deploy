using Common.Logging;
using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Deployment;
using Sriracha.Deploy.Data.Deployment.DeploymentImpl;
using Sriracha.Deploy.Data.Dto.Build;
using Sriracha.Deploy.Data.Dto.Project;
using Sriracha.Deploy.Data.Impl;
using Sriracha.Deploy.Data.Tasks;
using Sriracha.Deploy.Data.Tasks.TaskImpl;
using Sriracha.Deploy.Data.Utility.UtilityImpl;
using Sriracha.Deploy.Tasks.RoundhousE.DeployDatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sriracha.Deploy.Tasks.RoundhousE.Tests
{
    public class SpikeTests
    {
        [Test, Explicit]
        public void TestDeployDatabase()
        {
            var definition = new DeployRoundhouseDatabaseTaskDefinition(new ParameterParser());
            definition.Options = new DeployRoundhouseDatabaseTaskOptions
            {
                EnvironmentName = "LOCAL",
                DatabaseType = EnumRoundhouseDatabaseType.SqlServer,
                SqlFilesDirectory = @"C:\Projects\Sriracha.Deploy\Repository\SqlServer\Sriracha.Deploy.SqlServer.RoundhousE\db"
            };
            var executor = new DeployRoundhouseDatabaseExecutor(new ParameterEvaluator(), new DeploymentValidator(new DeployTaskFactory(new Mock<IDIFactory>().Object, new ModuleInspector())));
            var fixture = new Fixture();

            string deployStateId = fixture.Create<string>("DeployStateId");
            var deployTaskStatusManager = new Mock<IDeployTaskStatusManager>();
            var component = fixture.Create<DeployComponent>();
            var environmentComponent = fixture.Create<DeployEnvironmentConfiguration>();
            var machine = fixture.Create<DeployMachine>();
            var build = fixture.Create<DeployBuild>();
            var systemSettings = new RuntimeSystemSettings();
            executor.Execute(deployStateId, deployTaskStatusManager.Object, definition, component, environmentComponent, machine, build, systemSettings);
        }
    }
}
