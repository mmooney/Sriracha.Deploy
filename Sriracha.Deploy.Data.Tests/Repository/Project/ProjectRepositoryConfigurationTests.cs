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
    public abstract class ProjectRepositoryConfigurationTests : ProjectRepositoryTestBase
	{
        private void AssertConfiguration(DeployConfiguration expected, DeployConfiguration actual)
        {
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.ProjectId, actual.ProjectId);
            Assert.AreEqual(expected.ConfigurationName, actual.ConfigurationName);
            Assert.AreEqual(expected.IsolationType, actual.IsolationType);
            AssertDateEqual(expected.CreatedDateTimeUtc, actual.CreatedDateTimeUtc);
            Assert.AreEqual(expected.CreatedByUserName, actual.CreatedByUserName);
            AssertDateEqual(expected.UpdatedDateTimeUtc, actual.UpdatedDateTimeUtc);
            Assert.AreEqual(expected.UpdatedByUserName, actual.UpdatedByUserName);
        }

        private DeployConfiguration CreateTestConfiguration(IProjectRepository sut, string projectId)
        {
            return sut.CreateConfiguration(projectId, this.Fixture.Create<string>("ConfigurationName"), this.Fixture.Create<EnumDeploymentIsolationType>());
        }

        [Test]
        public void CreateConfiguration_StoresConfiguration()
        {
            var sut = this.GetRepository(); 

            var project = this.CreateTestProject(sut);
            string configurationName = this.Fixture.Create<string>("ConfigurationName");
            var isolationType = this.Fixture.Create<EnumDeploymentIsolationType>();

            var result = sut.CreateConfiguration(project.Id, configurationName, isolationType);

            Assert.IsNotNull(result);
            Assert.IsNotNullOrEmpty(result.Id);
            Assert.AreEqual(project.Id, result.ProjectId);
            Assert.AreEqual(configurationName, result.ConfigurationName);
            Assert.AreEqual(isolationType, result.IsolationType);
            AssertIsRecent(result.CreatedDateTimeUtc);
            Assert.AreEqual(this.UserName, result.CreatedByUserName);
            AssertIsRecent(result.UpdatedDateTimeUtc);
            Assert.AreEqual(this.UserName, result.UpdatedByUserName);

            var dbItem = sut.GetConfiguration(result.Id, result.ProjectId);
            AssertConfiguration(result, dbItem);

            var dbProject = sut.GetProject(project.Id);
            var dbProjectConfiguration = dbProject.ConfigurationList.SingleOrDefault(i=>i.Id == result.Id);
            Assert.IsNotNull(dbProjectConfiguration);
            AssertConfiguration(result, dbProjectConfiguration);
        }

        [Test]
        public void GetConfigurationList_CanRetrieveConfigurationList()
        {
            var sut = this.GetRepository();

            var project = CreateTestProject(sut);
            var configurationList = new List<DeployConfiguration>();
            for (int i = 0; i < 5; i++)
            {
                var configuration = CreateTestConfiguration(sut, project.Id);
                configurationList.Add(configuration);
            }

            var result = sut.GetConfigurationList(project.Id);

            Assert.GreaterOrEqual(result.Count, configurationList.Count);
            foreach (var configuration in configurationList)
            {
                var item = result.SingleOrDefault(i => i.Id == configuration.Id);
                Assert.IsNotNull(item);
                AssertConfiguration(configuration, item);
            }
        }

        [Test]
        public void GetConfigurationList_MissingProjectID_ThrowsNullException()
        {
            var sut = this.GetRepository();

            Assert.Throws<ArgumentNullException>(()=>sut.GetConfigurationList(null));
        }

        [Test]
        public void GetConfigurationList_BadProjectID_ThrowsRecordNotFoundException()
        {
            var sut = this.GetRepository();

            Assert.Throws<RecordNotFoundException>(() => sut.GetConfigurationList(Guid.NewGuid().ToString()));
        }

        [Test]
        public void GetConfiguration_WithProjectID_ReturnsConfiguration()
        {
            var sut = this.GetRepository();

            var project = CreateTestProject(sut);
            var configuration = CreateTestConfiguration(sut, project.Id);

            var result = sut.GetConfiguration(configuration.Id, project.Id);

            Assert.IsNotNull(result);
            AssertConfiguration(configuration, result);
        }

        [Test]
        public void GetConfiguration_WithoutProjectID_ReturnsConfiguration()
        {
            var sut = this.GetRepository();

            var project = CreateTestProject(sut);
            var configuration = CreateTestConfiguration(sut, project.Id);

            var result = sut.GetConfiguration(configuration.Id, project.Id);

            Assert.IsNotNull(result);
            AssertConfiguration(configuration, result);
        }

        [Test]
        public void GetConfiguration_WithBadProjectID_ThrowsRecordNotFoundException()
        {
            var sut = this.GetRepository();

            var project = CreateTestProject(sut);
            var configuration = CreateTestConfiguration(sut, project.Id);

            Assert.Throws<RecordNotFoundException>(()=>sut.GetConfiguration(configuration.Id, Guid.NewGuid().ToString()));
        }

        [Test]
        public void GetConfiguration_MissingConfigurationID_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);

            Assert.Throws<ArgumentNullException>(() => sut.GetConfiguration(null,project.Id));
        }

        [Test]
        public void GetConfiguration_BadConfigurationID_ThrowsRecordNotFoundException()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);

            Assert.Throws<RecordNotFoundException>(() => sut.GetConfiguration(Guid.NewGuid().ToString(), project.Id));
        }

        [Test]
        public void UpdateConfiguration_UpdatesConfiguration()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);
            var configuration = this.CreateTestConfiguration(sut, project.Id);

            string newConfigurationName = this.Fixture.Create<string>("ConfigurationName");
            var newIsolationType = this.Fixture.Create<EnumDeploymentIsolationType>();
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            var result = sut.UpdateConfiguration(configuration.Id, project.Id, newConfigurationName, newIsolationType);

            Assert.IsNotNull(result);
            Assert.AreEqual(configuration.Id, result.Id);
            Assert.AreEqual(configuration.ProjectId, result.ProjectId);
            Assert.AreEqual(newConfigurationName, result.ConfigurationName);
            Assert.AreEqual(newIsolationType, result.IsolationType);
            Assert.AreEqual(configuration.CreatedByUserName, configuration.CreatedByUserName);
            AssertDateEqual(configuration.CreatedDateTimeUtc, result.CreatedDateTimeUtc);
            Assert.AreEqual(newUserName, result.UpdatedByUserName);
            AssertIsRecent(result.UpdatedDateTimeUtc);

            var dbItem = sut.GetConfiguration(configuration.Id, project.Id);
            AssertConfiguration(result, dbItem);
        }

        [Test]
        public void UpdateConfiguration__MissingProjectID_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);
            var configuration = this.CreateTestConfiguration(sut, project.Id);

            string newConfigurationName = this.Fixture.Create<string>("ConfigurationName");
            var newIsolationType = this.Fixture.Create<EnumDeploymentIsolationType>();
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            Assert.Throws<ArgumentNullException>(()=>sut.UpdateConfiguration(configuration.Id, null, newConfigurationName, newIsolationType));
        }

        [Test]
        public void UpdateConfiguration_MissingConfigurationId_ThrowsNullException()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);
            var configuration = this.CreateTestConfiguration(sut, project.Id);

            string newConfigurationName = this.Fixture.Create<string>("ConfigurationName");
            var newIsolationType = this.Fixture.Create<EnumDeploymentIsolationType>();
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            Assert.Throws<ArgumentNullException>(()=>sut.UpdateConfiguration(null, project.Id, newConfigurationName, newIsolationType));
        }

        [Test]
        public void UpdateConfiguration_BadConfigurationId_ThrowsRecordNotFoundException()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);
            var configuration = this.CreateTestConfiguration(sut, project.Id);

            string newConfigurationName = this.Fixture.Create<string>("ConfigurationName");
            var newIsolationType = this.Fixture.Create<EnumDeploymentIsolationType>();
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            Assert.Throws<RecordNotFoundException>(() => sut.UpdateConfiguration(Guid.NewGuid().ToString(), project.Id, newConfigurationName, newIsolationType));
        }

        [Test]
        public void DeleteConfiguration_DeletesConfiguration()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);
            var configuration = this.CreateTestConfiguration(sut, project.Id);

            sut.DeleteConfiguration(configuration.Id, project.Id);

            var dbConfiguration = sut.TryGetConfiguration(configuration.Id, project.Id);
            Assert.IsNull(dbConfiguration);

            var dbProject = sut.GetProject(project.Id);
            var dbProjectConfiguration = dbProject.ConfigurationList.SingleOrDefault(i => i.Id == configuration.Id);
            Assert.IsNull(dbProjectConfiguration);
        }

        [Test]
        public void DeleteConfiguration_MissingConfigurationID_ThrowsNullException()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);

            Assert.Throws<ArgumentNullException>(() => sut.DeleteConfiguration(null, project.Id));
        }

        [Test]
        public void DeleteConfiguration_BadConfigurationID_ThrowsRecordNotFoundException()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);

            Assert.Throws<RecordNotFoundException>(() => sut.DeleteConfiguration(Guid.NewGuid().ToString(), project.Id));
        }

        [Test]
        public void DeleteProject_DeletesConfiguration()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);
            var configuration = this.CreateTestConfiguration(sut, project.Id);

            sut.DeleteProject(project.Id);

            var dbConfiguration = sut.TryGetConfiguration(configuration.Id, project.Id);
            Assert.IsNull(dbConfiguration);
        }
    }
}
