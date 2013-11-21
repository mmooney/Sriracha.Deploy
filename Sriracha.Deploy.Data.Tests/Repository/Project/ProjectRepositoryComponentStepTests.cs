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
    public abstract class ProjectRepositoryComponentStepTests : ProjectRepositoryTestBase
    {
        private DeployComponent CreateTestComponent(IProjectRepository sut, string projectId=null)
        {
            if(string.IsNullOrEmpty(projectId))
            {
                var project = this.CreateTestProject(sut);
                projectId = project.Id;
            }
            return sut.CreateComponent(projectId, this.Fixture.Create<string>("ComponentName"), false, null, this.Fixture.Create<EnumDeploymentIsolationType>());
        }

        private void AssertCreatedStep(DeployStep result, DeployComponent component, string stepName, string taskTypeName, string taskOptionsJson, IProjectRepository sut)
        {
            Assert.IsNotNull(result);
            Assert.IsNotNullOrEmpty(result.Id);
            Assert.AreEqual(component.ProjectId, result.ProjectId);
            Assert.AreEqual(component.Id, result.ParentId);
            Assert.AreEqual(EnumDeployStepParentType.Component, result.ParentType);
            Assert.AreEqual(stepName, result.StepName);
            Assert.AreEqual(taskTypeName, result.TaskTypeName);
            Assert.AreEqual(taskOptionsJson, result.TaskOptionsJson);
            Assert.IsNotNullOrEmpty(result.SharedDeploymentStepId);
            Assert.AreEqual(this.UserName, result.CreatedByUserName);
            AssertIsRecent(result.CreatedDateTimeUtc);
            Assert.AreEqual(this.UserName, result.UpdatedByUserName);
            AssertIsRecent(result.UpdatedDateTimeUtc);

            var dbItem = sut.GetComponentDeploymentStep(result.Id);
            AssertStep(result, dbItem);
        }

        private void AssertUpdatedStep(DeployStep result, DeployStep original, string newStepName, string newTaskTypeName, string newTaskOptionsJson, string newSharedDeploymentStepId, string newUserName, IProjectRepository sut)
        {
            Assert.IsNotNull(result);
            Assert.IsNotNullOrEmpty(result.Id);
            Assert.AreEqual(original.ProjectId, result.ProjectId);
            Assert.AreEqual(original.ParentId, result.ParentId);
            Assert.AreEqual(original.ParentType, result.ParentType);
            Assert.AreEqual(newStepName, result.StepName);
            Assert.AreEqual(newTaskTypeName, result.TaskTypeName);
            Assert.AreEqual(newTaskOptionsJson, result.TaskOptionsJson);
            Assert.AreEqual(newSharedDeploymentStepId, result.SharedDeploymentStepId);
            Assert.AreEqual(original.CreatedByUserName, result.CreatedByUserName);
            AssertDateEqual(original.CreatedDateTimeUtc, result.CreatedDateTimeUtc);
            Assert.AreEqual(newUserName, result.UpdatedByUserName);
            AssertIsRecent(result.UpdatedDateTimeUtc);

            var dbItem = sut.GetComponentDeploymentStep(result.Id);
            AssertStep(result, dbItem);
        }

        private DeployStep CreateTestStep(IProjectRepository sut, DeployComponent component = null)
        {
            if(component == null)
            {
                component = this.CreateTestComponent(sut);
            }
            return sut.CreateComponentDeploymentStep(component.ProjectId, component.Id, this.Fixture.Create<string>("StepName"), this.Fixture.Create<string>("TaskTypeName"), this.Fixture.Create<string>("TaskOptionsJson"), null);
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
        public void CreateComponentDeploymentStep_CreatesComponentDeploymentStep()
        {
            var sut = this.GetRepository();
            
            var component = this.CreateTestComponent(sut);

            string stepName = this.Fixture.Create<string>("StepName");
            string taskTypeName = this.Fixture.Create<string>("TaskTypeName");
            string taskOptionsJson = this.Fixture.Create<string>("TaskOptionsJson");
            var result = sut.CreateComponentDeploymentStep(component.ProjectId, component.Id, stepName, taskTypeName, taskOptionsJson, null);

            AssertCreatedStep(result, component, stepName, taskTypeName, taskOptionsJson, sut);
        }

        [Test]
        public void CreateComponentDeploymentStep_MissingProjectID_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            var component = this.CreateTestComponent(sut);

            string stepName = this.Fixture.Create<string>("StepName");
            string taskTypeName = this.Fixture.Create<string>("TaskTypeName");
            string taskOptionsJson = this.Fixture.Create<string>("TaskOptionsJson");
            Assert.Throws<ArgumentNullException>(() => sut.CreateComponentDeploymentStep(null, component.Id, stepName, taskTypeName, taskOptionsJson, null));
        }

        [Test]
        public void CreateComponentDeploymentStep_BadProjectID_ThrowsRecordNotFoundException()
        {
            var sut = this.GetRepository();

            var component = this.CreateTestComponent(sut);

            string stepName = this.Fixture.Create<string>("StepName");
            string taskTypeName = this.Fixture.Create<string>("TaskTypeName");
            string taskOptionsJson = this.Fixture.Create<string>("TaskOptionsJson");
            Assert.Throws<RecordNotFoundException>(() => sut.CreateComponentDeploymentStep(Guid.NewGuid().ToString(), component.Id, stepName, taskTypeName, taskOptionsJson, null));
        }

        [Test]
        public void CreateComponentDeploymentStep_MissingComponentID_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            var component = this.CreateTestComponent(sut);

            string stepName = this.Fixture.Create<string>("StepName");
            string taskTypeName = this.Fixture.Create<string>("TaskTypeName");
            string taskOptionsJson = this.Fixture.Create<string>("TaskOptionsJson");
            Assert.Throws<ArgumentNullException>(() => sut.CreateComponentDeploymentStep(component.ProjectId, null, stepName, taskTypeName, taskOptionsJson, null));
        }

        [Test]
        public void CreateComponentDeploymentStep_BadComponentID_ThrowsRecordNotFoundException()
        {
            var sut = this.GetRepository();

            var component = this.CreateTestComponent(sut);

            string stepName = this.Fixture.Create<string>("StepName");
            string taskTypeName = this.Fixture.Create<string>("TaskTypeName");
            string taskOptionsJson = this.Fixture.Create<string>("TaskOptionsJson");
            Assert.Throws<RecordNotFoundException>(() => sut.CreateComponentDeploymentStep(component.ProjectId, Guid.NewGuid().ToString(), stepName, taskTypeName, taskOptionsJson, null));
        }

        [Test]
        public void CreateComponentDeploymentStep_MissingStepName_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            var component = this.CreateTestComponent(sut);

            string stepName = null;
            string taskTypeName = this.Fixture.Create<string>("TaskTypeName");
            string taskOptionsJson = this.Fixture.Create<string>("TaskOptionsJson");
            Assert.Throws<ArgumentNullException>(() => sut.CreateComponentDeploymentStep(component.ProjectId, component.Id, stepName, taskTypeName, taskOptionsJson, null));
        }

        [Test]
        public void CreateComponentDeploymentStep_MissingTaskTypeName_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            var component = this.CreateTestComponent(sut);

            string stepName = this.Fixture.Create<string>("StepName");
            string taskTypeName = null;
            string taskOptionsJson = this.Fixture.Create<string>("TaskOptionsJson");
            Assert.Throws<ArgumentNullException>(() => sut.CreateComponentDeploymentStep(component.ProjectId, component.Id, stepName, taskTypeName, taskOptionsJson, null));
        }

        [Test]
        public void CreateComponentDeploymentStep_MissingTaskOptionsJson_CreatesStep()
        {
            var sut = this.GetRepository();
            
            var component = this.CreateTestComponent(sut);

            string stepName = this.Fixture.Create<string>("StepName");
            string taskTypeName = this.Fixture.Create<string>("TaskTypeName");
            string taskOptionsJson = null;
            var result = sut.CreateComponentDeploymentStep(component.ProjectId, component.Id, stepName, taskTypeName, taskOptionsJson, null);

            AssertCreatedStep(result, component, stepName, taskTypeName, taskOptionsJson, sut);
        }

        [Test]
        public void GetComponentDeploymentStepList_GetsComponentDeploymentStepList()
        {
            var sut = this.GetRepository();

            var component = this.CreateTestComponent(sut);
            var stepList = new List<DeployStep>();
            for(int i = 0; i < 5; i++)
            {
                var step = this.CreateTestStep(sut, component);
                stepList.Add(step);
            }
            var otherComponent = this.CreateTestComponent(sut);
            for (int i = 0; i < 5; i++)
            {
                this.CreateTestStep(sut, otherComponent);
            }

            var result = sut.GetComponentDeploymentStepList(component.Id);

            Assert.IsNotNull(result);
            Assert.AreEqual(stepList.Count, result.Count);
            foreach(var step in stepList)
            {
                var resultItem = result.SingleOrDefault(i=>i.Id == step.Id);
                AssertStep(step, resultItem);
            }
        }

        [Test]
        public void GetComponentDeploymentStepList_MissingComponentID_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            Assert.Throws<ArgumentNullException>(()=>sut.GetComponentDeploymentStepList(null));
        }

        [Test]
        public void GetComponentDeploymentStepList_BadComponentID_ThrowsRecordNotFoundException()
        {
            var sut = this.GetRepository();

            Assert.Throws<RecordNotFoundException>(() => sut.GetComponentDeploymentStepList(Guid.NewGuid().ToString()));
        }

        [Test]
        public void GetComponentDeploymentStep_GetsStep()
        {
            var sut = this.GetRepository();

            var step = this.CreateTestStep(sut);

            var result = sut.GetComponentDeploymentStep(step.Id);

            AssertStep(step, result);
        }

        [Test]
        public void GetComponentDeploymentStep_MissingStepID_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            var step = this.CreateTestStep(sut);

            Assert.Throws<ArgumentNullException>(() => sut.GetComponentDeploymentStep(null));
        }

        [Test]
        public void GetComponentDeploymentStep_BadStepID_ThrowsArgumentRecordNotFoundException()
        {
            var sut = this.GetRepository();

            var step = this.CreateTestStep(sut);

            Assert.Throws<ArgumentNullException>(() => sut.GetComponentDeploymentStep(null));
        }

        [Test]
        public void UpdateComponentDeploymentStep_WithStepID_UpdatesStep()
        {
            var sut = this.GetRepository();

            var step = this.CreateTestStep(sut);

            string newStepName = this.Fixture.Create<string>("StepName");
            string newTaskTypeName = this.Fixture.Create<string>("TaskTypeName");
            string newTaskOptionsJson = this.Fixture.Create<string>("TaskOptionsJson");
            string newUserName = this.Fixture.Create<string>("UserName");
            string newSharedDeploymentStepId = this.Fixture.Create<string>();
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            var result = sut.UpdateComponentDeploymentStep(step.Id, step.ProjectId, step.ParentId, newStepName, newTaskTypeName, newTaskOptionsJson, newSharedDeploymentStepId);

            AssertUpdatedStep(result, step, newStepName, newTaskTypeName, newTaskOptionsJson, newSharedDeploymentStepId, newUserName, sut);
        }

        [Test]
        public void UpdateComponentDeploymentStep_WithoutStepID_WithSharedStepID_UpdatesStep()
        {
            var sut = this.GetRepository();

            var step = this.CreateTestStep(sut);

            string newStepName = this.Fixture.Create<string>("StepName");
            string newTaskTypeName = this.Fixture.Create<string>("TaskTypeName");
            string newTaskOptionsJson = this.Fixture.Create<string>("TaskOptionsJson");
            string newUserName = this.Fixture.Create<string>("UserName");
            string newSharedDeploymentStepId = step.SharedDeploymentStepId;
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            var result = sut.UpdateComponentDeploymentStep(null, step.ProjectId, step.ParentId, newStepName, newTaskTypeName, newTaskOptionsJson, newSharedDeploymentStepId);

            AssertUpdatedStep(result, step, newStepName, newTaskTypeName, newTaskOptionsJson, newSharedDeploymentStepId, newUserName, sut);
        }

        [Test]
        public void UpdateComponentDeploymentStep_MissingStepIDAndDeploymenTStepID_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            var step = this.CreateTestStep(sut);

            string newStepName = this.Fixture.Create<string>("StepName");
            string newTaskTypeName = this.Fixture.Create<string>("TaskTypeName");
            string newTaskOptionsJson = this.Fixture.Create<string>("TaskOptionsJson");
            string newUserName = this.Fixture.Create<string>("UserName");
            string newSharedDeploymentStepId = null;
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            Assert.Throws<ArgumentNullException>(() => sut.UpdateComponentDeploymentStep(null, step.ProjectId, step.ParentId, newStepName, newTaskTypeName, newTaskOptionsJson, newSharedDeploymentStepId));
        }

        [Test]
        public void UpdateComponentDeploymentStep_BadSharedStepID_ThrowsRecordNotFoundException()
        {
            var sut = this.GetRepository();

            var step = this.CreateTestStep(sut);

            string newStepName = this.Fixture.Create<string>("StepName");
            string newTaskTypeName = this.Fixture.Create<string>("TaskTypeName");
            string newTaskOptionsJson = this.Fixture.Create<string>("TaskOptionsJson");
            string newUserName = this.Fixture.Create<string>("UserName");
            string newSharedDeploymentStepId = step.SharedDeploymentStepId;
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            Assert.Throws<RecordNotFoundException>(()=>sut.UpdateComponentDeploymentStep(null, step.ProjectId, step.ParentId, newStepName, newTaskTypeName, newTaskOptionsJson, Guid.NewGuid().ToString()));
        }

        [Test]
        public void UpdateComponentDeploymentStep_BadStepID_ThrowsRecordNotFoundException()
        {
            var sut = this.GetRepository();

            var step = this.CreateTestStep(sut);

            string newStepName = this.Fixture.Create<string>("StepName");
            string newTaskTypeName = this.Fixture.Create<string>("TaskTypeName");
            string newTaskOptionsJson = this.Fixture.Create<string>("TaskOptionsJson");
            string newUserName = this.Fixture.Create<string>("UserName");
            string newSharedDeploymentStepId = null;
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            Assert.Throws<RecordNotFoundException>(() => sut.UpdateComponentDeploymentStep(Guid.NewGuid().ToString(), step.ProjectId, step.ParentId, newStepName, newTaskTypeName, newTaskOptionsJson, newSharedDeploymentStepId));
        }

        [Test]
        public void UpdateComponentDeploymentStep_BadProjectID_ThrowsRecordNotFoundException()
        {
            var sut = this.GetRepository();

            var step = this.CreateTestStep(sut);

            string newStepName = this.Fixture.Create<string>("StepName");
            string newTaskTypeName = this.Fixture.Create<string>("TaskTypeName");
            string newTaskOptionsJson = this.Fixture.Create<string>("TaskOptionsJson");
            string newUserName = this.Fixture.Create<string>("UserName");
            string newSharedDeploymentStepId = null;
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            Assert.Throws<RecordNotFoundException>(() => sut.UpdateComponentDeploymentStep(step.Id, Guid.NewGuid().ToString(), step.ParentId, newStepName, newTaskTypeName, newTaskOptionsJson, newSharedDeploymentStepId));
        }

        [Test]
        public void UpdateComponentDeploymentStep_MissingProjectID_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            var step = this.CreateTestStep(sut);

            string newStepName = this.Fixture.Create<string>("StepName");
            string newTaskTypeName = this.Fixture.Create<string>("TaskTypeName");
            string newTaskOptionsJson = this.Fixture.Create<string>("TaskOptionsJson");
            string newUserName = this.Fixture.Create<string>("UserName");
            string newSharedDeploymentStepId = null;
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            Assert.Throws<ArgumentNullException>(() => sut.UpdateComponentDeploymentStep(step.Id, null, step.ParentId, newStepName, newTaskTypeName, newTaskOptionsJson, newSharedDeploymentStepId));
        }

        [Test]
        public void UpdateComponentDeploymentStep_BadComponentID_ThrowsRecordNotFoundException()
        {
            var sut = this.GetRepository();

            var step = this.CreateTestStep(sut);

            string newStepName = this.Fixture.Create<string>("StepName");
            string newTaskTypeName = this.Fixture.Create<string>("TaskTypeName");
            string newTaskOptionsJson = this.Fixture.Create<string>("TaskOptionsJson");
            string newUserName = this.Fixture.Create<string>("UserName");
            string newSharedDeploymentStepId = null;
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            Assert.Throws<RecordNotFoundException>(() => sut.UpdateComponentDeploymentStep(step.Id, step.ProjectId, Guid.NewGuid().ToString(), newStepName, newTaskTypeName, newTaskOptionsJson, newSharedDeploymentStepId));
        }

        [Test]
        public void UpdateComponentDeploymentStep_MissingComponentID_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            var step = this.CreateTestStep(sut);

            string newStepName = this.Fixture.Create<string>("StepName");
            string newTaskTypeName = this.Fixture.Create<string>("TaskTypeName");
            string newTaskOptionsJson = this.Fixture.Create<string>("TaskOptionsJson");
            string newUserName = this.Fixture.Create<string>("UserName");
            string newSharedDeploymentStepId = null;
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            Assert.Throws<ArgumentNullException>(() => sut.UpdateComponentDeploymentStep(step.Id, step.ProjectId, null, newStepName, newTaskTypeName, newTaskOptionsJson, newSharedDeploymentStepId));
        }

        [Test]
        public void UpdateComponentDeploymentStep_MissingStepName_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            var step = this.CreateTestStep(sut);

            string newStepName = null;
            string newTaskTypeName = this.Fixture.Create<string>("TaskTypeName");
            string newTaskOptionsJson = this.Fixture.Create<string>("TaskOptionsJson");
            string newUserName = this.Fixture.Create<string>("UserName");
            string newSharedDeploymentStepId = null;
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            Assert.Throws<ArgumentNullException>(() => sut.UpdateComponentDeploymentStep(step.Id, step.ProjectId, step.ParentId, newStepName, newTaskTypeName, newTaskOptionsJson, newSharedDeploymentStepId));
        }

        [Test]
        public void UpdateComponentDeploymentStep_MissingTaskTypeName_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            var step = this.CreateTestStep(sut);

            string newStepName = this.Fixture.Create<string>("StepName");
            string newTaskTypeName = null;
            string newTaskOptionsJson = this.Fixture.Create<string>("TaskOptionsJson");
            string newUserName = this.Fixture.Create<string>("UserName");
            string newSharedDeploymentStepId = null;
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            Assert.Throws<ArgumentNullException>(() => sut.UpdateComponentDeploymentStep(step.Id, step.ProjectId, step.ParentId, newStepName, newTaskTypeName, newTaskOptionsJson, newSharedDeploymentStepId));
        }

        [Test]
        public void UpdateComponentDeploymentStep_MissingTaskOptionsJson_UpdatesStep()
        {
            var sut = this.GetRepository();

            var step = this.CreateTestStep(sut);

            string newStepName = this.Fixture.Create<string>("StepName");
            string newTaskTypeName = this.Fixture.Create<string>("TaskTypeName");
            string newTaskOptionsJson = null;
            string newUserName = this.Fixture.Create<string>("UserName");
            string newSharedDeploymentStepId = this.Fixture.Create<string>();
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            var result = sut.UpdateComponentDeploymentStep(step.Id, step.ProjectId, step.ParentId, newStepName, newTaskTypeName, newTaskOptionsJson, newSharedDeploymentStepId);

            AssertUpdatedStep(result, step, newStepName, newTaskTypeName, newTaskOptionsJson, newSharedDeploymentStepId, newUserName, sut);
        }

        [Test]
        public void DeleteComponentDeploymentStep_DeletesStep()
        {
            var sut = this.GetRepository();

            var step = this.CreateTestStep(sut);

            sut.DeleteComponentDeploymentStep(step.Id);

            Assert.Throws<RecordNotFoundException>(() => sut.GetComponentDeploymentStep(step.Id));

            var dbComponent = sut.GetComponent(step.ParentId, step.ProjectId);
            var dbStep = dbComponent.DeploymentStepList.SingleOrDefault(i => i.Id == step.Id);
            Assert.IsNull(dbStep);
        }

        [Test]
        public void DeleteComponentDeploymentStep_MissingStepID_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            Assert.Throws<ArgumentNullException>(() => sut.DeleteComponentDeploymentStep(null));
        }

        [Test]
        public void DeleteComponentDeploymentStep_BadStepID_ThrowsRecordNotFoundException()
        {
            var sut = this.GetRepository();

            Assert.Throws<RecordNotFoundException>(() => sut.DeleteComponentDeploymentStep(Guid.NewGuid().ToString()));
        }

        [Test]
        public void GetComponent_GetsSteps()
        {
            var sut = this.GetRepository();

            var step = this.CreateTestStep(sut);

            var result = sut.GetComponent(step.ParentId);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.DeploymentStepList);
            var dbStep = result.DeploymentStepList.SingleOrDefault(i=>i.Id == step.Id);
            AssertStep(step, dbStep);
        }

        [Test]
        public void GetComponentList_GetsSteps()
        {
            var sut = this.GetRepository();

            var step = this.CreateTestStep(sut);

            var result = sut.GetComponentList(step.ProjectId);

            Assert.IsNotNull(result);
            var dbComponent = result.SingleOrDefault(i=>i.Id == step.ParentId);
            Assert.IsNotNull(dbComponent);
            Assert.IsNotNull(dbComponent.DeploymentStepList);
            var dbStep = dbComponent.DeploymentStepList.SingleOrDefault(i => i.Id == step.Id);
            AssertStep(step, dbStep);
        }

        [Test]
        public void DeleteComponent_DeleteStep()
        {
            var sut = this.GetRepository();

            var step = this.CreateTestStep(sut);

            sut.DeleteComponent(step.ProjectId, step.ParentId);

            Assert.Throws<RecordNotFoundException>(() => sut.GetComponentDeploymentStep(step.Id));
            Assert.Throws<RecordNotFoundException>(() => sut.GetComponent(step.ParentId, step.ProjectId));
        }

        [Test]
        public void DeleteProject_DeletsStep()
        {
            var sut = this.GetRepository();

            var step = this.CreateTestStep(sut);

            sut.DeleteProject(step.ProjectId);

            Assert.Throws<RecordNotFoundException>(() => sut.GetComponentDeploymentStep(step.Id));
            Assert.Throws<RecordNotFoundException>(() => sut.GetComponent(step.ParentId, step.ProjectId));
        }
    }
}
