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
            
            Assert.AreEqual(environmentComponentList.Count(), result.ComponentList.Count);
            foreach(var item in environmentComponentList)
            {
                var createdItem = result.ComponentList.SingleOrDefault(i=>i.ParentId == item.ParentId);
                AssertCreatedEnvironmentConfiguration(item, createdItem, project, result, EnumDeployStepParentType.Component);
            }
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

        //private DeployComponent CreateTestComponent(IProjectRepository sut, string projectId)
        //{
        //    return sut.CreateComponent(projectId, this.Fixture.Create<string>("ComponentName"), false, null, this.Fixture.Create<EnumDeploymentIsolationType>());
        //}

        [Test]
        public void CreateEnvironment_StoresEnvironment()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);
            var componentList = this.CreateTestComponentList(project.Id, 5, sut);
            var configurationList = this.CreateTestConfigurationList(project.Id, 3, sut);

            var environmentComponentList = (from i in componentList
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

            var environmentConfigurationList = (from i in configurationList
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

            string environmentName = this.Fixture.Create<string>("EnvironmentName");
            var result = sut.CreateEnvironment(project.Id, environmentName, environmentComponentList, environmentConfigurationList);

            AssertCreatedEnvironment(result, project, environmentName, environmentComponentList, environmentConfigurationList, sut);
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
            for(int i = 0; i < count; i++)
            {
                var item = sut.CreateComponent(projectId, this.Fixture.Create<string>("ComponentName"), false, null, this.Fixture.Create<EnumDeploymentIsolationType>());
                returnList.Add(item);
            }
            return returnList;
        }

        //[Test]
        //public void GetComponentList_CanRetrieveComponentList()
        //{
        //    var sut = this.GetRepository();

        //    var project = CreateTestProject(sut);
        //    var componentList = new List<DeployComponent>();
        //    for (int i = 0; i < 5; i++)
        //    {
        //        var component = CreateTestComponent(sut, project.Id);
        //        componentList.Add(component);
        //    }

        //    var result = sut.GetComponentList(project.Id);

        //    Assert.GreaterOrEqual(result.Count, componentList.Count);
        //    foreach (var component in componentList)
        //    {
        //        var item = result.SingleOrDefault(i => i.Id == component.Id);
        //        Assert.IsNotNull(item);
        //        AssertComponent(component, item);
        //    }
        //}

        //[Test]
        //public void GetComponentList_MissingProjectID_ThrowsNullException()
        //{
        //    var sut = this.GetRepository();

        //    Assert.Throws<ArgumentNullException>(() => sut.GetComponentList(null));
        //}

        //[Test]
        //public void GetComponentList_BadProjectID_ThrowsRecordNotFoundException()
        //{
        //    var sut = this.GetRepository();

        //    Assert.Throws<RecordNotFoundException>(() => sut.GetComponentList(Guid.NewGuid().ToString()));
        //}

        //[Test]
        //public void GetComponent_WithProjectID_ReturnsComponent()
        //{
        //    var sut = this.GetRepository();

        //    var project = CreateTestProject(sut);
        //    var component = CreateTestComponent(sut, project.Id);

        //    var result = sut.GetComponent(component.Id, project.Id);

        //    Assert.IsNotNull(result);
        //    AssertComponent(component, result);
        //}

        //[Test]
        //public void GetComponent_WithoutProjectID_ReturnsComponent()
        //{
        //    var sut = this.GetRepository();

        //    var project = CreateTestProject(sut);
        //    var component = CreateTestComponent(sut, project.Id);

        //    var result = sut.GetComponent(component.Id);

        //    Assert.IsNotNull(result);
        //    AssertComponent(component, result);
        //}

        //[Test]
        //public void GetComponent_WithBadProjectID_ThrowsRecordNotFoundException()
        //{
        //    var sut = this.GetRepository();

        //    var project = CreateTestProject(sut);
        //    var component = CreateTestComponent(sut, project.Id);

        //    Assert.Throws<RecordNotFoundException>(() => sut.GetComponent(component.Id, Guid.NewGuid().ToString()));
        //}

        //[Test]
        //public void GetComponent_MissingComponentID_ThrowsArgumentNullException()
        //{
        //    var sut = this.GetRepository();

        //    Assert.Throws<ArgumentNullException>(() => sut.GetComponent(null));
        //}

        //[Test]
        //public void GetComponent_BadComponentID_ThrowsRecordNotFoundException()
        //{
        //    var sut = this.GetRepository();

        //    Assert.Throws<RecordNotFoundException>(() => sut.GetComponent(Guid.NewGuid().ToString()));
        //}

        //[Test]
        //public void UpdateComponent_UpdatesComponent()
        //{
        //    var sut = this.GetRepository();

        //    var project = this.CreateTestProject(sut);
        //    var component = this.CreateTestComponent(sut, project.Id);

        //    string newComponentName = this.Fixture.Create<string>("ComponentName");
        //    var newIsolationType = this.Fixture.Create<EnumDeploymentIsolationType>();
        //    string newUserName = this.Fixture.Create<string>("UserName");
        //    this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);
        //    bool newUseConfigurationGroup = false;
        //    string newConfigurationId = null;

        //    var result = sut.UpdateComponent(component.Id, project.Id, newComponentName, newUseConfigurationGroup, newConfigurationId, newIsolationType);

        //    Assert.IsNotNull(result);
        //    Assert.AreEqual(component.Id, result.Id);
        //    Assert.AreEqual(component.ProjectId, result.ProjectId);
        //    Assert.AreEqual(newComponentName, result.ComponentName);
        //    Assert.AreEqual(newIsolationType, result.IsolationType);
        //    Assert.AreEqual(newUseConfigurationGroup, result.UseConfigurationGroup);
        //    Assert.AreEqual(newConfigurationId, result.ConfigurationId);
        //    Assert.AreEqual(component.CreatedByUserName, component.CreatedByUserName);
        //    AssertDateEqual(component.CreatedDateTimeUtc, result.CreatedDateTimeUtc);
        //    Assert.AreEqual(newUserName, result.UpdatedByUserName);
        //    AssertIsRecent(result.UpdatedDateTimeUtc);

        //    var dbItem = sut.GetComponent(component.Id, project.Id);
        //    AssertComponent(result, dbItem);
        //}

        //[Test]
        //public void UpdateComponent__MissingProjectID_ThrowsArgumentNullException()
        //{
        //    var sut = this.GetRepository();

        //    var project = this.CreateTestProject(sut);
        //    var component = this.CreateTestComponent(sut, project.Id);

        //    string newComponentName = this.Fixture.Create<string>("ComponentName");
        //    var newIsolationType = this.Fixture.Create<EnumDeploymentIsolationType>();
        //    string newUserName = this.Fixture.Create<string>("UserName");
        //    this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

        //    Assert.Throws<ArgumentNullException>(() => sut.UpdateComponent(component.Id, null, newComponentName, false, null, newIsolationType));
        //}

        //[Test]
        //public void UpdateComponent_MissingComponentId_ThrowsNullException()
        //{
        //    var sut = this.GetRepository();

        //    var project = this.CreateTestProject(sut);
        //    var component = this.CreateTestComponent(sut, project.Id);

        //    string newComponentName = this.Fixture.Create<string>("ComponentName");
        //    var newIsolationType = this.Fixture.Create<EnumDeploymentIsolationType>();
        //    string newUserName = this.Fixture.Create<string>("UserName");
        //    this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

        //    Assert.Throws<ArgumentNullException>(() => sut.UpdateComponent(null, project.Id, newComponentName, false, null, newIsolationType));
        //}

        //[Test]
        //public void UpdateConfiguration_BadComponentId_ThrowsRecordNotFoundException()
        //{
        //    var sut = this.GetRepository();

        //    var project = this.CreateTestProject(sut);
        //    var configuration = this.CreateTestComponent(sut, project.Id);

        //    string newComponentName = this.Fixture.Create<string>("ComponentName");
        //    var newIsolationType = this.Fixture.Create<EnumDeploymentIsolationType>();
        //    string newUserName = this.Fixture.Create<string>("UserName");
        //    this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

        //    Assert.Throws<RecordNotFoundException>(() => sut.UpdateComponent(Guid.NewGuid().ToString(), project.Id, newComponentName, false, null, newIsolationType));
        //}

        //[Test]
        //public void DeleteComponent_DeletesComponent()
        //{
        //    var sut = this.GetRepository();

        //    var project = this.CreateTestProject(sut);
        //    var component = this.CreateTestComponent(sut, project.Id);

        //    sut.DeleteComponent(project.Id, component.Id);

        //    var dbConfiguration = sut.TryGetConfiguration(component.Id);
        //    Assert.IsNull(dbConfiguration);

        //    var dbProject = sut.GetProject(project.Id);
        //    var dbProjectComponent = dbProject.ComponentList.SingleOrDefault(i => i.Id == component.Id);
        //    Assert.IsNull(dbProjectComponent);
        //}

        //[Test]
        //public void DeleteComponent_MissingConfigurationID_ThrowsNullException()
        //{
        //    var sut = this.GetRepository();

        //    var project = this.CreateTestProject(sut);
        //    Assert.Throws<ArgumentNullException>(() => sut.DeleteComponent(project.Id, null));
        //}

        //[Test]
        //public void DeleteComponent_BadComponentID_ThrowsRecordNotFoundException()
        //{
        //    var sut = this.GetRepository();

        //    var project = this.CreateTestProject(sut);
        //    Assert.Throws<RecordNotFoundException>(() => sut.DeleteComponent(project.Id, Guid.NewGuid().ToString()));
        //}

        //[Test]
        //public void DeleteProject_DeletesConfiguration()
        //{
        //    var sut = this.GetRepository();

        //    var project = this.CreateTestProject(sut);
        //    var component = this.CreateTestComponent(sut, project.Id);

        //    sut.DeleteProject(project.Id);

        //    var dbComponent = sut.TryGetComponent(component.Id);
        //    Assert.IsNull(dbComponent);
        //}

        //[Test]
        //public void GetOrCreateComponent_CreatesNewBranch()
        //{
        //    var sut = this.GetRepository();

        //    var project = this.CreateTestProject(sut);
        //    string componentName = this.Fixture.Create<string>("ComponentName");

        //    var result = sut.GetOrCreateComponent(project.Id, componentName);

        //    AssertCreatedComponent(result, project.Id, componentName, EnumDeploymentIsolationType.IsolatedPerMachine, sut);
        //}

        //[Test]
        //public void GetOrCreateComponent_GetsBranchByID()
        //{
        //    var sut = this.GetRepository();

        //    var project = this.CreateTestProject(sut);
        //    var component = this.CreateTestComponent(sut, project.Id);

        //    var result = sut.GetOrCreateComponent(project.Id, component.Id);

        //    AssertComponent(component, result);
        //}

        //[Test]
        //public void GetOrCreateComponent_GetsBranchByName()
        //{
        //    var sut = this.GetRepository();

        //    var project = this.CreateTestProject(sut);
        //    var component = this.CreateTestComponent(sut, project.Id);

        //    var result = sut.GetOrCreateComponent(project.Id, component.ComponentName);

        //    AssertComponent(component, result);
        //}

        //[Test]
        //public void GetOrCreateComponent_BadProjectID_ThrowsRecordNotFoundException()
        //{
        //    var sut = this.GetRepository();

        //    var project = this.CreateTestProject(sut);
        //    var component = this.CreateTestComponent(sut, project.Id);

        //    Assert.Throws<RecordNotFoundException>(() => sut.GetOrCreateComponent(Guid.NewGuid().ToString(), component.Id));
        //}

        //[Test]
        //public void GetOrCreateComponent_MissingProjectID_ThrowsArgumentNullException()
        //{
        //    var sut = this.GetRepository();

        //    var project = this.CreateTestProject(sut);
        //    var component = this.CreateTestComponent(sut, project.Id);

        //    Assert.Throws<ArgumentNullException>(() => sut.GetOrCreateComponent(null, component.Id));
        //}

        //[Test]
        //public void GetOrCreateComponent_MissingBranchIdOrName_ThrowsArgumentNullException()
        //{
        //    var sut = this.GetRepository();

        //    var project = this.CreateTestProject(sut);
        //    var component = this.CreateTestComponent(sut, project.Id);

        //    Assert.Throws<ArgumentNullException>(() => sut.GetOrCreateComponent(project.Id, null));
        //}

    }
}
