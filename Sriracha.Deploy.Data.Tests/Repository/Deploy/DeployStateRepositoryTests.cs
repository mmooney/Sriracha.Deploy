using NUnit.Framework;
using Sriracha.Deploy.Data.Dto.Project;
using Sriracha.Deploy.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ploeh.AutoFixture;
using Sriracha.Deploy.Data.Dto.Build;
using Sriracha.Deploy.Data.Dto.Deployment;

namespace Sriracha.Deploy.Data.Tests.Repository.Deploy
{
    public abstract class DeployStateRepositoryTests : RepositoryTestBase<IDeployStateRepository>
    {
        private class CreateTestData
        {
            public string ProjectId { get; set; }
            public DeployProjectBranch Branch { get; set; }
            public DeployEnvironment Environment { get; set; }
            public DeployComponent Component { get; set; }
            public DeployBuild Build { get; set; }
            public List<DeployMachine> MachineList { get; set; }
            public string DeployBatchRequestId { get; set; }
        }

        private void AssertCreatedDeployState(IDeployStateRepository sut, DeployState result, CreateTestData testData)
        {
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Id);
            AssertHelpers.AssertBranch(testData.Branch, result.Branch);
            AssertHelpers.AssertBuild(testData.Build, result.Build);
            AssertHelpers.AssertComponent(testData.Component, result.Component);
            AssertHelpers.AssertMachineList(testData.MachineList, result.MachineList);

            //Assert.AreEqual(this.UserName, result.UserName);
            Assert.IsNullOrEmpty(result.ErrorDetails);
            Assert.AreEqual(EnumDeployStatus.NotStarted, result.Status);
            Assert.IsNull(result.DeploymentCompleteDateTimeUtc);
            Assert.IsNull(result.DeploymentStartedDateTimeUtc);
            Assert.AreEqual(testData.DeployBatchRequestId, result.DeployBatchRequestItemId);
            Assert.AreEqual(0, result.MessageList.Count);
            Assert.AreEqual(testData.ProjectId, result.ProjectId);
            AssertIsRecent(result.SubmittedDateTimeUtc);

            Assert.AreEqual(this.UserName, result.CreatedByUserName);
            AssertIsRecent(result.CreatedDateTimeUtc);
            Assert.AreEqual(this.UserName, result.UpdatedByUserName);
            AssertIsRecent(result.UpdatedDateTimeUtc);

            var dbState = sut.GetDeployState(result.Id);
            AssertDeployState(result, dbState);
        }

        private void AssertDeployState(DeployState expected, DeployState actual)
        {
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected.Id, actual.Id);
            AssertHelpers.AssertBranch(expected.Branch, actual.Branch);
            AssertHelpers.AssertBuild(expected.Build, actual.Build);
            AssertHelpers.AssertComponent(expected.Component, actual.Component);
            AssertHelpers.AssertMachineList(expected.MachineList, actual.MachineList);

            //Assert.AreEqual(this.UserName, result.UserName);
            Assert.AreEqual(expected.ErrorDetails, actual.ErrorDetails);
            Assert.AreEqual(expected.Status, actual.Status);
            Assert.AreEqual(expected.DeploymentCompleteDateTimeUtc, actual.DeploymentCompleteDateTimeUtc);
            Assert.AreEqual(expected.DeploymentStartedDateTimeUtc, actual.DeploymentStartedDateTimeUtc);
            Assert.AreEqual(expected.DeployBatchRequestItemId, actual.DeployBatchRequestItemId);
            AssertMessageList(expected.MessageList, actual.MessageList);
            Assert.AreEqual(expected.ProjectId, actual.ProjectId);
            AssertDateEqual(expected.SubmittedDateTimeUtc, actual.SubmittedDateTimeUtc);

            Assert.AreEqual(expected.CreatedByUserName, actual.CreatedByUserName);
            AssertDateEqual(expected.CreatedDateTimeUtc, actual.CreatedDateTimeUtc);
            Assert.AreEqual(expected.UpdatedByUserName, actual.UpdatedByUserName);
            AssertDateEqual(expected.UpdatedDateTimeUtc, actual.UpdatedDateTimeUtc);
        }

        private void AssertMessageList(List<DeployStateMessage> expected, List<DeployStateMessage> actual)
        {
            if(expected == null)
            {
                Assert.IsNull(actual);
            }
            else
            {
                Assert.IsNotNull(actual);
                Assert.AreEqual(expected.Count, actual.Count);
                for(int i = 0; i < expected.Count; i++)
                {
                    var expectedItem = expected[i];
                    var actualItem = actual[i];

                    Assert.AreEqual(expectedItem.Id, actualItem.Id);
                    AssertDateEqual(expectedItem.DateTimeUtc, actualItem.DateTimeUtc);
                    Assert.AreEqual(expectedItem.DeployStateId, actualItem.DeployStateId);
                    Assert.AreEqual(expectedItem.Message, actualItem.Message);
                    Assert.AreEqual(expectedItem.MessageUserName, actualItem.MessageUserName);
                }
            }
        }

        private CreateTestData GetCreateTestData()
        {
            string projectId = this.Fixture.Create<string>();
            string projectName = this.Fixture.Create<string>("ProjectName");
            var returnValue = new CreateTestData()
            {
                ProjectId = projectId,
                Branch = this.Fixture.Build<DeployProjectBranch>()
                                .With(i=>i.ProjectId, projectId)
                                .Create(),
                Environment = this.Fixture.Build<DeployEnvironment>()
                                .With(i=>i.ProjectId, projectId)
                                .Create(),
                Component = this.Fixture.Build<DeployComponent>()
                                .With(i=>i.ProjectId, projectId)
                                .Create(),
                DeployBatchRequestId = this.Fixture.Create<string>()
            };
            returnValue.Environment.ComponentList[0].ProjectId = projectId;
            returnValue.Environment.ComponentList[0].EnvironmentId = returnValue.Environment.Id;
            returnValue.Environment.ComponentList[0].ParentType = EnumDeployStepParentType.Component;
            returnValue.Environment.ComponentList[0].ParentId = returnValue.Component.Id;
            foreach(var machine in returnValue.Environment.ComponentList[0].MachineList)
            {
                machine.EnvironmentId = returnValue.Environment.Id;
                machine.EnvironmentName = returnValue.Environment.EnvironmentName;
                machine.ProjectId = projectId;
            }
            returnValue.MachineList = returnValue.Environment.ComponentList[0].MachineList;

            returnValue.Build = this.Fixture.Build<DeployBuild>()
                                    .With(i=>i.ProjectId, returnValue.ProjectId)
                                    .With(i=>i.ProjectBranchId, returnValue.Branch.Id)
                                    .With(i=>i.ProjectBranchName, returnValue.Branch.BranchName)
                                    .With(i=>i.ProjectComponentId, returnValue.Component.Id)
                                    .With(i=>i.ProjectComponentName, returnValue.Component.ComponentName)
                                    .Create();

            return returnValue;
        }

        [Test]
        public void CreateDeployment_CreatesDeployment()
        {
            var sut = this.GetRepository();

            var testData = this.GetCreateTestData();

            var result = sut.CreateDeployment(testData.Build, testData.Branch, testData.Environment, testData.Component, testData.MachineList, testData.DeployBatchRequestId);

            AssertCreatedDeployState(sut, result, testData);
        }

    }
}
