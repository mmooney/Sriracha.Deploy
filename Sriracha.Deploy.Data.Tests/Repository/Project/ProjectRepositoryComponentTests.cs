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
    public abstract class ProjectRepositoryComponentTests : ProjectRepositoryTestBase
	{
        private void AssertCreatedComponent(DeployComponent result, string projectId, string componentName, EnumDeploymentIsolationType isolationType, IProjectRepository sut)
        {
            Assert.IsNotNull(result);
            Assert.IsNotNullOrEmpty(result.Id);
            Assert.AreEqual(projectId, result.ProjectId);
            Assert.AreEqual(componentName, result.ComponentName);
            Assert.AreEqual(false, result.UseConfigurationGroup);
            Assert.AreEqual(null, result.ConfigurationId);
            Assert.AreEqual(isolationType, result.IsolationType);
            AssertIsRecent(result.CreatedDateTimeUtc);
            Assert.AreEqual(this.UserName, result.CreatedByUserName);
            AssertIsRecent(result.UpdatedDateTimeUtc);
            Assert.AreEqual(this.UserName, result.UpdatedByUserName);

            var dbItem = sut.GetComponent(result.Id, projectId);
            AssertHelpers.AssertComponent(result, dbItem);

            var dbProject = sut.GetProject(projectId);
            var dbProjectComponent = dbProject.ComponentList.SingleOrDefault(i => i.Id == result.Id);
            Assert.IsNotNull(dbProjectComponent);
            AssertHelpers.AssertComponent(result, dbProjectComponent);
        }

        private DeployComponent CreateTestComponent(IProjectRepository sut, string projectId)
        {
            return sut.CreateComponent(projectId, this.Fixture.Create<string>("ComponentName"), false, null, this.Fixture.Create<EnumDeploymentIsolationType>());
        }

        [Test]
        public void CreateComponent_StoresComponent()
        {
            var sut = this.GetRepository();
            
            var project = this.CreateTestProject(sut);
            string componentName = this.Fixture.Create<string>("ComponentName");
            var isolationType = this.Fixture.Create<EnumDeploymentIsolationType>();

            var result = sut.CreateComponent(project.Id, componentName, false, null, isolationType);

            AssertCreatedComponent(result, project.Id, componentName, isolationType, sut);
        }

        [Test]
        public void GetComponentList_CanRetrieveComponentList()
        {
            var sut = this.GetRepository();

            var project = CreateTestProject(sut);
            var componentList = new List<DeployComponent>();
            for (int i = 0; i < 5; i++)
            {
                var component = CreateTestComponent(sut, project.Id);
                componentList.Add(component);
            }

            var result = sut.GetComponentList(project.Id);

            Assert.GreaterOrEqual(result.Count, componentList.Count);
            foreach (var component in componentList)
            {
                var item = result.SingleOrDefault(i => i.Id == component.Id);
                Assert.IsNotNull(item);
                AssertHelpers.AssertComponent(component, item);
            }
        }

        [Test]
        public void GetComponentList_MissingProjectID_ThrowsNullException()
        {
            var sut = this.GetRepository();

            Assert.Throws<ArgumentNullException>(() => sut.GetComponentList(null));
        }

        [Test]
        public void GetComponentList_BadProjectID_ThrowsRecordNotFoundException()
        {
            var sut = this.GetRepository();

            Assert.Throws<RecordNotFoundException>(() => sut.GetComponentList(Guid.NewGuid().ToString()));
        }

        [Test]
        public void GetComponent_WithProjectID_ReturnsComponent()
        {
            var sut = this.GetRepository();

            var project = CreateTestProject(sut);
            var component = CreateTestComponent(sut, project.Id);

            var result = sut.GetComponent(component.Id, project.Id);

            Assert.IsNotNull(result);
            AssertHelpers.AssertComponent(component, result);
        }

        [Test]
        public void GetComponent_WithoutProjectID_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            var project = CreateTestProject(sut);
            var component = CreateTestComponent(sut, project.Id);

            Assert.Throws<ArgumentNullException>(()=>sut.GetComponent(component.Id, null));
        }

        [Test]
        public void GetComponent_WithBadProjectID_ThrowsRecordNotFoundException()
        {
            var sut = this.GetRepository();

            var project = CreateTestProject(sut);
            var component = CreateTestComponent(sut, project.Id);

            Assert.Throws<RecordNotFoundException>(() => sut.GetComponent(component.Id, Guid.NewGuid().ToString()));
        }

        [Test]
        public void GetComponent_MissingComponentID_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            var project = CreateTestProject(sut);

            Assert.Throws<ArgumentNullException>(() => sut.GetComponent(null, project.Id));
        }

        [Test]
        public void GetComponent_BadComponentID_ThrowsRecordNotFoundException()
        {
            var sut = this.GetRepository();

            var project = CreateTestProject(sut);

            Assert.Throws<RecordNotFoundException>(() => sut.GetComponent(Guid.NewGuid().ToString(), project.Id));
        }

        [Test]
        public void UpdateComponent_UpdatesComponent()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);
            var component = this.CreateTestComponent(sut, project.Id);

            string newComponentName = this.Fixture.Create<string>("ComponentName");
            var newIsolationType = this.Fixture.Create<EnumDeploymentIsolationType>();
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);
            bool newUseConfigurationGroup = false;
            string newConfigurationId = null;

            var result = sut.UpdateComponent(component.Id, project.Id, newComponentName, newUseConfigurationGroup, newConfigurationId, newIsolationType);

            Assert.IsNotNull(result);
            Assert.AreEqual(component.Id, result.Id);
            Assert.AreEqual(component.ProjectId, result.ProjectId);
            Assert.AreEqual(newComponentName, result.ComponentName);
            Assert.AreEqual(newIsolationType, result.IsolationType);
            Assert.AreEqual(newUseConfigurationGroup, result.UseConfigurationGroup);
            Assert.AreEqual(newConfigurationId, result.ConfigurationId);
            Assert.AreEqual(component.CreatedByUserName, component.CreatedByUserName);
            AssertDateEqual(component.CreatedDateTimeUtc, result.CreatedDateTimeUtc);
            Assert.AreEqual(newUserName, result.UpdatedByUserName);
            AssertIsRecent(result.UpdatedDateTimeUtc);

            var dbItem = sut.GetComponent(component.Id, project.Id);
            AssertHelpers.AssertComponent(result, dbItem);
        }

        [Test]
        public void UpdateComponent__MissingProjectID_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);
            var component = this.CreateTestComponent(sut, project.Id);

            string newComponentName = this.Fixture.Create<string>("ComponentName");
            var newIsolationType = this.Fixture.Create<EnumDeploymentIsolationType>();
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            Assert.Throws<ArgumentNullException>(() => sut.UpdateComponent(component.Id, null, newComponentName, false, null, newIsolationType));
        }

        [Test]
        public void UpdateComponent_MissingComponentId_ThrowsNullException()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);
            var component = this.CreateTestComponent(sut, project.Id);

            string newComponentName = this.Fixture.Create<string>("ComponentName");
            var newIsolationType = this.Fixture.Create<EnumDeploymentIsolationType>();
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            Assert.Throws<ArgumentNullException>(() => sut.UpdateComponent(null, project.Id, newComponentName, false, null, newIsolationType));
        }

        [Test]
        public void UpdateConfiguration_BadComponentId_ThrowsRecordNotFoundException()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);
            var configuration = this.CreateTestComponent(sut, project.Id);

            string newComponentName = this.Fixture.Create<string>("ComponentName");
            var newIsolationType = this.Fixture.Create<EnumDeploymentIsolationType>();
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            Assert.Throws<RecordNotFoundException>(() => sut.UpdateComponent(Guid.NewGuid().ToString(), project.Id, newComponentName, false, null, newIsolationType));
        }

        [Test]
        public void DeleteComponent_DeletesComponent()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);
            var component = this.CreateTestComponent(sut, project.Id);

            sut.DeleteComponent(project.Id, component.Id);

            var dbConfiguration = sut.TryGetConfiguration(component.Id, project.Id);
            Assert.IsNull(dbConfiguration);

            var dbProject = sut.GetProject(project.Id);
            var dbProjectComponent = dbProject.ComponentList.SingleOrDefault(i => i.Id == component.Id);
            Assert.IsNull(dbProjectComponent);
        }

        [Test]
        public void DeleteComponent_MissingConfigurationID_ThrowsNullException()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);
            Assert.Throws<ArgumentNullException>(() => sut.DeleteComponent(project.Id, null));
        }

        [Test]
        public void DeleteComponent_BadComponentID_ThrowsRecordNotFoundException()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);
            Assert.Throws<RecordNotFoundException>(() => sut.DeleteComponent(project.Id, Guid.NewGuid().ToString()));
        }

        [Test]
        public void DeleteProject_DeletesConfiguration()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);
            var component = this.CreateTestComponent(sut, project.Id);

            sut.DeleteProject(project.Id);

            var dbComponent = sut.TryGetComponent(component.Id, project.Id);
            Assert.IsNull(dbComponent);
        }

        [Test]
        public void GetOrCreateComponent_CreatesNewBranch()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);
            string componentName = this.Fixture.Create<string>("ComponentName");

            var result = sut.GetOrCreateComponent(project.Id, componentName);

            AssertCreatedComponent(result, project.Id, componentName, EnumDeploymentIsolationType.IsolatedPerMachine, sut);
        }

        [Test]
        public void GetOrCreateComponent_GetsBranchByID()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);
            var component = this.CreateTestComponent(sut, project.Id);

            var result = sut.GetOrCreateComponent(project.Id, component.Id);

            AssertHelpers.AssertComponent(component, result);
        }

        [Test]
        public void GetOrCreateComponent_GetsBranchByName()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);
            var component = this.CreateTestComponent(sut, project.Id);

            var result = sut.GetOrCreateComponent(project.Id, component.ComponentName);

            AssertHelpers.AssertComponent(component, result);
        }

        [Test]
        public void GetOrCreateComponent_BadProjectID_ThrowsRecordNotFoundException()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);
            var component = this.CreateTestComponent(sut, project.Id);

            Assert.Throws<RecordNotFoundException>(() => sut.GetOrCreateComponent(Guid.NewGuid().ToString(), component.Id));
        }

        [Test]
        public void GetOrCreateComponent_MissingProjectID_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);
            var component = this.CreateTestComponent(sut, project.Id);

            Assert.Throws<ArgumentNullException>(() => sut.GetOrCreateComponent(null, component.Id));
        }

        [Test]
        public void GetOrCreateComponent_MissingBranchIdOrName_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);
            var component = this.CreateTestComponent(sut, project.Id);

            Assert.Throws<ArgumentNullException>(() => sut.GetOrCreateComponent(project.Id, null));
        }

    }
}
