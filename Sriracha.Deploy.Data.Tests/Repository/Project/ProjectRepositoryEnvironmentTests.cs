using MMDB.Shared;
using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Sriracha.Deploy.Data.Dto.Project;
using Sriracha.Deploy.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Tests.Repository.Project
{
	[TestFixture]
    public abstract class ProjectRepositoryEnvironmentTests : ProjectRepositoryTestBase
	{
        private void AssertCreatedEnvironment(DeployEnvironment result, DeployProject project, string environmentName, List<DeployEnvironmentConfiguration> environmentComponentList, List<DeployEnvironmentConfiguration> environmentConfigurationList, IProjectRepository sut)
        {
            Assert.IsNotNull(result);
            Assert.IsNotNullOrEmpty(result.Id);
            Assert.AreEqual(project.Id, result.ProjectId);
            Assert.AreEqual(environmentName, result.EnvironmentName);
            AssertIsRecent(result.CreatedDateTimeUtc);
            Assert.AreEqual(this.UserName, result.CreatedByUserName);
            AssertIsRecent(result.UpdatedDateTimeUtc);
            Assert.AreEqual(this.UserName, result.UpdatedByUserName);
            
            environmentComponentList = environmentComponentList ?? new List<DeployEnvironmentConfiguration>();
            Assert.AreEqual(environmentComponentList.Count(), result.ComponentList.Count);
            foreach(var item in environmentComponentList)
            {
                var createdItem = result.ComponentList.SingleOrDefault(i=>i.ParentId == item.ParentId);
                AssertCreatedEnvironmentConfiguration(item, createdItem, project, result, EnumDeployStepParentType.Component);
            }
            environmentConfigurationList = environmentConfigurationList ?? new List<DeployEnvironmentConfiguration>();
            Assert.AreEqual(environmentConfigurationList.Count(), result.ConfigurationList.Count);
            foreach (var item in environmentConfigurationList)
            {
                var createdItem = result.ConfigurationList.SingleOrDefault(i => i.ParentId == item.ParentId);
                AssertCreatedEnvironmentConfiguration(item, createdItem, project, result, EnumDeployStepParentType.Configuration);
            }

            var dbItem = sut.GetEnvironment(result.Id, result.ProjectId);
            AssertEnvironment(result, dbItem);

            var dbProject = sut.GetProject(project.Id);
            var dbProjectEnvironment = dbProject.EnvironmentList.SingleOrDefault(i => i.Id == result.Id);
            Assert.IsNotNull(dbProjectEnvironment);
            AssertEnvironment(result, dbProjectEnvironment);
        }

        private void AssertEnvironment(DeployEnvironment expected, DeployEnvironment actual)
        {
            Assert.IsNotNull(actual);
            Assert.IsNotNullOrEmpty(actual.Id);
            Assert.AreEqual(expected.ProjectId, actual.ProjectId);
            Assert.AreEqual(expected.EnvironmentName, actual.EnvironmentName);
            AssertDateEqual(expected.CreatedDateTimeUtc, actual.CreatedDateTimeUtc);
            Assert.AreEqual(expected.CreatedByUserName, actual.CreatedByUserName);
            AssertDateEqual(expected.UpdatedDateTimeUtc, actual.UpdatedDateTimeUtc);
            Assert.AreEqual(expected.UpdatedByUserName, actual.UpdatedByUserName);
            
            AssertEnvironmentConfigurationList(expected.ComponentList, actual.ComponentList);
            AssertEnvironmentConfigurationList(expected.ConfigurationList, actual.ConfigurationList);
        }

        private void AssertEnvironmentConfigurationList(List<DeployEnvironmentConfiguration> expectedList, List<DeployEnvironmentConfiguration> actualList)
        {
            expectedList = (expectedList ?? new List<DeployEnvironmentConfiguration>());
            actualList = (actualList ?? new List<DeployEnvironmentConfiguration>());
            Assert.AreEqual(expectedList.Count(), actualList.Count);
            foreach (var expectedItem in expectedList)
            {
                var actualItem = actualList.SingleOrDefault(i => i.ParentId == expectedItem.ParentId);
                AssertEnvironmentConfiguration(expectedItem, actualItem);
            }
        }

        private void AssertEnvironmentConfiguration(DeployEnvironmentConfiguration expected, DeployEnvironmentConfiguration actual)
        {
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.ProjectId, actual.ProjectId);
            Assert.AreEqual(expected.ParentId, actual.ParentId);
            Assert.AreEqual(expected.ParentType, actual.ParentType);
            Assert.AreEqual(expected.DeployCredentialsId, actual.DeployCredentialsId);
            Assert.AreEqual(expected.CreatedByUserName, actual.CreatedByUserName);
            AssertDateEqual(expected.CreatedDateTimeUtc, actual.CreatedDateTimeUtc);
            Assert.AreEqual(expected.UpdatedByUserName, actual.UpdatedByUserName);
            AssertDateEqual(expected.UpdatedDateTimeUtc, actual.UpdatedDateTimeUtc);
            AssertDictionary(expected.ConfigurationValueList, actual.ConfigurationValueList);
            Assert.AreEqual(expected.MachineList.Count, actual.MachineList.Count);
            foreach (var expectedMachine in expected.MachineList)
            {
                var actualMachine = actual.MachineList.SingleOrDefault(i => i.MachineName == expectedMachine.MachineName);
                AssertMachine(expectedMachine, actualMachine);
            }
        }

        private void AssertMachine(DeployMachine expectedMachine, DeployMachine actualMachine)
        {
            Assert.IsNotNull(actualMachine);
            Assert.AreEqual(expectedMachine.Id, actualMachine.Id);
            Assert.AreEqual(expectedMachine.ProjectId, actualMachine.ProjectId);
            Assert.AreEqual(expectedMachine.EnvironmentId, actualMachine.EnvironmentId);
            Assert.AreEqual(expectedMachine.EnvironmentName, actualMachine.EnvironmentName);
            Assert.AreEqual(expectedMachine.ParentId, actualMachine.ParentId);
            Assert.AreEqual(expectedMachine.CreatedByUserName, actualMachine.CreatedByUserName);
            AssertDateEqual(expectedMachine.CreatedDateTimeUtc, actualMachine.CreatedDateTimeUtc);
            Assert.AreEqual(expectedMachine.UpdatedByUserName, actualMachine.UpdatedByUserName);
            AssertDateEqual(expectedMachine.UpdatedDateTimeUtc, actualMachine.UpdatedDateTimeUtc);
            AssertDictionary(expectedMachine.ConfigurationValueList, actualMachine.ConfigurationValueList);
        }

        private void AssertCreatedEnvironmentConfiguration(DeployEnvironmentConfiguration sourceItem, DeployEnvironmentConfiguration createdItem, DeployProject project, DeployEnvironment environment, EnumDeployStepParentType parentType)
        {
            Assert.IsNotNull(createdItem);
            Assert.IsNotNullOrEmpty(createdItem.Id);
            Assert.AreEqual(project.Id, createdItem.ProjectId);
            Assert.AreEqual(sourceItem.ParentId, createdItem.ParentId);
            Assert.AreEqual(parentType, createdItem.ParentType);
            Assert.AreEqual(sourceItem.DeployCredentialsId, createdItem.DeployCredentialsId);
            Assert.AreEqual(this.UserName, createdItem.CreatedByUserName);
            AssertIsRecent(createdItem.CreatedDateTimeUtc);
            Assert.AreEqual(this.UserName, createdItem.UpdatedByUserName);
            AssertIsRecent(createdItem.UpdatedDateTimeUtc);

            AssertDictionary(sourceItem.ConfigurationValueList, createdItem.ConfigurationValueList);
            Assert.AreEqual(sourceItem.MachineList.Count, createdItem.MachineList.Count);
            foreach(var sourceMachine in sourceItem.MachineList)
            {
                var createdMachine = createdItem.MachineList.SingleOrDefault(i=>i.MachineName == sourceMachine.MachineName);
                Assert.IsNotNull(createdMachine);
                Assert.IsNotNullOrEmpty(createdMachine.Id);
                Assert.AreEqual(project.Id, createdMachine.ProjectId);
                Assert.AreEqual(environment.Id, createdMachine.EnvironmentId);
                Assert.AreEqual(environment.EnvironmentName, createdMachine.EnvironmentName);
                Assert.AreEqual(sourceItem.Id, createdMachine.ParentId);
                Assert.AreEqual(this.UserName, createdMachine.CreatedByUserName);
                AssertIsRecent(createdMachine.CreatedDateTimeUtc);
                Assert.AreEqual(this.UserName, createdMachine.UpdatedByUserName);
                AssertIsRecent(createdMachine.UpdatedDateTimeUtc);
                AssertDictionary(sourceMachine.ConfigurationValueList, createdMachine.ConfigurationValueList);
            }
        }

        private void AssertDictionary<T1,T2>(Dictionary<T1, T2> expected, Dictionary<T1,T2> actual)
        {
            if(expected == null)
            {
                Assert.IsNull(actual);
            }
            else 
            {
                Assert.AreEqual(expected.Count, actual.Count);
                foreach(var expectedKey in expected.Keys)
                {
                    Assert.IsTrue(actual.ContainsKey(expectedKey));
                    Assert.AreEqual(expected[expectedKey], actual[expectedKey]);
                }
            }
        }

        private Dictionary<string, string> CreateTestConfigurationValueList()
        {
            return this.Fixture.Create<Dictionary<string, string>>();
        }

        private List<DeployConfiguration> CreateTestConfigurationList(string projectId, int count, IProjectRepository sut)
        {
            var returnList = new List<DeployConfiguration>();
            for (int i = 0; i < count; i++)
            {
                var item = CreateTestConfiguration(sut, projectId);
                returnList.Add(item);
            }
            return returnList;
        }

        private DeployConfiguration CreateTestConfiguration(IProjectRepository sut, string projectId)
        {
            return sut.CreateConfiguration(projectId, this.Fixture.Create<string>("ConfigurationName"), this.Fixture.Create<EnumDeploymentIsolationType>());
        }

        private List<DeployComponent> CreateTestComponentList(string projectId, int count, IProjectRepository sut)
        {
            var returnList = new List<DeployComponent>();
            for (int i = 0; i < count; i++)
            {
                var item = this.CreateTestComponent(sut, projectId);
                returnList.Add(item);
            }
            return returnList;
        }

        private DeployComponent CreateTestComponent(IProjectRepository sut, string projectId)
        {
            return sut.CreateComponent(projectId, this.Fixture.Create<string>("ComponentName"), false, null, this.Fixture.Create<EnumDeploymentIsolationType>());
        }

        private class CreateTestData
        {
            public DeployProject Project { get; set; }
            public string EnvironmentName { get; set; }
            public List<DeployEnvironmentConfiguration> EnvironmentComponentList { get; set; }
            public List<DeployEnvironmentConfiguration> EnvironmentConfigurationList { get; set; }
        }


        private CreateTestData GetCreateTestData(IProjectRepository sut, string projectId=null)
        {
            var returnValue = new CreateTestData();
            if(string.IsNullOrEmpty(projectId))
            {
                returnValue.Project = this.CreateTestProject(sut);
            }
            else 
            {
                returnValue.Project = sut.GetProject(projectId);
            }
            var componentList = this.CreateTestComponentList(returnValue.Project.Id, 5, sut);
            var configurationList = this.CreateTestConfigurationList(returnValue.Project.Id, 3, sut);

            returnValue.EnvironmentComponentList = (from i in componentList
                                                    select GetCreateTestDeployEnvironmentConfiguration(i)).ToList();

            returnValue.EnvironmentConfigurationList = (from i in configurationList
                                                            select GetCreateTestDeployEnvironmentConfiguration(i)).ToList();

            returnValue.EnvironmentName = this.Fixture.Create<string>("EnvironmentName");

            return returnValue;
        }

        private DeployEnvironmentConfiguration GetCreateTestDeployEnvironmentConfiguration(DeployConfiguration configuration)
        {
            return new DeployEnvironmentConfiguration
            {
                ConfigurationValueList = this.CreateTestConfigurationValueList(),
                DeployCredentialsId = null,
                ParentId = configuration.Id,
                MachineList = CreateTestMachineList(5)
            };
        }

        private List<DeployMachine> CreateTestMachineList(int count)
        {
            var machineList = new List<DeployMachine>();
            for (int i = 0; i < count; i++)
            {
                machineList.Add(CreateTestMachine());
            }
            return machineList;
        }

        private DeployMachine CreateTestMachine()
        {
            return new DeployMachine
            {
                ConfigurationValueList = this.CreateTestConfigurationValueList(),
                MachineName = this.Fixture.Create<string>("MachineName")
            };
        }

        private DeployEnvironmentConfiguration GetCreateTestDeployEnvironmentConfiguration(DeployComponent component)
        {
            return new DeployEnvironmentConfiguration
            {
                ConfigurationValueList = this.CreateTestConfigurationValueList(),
                DeployCredentialsId = null,
                ParentId = component.Id,
                MachineList = this.CreateTestMachineList(5)
            };
        }

        private DeployEnvironment CreateTestEnvironment(IProjectRepository sut, string projectId=null)
        {
            var testData = GetCreateTestData(sut, projectId);
            return sut.CreateEnvironment(testData.Project.Id, testData.EnvironmentName, testData.EnvironmentComponentList, testData.EnvironmentConfigurationList);
        }

        [Test]
        public void CreateEnvironment_StoresEnvironment()
        {
            var sut = this.GetRepository();

            var testData = GetCreateTestData(sut);
            var result = sut.CreateEnvironment(testData.Project.Id, testData.EnvironmentName, testData.EnvironmentComponentList, testData.EnvironmentConfigurationList);

            AssertCreatedEnvironment(result, testData.Project, testData.EnvironmentName, testData.EnvironmentComponentList, testData.EnvironmentConfigurationList, sut);
        }

        [Test]
        public void CreateEnvironment_MissingProjectId_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            var testData = GetCreateTestData(sut);
            Assert.Throws<ArgumentNullException>(() => sut.CreateEnvironment(null, testData.EnvironmentName, testData.EnvironmentComponentList, testData.EnvironmentConfigurationList));
        }

        [Test]
        public void CreateEnvironment_BadProjectId_ThrowsRecordNotFoundException()
        {
            var sut = this.GetRepository();
            
            var testData = GetCreateTestData(sut);
            Assert.Throws<RecordNotFoundException>(() => sut.CreateEnvironment(Guid.NewGuid().ToString(), testData.EnvironmentName, testData.EnvironmentComponentList, testData.EnvironmentConfigurationList));
        }

        [Test]
        public void CreateEnvironment_MissingEnvironmentName_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            var testData = GetCreateTestData(sut);
            Assert.Throws<ArgumentNullException>(() => sut.CreateEnvironment(testData.Project.Id, null, testData.EnvironmentComponentList, testData.EnvironmentConfigurationList));
        }

        [Test]
        public void CreateEnvironment_MissingComponentList_CreatesEnvironment()
        {
            var sut = this.GetRepository();

            var testData = GetCreateTestData(sut);
            testData.EnvironmentComponentList = null;
            var result = sut.CreateEnvironment(testData.Project.Id, testData.EnvironmentName, testData.EnvironmentComponentList, testData.EnvironmentConfigurationList);

            AssertCreatedEnvironment(result, testData.Project, testData.EnvironmentName, testData.EnvironmentComponentList, testData.EnvironmentConfigurationList, sut);
        }

        [Test]
        public void CreateEnvironment_EmptyComponentList_CreatesEnvironment()
        {
            var sut = this.GetRepository();

            var testData = GetCreateTestData(sut);
            testData.EnvironmentComponentList.Clear();
            var result = sut.CreateEnvironment(testData.Project.Id, testData.EnvironmentName, testData.EnvironmentComponentList, testData.EnvironmentConfigurationList);

            AssertCreatedEnvironment(result, testData.Project, testData.EnvironmentName, testData.EnvironmentComponentList, testData.EnvironmentConfigurationList, sut);
        }

        [Test]
        public void CreateEnvironment_MissingConfigurationList_CreatesEnvironment()
        {
            var sut = this.GetRepository();

            var testData = GetCreateTestData(sut);
            testData.EnvironmentConfigurationList = null;
            var result = sut.CreateEnvironment(testData.Project.Id, testData.EnvironmentName, testData.EnvironmentComponentList, testData.EnvironmentConfigurationList);

            AssertCreatedEnvironment(result, testData.Project, testData.EnvironmentName, testData.EnvironmentComponentList, testData.EnvironmentConfigurationList, sut);
        }

        [Test]
        public void CreateEnvironment_EmptyConfiguationList_CreatesEnvironment()
        {
            var sut = this.GetRepository();

            var testData = GetCreateTestData(sut);
            testData.EnvironmentConfigurationList.Clear();
            var result = sut.CreateEnvironment(testData.Project.Id, testData.EnvironmentName, testData.EnvironmentComponentList, testData.EnvironmentConfigurationList);

            AssertCreatedEnvironment(result, testData.Project, testData.EnvironmentName, testData.EnvironmentComponentList, testData.EnvironmentConfigurationList, sut);
        }

        [Test]
        public void CreateEnvironment_BadComponentId_ThrowsRecordNotFoundException()
        {
            var sut = this.GetRepository();

            var testData = GetCreateTestData(sut);
            var componentList = sut.GetComponentList(testData.Project.Id).ToArray();
            foreach(var item in componentList)
            {
                sut.DeleteComponent(item.ProjectId, item.Id);
            }
            testData.Project.ComponentList.Clear();
            Assert.Throws<RecordNotFoundException>(()=> sut.CreateEnvironment(testData.Project.Id, testData.EnvironmentName, testData.EnvironmentComponentList, testData.EnvironmentConfigurationList));
        }

        [Test]
        public void CreateEnvironment_BadConfigurationId_ThrowsRecordNotFoundException()
        {
            var sut = this.GetRepository();

            var testData = GetCreateTestData(sut);
            var configurationList = sut.GetConfigurationList(testData.Project.Id).ToArray();
            foreach (var item in configurationList)
            {
                sut.DeleteConfiguration(item.Id, testData.Project.Id);
            }
            testData.Project.ConfigurationList.Clear();
            Assert.Throws<RecordNotFoundException>(() => sut.CreateEnvironment(testData.Project.Id, testData.EnvironmentName, testData.EnvironmentComponentList, testData.EnvironmentConfigurationList));
        }
        [Test]
        public void GetEnvironmentList_GetsList()
        {
            var sut = this.GetRepository();

            var project = CreateTestProject(sut);
            var environmentList = new List<DeployEnvironment>();
            for (int i = 0; i < 5; i++)
            {
                var component = CreateTestEnvironment(sut, project.Id);
                environmentList.Add(component);
            }

            var result = sut.GetEnvironmentList(project.Id);

            Assert.GreaterOrEqual(result.Count, environmentList.Count);
            foreach (var environment in environmentList)
            {
                var item = result.SingleOrDefault(i => i.Id == environment.Id);
                Assert.IsNotNull(item);
                AssertEnvironment(environment, item);
            }
        }

        [Test]
        public void GetEnvironmentList_MissingProjectID_ThrowsNullException()
        {
            var sut = this.GetRepository();

            Assert.Throws<ArgumentNullException>(() => sut.GetEnvironmentList(null));
        }

        [Test]
        public void GetEnvironmentList_BadProjectID_ThrowsRecordNotFoundException()
        {
            var sut = this.GetRepository();

            Assert.Throws<RecordNotFoundException>(() => sut.GetEnvironmentList(Guid.NewGuid().ToString()));
        }

        [Test]
        public void GetEnvironment_ReturnsEnvironment()
        {
            var sut = this.GetRepository();

            var environment = CreateTestEnvironment(sut);
         
            var result = sut.GetEnvironment(environment.Id, environment.ProjectId);

            Assert.IsNotNull(result);
            AssertEnvironment(environment, result);
        }

        [Test]
        public void GetEnvironment_MissingEnvironmentID_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);

            Assert.Throws<ArgumentNullException>(() => sut.GetEnvironment(null, project.Id));
        }

        [Test]
        public void GetEnvironment_BadEnvironmentID_ThrowsRecordNotFoundException()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);

            Assert.Throws<RecordNotFoundException>(() => sut.GetEnvironment(Guid.NewGuid().ToString(), project.Id));
        }

        [Test]
        public void UpdateEnvironment_NewEnvironmentName_UpdatesEnvironment()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);
            var environment = this.CreateTestEnvironment(sut, project.Id);

            string newEnvironmentName = this.Fixture.Create<string>("EnvironmentName");
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);
            
            var result = sut.UpdateEnvironment(environment.Id, project.Id, newEnvironmentName, environment.ComponentList, environment.ConfigurationList);

            Assert.IsNotNull(result);
            Assert.AreEqual(environment.Id, result.Id);
            Assert.AreEqual(environment.ProjectId, result.ProjectId);
            Assert.AreEqual(newEnvironmentName, result.EnvironmentName);
            Assert.AreEqual(environment.CreatedByUserName, environment.CreatedByUserName);
            AssertDateEqual(environment.CreatedDateTimeUtc, result.CreatedDateTimeUtc);
            Assert.AreEqual(newUserName, result.UpdatedByUserName);
            AssertIsRecent(result.UpdatedDateTimeUtc);
            AssertEnvironmentConfigurationList(environment.ComponentList, result.ComponentList);
            AssertEnvironmentConfigurationList(environment.ConfigurationList, result.ConfigurationList);

            var dbItem = sut.GetEnvironment(environment.Id, project.Id);
            AssertEnvironment(result, dbItem);
        }

        [Test]
        public void UpdateEnvironment__MissingProjectID_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);
            var environment = this.CreateTestEnvironment(sut, project.Id);

            string newEnvironmentName = this.Fixture.Create<string>("EnvironmentName");
            var newIsolationType = this.Fixture.Create<EnumDeploymentIsolationType>();
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            Assert.Throws<ArgumentNullException>(() => sut.UpdateEnvironment(environment.Id, null, newEnvironmentName, environment.ComponentList, environment.ConfigurationList));
        }

        [Test]
        public void UpdateEnvironment_MissingEnvironmentId_ThrowsNullException()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);
            var environment = this.CreateTestEnvironment(sut, project.Id);

            string newEnvironmentName = this.Fixture.Create<string>("EnvironmentName");
            var newIsolationType = this.Fixture.Create<EnumDeploymentIsolationType>();
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            Assert.Throws<ArgumentNullException>(() => sut.UpdateEnvironment(null, project.Id, newEnvironmentName, environment.ComponentList, environment.ConfigurationList));
        }

        [Test]
        public void UpdateEnvironment_BadEnvironmentId_ThrowsRecordNotFoundException()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);
            var environment = this.CreateTestEnvironment(sut, project.Id);

            string newEnvironmentName = this.Fixture.Create<string>("EnvironmentName");
            var newIsolationType = this.Fixture.Create<EnumDeploymentIsolationType>();
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            Assert.Throws<RecordNotFoundException>(() => sut.UpdateEnvironment(Guid.NewGuid().ToString(), project.Id, newEnvironmentName, environment.ComponentList, environment.ConfigurationList));
        }

        [Test]
        public void UpdateEnvironment_BadComponentID_ThrowRecordNotFoundException()
        {
            var sut = this.GetRepository();

            var environment = this.CreateTestEnvironment(sut);

            string newEnvironmentName = this.Fixture.Create<string>("EnvironmentName");
            var newIsolationType = this.Fixture.Create<EnumDeploymentIsolationType>();
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            environment.ComponentList[0].ParentId = Guid.NewGuid().ToString();

            Assert.Throws<RecordNotFoundException>(() => sut.UpdateEnvironment(environment.Id, environment.ProjectId, newEnvironmentName, environment.ComponentList, environment.ConfigurationList));
        }

        [Test]
        public void UpdateEnvironment_MissingComponentID_ThrowArgumentNullException()
        {
            var sut = this.GetRepository();

            var environment = this.CreateTestEnvironment(sut);

            string newEnvironmentName = this.Fixture.Create<string>("EnvironmentName");
            var newIsolationType = this.Fixture.Create<EnumDeploymentIsolationType>();
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            environment.ComponentList[0].ParentId = null;

            Assert.Throws<ArgumentNullException>(() => sut.UpdateEnvironment(environment.Id, environment.ProjectId, newEnvironmentName, environment.ComponentList, environment.ConfigurationList));
        }

        [Test]
        public void UpdateEnvironment_MissingEnvironmentName_ThrowArgumentNullException()
        {
            var sut = this.GetRepository();

            var environment = this.CreateTestEnvironment(sut);

            string newEnvironmentName = null;
            var newIsolationType = this.Fixture.Create<EnumDeploymentIsolationType>();
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            Assert.Throws<ArgumentNullException>(() => sut.UpdateEnvironment(environment.Id, environment.ProjectId, newEnvironmentName, environment.ComponentList, environment.ConfigurationList));
        }

        [Test]
        public void UpdateEnvironment_RemoveComponent_UpdatesEnvironment()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);
            var environment = this.CreateTestEnvironment(sut, project.Id);

            string newEnvironmentName = this.Fixture.Create<string>("EnvironmentName");
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            environment.ComponentList.RemoveAt(0);

            var result = sut.UpdateEnvironment(environment.Id, project.Id, newEnvironmentName, environment.ComponentList, environment.ConfigurationList);

            Assert.IsNotNull(result);
            Assert.AreEqual(environment.Id, result.Id);
            Assert.AreEqual(environment.ProjectId, result.ProjectId);
            Assert.AreEqual(newEnvironmentName, result.EnvironmentName);
            Assert.AreEqual(environment.CreatedByUserName, environment.CreatedByUserName);
            AssertDateEqual(environment.CreatedDateTimeUtc, result.CreatedDateTimeUtc);
            Assert.AreEqual(newUserName, result.UpdatedByUserName);
            AssertIsRecent(result.UpdatedDateTimeUtc);
            AssertEnvironmentConfigurationList(environment.ComponentList, result.ComponentList);
            AssertEnvironmentConfigurationList(environment.ConfigurationList, result.ConfigurationList);

            var dbItem = sut.GetEnvironment(environment.Id, environment.ProjectId);
            AssertEnvironment(result, dbItem);
        }

        [Test]
        public void UpdateEnvironment_RemoveConfiguration_UpdatesEnvironment()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);
            var environment = this.CreateTestEnvironment(sut, project.Id);

            string newEnvironmentName = this.Fixture.Create<string>("EnvironmentName");
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            environment.ConfigurationList.RemoveAt(0);

            var result = sut.UpdateEnvironment(environment.Id, project.Id, newEnvironmentName, environment.ComponentList, environment.ConfigurationList);

            Assert.IsNotNull(result);
            Assert.AreEqual(environment.Id, result.Id);
            Assert.AreEqual(environment.ProjectId, result.ProjectId);
            Assert.AreEqual(newEnvironmentName, result.EnvironmentName);
            Assert.AreEqual(environment.CreatedByUserName, environment.CreatedByUserName);
            AssertDateEqual(environment.CreatedDateTimeUtc, result.CreatedDateTimeUtc);
            Assert.AreEqual(newUserName, result.UpdatedByUserName);
            AssertIsRecent(result.UpdatedDateTimeUtc);
            AssertEnvironmentConfigurationList(environment.ComponentList, result.ComponentList);
            AssertEnvironmentConfigurationList(environment.ConfigurationList, result.ConfigurationList);

            var dbItem = sut.GetEnvironment(environment.Id, environment.ProjectId);
            AssertEnvironment(result, dbItem);
        }

        [Test]
        public void UpdateEnvironment_AddComponent_UpdatesEnvironment()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);
            var environment = this.CreateTestEnvironment(sut, project.Id);

            string newEnvironmentName = this.Fixture.Create<string>("EnvironmentName");
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            var component = this.CreateTestComponent(sut, project.Id);
            environment.ComponentList.Add(this.GetCreateTestDeployEnvironmentConfiguration(component));

            var result = sut.UpdateEnvironment(environment.Id, project.Id, newEnvironmentName, environment.ComponentList, environment.ConfigurationList);

            Assert.IsNotNull(result);
            Assert.AreEqual(environment.Id, result.Id);
            Assert.AreEqual(environment.ProjectId, result.ProjectId);
            Assert.AreEqual(newEnvironmentName, result.EnvironmentName);
            Assert.AreEqual(environment.CreatedByUserName, environment.CreatedByUserName);
            AssertDateEqual(environment.CreatedDateTimeUtc, result.CreatedDateTimeUtc);
            Assert.AreEqual(newUserName, result.UpdatedByUserName);
            AssertIsRecent(result.UpdatedDateTimeUtc);
            AssertEnvironmentConfigurationList(environment.ComponentList, result.ComponentList);
            AssertEnvironmentConfigurationList(environment.ConfigurationList, result.ConfigurationList);

            var dbItem = sut.GetEnvironment(environment.Id, environment.ProjectId);
            AssertEnvironment(result, dbItem);
        }

        [Test]
        public void UpdateEnvironment_AddConfiguration_UpdatesEnvironment()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);
            var environment = this.CreateTestEnvironment(sut, project.Id);

            string newEnvironmentName = this.Fixture.Create<string>("EnvironmentName");
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            var configuration = this.CreateTestConfiguration(sut, project.Id);
            environment.ConfigurationList.Add(this.GetCreateTestDeployEnvironmentConfiguration(configuration));

            var result = sut.UpdateEnvironment(environment.Id, project.Id, newEnvironmentName, environment.ComponentList, environment.ConfigurationList);

            Assert.IsNotNull(result);
            Assert.AreEqual(environment.Id, result.Id);
            Assert.AreEqual(environment.ProjectId, result.ProjectId);
            Assert.AreEqual(newEnvironmentName, result.EnvironmentName);
            Assert.AreEqual(environment.CreatedByUserName, environment.CreatedByUserName);
            AssertDateEqual(environment.CreatedDateTimeUtc, result.CreatedDateTimeUtc);
            Assert.AreEqual(newUserName, result.UpdatedByUserName);
            AssertIsRecent(result.UpdatedDateTimeUtc);
            AssertEnvironmentConfigurationList(environment.ComponentList, result.ComponentList);
            AssertEnvironmentConfigurationList(environment.ConfigurationList, result.ConfigurationList);

            var dbItem = sut.GetEnvironment(environment.Id, environment.ProjectId);
            AssertEnvironment(result, dbItem);
        }

        [Test]
        public void UpdateEnvironment_AddComponentMachine_UpdatesEnvironment()
        {
            var sut = this.GetRepository();
            var project = this.CreateTestProject(sut);
            var environment = this.CreateTestEnvironment(sut, project.Id);

            string newEnvironmentName = this.Fixture.Create<string>("EnvironmentName");
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            var machine = this.CreateTestMachine();
            environment.ComponentList[0].MachineList.Add(machine);

            var result = sut.UpdateEnvironment(environment.Id, project.Id, newEnvironmentName, environment.ComponentList, environment.ConfigurationList);

            Assert.IsNotNull(result);
            Assert.AreEqual(environment.Id, result.Id);
            Assert.AreEqual(environment.ProjectId, result.ProjectId);
            Assert.AreEqual(newEnvironmentName, result.EnvironmentName);
            Assert.AreEqual(environment.CreatedByUserName, environment.CreatedByUserName);
            AssertDateEqual(environment.CreatedDateTimeUtc, result.CreatedDateTimeUtc);
            Assert.AreEqual(newUserName, result.UpdatedByUserName);
            AssertIsRecent(result.UpdatedDateTimeUtc);
            AssertEnvironmentConfigurationList(environment.ComponentList, result.ComponentList);
            AssertEnvironmentConfigurationList(environment.ConfigurationList, result.ConfigurationList);

            var dbItem = sut.GetEnvironment(environment.Id, environment.ProjectId);
            AssertEnvironment(result, dbItem);
        }

        [Test]
        public void UpdateEnvironment_AddConfigurationMachine_UpdatesEnvironment()
        {
            var sut = this.GetRepository();
            var project = this.CreateTestProject(sut);
            var environment = this.CreateTestEnvironment(sut, project.Id);

            string newEnvironmentName = this.Fixture.Create<string>("EnvironmentName");
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            var machine = this.CreateTestMachine();
            environment.ConfigurationList[0].MachineList.Add(machine);

            var result = sut.UpdateEnvironment(environment.Id, project.Id, newEnvironmentName, environment.ComponentList, environment.ConfigurationList);

            Assert.IsNotNull(result);
            Assert.AreEqual(environment.Id, result.Id);
            Assert.AreEqual(environment.ProjectId, result.ProjectId);
            Assert.AreEqual(newEnvironmentName, result.EnvironmentName);
            Assert.AreEqual(environment.CreatedByUserName, environment.CreatedByUserName);
            AssertDateEqual(environment.CreatedDateTimeUtc, result.CreatedDateTimeUtc);
            Assert.AreEqual(newUserName, result.UpdatedByUserName);
            AssertIsRecent(result.UpdatedDateTimeUtc);
            AssertEnvironmentConfigurationList(environment.ComponentList, result.ComponentList);
            AssertEnvironmentConfigurationList(environment.ConfigurationList, result.ConfigurationList);

            var dbItem = sut.GetEnvironment(environment.Id, project.Id);
            AssertEnvironment(result, dbItem);
        }

        [Test]
        public void UpdateEnvironment_EditComponentMachine_UpdatesEnvironment()
        {
            var sut = this.GetRepository();
            var project = this.CreateTestProject(sut);
            var environment = this.CreateTestEnvironment(sut, project.Id);

            string newEnvironmentName = this.Fixture.Create<string>("EnvironmentName");
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            var machine = this.CreateTestMachine();
            environment.ComponentList[0].MachineList[0].MachineName = this.Fixture.Create<string>("MachineName");

            var result = sut.UpdateEnvironment(environment.Id, project.Id, newEnvironmentName, environment.ComponentList, environment.ConfigurationList);

            Assert.IsNotNull(result);
            Assert.AreEqual(environment.Id, result.Id);
            Assert.AreEqual(environment.ProjectId, result.ProjectId);
            Assert.AreEqual(newEnvironmentName, result.EnvironmentName);
            Assert.AreEqual(environment.CreatedByUserName, environment.CreatedByUserName);
            AssertDateEqual(environment.CreatedDateTimeUtc, result.CreatedDateTimeUtc);
            Assert.AreEqual(newUserName, result.UpdatedByUserName);
            AssertIsRecent(result.UpdatedDateTimeUtc);
            AssertEnvironmentConfigurationList(environment.ComponentList, result.ComponentList);
            AssertEnvironmentConfigurationList(environment.ConfigurationList, result.ConfigurationList);

            var dbItem = sut.GetEnvironment(environment.Id, project.Id);
            AssertEnvironment(result, dbItem);
        }

        [Test]
        public void UpdateEnvironment_EditConfigurationMachine_UpdatesEnvironment()
        {
            var sut = this.GetRepository();
            var project = this.CreateTestProject(sut);
            var environment = this.CreateTestEnvironment(sut, project.Id);

            string newEnvironmentName = this.Fixture.Create<string>("EnvironmentName");
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            var machine = this.CreateTestMachine();
            environment.ConfigurationList[0].MachineList[0].MachineName = this.Fixture.Create<string>("MachineName");

            var result = sut.UpdateEnvironment(environment.Id, project.Id, newEnvironmentName, environment.ComponentList, environment.ConfigurationList);

            Assert.IsNotNull(result);
            Assert.AreEqual(environment.Id, result.Id);
            Assert.AreEqual(environment.ProjectId, result.ProjectId);
            Assert.AreEqual(newEnvironmentName, result.EnvironmentName);
            Assert.AreEqual(environment.CreatedByUserName, environment.CreatedByUserName);
            AssertDateEqual(environment.CreatedDateTimeUtc, result.CreatedDateTimeUtc);
            Assert.AreEqual(newUserName, result.UpdatedByUserName);
            AssertIsRecent(result.UpdatedDateTimeUtc);
            AssertEnvironmentConfigurationList(environment.ComponentList, result.ComponentList);
            AssertEnvironmentConfigurationList(environment.ConfigurationList, result.ConfigurationList);

            var dbItem = sut.GetEnvironment(environment.Id, project.Id);
            AssertEnvironment(result, dbItem);
        }

        [Test]
        public void UpdateEnvironment_RemoveComponentMachine_UpdatesEnvironment()
        {
            var sut = this.GetRepository();
            var project = this.CreateTestProject(sut);
            var environment = this.CreateTestEnvironment(sut, project.Id);

            string newEnvironmentName = this.Fixture.Create<string>("EnvironmentName");
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            environment.ComponentList[0].MachineList.RemoveAt(0);

            var result = sut.UpdateEnvironment(environment.Id, project.Id, newEnvironmentName, environment.ComponentList, environment.ConfigurationList);

            Assert.IsNotNull(result);
            Assert.AreEqual(environment.Id, result.Id);
            Assert.AreEqual(environment.ProjectId, result.ProjectId);
            Assert.AreEqual(newEnvironmentName, result.EnvironmentName);
            Assert.AreEqual(environment.CreatedByUserName, environment.CreatedByUserName);
            AssertDateEqual(environment.CreatedDateTimeUtc, result.CreatedDateTimeUtc);
            Assert.AreEqual(newUserName, result.UpdatedByUserName);
            AssertIsRecent(result.UpdatedDateTimeUtc);
            AssertEnvironmentConfigurationList(environment.ComponentList, result.ComponentList);
            AssertEnvironmentConfigurationList(environment.ConfigurationList, result.ConfigurationList);

            var dbItem = sut.GetEnvironment(environment.Id, project.Id);
            AssertEnvironment(result, dbItem);
        }

        [Test]
        public void UpdateEnvironment_RemoveConfigurationMachine_UpdatesEnvironment()
        {
            var sut = this.GetRepository();
            var project = this.CreateTestProject(sut);
            var environment = this.CreateTestEnvironment(sut, project.Id);

            string newEnvironmentName = this.Fixture.Create<string>("EnvironmentName");
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            environment.ConfigurationList[0].MachineList.RemoveAt(0);

            var result = sut.UpdateEnvironment(environment.Id, project.Id, newEnvironmentName, environment.ComponentList, environment.ConfigurationList);

            Assert.IsNotNull(result);
            Assert.AreEqual(environment.Id, result.Id);
            Assert.AreEqual(environment.ProjectId, result.ProjectId);
            Assert.AreEqual(newEnvironmentName, result.EnvironmentName);
            Assert.AreEqual(environment.CreatedByUserName, environment.CreatedByUserName);
            AssertDateEqual(environment.CreatedDateTimeUtc, result.CreatedDateTimeUtc);
            Assert.AreEqual(newUserName, result.UpdatedByUserName);
            AssertIsRecent(result.UpdatedDateTimeUtc);
            AssertEnvironmentConfigurationList(environment.ComponentList, result.ComponentList);
            AssertEnvironmentConfigurationList(environment.ConfigurationList, result.ConfigurationList);

            var dbItem = sut.GetEnvironment(environment.Id, project.Id);
            AssertEnvironment(result, dbItem);
        }

        [Test]
        public void UpdateEnvironment_AddComponentConfigValue_UpdatesEnvironment()
        {
            var sut = this.GetRepository();
            var project = this.CreateTestProject(sut);
            var environment = this.CreateTestEnvironment(sut, project.Id);

            string newEnvironmentName = this.Fixture.Create<string>("EnvironmentName");
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            environment.ComponentList[0].ConfigurationValueList.Add(this.Fixture.Create<string>("ConfigName"),this.Fixture.Create<string>("ConfigValue"));

            var result = sut.UpdateEnvironment(environment.Id, project.Id, newEnvironmentName, environment.ComponentList, environment.ConfigurationList);

            Assert.IsNotNull(result);
            Assert.AreEqual(environment.Id, result.Id);
            Assert.AreEqual(environment.ProjectId, result.ProjectId);
            Assert.AreEqual(newEnvironmentName, result.EnvironmentName);
            Assert.AreEqual(environment.CreatedByUserName, environment.CreatedByUserName);
            AssertDateEqual(environment.CreatedDateTimeUtc, result.CreatedDateTimeUtc);
            Assert.AreEqual(newUserName, result.UpdatedByUserName);
            AssertIsRecent(result.UpdatedDateTimeUtc);
            AssertEnvironmentConfigurationList(environment.ComponentList, result.ComponentList);
            AssertEnvironmentConfigurationList(environment.ConfigurationList, result.ConfigurationList);

            var dbItem = sut.GetEnvironment(environment.Id, environment.ProjectId);
            AssertEnvironment(result, dbItem);
        }

        [Test]
        public void UpdateEnvironment_AddConfigurationConfigValue_UpdatesEnvironment()
        {
            var sut = this.GetRepository();
            var project = this.CreateTestProject(sut);
            var environment = this.CreateTestEnvironment(sut, project.Id);

            string newEnvironmentName = this.Fixture.Create<string>("EnvironmentName");
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            environment.ConfigurationList[0].ConfigurationValueList.Add(this.Fixture.Create<string>("ConfigName"), this.Fixture.Create<string>("ConfigValue"));

            var result = sut.UpdateEnvironment(environment.Id, project.Id, newEnvironmentName, environment.ComponentList, environment.ConfigurationList);

            Assert.IsNotNull(result);
            Assert.AreEqual(environment.Id, result.Id);
            Assert.AreEqual(environment.ProjectId, result.ProjectId);
            Assert.AreEqual(newEnvironmentName, result.EnvironmentName);
            Assert.AreEqual(environment.CreatedByUserName, environment.CreatedByUserName);
            AssertDateEqual(environment.CreatedDateTimeUtc, result.CreatedDateTimeUtc);
            Assert.AreEqual(newUserName, result.UpdatedByUserName);
            AssertIsRecent(result.UpdatedDateTimeUtc);
            AssertEnvironmentConfigurationList(environment.ComponentList, result.ComponentList);
            AssertEnvironmentConfigurationList(environment.ConfigurationList, result.ConfigurationList);

            var dbItem = sut.GetEnvironment(environment.Id, environment.ProjectId);
            AssertEnvironment(result, dbItem);
        }

        [Test]
        public void UpdateEnvironment_EditComponentConfigValue_UpdatesEnvironment()
        {
            var sut = this.GetRepository();
            var project = this.CreateTestProject(sut);
            var environment = this.CreateTestEnvironment(sut, project.Id);

            string newEnvironmentName = this.Fixture.Create<string>("EnvironmentName");
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            var key = environment.ComponentList[0].ConfigurationValueList.Keys.First();
            environment.ComponentList[0].ConfigurationValueList[key] = this.Fixture.Create<string>("ConfigValue");

            var result = sut.UpdateEnvironment(environment.Id, project.Id, newEnvironmentName, environment.ComponentList, environment.ConfigurationList);

            Assert.IsNotNull(result);
            Assert.AreEqual(environment.Id, result.Id);
            Assert.AreEqual(environment.ProjectId, result.ProjectId);
            Assert.AreEqual(newEnvironmentName, result.EnvironmentName);
            Assert.AreEqual(environment.CreatedByUserName, environment.CreatedByUserName);
            AssertDateEqual(environment.CreatedDateTimeUtc, result.CreatedDateTimeUtc);
            Assert.AreEqual(newUserName, result.UpdatedByUserName);
            AssertIsRecent(result.UpdatedDateTimeUtc);
            AssertEnvironmentConfigurationList(environment.ComponentList, result.ComponentList);
            AssertEnvironmentConfigurationList(environment.ConfigurationList, result.ConfigurationList);

            var dbItem = sut.GetEnvironment(environment.Id, environment.ProjectId);
            AssertEnvironment(result, dbItem);
        }

        [Test]
        public void UpdateEnvironment_EditConfigurationConfigValue_UpdatesEnvironment()
        {
            var sut = this.GetRepository();
            var project = this.CreateTestProject(sut);
            var environment = this.CreateTestEnvironment(sut, project.Id);

            string newEnvironmentName = this.Fixture.Create<string>("EnvironmentName");
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            var key = environment.ConfigurationList[0].ConfigurationValueList.Keys.First();
            environment.ConfigurationList[0].ConfigurationValueList[key] = this.Fixture.Create<string>("ConfigValue");

            var result = sut.UpdateEnvironment(environment.Id, project.Id, newEnvironmentName, environment.ComponentList, environment.ConfigurationList);

            Assert.IsNotNull(result);
            Assert.AreEqual(environment.Id, result.Id);
            Assert.AreEqual(environment.ProjectId, result.ProjectId);
            Assert.AreEqual(newEnvironmentName, result.EnvironmentName);
            Assert.AreEqual(environment.CreatedByUserName, environment.CreatedByUserName);
            AssertDateEqual(environment.CreatedDateTimeUtc, result.CreatedDateTimeUtc);
            Assert.AreEqual(newUserName, result.UpdatedByUserName);
            AssertIsRecent(result.UpdatedDateTimeUtc);
            AssertEnvironmentConfigurationList(environment.ComponentList, result.ComponentList);
            AssertEnvironmentConfigurationList(environment.ConfigurationList, result.ConfigurationList);

            var dbItem = sut.GetEnvironment(environment.Id, environment.ProjectId);
            AssertEnvironment(result, dbItem);
        }

        [Test]
        public void UpdateEnvironment_RemoveComponentConfigValue_UpdatesEnvironment()
        {
            var sut = this.GetRepository();
            var project = this.CreateTestProject(sut);
            var environment = this.CreateTestEnvironment(sut, project.Id);

            string newEnvironmentName = this.Fixture.Create<string>("EnvironmentName");
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            environment.ComponentList[0].ConfigurationValueList.Remove(environment.ComponentList[0].ConfigurationValueList.Keys.First());

            var result = sut.UpdateEnvironment(environment.Id, project.Id, newEnvironmentName, environment.ComponentList, environment.ConfigurationList);

            Assert.IsNotNull(result);
            Assert.AreEqual(environment.Id, result.Id);
            Assert.AreEqual(environment.ProjectId, result.ProjectId);
            Assert.AreEqual(newEnvironmentName, result.EnvironmentName);
            Assert.AreEqual(environment.CreatedByUserName, environment.CreatedByUserName);
            AssertDateEqual(environment.CreatedDateTimeUtc, result.CreatedDateTimeUtc);
            Assert.AreEqual(newUserName, result.UpdatedByUserName);
            AssertIsRecent(result.UpdatedDateTimeUtc);
            AssertEnvironmentConfigurationList(environment.ComponentList, result.ComponentList);
            AssertEnvironmentConfigurationList(environment.ConfigurationList, result.ConfigurationList);

            var dbItem = sut.GetEnvironment(environment.Id, environment.ProjectId);
            AssertEnvironment(result, dbItem);
        }

        [Test]
        public void UpdateEnvironment_RemoveConfigurationConfigValue_UpdatesEnvironment()
        {
            var sut = this.GetRepository();
            var project = this.CreateTestProject(sut);
            var environment = this.CreateTestEnvironment(sut, project.Id);

            string newEnvironmentName = this.Fixture.Create<string>("EnvironmentName");
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            environment.ConfigurationList[0].ConfigurationValueList.Remove(environment.ConfigurationList[0].ConfigurationValueList.Keys.First());

            var result = sut.UpdateEnvironment(environment.Id, project.Id, newEnvironmentName, environment.ComponentList, environment.ConfigurationList);

            Assert.IsNotNull(result);
            Assert.AreEqual(environment.Id, result.Id);
            Assert.AreEqual(environment.ProjectId, result.ProjectId);
            Assert.AreEqual(newEnvironmentName, result.EnvironmentName);
            Assert.AreEqual(environment.CreatedByUserName, environment.CreatedByUserName);
            AssertDateEqual(environment.CreatedDateTimeUtc, result.CreatedDateTimeUtc);
            Assert.AreEqual(newUserName, result.UpdatedByUserName);
            AssertIsRecent(result.UpdatedDateTimeUtc);
            AssertEnvironmentConfigurationList(environment.ComponentList, result.ComponentList);
            AssertEnvironmentConfigurationList(environment.ConfigurationList, result.ConfigurationList);

            var dbItem = sut.GetEnvironment(environment.Id, environment.ProjectId);
            AssertEnvironment(result, dbItem);
        }


        [Test]
        public void UpdateEnvironment_AddComponentMachineConfigValue_UpdatesEnvironment()
        {
            var sut = this.GetRepository();
            var project = this.CreateTestProject(sut);
            var environment = this.CreateTestEnvironment(sut, project.Id);

            string newEnvironmentName = this.Fixture.Create<string>("EnvironmentName");
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            environment.ComponentList[0].MachineList[0].ConfigurationValueList.Add(this.Fixture.Create<string>("ConfigName"), this.Fixture.Create<string>("ConfigValue"));

            var result = sut.UpdateEnvironment(environment.Id, project.Id, newEnvironmentName, environment.ComponentList, environment.ConfigurationList);

            Assert.IsNotNull(result);
            Assert.AreEqual(environment.Id, result.Id);
            Assert.AreEqual(environment.ProjectId, result.ProjectId);
            Assert.AreEqual(newEnvironmentName, result.EnvironmentName);
            Assert.AreEqual(environment.CreatedByUserName, environment.CreatedByUserName);
            AssertDateEqual(environment.CreatedDateTimeUtc, result.CreatedDateTimeUtc);
            Assert.AreEqual(newUserName, result.UpdatedByUserName);
            AssertIsRecent(result.UpdatedDateTimeUtc);
            AssertEnvironmentConfigurationList(environment.ComponentList, result.ComponentList);
            AssertEnvironmentConfigurationList(environment.ConfigurationList, result.ConfigurationList);

            var dbItem = sut.GetEnvironment(environment.Id, environment.ProjectId);
            AssertEnvironment(result, dbItem);
        }

        [Test]
        public void UpdateEnvironment_AddConfigurationMachineConfigValue_UpdatesEnvironment()
        {
            var sut = this.GetRepository();
            var project = this.CreateTestProject(sut);
            var environment = this.CreateTestEnvironment(sut, project.Id);

            string newEnvironmentName = this.Fixture.Create<string>("EnvironmentName");
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            environment.ConfigurationList[0].MachineList[0].ConfigurationValueList.Add(this.Fixture.Create<string>("ConfigName"), this.Fixture.Create<string>("ConfigValue"));

            var result = sut.UpdateEnvironment(environment.Id, project.Id, newEnvironmentName, environment.ComponentList, environment.ConfigurationList);

            Assert.IsNotNull(result);
            Assert.AreEqual(environment.Id, result.Id);
            Assert.AreEqual(environment.ProjectId, result.ProjectId);
            Assert.AreEqual(newEnvironmentName, result.EnvironmentName);
            Assert.AreEqual(environment.CreatedByUserName, environment.CreatedByUserName);
            AssertDateEqual(environment.CreatedDateTimeUtc, result.CreatedDateTimeUtc);
            Assert.AreEqual(newUserName, result.UpdatedByUserName);
            AssertIsRecent(result.UpdatedDateTimeUtc);
            AssertEnvironmentConfigurationList(environment.ComponentList, result.ComponentList);
            AssertEnvironmentConfigurationList(environment.ConfigurationList, result.ConfigurationList);

            var dbItem = sut.GetEnvironment(environment.Id, environment.ProjectId);
            AssertEnvironment(result, dbItem);
        }

        [Test]
        public void UpdateEnvironment_EditComponentMachineConfigValue_UpdatesEnvironment()
        {
            var sut = this.GetRepository();
            var project = this.CreateTestProject(sut);
            var environment = this.CreateTestEnvironment(sut, project.Id);

            string newEnvironmentName = this.Fixture.Create<string>("EnvironmentName");
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            var key = environment.ComponentList[0].MachineList[0].ConfigurationValueList.Keys.First();
            environment.ComponentList[0].MachineList[0].ConfigurationValueList[key] = this.Fixture.Create<string>("ConfigValue");

            var result = sut.UpdateEnvironment(environment.Id, project.Id, newEnvironmentName, environment.ComponentList, environment.ConfigurationList);

            Assert.IsNotNull(result);
            Assert.AreEqual(environment.Id, result.Id);
            Assert.AreEqual(environment.ProjectId, result.ProjectId);
            Assert.AreEqual(newEnvironmentName, result.EnvironmentName);
            Assert.AreEqual(environment.CreatedByUserName, environment.CreatedByUserName);
            AssertDateEqual(environment.CreatedDateTimeUtc, result.CreatedDateTimeUtc);
            Assert.AreEqual(newUserName, result.UpdatedByUserName);
            AssertIsRecent(result.UpdatedDateTimeUtc);
            AssertEnvironmentConfigurationList(environment.ComponentList, result.ComponentList);
            AssertEnvironmentConfigurationList(environment.ConfigurationList, result.ConfigurationList);

            var dbItem = sut.GetEnvironment(environment.Id, environment.ProjectId);
            AssertEnvironment(result, dbItem);
        }

        [Test]
        public void UpdateEnvironment_EditConfigurationMachineConfigValue_UpdatesEnvironment()
        {
            var sut = this.GetRepository();
            var project = this.CreateTestProject(sut);
            var environment = this.CreateTestEnvironment(sut, project.Id);

            string newEnvironmentName = this.Fixture.Create<string>("EnvironmentName");
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            var key = environment.ConfigurationList[0].MachineList[0].ConfigurationValueList.Keys.First();
            environment.ConfigurationList[0].MachineList[0].ConfigurationValueList[key] = this.Fixture.Create<string>("ConfigValue");

            var result = sut.UpdateEnvironment(environment.Id, project.Id, newEnvironmentName, environment.ComponentList, environment.ConfigurationList);

            Assert.IsNotNull(result);
            Assert.AreEqual(environment.Id, result.Id);
            Assert.AreEqual(environment.ProjectId, result.ProjectId);
            Assert.AreEqual(newEnvironmentName, result.EnvironmentName);
            Assert.AreEqual(environment.CreatedByUserName, environment.CreatedByUserName);
            AssertDateEqual(environment.CreatedDateTimeUtc, result.CreatedDateTimeUtc);
            Assert.AreEqual(newUserName, result.UpdatedByUserName);
            AssertIsRecent(result.UpdatedDateTimeUtc);
            AssertEnvironmentConfigurationList(environment.ComponentList, result.ComponentList);
            AssertEnvironmentConfigurationList(environment.ConfigurationList, result.ConfigurationList);

            var dbItem = sut.GetEnvironment(environment.Id, environment.ProjectId);
            AssertEnvironment(result, dbItem);
        }

        [Test]
        public void UpdateEnvironment_RemoveComponentMachineConfigValue_UpdatesEnvironment()
        {
            var sut = this.GetRepository();
            var project = this.CreateTestProject(sut);
            var environment = this.CreateTestEnvironment(sut, project.Id);

            string newEnvironmentName = this.Fixture.Create<string>("EnvironmentName");
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            environment.ComponentList[0].MachineList[0].ConfigurationValueList.Remove(environment.ComponentList[0].ConfigurationValueList.Keys.First());

            var result = sut.UpdateEnvironment(environment.Id, project.Id, newEnvironmentName, environment.ComponentList, environment.ConfigurationList);

            Assert.IsNotNull(result);
            Assert.AreEqual(environment.Id, result.Id);
            Assert.AreEqual(environment.ProjectId, result.ProjectId);
            Assert.AreEqual(newEnvironmentName, result.EnvironmentName);
            Assert.AreEqual(environment.CreatedByUserName, environment.CreatedByUserName);
            AssertDateEqual(environment.CreatedDateTimeUtc, result.CreatedDateTimeUtc);
            Assert.AreEqual(newUserName, result.UpdatedByUserName);
            AssertIsRecent(result.UpdatedDateTimeUtc);
            AssertEnvironmentConfigurationList(environment.ComponentList, result.ComponentList);
            AssertEnvironmentConfigurationList(environment.ConfigurationList, result.ConfigurationList);

            var dbItem = sut.GetEnvironment(environment.Id, environment.ProjectId);
            AssertEnvironment(result, dbItem);
        }

        [Test]
        public void UpdateEnvironment_RemoveConfigurationMachineConfigValue_UpdatesEnvironment()
        {
            var sut = this.GetRepository();
            var project = this.CreateTestProject(sut);
            var environment = this.CreateTestEnvironment(sut, project.Id);

            string newEnvironmentName = this.Fixture.Create<string>("EnvironmentName");
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            environment.ConfigurationList[0].MachineList[0].ConfigurationValueList.Remove(environment.ConfigurationList[0].ConfigurationValueList.Keys.First());

            var result = sut.UpdateEnvironment(environment.Id, project.Id, newEnvironmentName, environment.ComponentList, environment.ConfigurationList);

            Assert.IsNotNull(result);
            Assert.AreEqual(environment.Id, result.Id);
            Assert.AreEqual(environment.ProjectId, result.ProjectId);
            Assert.AreEqual(newEnvironmentName, result.EnvironmentName);
            Assert.AreEqual(environment.CreatedByUserName, environment.CreatedByUserName);
            AssertDateEqual(environment.CreatedDateTimeUtc, result.CreatedDateTimeUtc);
            Assert.AreEqual(newUserName, result.UpdatedByUserName);
            AssertIsRecent(result.UpdatedDateTimeUtc);
            AssertEnvironmentConfigurationList(environment.ComponentList, result.ComponentList);
            AssertEnvironmentConfigurationList(environment.ConfigurationList, result.ConfigurationList);

            var dbItem = sut.GetEnvironment(environment.Id, environment.ProjectId);
            AssertEnvironment(result, dbItem);
        }

        [Test]
        public void DeleteEnvironment_DeletesEnvironment()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);
            var environment = this.CreateTestEnvironment(sut, project.Id);

            sut.DeleteEnvironment(environment.Id, environment.ProjectId);

            var dbConfiguration = sut.TryGetConfiguration(environment.Id, project.Id);
            Assert.IsNull(dbConfiguration);

            var dbProject = sut.GetProject(project.Id);
            var dbProjectEnvironment = dbProject.EnvironmentList.SingleOrDefault(i => i.Id == environment.Id);
            Assert.IsNull(dbProjectEnvironment);
        }

        [Test]
        public void DeleteEnvironment_MissingConfigurationID_ThrowsNullException()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);
            
            Assert.Throws<ArgumentNullException>(() => sut.DeleteEnvironment(null, project.Id));
        }

        [Test]
        public void DeleteEnvironment_BadEnvironmentID_ThrowsRecordNotFoundException()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut); 
            
            Assert.Throws<RecordNotFoundException>(() => sut.DeleteEnvironment(Guid.NewGuid().ToString(), project.Id));
        }

        [Test]
        public void DeleteProject_DeletesEnvironment()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);
            var environment = this.CreateTestEnvironment(sut, project.Id);

            sut.DeleteProject(project.Id);

            Assert.Throws<RecordNotFoundException>(()=>sut.GetEnvironment(environment.Id, environment.ProjectId));
        }

        [Test]
        public void GetProject_GetsEnvironmentList()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);
            var environmentList = new List<DeployEnvironment>();
            for(int i = 0; i < 5; i++)
            {
                var environment = this.CreateTestEnvironment(sut, project.Id);
                environmentList.Add(environment);
            }

            var dbProject = sut.GetProject(project.Id);
            Assert.IsNotNull(project.EnvironmentList);
            Assert.AreEqual(environmentList.Count, dbProject.EnvironmentList.Count);
            foreach(var environment in environmentList)
            {
                var dbEnvironment = dbProject.EnvironmentList.SingleOrDefault(i=>i.Id == environment.Id);
                AssertEnvironment(environment, dbEnvironment);
            }
        }

        [Test]
        public void GetProjectList_GetsEnvironmentList()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);
            var environmentList = new List<DeployEnvironment>();
            for (int i = 0; i < 5; i++)
            {
                var environment = this.CreateTestEnvironment(sut, project.Id);
                environmentList.Add(environment);
            }

            var dbProjectList = sut.GetProjectList();
            var dbProject = dbProjectList.SingleOrDefault(i=>i.Id == project.Id);
            Assert.IsNotNull(dbProject);
            Assert.IsNotNull(project.EnvironmentList);
            Assert.AreEqual(environmentList.Count, dbProject.EnvironmentList.Count);
            foreach (var environment in environmentList)
            {
                var dbEnvironment = dbProject.EnvironmentList.SingleOrDefault(i => i.Id == environment.Id);
                AssertEnvironment(environment, dbEnvironment);
            }
        }

    }
}
