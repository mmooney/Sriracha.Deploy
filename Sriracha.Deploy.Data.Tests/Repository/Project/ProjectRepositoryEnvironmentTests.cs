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

            var dbItem = sut.GetEnvironment(result.Id);
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
            
            Assert.AreEqual(expected.ComponentList.Count(), actual.ComponentList.Count);
            foreach(var expectedItem in expected.ComponentList)
            {
                var actualItem = actual.ComponentList.SingleOrDefault(i=>i.ParentId == expectedItem.ParentId);
                AssertEnvironmentConfiguration(expectedItem, actualItem);
            }
            Assert.AreEqual(expected.ConfigurationList.Count(), actual.ConfigurationList.Count);
            foreach(var expectedItem in expected.ConfigurationList)
            {
                var actualItem = actual.ConfigurationList.SingleOrDefault(i=>i.ParentId == expectedItem.ParentId);
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
                var item = sut.CreateConfiguration(projectId, this.Fixture.Create<string>("ConfigurationName"), this.Fixture.Create<EnumDeploymentIsolationType>());
                returnList.Add(item);
            }
            return returnList;
        }

        private List<DeployComponent> CreateTestComponentList(string projectId, int count, IProjectRepository sut)
        {
            var returnList = new List<DeployComponent>();
            for (int i = 0; i < count; i++)
            {
                var item = sut.CreateComponent(projectId, this.Fixture.Create<string>("ComponentName"), false, null, this.Fixture.Create<EnumDeploymentIsolationType>());
                returnList.Add(item);
            }
            return returnList;
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
                                                    select new DeployEnvironmentConfiguration
                                                    {
                                                        ConfigurationValueList = this.CreateTestConfigurationValueList(),
                                                        DeployCredentialsId = null,
                                                        ParentId = i.Id,
                                                        MachineList = (from machineName in this.Fixture.CreateMany<string>("MachineName")
                                                                       select new DeployMachine
                                                                       {
                                                                           ConfigurationValueList = this.CreateTestConfigurationValueList(),
                                                                           MachineName = machineName
                                                                       }).ToList()
                                                    }).ToList();

            returnValue.EnvironmentConfigurationList = (from i in configurationList
                                                        select new DeployEnvironmentConfiguration
                                                        {
                                                            ConfigurationValueList = this.CreateTestConfigurationValueList(),
                                                            DeployCredentialsId = null,
                                                            ParentId = i.Id,
                                                            MachineList = (from machineName in this.Fixture.CreateMany<string>("MachineName")
                                                                           select new DeployMachine
                                                                           {
                                                                               ConfigurationValueList = this.CreateTestConfigurationValueList(),
                                                                               MachineName = machineName
                                                                           }).ToList()
                                                        }).ToList();

            returnValue.EnvironmentName = this.Fixture.Create<string>("EnvironmentName");

            return returnValue;
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
                sut.DeleteConfiguration(item.Id);
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
         
            var result = sut.GetEnvironment(environment.Id);

            Assert.IsNotNull(result);
            AssertEnvironment(environment, result);
        }

        [Test]
        public void GetEnvironment_MissingEnvironmentID_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            Assert.Throws<ArgumentNullException>(() => sut.GetEnvironment(null));
        }

        [Test]
        public void GetEnvironment_BadEnvironmentID_ThrowsRecordNotFoundException()
        {
            var sut = this.GetRepository();

            Assert.Throws<RecordNotFoundException>(() => sut.GetEnvironment(Guid.NewGuid().ToString()));
        }

        //[Test]
        //public void UpdateEnvironment_UpdatesEnvironment()
        //{
        //    var sut = this.GetRepository();

        //    var project = this.CreateTestProject(sut);
        //    var Environment = this.CreateTestEnvironment(sut, project.Id);

        //    string newEnvironmentName = this.Fixture.Create<string>("EnvironmentName");
        //    var newIsolationType = this.Fixture.Create<EnumDeploymentIsolationType>();
        //    string newUserName = this.Fixture.Create<string>("UserName");
        //    this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);
        //    bool newUseConfigurationGroup = false;
        //    string newConfigurationId = null;

        //    var result = sut.UpdateEnvironment(Environment.Id, project.Id, newEnvironmentName, newUseConfigurationGroup, newConfigurationId, newIsolationType);

        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(Environment.Id, result.Id);
        //    Assert.AreEqual(Environment.ProjectId, result.ProjectId);
        //    Assert.AreEqual(newEnvironmentName, result.EnvironmentName);
        //    Assert.AreEqual(newIsolationType, result.IsolationType);
        //    Assert.AreEqual(newUseConfigurationGroup, result.UseConfigurationGroup);
        //    Assert.AreEqual(newConfigurationId, result.ConfigurationId);
        //    Assert.AreEqual(Environment.CreatedByUserName, Environment.CreatedByUserName);
        //    AssertDateEqual(Environment.CreatedDateTimeUtc, result.CreatedDateTimeUtc);
        //    Assert.AreEqual(newUserName, result.UpdatedByUserName);
        //    AssertIsRecent(result.UpdatedDateTimeUtc);

        //    var dbItem = sut.GetEnvironment(Environment.Id, project.Id);
        //    AssertEnvironment(result, dbItem);
        //}

        //[Test]
        //public void UpdateEnvironment__MissingProjectID_ThrowsArgumentNullException()
        //{
        //    var sut = this.GetRepository();

        //    var project = this.CreateTestProject(sut);
        //    var Environment = this.CreateTestEnvironment(sut, project.Id);

        //    string newEnvironmentName = this.Fixture.Create<string>("EnvironmentName");
        //    var newIsolationType = this.Fixture.Create<EnumDeploymentIsolationType>();
        //    string newUserName = this.Fixture.Create<string>("UserName");
        //    this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

        //    Assert.Throws<ArgumentNullException>(() => sut.UpdateEnvironment(Environment.Id, null, newEnvironmentName, false, null, newIsolationType));
        //}

        //[Test]
        //public void UpdateEnvironment_MissingEnvironmentId_ThrowsNullException()
        //{
        //    var sut = this.GetRepository();

        //    var project = this.CreateTestProject(sut);
        //    var Environment = this.CreateTestEnvironment(sut, project.Id);

        //    string newEnvironmentName = this.Fixture.Create<string>("EnvironmentName");
        //    var newIsolationType = this.Fixture.Create<EnumDeploymentIsolationType>();
        //    string newUserName = this.Fixture.Create<string>("UserName");
        //    this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

        //    Assert.Throws<ArgumentNullException>(() => sut.UpdateEnvironment(null, project.Id, newEnvironmentName, false, null, newIsolationType));
        //}

        //[Test]
        //public void UpdateConfiguration_BadEnvironmentId_ThrowsRecordNotFoundException()
        //{
        //    var sut = this.GetRepository();

        //    var project = this.CreateTestProject(sut);
        //    var configuration = this.CreateTestEnvironment(sut, project.Id);

        //    string newEnvironmentName = this.Fixture.Create<string>("EnvironmentName");
        //    var newIsolationType = this.Fixture.Create<EnumDeploymentIsolationType>();
        //    string newUserName = this.Fixture.Create<string>("UserName");
        //    this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

        //    Assert.Throws<RecordNotFoundException>(() => sut.UpdateEnvironment(Guid.NewGuid().ToString(), project.Id, newEnvironmentName, false, null, newIsolationType));
        //}

        //[Test]
        //public void DeleteEnvironment_DeletesEnvironment()
        //{
        //    var sut = this.GetRepository();

        //    var project = this.CreateTestProject(sut);
        //    var Environment = this.CreateTestEnvironment(sut, project.Id);

        //    sut.DeleteEnvironment(project.Id, Environment.Id);

        //    var dbConfiguration = sut.TryGetConfiguration(Environment.Id);
        //    Assert.IsNull(dbConfiguration);

        //    var dbProject = sut.GetProject(project.Id);
        //    var dbProjectEnvironment = dbProject.EnvironmentList.SingleOrDefault(i => i.Id == Environment.Id);
        //    Assert.IsNull(dbProjectEnvironment);
        //}

        //[Test]
        //public void DeleteEnvironment_MissingConfigurationID_ThrowsNullException()
        //{
        //    var sut = this.GetRepository();

        //    var project = this.CreateTestProject(sut);
        //    Assert.Throws<ArgumentNullException>(() => sut.DeleteEnvironment(project.Id, null));
        //}

        //[Test]
        //public void DeleteEnvironment_BadEnvironmentID_ThrowsRecordNotFoundException()
        //{
        //    var sut = this.GetRepository();

        //    var project = this.CreateTestProject(sut);
        //    Assert.Throws<RecordNotFoundException>(() => sut.DeleteEnvironment(project.Id, Guid.NewGuid().ToString()));
        //}

        //[Test]
        //public void DeleteProject_DeletesConfiguration()
        //{
        //    var sut = this.GetRepository();

        //    var project = this.CreateTestProject(sut);
        //    var Environment = this.CreateTestEnvironment(sut, project.Id);

        //    sut.DeleteProject(project.Id);

        //    var dbEnvironment = sut.TryGetEnvironment(Environment.Id);
        //    Assert.IsNull(dbEnvironment);
        //}

        //[Test]
        //public void GetOrCreateEnvironment_CreatesNewBranch()
        //{
        //    var sut = this.GetRepository();

        //    var project = this.CreateTestProject(sut);
        //    string EnvironmentName = this.Fixture.Create<string>("EnvironmentName");

        //    var result = sut.GetOrCreateEnvironment(project.Id, EnvironmentName);

        //    AssertCreatedEnvironment(result, project.Id, EnvironmentName, EnumDeploymentIsolationType.IsolatedPerMachine, sut);
        //}

        //[Test]
        //public void GetOrCreateEnvironment_GetsBranchByID()
        //{
        //    var sut = this.GetRepository();

        //    var project = this.CreateTestProject(sut);
        //    var Environment = this.CreateTestEnvironment(sut, project.Id);

        //    var result = sut.GetOrCreateEnvironment(project.Id, Environment.Id);

        //    AssertEnvironment(Environment, result);
        //}

        //[Test]
        //public void GetOrCreateEnvironment_GetsBranchByName()
        //{
        //    var sut = this.GetRepository();

        //    var project = this.CreateTestProject(sut);
        //    var Environment = this.CreateTestEnvironment(sut, project.Id);

        //    var result = sut.GetOrCreateEnvironment(project.Id, Environment.EnvironmentName);

        //    AssertEnvironment(Environment, result);
        //}

        //[Test]
        //public void GetOrCreateEnvironment_BadProjectID_ThrowsRecordNotFoundException()
        //{
        //    var sut = this.GetRepository();

        //    var project = this.CreateTestProject(sut);
        //    var Environment = this.CreateTestEnvironment(sut, project.Id);

        //    Assert.Throws<RecordNotFoundException>(() => sut.GetOrCreateEnvironment(Guid.NewGuid().ToString(), Environment.Id));
        //}

        //[Test]
        //public void GetOrCreateEnvironment_MissingProjectID_ThrowsArgumentNullException()
        //{
        //    var sut = this.GetRepository();

        //    var project = this.CreateTestProject(sut);
        //    var Environment = this.CreateTestEnvironment(sut, project.Id);

        //    Assert.Throws<ArgumentNullException>(() => sut.GetOrCreateEnvironment(null, Environment.Id));
        //}

        //[Test]
        //public void GetOrCreateEnvironment_MissingBranchIdOrName_ThrowsArgumentNullException()
        //{
        //    var sut = this.GetRepository();

        //    var project = this.CreateTestProject(sut);
        //    var Environment = this.CreateTestEnvironment(sut, project.Id);

        //    Assert.Throws<ArgumentNullException>(() => sut.GetOrCreateEnvironment(project.Id, null));
        //}

    }
}
