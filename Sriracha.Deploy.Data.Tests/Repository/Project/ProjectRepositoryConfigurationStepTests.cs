using NUnit.Framework;
using Sriracha.Deploy.Data.Dto.Project;
using Sriracha.Deploy.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ploeh.AutoFixture;
using MMDB.Shared;

namespace Sriracha.Deploy.Data.Tests.Repository.Project
{
    [TestFixture]
    public abstract class ProjectRepositoryConfigurationStepTests : ProjectRepositoryTestBase
    {
        private DeployConfiguration CreateTestConfiguration(IProjectRepository sut, string projectId = null)
        {
            if (string.IsNullOrEmpty(projectId))
            {
                var project = this.CreateTestProject(sut);
                projectId = project.Id;
            }
            return sut.CreateConfiguration(projectId, this.Fixture.Create<string>("ConfigurationName"), this.Fixture.Create<EnumDeploymentIsolationType>());
        }

        private void AssertCreatedStep(DeployStep result, DeployConfiguration component, string stepName, string taskTypeName, string taskOptionsJson, IProjectRepository sut)
        {
            Assert.IsNotNull(result);
            Assert.IsNotNullOrEmpty(result.Id);
            Assert.AreEqual(component.ProjectId, result.ProjectId);
            Assert.AreEqual(component.Id, result.ParentId);
            Assert.AreEqual(EnumDeployStepParentType.Configuration, result.ParentType);
            Assert.AreEqual(stepName, result.StepName);
            Assert.AreEqual(taskTypeName, result.TaskTypeName);
            Assert.AreEqual(taskOptionsJson, result.TaskOptionsJson);
            Assert.IsNull(result.SharedDeploymentStepId);
            Assert.AreEqual(this.UserName, result.CreatedByUserName);
            AssertIsRecent(result.CreatedDateTimeUtc);
            Assert.AreEqual(this.UserName, result.UpdatedByUserName);
            AssertIsRecent(result.UpdatedDateTimeUtc);

            var dbItem = sut.GetConfigurationDeploymentStep(result.Id, result.ProjectId);
            AssertStep(result, dbItem);
        }

        private void AssertUpdatedStep(DeployStep result, DeployStep original, string newStepName, string newTaskTypeName, string newTaskOptionsJson, string newUserName, IProjectRepository sut)
        {
            Assert.IsNotNull(result);
            Assert.IsNotNullOrEmpty(result.Id);
            Assert.AreEqual(original.ProjectId, result.ProjectId);
            Assert.AreEqual(original.ParentId, result.ParentId);
            Assert.AreEqual(original.ParentType, result.ParentType);
            Assert.AreEqual(newStepName, result.StepName);
            Assert.AreEqual(newTaskTypeName, result.TaskTypeName);
            Assert.AreEqual(newTaskOptionsJson, result.TaskOptionsJson);
            Assert.AreEqual(original.CreatedByUserName, result.CreatedByUserName);
            AssertDateEqual(original.CreatedDateTimeUtc, result.CreatedDateTimeUtc);
            Assert.AreEqual(newUserName, result.UpdatedByUserName);
            AssertIsRecent(result.UpdatedDateTimeUtc);
             
            var dbItem = sut.GetConfigurationDeploymentStep(result.Id, result.ProjectId);
            AssertStep(result, dbItem);
        }

        private DeployStep CreateTestStep(IProjectRepository sut, DeployConfiguration configuration = null)
        {
            if (configuration == null)
            {
                configuration = this.CreateTestConfiguration(sut);
            }
            return sut.CreateConfigurationDeploymentStep(configuration.ProjectId, configuration.Id, this.Fixture.Create<string>("StepName"), this.Fixture.Create<string>("TaskTypeName"), this.Fixture.Create<string>("TaskOptionsJson"));
        }

        private void AssertStep(DeployStep expected, DeployStep actual)
        {
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.ProjectId, actual.ProjectId);
            Assert.AreEqual(expected.ParentType, actual.ParentType);
            Assert.AreEqual(expected.ParentId, actual.ParentId);
            Assert.AreEqual(expected.StepName, actual.StepName);
            Assert.AreEqual(expected.TaskTypeName, actual.TaskTypeName);
            Assert.AreEqual(expected.TaskOptionsJson, actual.TaskOptionsJson);
            Assert.AreEqual(expected.SharedDeploymentStepId, actual.SharedDeploymentStepId);
            AssertDateEqual(expected.CreatedDateTimeUtc, actual.CreatedDateTimeUtc);
            Assert.AreEqual(expected.CreatedByUserName, actual.CreatedByUserName);
            AssertDateEqual(expected.UpdatedDateTimeUtc, actual.UpdatedDateTimeUtc);
            Assert.AreEqual(expected.UpdatedByUserName, actual.UpdatedByUserName);
        }

