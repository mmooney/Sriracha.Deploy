﻿using Common.Logging;
using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Sriracha.Deploy.Data.Credentials;
using Sriracha.Deploy.Data.Deployment;
using Sriracha.Deploy.Data.Deployment.DeploymentImpl;
using Sriracha.Deploy.Data.Dropkick;
using Sriracha.Deploy.Data.Dropkick.DropkickImpl;
using Sriracha.Deploy.Data.Dto.Build;
using Sriracha.Deploy.Data.Dto.Project;
using Sriracha.Deploy.Data.Dto.Validation;
using Sriracha.Deploy.Data.Tasks;
using Sriracha.Deploy.Data.Tasks.TaskImpl;
using Sriracha.Deploy.Data.Utility.UtilityImpl;
using Sriracha.Deploy.Tasks.Common.DeployWindowsService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Tasks.Common.Tests.DeployWindowsService
{
    public class DeployWindowsServiceTaskExecutorTests
    {
        private class TestData
        {
            public Fixture Fixture { get; set; }
            public Mock<IDeploymentValidator> DeploymentValidator { get; set; }
            public IDeployTaskStatusManager DeployTaskStatusManager { get; set; }
            public IDropkickRunner DropkickRunner { get; set; }
            public DeployWindowsServiceTaskDefinition TaskDefinition { get; set; }
            public string DeployStateId { get; set; }
            public DeployComponent Component { get; set; }
            public DeployEnvironment Environment { get; set; }
            public DeployEnvironmentConfiguration EnvironmentComponent { get; set; }
            public DeployMachine Machine { get; set; }
            public DeployBuild Build { get; set; }
            public DeployWindowsServiceTaskExecutor Sut { get; set; }

            public static TestData Create()
            {
                var fixture = new Fixture();
                var project = fixture.Create<DeployProject>();
                var testData = new TestData
                {
                    Fixture = fixture,
                    DeploymentValidator = new Mock<IDeploymentValidator>(),
                    TaskDefinition = new DeployWindowsServiceTaskDefinition(new ParameterParser()),
                    DeployTaskStatusManager = new DeployTaskStatusManager(new Mock<ILog>().Object, new Mock<IDeployStateManager>().Object, new Mock<IDeployStatusNotifier>().Object),
                    DropkickRunner = new DropkickRunner(new Zipper(new Mock<ILog>().Object), new ProcessRunner(), new Mock<ICredentialsManager>().Object, new Mock<IImpersonator>().Object),
                    Component = project.ComponentList.First(),
                    Environment = project.EnvironmentList.First(),
                    EnvironmentComponent = project.EnvironmentList.First().ComponentList.First(),
                    Machine = project.EnvironmentList.First().ComponentList.First().MachineList.First(),
                    Build = fixture.Create<DeployBuild>(),
                    DeployStateId = fixture.Create<string>("DeployStateId"),
                };
                testData.Sut = new DeployWindowsServiceTaskExecutor(new ParameterEvaluator(new ParameterParser()), testData.DeploymentValidator.Object, testData.DropkickRunner);

                testData.Component.ProjectId = project.Id;

                testData.Environment.ProjectId = project.Id;

                testData.EnvironmentComponent.ProjectId = project.Id;
                testData.EnvironmentComponent.EnvironmentId = testData.Environment.Id;
                testData.EnvironmentComponent.ParentId = testData.Component.Id;
                testData.EnvironmentComponent.ParentType = EnumDeployStepParentType.Component;
                testData.EnvironmentComponent.DeployCredentialsId = null;

                testData.Machine.ProjectId = project.Id;
                testData.Machine.EnvironmentId = testData.Environment.Id;
                testData.Machine.ParentId = testData.Component.Id;

                var validationResult = new TaskDefinitionValidationResult();
                validationResult.MachineResultList.Add(testData.Machine.Id, new List<TaskDefinitionValidationResult.TaskDefinitionValidationResultItem>());
                testData.DeploymentValidator.Setup(i=>i.ValidateMachineTaskDefinition(testData.TaskDefinition, testData.EnvironmentComponent, testData.Machine))
                                            .Returns(validationResult);
                
                testData.TaskDefinition.Options = new DeployWindowsServiceTaskOptions
                {
                    TargetMachineName = "localhost",
                    DependencyList = new List<DeployWindowsServiceTaskOptions.ServiceDependency>
                    {
                        new DeployWindowsServiceTaskOptions.ServiceDependency { ServiceName="RavenDB" },
                        new DeployWindowsServiceTaskOptions.ServiceDependency { ServiceName="MSSQL" }
                    },
                    ServiceExeName = "Sriracha.Deploy.Server.exe", 
                    ServiceName = "Sriracha - TEST Service Deploy Task",
                    ServiceSourceExeConfigPath = @"C:\Projects\Sriracha.Deploy\Sriracha.Deploy.Server\App.Config",
                    ServiceSourcePath = @"C:\Projects\Sriracha.Deploy\Sriracha.Deploy.Server\bin\Debug",
                    ServiceStartMode = dropkick.Wmi.ServiceStartMode.Manual,
                    ServiceTargetPath = @"C:\Test\TestServiceDeployTask",
                    ServiceUserName = "NT AUTHORITY\\SYSTEM",
                    ServiceUserPassword = null,
                    StartImmediately = false,
                    TargetMachinePassword = null,
                    TargetMachineUserName = null
                };
                return testData;
            }
        }

        [Test, Explicit, Category("Integration")]
        public void Test1()
        {
            var testData = TestData.Create();
            var systemSettings = new RuntimeSystemSettings()
            {
                LocalDeployDirectory = "C:\\Temp\\DeployTest",
                LocalMachineName = Environment.MachineName
            };

            var result = testData.Sut.Execute(testData.DeployStateId, testData.DeployTaskStatusManager, testData.TaskDefinition, testData.Component, testData.EnvironmentComponent, testData.Machine, testData.Build, systemSettings);

            Assert.AreEqual(EnumDeployTaskExecutionResultStatus.Success, result.Status);
        }
    }
}
