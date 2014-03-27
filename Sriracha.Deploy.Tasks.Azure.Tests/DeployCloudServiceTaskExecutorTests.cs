using Common.Logging;
using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Sriracha.Deploy.Data.Deployment;
using Sriracha.Deploy.Data.Dto.Build;
using Sriracha.Deploy.Data.Dto.Project;
using Sriracha.Deploy.Data.Dto.Validation;
using Sriracha.Deploy.Data.Tasks;
using Sriracha.Deploy.Tasks.Azure.DeployCloudService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Sriracha.Deploy.Tasks.Azure.Tests
{
    /// <summary>
    /// http://www.bradygaster.com/post/getting-started-with-the-windows-azure-management-libraries
    /// </summary>
    public class DeployCloudServiceTaskExecutorTests
    {
        private class TestData
        {
            public Fixture Fixture { get; set; }
            public Mock<IParameterEvaluator> ParameterEvaluator { get; set; }
            public Mock<IDeployTaskStatusManager> DeployTaskStatusManager { get; set; }
            public Mock<ILog> Logger { get; set; }
            public Mock<IDeploymentValidator> DeploymentValidator { get; set; }
            public DeployCloudServiceTaskDefinition TaskDefinition { get; set; }
            public DeployComponent DeployComponent { get; set; }
            public DeployEnvironmentConfiguration EnvironmentComponent { get; set; }
            public DeployMachine DeployMachine { get; set; }
            public DeployBuild DeployBuild { get; set; }
            public RuntimeSystemSettings SystemSettings { get; set; }
            public TaskDefinitionValidationResult TaskValidationResult { get; set; }
            public string DeployStateId { get; set; }
            public DeployCloudServiceTaskExecutor Sut { get; set; }

            public static TestData Create()
            {
                var fixture = new Fixture();
                var testData = new TestData
                {
                    Fixture = fixture,
                    ParameterEvaluator = new Mock<IParameterEvaluator>(),
                    DeployTaskStatusManager = new Mock<IDeployTaskStatusManager>(),
                    DeploymentValidator = new Mock<IDeploymentValidator>(),
                    Logger = new Mock<ILog>(),
                    TaskDefinition = new DeployCloudServiceTaskDefinition
                                        {
                                            Options = fixture.Create<DeployCloudServiceTaskOptions>()
                                        },
                    DeployStateId = fixture.Create<string>("DeployStateId"),
                    DeployComponent = fixture.Create<DeployComponent>(),
                    DeployBuild = fixture.Create<DeployBuild>(),
                    DeployMachine = fixture.Create<DeployMachine>(),
                    EnvironmentComponent = fixture.Create<DeployEnvironmentConfiguration>(),
                    SystemSettings = fixture.Create<RuntimeSystemSettings>()
                };
                testData.Sut = new DeployCloudServiceTaskExecutor(testData.ParameterEvaluator.Object, testData.Logger.Object, testData.DeploymentValidator.Object);

                string settingsFilePath = Path.GetFullPath(".\\Azure.publishsettings.private");
                if(!File.Exists(settingsFilePath))
                {
                    throw new Exception("No azure publish settings file found at " + settingsFilePath);
                }
                var xml = new XmlDocument();
                xml.Load(settingsFilePath);
                var managementCertificateNode = xml.SelectSingleNode("/PublishData/PublishProfile/@ManagementCertificate");
                if(managementCertificateNode == null || string.IsNullOrEmpty(managementCertificateNode.Value))
                {
                    throw new Exception("Missing /PublishData/PublishProfile/@ManagementCertificate in " + settingsFilePath);
                }
                testData.TaskDefinition.Options.AzureManagementCertificate = managementCertificateNode.Value;

                var subscriptionNode = xml.SelectSingleNode("/PublishData/PublishProfile/Subscription/@Id");
                if(subscriptionNode == null || string.IsNullOrEmpty(subscriptionNode.Value))
                {
                    throw new Exception("Missing /PublishData/PublishProfile/Subscription/@Id node in " + settingsFilePath);
                }
                testData.TaskDefinition.Options.AzureSubscriptionIdentifier = subscriptionNode.Value;
                
                testData.TaskValidationResult = new TaskDefinitionValidationResult
                {
                    MachineResultList = new Dictionary<string,List<TaskDefinitionValidationResult.TaskDefinitionValidationResultItem>>
                    {
                        { testData.DeployMachine.Id, new List<TaskDefinitionValidationResult.TaskDefinitionValidationResultItem>() }
                    }   
                };
                testData.DeploymentValidator.Setup(i=>i.ValidateMachineTaskDefinition(testData.TaskDefinition, testData.EnvironmentComponent, testData.DeployMachine))
                                        .Returns(testData.TaskValidationResult);

                //Storage account names must be just numbers and lowercase letters between 3 and 24 characters.  Seriously.
                testData.TaskDefinition.Options.StorageAccountName = ("StorageAccountName" + Guid.NewGuid().ToString())
                                                                        .Replace("-", "").Replace("{", "").Replace("}","")
                                                                        .ToLower().Substring(0, 24);

                string packageName = "MMDB.AzureSample.Web.Azure.cspkg";
                var packagePath = Path.Combine(Environment.CurrentDirectory, packageName);
                if(!File.Exists(packagePath))
                {
                    packagePath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..", packageName));
                    if(!File.Exists(packagePath))
                    {
                        throw new Exception("Unable to find package " + packageName);
                    }
                }
                testData.TaskDefinition.Options.AzurePackagePath = packagePath;

                string configName = "ServiceConfiguration.Cloud.cscfg";
                var configPath = Path.Combine(Environment.CurrentDirectory, packageName);
                if (!File.Exists(configPath))
                {
                    configPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..", configName));
                    if (!File.Exists(configPath))
                    {
                        throw new Exception("Unable to find config " + configName);
                    }
                }
                testData.TaskDefinition.Options.AzureConfigPath = configPath;

                testData.TaskDefinition.Options.DeploymentSlot = "production";
                testData.TaskDefinition.Options.AzureTimeoutMinutes = 30;
                return testData;
            }

        }

        [Test, Explicit]
        public void NewDeployment_CreatesCloudService()
        {
            var testData = TestData.Create();

            testData.Sut.Execute(testData.DeployStateId, testData.DeployTaskStatusManager.Object, testData.TaskDefinition,
                                testData.DeployComponent, testData.EnvironmentComponent, testData.DeployMachine, testData.DeployBuild,
                                testData.SystemSettings);
        }

        [Test, Explicit]
        public void ExistingDeployment_UpdatesCloudService()
        {
            var testData = TestData.Create();
            testData.TaskDefinition.Options.ServiceName = "SrirachaExistingService";

            testData.Sut.Execute(testData.DeployStateId, testData.DeployTaskStatusManager.Object, testData.TaskDefinition,
                                testData.DeployComponent, testData.EnvironmentComponent, testData.DeployMachine, testData.DeployBuild,
                                testData.SystemSettings);
        }

        [Test, Explicit]
        public void NewDeployment_CloudServiceNameNotAvailable_ThrowsErrors()
        {
            var testData = TestData.Create();
            testData.TaskDefinition.Options.ServiceName = "sportscommanderprod";

            Assert.Throws<ArgumentException>(()=>
                            testData.Sut.Execute(testData.DeployStateId, testData.DeployTaskStatusManager.Object, testData.TaskDefinition,
                                                testData.DeployComponent, testData.EnvironmentComponent, testData.DeployMachine, testData.DeployBuild,
                                                testData.SystemSettings));
        }

        [Test, Explicit]
        public void ExistingStorageAccount_ReusesStorageAccount()
        {
            var testData = TestData.Create();
            testData.TaskDefinition.Options.StorageAccountName = "srirachastorage";

            testData.Sut.Execute(testData.DeployStateId, testData.DeployTaskStatusManager.Object, testData.TaskDefinition,
                                testData.DeployComponent, testData.EnvironmentComponent, testData.DeployMachine, testData.DeployBuild,
                                testData.SystemSettings);
        }

        [Test, Explicit]
        public void NewDeployment_StorageAccountNameNotAvailable_ThrowsErrors()
        {
            var testData = TestData.Create();
            testData.TaskDefinition.Options.StorageAccountName = "sportscommander";

            Assert.Throws<ArgumentException>(() =>
                            testData.Sut.Execute(testData.DeployStateId, testData.DeployTaskStatusManager.Object, testData.TaskDefinition,
                                                testData.DeployComponent, testData.EnvironmentComponent, testData.DeployMachine, testData.DeployBuild,
                                                testData.SystemSettings));
        }
    }
}