        [Test]
        public void CreateConfigurationDeploymentStep_CreatesConfigurationDeploymentStep()
        {
            var sut = this.GetRepository();
            
            var configuration = this.CreateTestConfiguration(sut);

            string stepName = this.Fixture.Create<string>("StepName");
            string taskTypeName = this.Fixture.Create<string>("TaskTypeName");
            string taskOptionsJson = this.Fixture.Create<string>("TaskOptionsJson");
            var result = sut.CreateConfigurationDeploymentStep(configuration.ProjectId, configuration.Id, stepName, taskTypeName, taskOptionsJson);

            AssertCreatedStep(result, configuration, stepName, taskTypeName, taskOptionsJson, sut);
        }

        [Test]
        public void CreateConfigurationDeploymentStep_MissingProjectID_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            var configuration = this.CreateTestConfiguration(sut);

            string stepName = this.Fixture.Create<string>("StepName");
            string taskTypeName = this.Fixture.Create<string>("TaskTypeName");
            string taskOptionsJson = this.Fixture.Create<string>("TaskOptionsJson");
            Assert.Throws<ArgumentNullException>(() => sut.CreateConfigurationDeploymentStep(null, configuration.Id, stepName, taskTypeName, taskOptionsJson));
        }

        [Test]
        public void CreateConfigurationDeploymentStep_BadProjectID_ThrowsRecordNotFoundException()
        {
            var sut = this.GetRepository();

            var configuration = this.CreateTestConfiguration(sut);

            string stepName = this.Fixture.Create<string>("StepName");
            string taskTypeName = this.Fixture.Create<string>("TaskTypeName");
            string taskOptionsJson = this.Fixture.Create<string>("TaskOptionsJson");
            Assert.Throws<RecordNotFoundException>(() => sut.CreateConfigurationDeploymentStep(Guid.NewGuid().ToString(), configuration.Id, stepName, taskTypeName, taskOptionsJson));
        }

        [Test]
        public void CreateConfigurationDeploymentStep_MissingConfigurationID_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            var configuration = this.CreateTestConfiguration(sut);

            string stepName = this.Fixture.Create<string>("StepName");
            string taskTypeName = this.Fixture.Create<string>("TaskTypeName");
            string taskOptionsJson = this.Fixture.Create<string>("TaskOptionsJson");
            Assert.Throws<ArgumentNullException>(() => sut.CreateConfigurationDeploymentStep(configuration.ProjectId, null, stepName, taskTypeName, taskOptionsJson));
        }

        [Test]
        public void CreateConfigurationDeploymentStep_BadConfigurationID_ThrowsRecordNotFoundException()
        {
            var sut = this.GetRepository();

            var configuration = this.CreateTestConfiguration(sut);

            string stepName = this.Fixture.Create<string>("StepName");
            string taskTypeName = this.Fixture.Create<string>("TaskTypeName");
            string taskOptionsJson = this.Fixture.Create<string>("TaskOptionsJson");
            Assert.Throws<RecordNotFoundException>(() => sut.CreateConfigurationDeploymentStep(configuration.ProjectId, Guid.NewGuid().ToString(), stepName, taskTypeName, taskOptionsJson));
        }

        [Test]
        public void CreateConfigurationDeploymentStep_MissingStepName_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            var configuration = this.CreateTestConfiguration(sut);

            string stepName = null;
            string taskTypeName = this.Fixture.Create<string>("TaskTypeName");
            string taskOptionsJson = this.Fixture.Create<string>("TaskOptionsJson");
            Assert.Throws<ArgumentNullException>(() => sut.CreateConfigurationDeploymentStep(configuration.ProjectId, configuration.Id, stepName, taskTypeName, taskOptionsJson));
        }

        [Test]
        public void CreateConfigurationDeploymentStep_MissingTaskTypeName_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            var configuration = this.CreateTestConfiguration(sut);

            string stepName = this.Fixture.Create<string>("StepName");
            string taskTypeName = null;
            string taskOptionsJson = this.Fixture.Create<string>("TaskOptionsJson");
            Assert.Throws<ArgumentNullException>(() => sut.CreateConfigurationDeploymentStep(configuration.ProjectId, configuration.Id, stepName, taskTypeName, taskOptionsJson));
        }

        [Test]
        public void CreateConfigurationDeploymentStep_MissingTaskOptionsJson_CreatesStep()
        {
            var sut = this.GetRepository();

            var configuration = this.CreateTestConfiguration(sut);

            string stepName = this.Fixture.Create<string>("StepName");
            string taskTypeName = this.Fixture.Create<string>("TaskTypeName");
            string taskOptionsJson = null;
            var result = sut.CreateConfigurationDeploymentStep(configuration.ProjectId, configuration.Id, stepName, taskTypeName, taskOptionsJson);
            
            AssertCreatedStep(result, configuration, stepName, taskTypeName, taskOptionsJson, sut);
        }

        [Test]
        public void GetConfigurationDeploymentStepList_GetsStepList()
        {
            var sut = this.GetRepository();

            var configuration = this.CreateTestConfiguration(sut);
            var stepList = new List<DeployStep>();
            for (int i = 0; i < 5; i++)
            {
                var step = this.CreateTestStep(sut, configuration);
                stepList.Add(step);
            }
            var otherComponent = this.CreateTestConfiguration(sut);
            for (int i = 0; i < 5; i++)
            {
                this.CreateTestStep(sut, otherComponent);
            }

            var result = sut.GetConfigurationDeploymentStepList(configuration.Id, configuration.ProjectId);

            Assert.IsNotNull(result);
            Assert.AreEqual(stepList.Count, result.Count);
            foreach (var step in stepList)
            {
                var resultItem = result.SingleOrDefault(i => i.Id == step.Id);
                AssertStep(step, resultItem);
            }
        }

        [Test]
        public void GetConfigurationDeploymentStepList_MissingComponentID_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut); 
            
            Assert.Throws<ArgumentNullException>(() => sut.GetComponentDeploymentStepList(null, project.Id));
        }

        [Test]
        public void GetConfigurationDeploymentStepList_BadComponentID_ThrowsRecordNotFoundException()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);

            Assert.Throws<RecordNotFoundException>(() => sut.GetConfigurationDeploymentStepList(Guid.NewGuid().ToString(), project.Id));
        }

        [Test]
        public void GetConfigurationDeploymentStep_GetsStep()
        {
            var sut = this.GetRepository();

            var step = this.CreateTestStep(sut);

            var result = sut.GetConfigurationDeploymentStep(step.Id, step.ProjectId);

            AssertStep(step, result);
        }

        [Test]
        public void GetConfigurationDeploymentStep_MissingStepID_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            var step = this.CreateTestStep(sut);

            Assert.Throws<ArgumentNullException>(() => sut.GetConfigurationDeploymentStep(null, step.ProjectId));
        }

        [Test]
        public void GetConfigurationDeploymentStep_BadStepID_ThrowsArgumentRecordNotFoundException()
        {
            var sut = this.GetRepository();

            var step = this.CreateTestStep(sut);

            Assert.Throws<ArgumentNullException>(() => sut.GetConfigurationDeploymentStep(null, step.ProjectId));
        }

        [Test]
        public void UpdateConfigurationDeploymentStep_WithStepID_UpdatesStep()
        {
            var sut = this.GetRepository();

            var step = this.CreateTestStep(sut);

            string newStepName = this.Fixture.Create<string>("StepName");
            string newTaskTypeName = this.Fixture.Create<string>("TaskTypeName");
            string newTaskOptionsJson = this.Fixture.Create<string>("TaskOptionsJson");
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            var result = sut.UpdateConfigurationDeploymentStep(step.Id, step.ProjectId, step.ParentId, newStepName, newTaskTypeName, newTaskOptionsJson);

            AssertUpdatedStep(result, step, newStepName, newTaskTypeName, newTaskOptionsJson, newUserName, sut);
        }

        [Test]
        public void UpdateConfigurationDeploymentStep_MissingStepID_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            var step = this.CreateTestStep(sut);

            string newStepName = this.Fixture.Create<string>("StepName");
            string newTaskTypeName = this.Fixture.Create<string>("TaskTypeName");
            string newTaskOptionsJson = this.Fixture.Create<string>("TaskOptionsJson");
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            Assert.Throws<ArgumentNullException>(()=>sut.UpdateConfigurationDeploymentStep(null, step.ProjectId, step.ParentId, newStepName, newTaskTypeName, newTaskOptionsJson));
        }

        [Test]
        public void UpdateConfigurationDeploymentStep_BadStepID_ThrowsRecordNotFoundException()
        {
            var sut = this.GetRepository();

            var step = this.CreateTestStep(sut);

            string newStepName = this.Fixture.Create<string>("StepName");
            string newTaskTypeName = this.Fixture.Create<string>("TaskTypeName");
            string newTaskOptionsJson = this.Fixture.Create<string>("TaskOptionsJson");
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            Assert.Throws<RecordNotFoundException>(() => sut.UpdateConfigurationDeploymentStep(Guid.NewGuid().ToString(), step.ProjectId, step.ParentId, newStepName, newTaskTypeName, newTaskOptionsJson));
        }

        [Test]
        public void UpdateConfigurationDeploymentStep_BadProjectID_ThrowsRecordNotFoundException()
        {
            var sut = this.GetRepository();

            var step = this.CreateTestStep(sut);

            string newStepName = this.Fixture.Create<string>("StepName");
            string newTaskTypeName = this.Fixture.Create<string>("TaskTypeName");
            string newTaskOptionsJson = this.Fixture.Create<string>("TaskOptionsJson");
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            Assert.Throws<RecordNotFoundException>(() => sut.UpdateConfigurationDeploymentStep(step.Id, Guid.NewGuid().ToString(), step.ParentId, newStepName, newTaskTypeName, newTaskOptionsJson));
        }

        [Test]
        public void UpdateConfigurationDeploymentStep_MissingProjectID_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            var step = this.CreateTestStep(sut);

            string newStepName = this.Fixture.Create<string>("StepName");
            string newTaskTypeName = this.Fixture.Create<string>("TaskTypeName");
            string newTaskOptionsJson = this.Fixture.Create<string>("TaskOptionsJson");
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            Assert.Throws<ArgumentNullException>(() => sut.UpdateConfigurationDeploymentStep(step.Id, null, step.ParentId, newStepName, newTaskTypeName, newTaskOptionsJson));
        }

        [Test]
        public void UpdateConfigurationDeploymentStep_BadComponentID_ThrowsRecordNotFoundException()
        {
            var sut = this.GetRepository();

            var step = this.CreateTestStep(sut);

            string newStepName = this.Fixture.Create<string>("StepName");
            string newTaskTypeName = this.Fixture.Create<string>("TaskTypeName");
            string newTaskOptionsJson = this.Fixture.Create<string>("TaskOptionsJson");
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            Assert.Throws<RecordNotFoundException>(() => sut.UpdateConfigurationDeploymentStep(step.Id, step.ProjectId, Guid.NewGuid().ToString(), newStepName, newTaskTypeName, newTaskOptionsJson));
        }

        [Test]
        public void UpdateConfigurationDeploymentStep_MissingComponentID_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            var step = this.CreateTestStep(sut);

            string newStepName = this.Fixture.Create<string>("StepName");
            string newTaskTypeName = this.Fixture.Create<string>("TaskTypeName");
            string newTaskOptionsJson = this.Fixture.Create<string>("TaskOptionsJson");
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            Assert.Throws<ArgumentNullException>(() => sut.UpdateConfigurationDeploymentStep(step.Id, step.ProjectId, null, newStepName, newTaskTypeName, newTaskOptionsJson));
        }

        [Test]
        public void UpdateConfigurationDeploymentStep_MissingStepName_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            var step = this.CreateTestStep(sut);

            string newStepName = null;
            string newTaskTypeName = this.Fixture.Create<string>("TaskTypeName");
            string newTaskOptionsJson = this.Fixture.Create<string>("TaskOptionsJson");
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            Assert.Throws<ArgumentNullException>(() => sut.UpdateConfigurationDeploymentStep(step.Id, step.ProjectId, step.ParentId, newStepName, newTaskTypeName, newTaskOptionsJson));
        }

        [Test]
        public void UpdateConfigurationDeploymentStep_MissingTaskTypeName_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            var step = this.CreateTestStep(sut);

            string newStepName = this.Fixture.Create<string>("StepName");
            string newTaskTypeName = null;
            string newTaskOptionsJson = this.Fixture.Create<string>("TaskOptionsJson");
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            Assert.Throws<ArgumentNullException>(() => sut.UpdateConfigurationDeploymentStep(step.Id, step.ProjectId, step.ParentId, newStepName, newTaskTypeName, newTaskOptionsJson));
        }

        [Test]
        public void UpdateConfigurationDeploymentStep_MissingTaskOptionsJson_UpdatesStep()
        {
            var sut = this.GetRepository();

            var step = this.CreateTestStep(sut);

            string newStepName = this.Fixture.Create<string>("StepName");
            string newTaskTypeName = this.Fixture.Create<string>("TaskTypeName");
            string newTaskOptionsJson = null;
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            var result = sut.UpdateConfigurationDeploymentStep(step.Id, step.ProjectId, step.ParentId, newStepName, newTaskTypeName, newTaskOptionsJson);

            AssertUpdatedStep(result, step, newStepName, newTaskTypeName, newTaskOptionsJson, newUserName, sut);
        }

        [Test]
        public void DeleteConfigurationDeploymentStep_DeletesStep()
        {
            var sut = this.GetRepository();

            var step = this.CreateTestStep(sut);
            
            sut.DeleteConfigurationDeploymentStep(step.Id, step.ProjectId);

            Assert.Throws<RecordNotFoundException>(() => sut.GetConfigurationDeploymentStep(step.Id, step.ProjectId));

            var dbComponent = sut.GetConfiguration(step.ParentId, step.ProjectId);
            var dbStep = dbComponent.DeploymentStepList.SingleOrDefault(i => i.Id == step.Id);
            Assert.IsNull(dbStep);
        }

        [Test]
        public void DeleteConfigurationDeploymentStep_MissingStepID_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);
            
            Assert.Throws<ArgumentNullException>(() => sut.DeleteConfigurationDeploymentStep(null, project.Id));
        }

        [Test]
        public void DeleteConfigurationDeploymentStep_BadStepID_ThrowsRecordNotFoundException()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);

            Assert.Throws<RecordNotFoundException>(() => sut.DeleteConfigurationDeploymentStep(Guid.NewGuid().ToString(), project.Id));
        }

        [Test]
        public void GetConfiguation_GetsSteps()
        {
            var sut = this.GetRepository();

            var step = this.CreateTestStep(sut);
            
            var result = sut.GetConfiguration(step.ParentId, step.ProjectId);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.DeploymentStepList);
            var dbStep = result.DeploymentStepList.SingleOrDefault(i => i.Id == step.Id);
            AssertStep(step, dbStep);
        }

        [Test]
        public void GetConfigurationList_GetsSteps()
        {
            var sut = this.GetRepository();

            var step = this.CreateTestStep(sut);

            var result = sut.GetConfigurationList(step.ProjectId);

            Assert.IsNotNull(result);
            var dbConfiguration = result.SingleOrDefault(i => i.Id == step.ParentId);
            Assert.IsNotNull(dbConfiguration);
            Assert.IsNotNull(dbConfiguration.DeploymentStepList);
            var dbStep = dbConfiguration.DeploymentStepList.SingleOrDefault(i => i.Id == step.Id);
            AssertStep(step, dbStep);
        }

        [Test]
        public void DeleteConfiguration_DeleteStep()
        {
            var sut = this.GetRepository();

            var step = this.CreateTestStep(sut);

            sut.DeleteConfiguration(step.ParentId, step.ProjectId);

            Assert.Throws<RecordNotFoundException>(() => sut.GetConfigurationDeploymentStep(step.Id, step.ProjectId));
            Assert.Throws<RecordNotFoundException>(() => sut.GetConfiguration(step.ParentId, step.ProjectId));
        }

        [Test]
        public void DeleteProject_DeletsStep()
        {
            var sut = this.GetRepository();

            var step = this.CreateTestStep(sut);

            sut.DeleteProject(step.ProjectId);

            Assert.Throws<RecordNotFoundException>(() => sut.GetConfigurationDeploymentStep(step.Id, step.ProjectId));
            Assert.Throws<RecordNotFoundException>(() => sut.GetConfiguration(step.ParentId, step.ProjectId));
        }
    }
}
