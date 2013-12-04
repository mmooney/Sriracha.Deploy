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
using MMDB.Shared;

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

        private DeployState CreateTestDeployState(IDeployStateRepository sut, string buildId=null, string environmentId=null, string machineId=null)
        {
            var testData = this.GetCreateTestData(buildId: buildId, environmentId: environmentId, machineId:machineId);
            return sut.CreateDeployment(testData.Build, testData.Branch, testData.Environment, testData.Component, testData.MachineList, testData.DeployBatchRequestId);
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

                    AssertMessage(expectedItem, actualItem);
                }
            }
        }

        private void AssertMessage(DeployStateMessage expectedItem, DeployStateMessage actualItem)
        {
            Assert.AreEqual(expectedItem.Id, actualItem.Id);
            AssertDateEqual(expectedItem.DateTimeUtc, actualItem.DateTimeUtc);
            Assert.AreEqual(expectedItem.DeployStateId, actualItem.DeployStateId);
            Assert.AreEqual(expectedItem.Message, actualItem.Message);
            Assert.AreEqual(expectedItem.MessageUserName, actualItem.MessageUserName);
        }

        private CreateTestData GetCreateTestData(string buildId=null, string environmentId=null, string machineId=null)
        {
            string projectId = this.Fixture.Create<string>();
            string projectName = this.Fixture.Create<string>("ProjectName");
            environmentId = StringHelper.IsNullOrEmpty(environmentId, this.Fixture.Create<string>());
            machineId = StringHelper.IsNullOrEmpty(machineId, this.Fixture.Create<string>());
            buildId = StringHelper.IsNullOrEmpty(buildId, this.Fixture.Create<string>());
            var returnValue = new CreateTestData()
            {
                ProjectId = projectId,
                Branch = this.Fixture.Build<DeployProjectBranch>()
                                .With(i=>i.ProjectId, projectId)
                                .Create(),
                Environment = this.Fixture.Build<DeployEnvironment>()
                                .With(i=>i.ProjectId, projectId)
                                .With(i=>i.Id, environmentId)
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
            returnValue.Environment.ComponentList[0].MachineList[0].Id = machineId;
            returnValue.MachineList = returnValue.Environment.ComponentList[0].MachineList;

            returnValue.Build = this.Fixture.Build<DeployBuild>()
                                    .With(i=>i.Id, buildId)
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

        [Test]
        public void CreateDeployment_MissingBuild_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            var testData = this.GetCreateTestData();

            Assert.Throws<ArgumentNullException>(() => sut.CreateDeployment(null, testData.Branch, testData.Environment, testData.Component, testData.MachineList, testData.DeployBatchRequestId));
        }

        [Test]
        public void CreateDeployment_MissingBranch_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            var testData = this.GetCreateTestData();

            Assert.Throws<ArgumentNullException>(() => sut.CreateDeployment(testData.Build, null, testData.Environment, testData.Component, testData.MachineList, testData.DeployBatchRequestId));
        }

        [Test]
        public void CreateDeployment_MissingEnvironment_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            var testData = this.GetCreateTestData();

            Assert.Throws<ArgumentNullException>(() => sut.CreateDeployment(testData.Build, testData.Branch, null, testData.Component, testData.MachineList, testData.DeployBatchRequestId));
        }

        [Test]
        public void CreateDeployment_MissingComponent_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            var testData = this.GetCreateTestData();

            Assert.Throws<ArgumentNullException>(() => sut.CreateDeployment(testData.Build, testData.Branch, testData.Environment, null, testData.MachineList, testData.DeployBatchRequestId));
        }

        [Test]
        public void CreateDeployment_MissingMachineList_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            var testData = this.GetCreateTestData();

            Assert.Throws<ArgumentNullException>(() => sut.CreateDeployment(testData.Build, testData.Branch, testData.Environment, testData.Component, null, testData.DeployBatchRequestId));
        }

        [Test]
        public void CreateDeployment_MissingDeployBatchRequestItemID_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            var testData = this.GetCreateTestData();

            Assert.Throws<ArgumentNullException>(() => sut.CreateDeployment(testData.Build, testData.Branch, testData.Environment, testData.Component, testData.MachineList, null));
        }

        [Test]
        public void GetDeployState_GetsDeployState()
        {
            var sut = this.GetRepository();

            var deployState = this.CreateTestDeployState(sut);

            var result = sut.GetDeployState(deployState.Id);

            AssertDeployState(deployState, result);
        }

        [Test]
        public void GetDeployState_MissingDeployStateID_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            Assert.Throws<ArgumentNullException>(() => sut.GetDeployState(null));
        }

        [Test]
        public void GetDeployState_BadDeployStateID_ThrowsRecordNotFoundException()
        {
            var sut = this.GetRepository();

            Assert.Throws<RecordNotFoundException>(() => sut.GetDeployState(Guid.NewGuid().ToString()));
        }

        [Test]
        public void FindDeployStateListForEnvironment_FindsList()
        {
            var sut = this.GetRepository();

            string environmentId = this.Fixture.Create<string>();
            string buildId = this.Fixture.Create<string>();
            var deployStateList = new List<DeployState>();
            for(int i = 0; i < 5; i++)
            {
                var deployState = this.CreateTestDeployState(sut, environmentId: environmentId, buildId: buildId);
                deployStateList.Add(deployState);
            }

            var resultList = sut.FindDeployStateListForEnvironment(buildId, environmentId);

            Assert.IsNotNull(resultList);
            Assert.AreEqual(deployStateList.Count, resultList.Count);
            foreach(var expectedItem in deployStateList)
            {  
                var actualItem = resultList.SingleOrDefault(i=>i.Id == expectedItem.Id);
                Assert.IsNotNull(actualItem);
                AssertDeployState(expectedItem, actualItem);
            }
        }

        [Test]
        public void FindDeployStateListForEnvironment_MissingBuildID_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            string environmentId = this.Fixture.Create<string>();
            string buildId = this.Fixture.Create<string>();
            var deployStateList = new List<DeployState>();
            for (int i = 0; i < 5; i++)
            {
                var deployState = this.CreateTestDeployState(sut, environmentId: environmentId, buildId: buildId);
                deployStateList.Add(deployState);
            }

            Assert.Throws<ArgumentNullException>(() => sut.FindDeployStateListForEnvironment(null, environmentId));
        }

        [Test]
        public void FindDeployStateListForEnvironment_MissingEnvironmentID_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            string environmentId = this.Fixture.Create<string>();
            string buildId = this.Fixture.Create<string>();
            var deployStateList = new List<DeployState>();
            for (int i = 0; i < 5; i++)
            {
                var deployState = this.CreateTestDeployState(sut, environmentId: environmentId, buildId: buildId);
                deployStateList.Add(deployState);
            }

            Assert.Throws<ArgumentNullException>(() => sut.FindDeployStateListForEnvironment(buildId, null));
        }

        [Test]
        public void FindDeployStateListForEnvironment_BadBuildID_ReturnsEmptyList()
        {
            var sut = this.GetRepository();

            string environmentId = this.Fixture.Create<string>();
            string buildId = this.Fixture.Create<string>();
            var deployStateList = new List<DeployState>();
            for (int i = 0; i < 5; i++)
            {
                var deployState = this.CreateTestDeployState(sut, environmentId: environmentId, buildId: buildId);
                deployStateList.Add(deployState);
            }

            var result = sut.FindDeployStateListForEnvironment(Guid.NewGuid().ToString(), environmentId);

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void FindDeployStateListForEnvironment_BadEnvironmentID_ReturnsEmptyList()
        {
            var sut = this.GetRepository();

            string environmentId = this.Fixture.Create<string>();
            string buildId = this.Fixture.Create<string>();
            var deployStateList = new List<DeployState>();
            for (int i = 0; i < 5; i++)
            {
                var deployState = this.CreateTestDeployState(sut, environmentId: environmentId, buildId: buildId);
                deployStateList.Add(deployState);
            }

            var result = sut.FindDeployStateListForEnvironment(buildId, Guid.NewGuid().ToString());

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [Test]
        public void FindDeployStateListForMachine_FindsList()
        {
            var sut = this.GetRepository();

            string environmentId = this.Fixture.Create<string>();
            string machineId = this.Fixture.Create<string>();
            string buildId = this.Fixture.Create<string>();
            var deployStateList = new List<DeployState>();
            for (int i = 0; i < 5; i++)
            {
                var deployState = this.CreateTestDeployState(sut, buildId: buildId, environmentId: environmentId, machineId: machineId);
                deployStateList.Add(deployState);
            }

            var resultList = sut.FindDeployStateListForMachine(buildId, environmentId, machineId);

            Assert.IsNotNull(resultList);
            Assert.AreEqual(deployStateList.Count, resultList.Count);
            foreach (var expectedItem in deployStateList)
            {
                var actualItem = resultList.SingleOrDefault(i => i.Id == expectedItem.Id);
                Assert.IsNotNull(actualItem);
                AssertDeployState(expectedItem, actualItem);
            }
        }

        [Test]
        public void FindDeployStateListForMachine_MissingBuildId_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            string environmentId = this.Fixture.Create<string>();
            string machineId = this.Fixture.Create<string>();
            string buildId = this.Fixture.Create<string>();
            var deployStateList = new List<DeployState>();
            for (int i = 0; i < 5; i++)
            {
                var deployState = this.CreateTestDeployState(sut, buildId: buildId, environmentId: environmentId, machineId: machineId);
                deployStateList.Add(deployState);
            }

            Assert.Throws<ArgumentNullException>(() => sut.FindDeployStateListForMachine(null, environmentId, machineId));
        }

        [Test]
        public void FindDeployStateListForMachine_MissingEnvironmentId_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            string environmentId = this.Fixture.Create<string>();
            string machineId = this.Fixture.Create<string>();
            string buildId = this.Fixture.Create<string>();
            var deployStateList = new List<DeployState>();
            for (int i = 0; i < 5; i++)
            {
                var deployState = this.CreateTestDeployState(sut, buildId: buildId, environmentId: environmentId, machineId: machineId);
                deployStateList.Add(deployState);
            }

            Assert.Throws<ArgumentNullException>(() => sut.FindDeployStateListForMachine(buildId, null, machineId));
        }

        [Test]
        public void FindDeployStateListForMachine_MissingMachineId_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            string environmentId = this.Fixture.Create<string>();
            string machineId = this.Fixture.Create<string>();
            string buildId = this.Fixture.Create<string>();
            var deployStateList = new List<DeployState>();
            for (int i = 0; i < 5; i++)
            {
                var deployState = this.CreateTestDeployState(sut, buildId: buildId, environmentId: environmentId, machineId: machineId);
                deployStateList.Add(deployState);
            }

            Assert.Throws<ArgumentNullException>(() => sut.FindDeployStateListForMachine(buildId, environmentId, null));
        }

        [Test]
        public void FindDeployStateListForMachine_BadBuildID_ReturnsEmptyList()
        {
            var sut = this.GetRepository();

            string environmentId = this.Fixture.Create<string>();
            string machineId = this.Fixture.Create<string>();
            string buildId = this.Fixture.Create<string>();
            var deployStateList = new List<DeployState>();
            for (int i = 0; i < 5; i++)
            {
                var deployState = this.CreateTestDeployState(sut, buildId: buildId, environmentId: environmentId, machineId: machineId);
                deployStateList.Add(deployState);
            }

            var resultList = sut.FindDeployStateListForMachine(Guid.NewGuid().ToString(), environmentId, machineId);

            Assert.IsNotNull(resultList);
            Assert.AreEqual(0, resultList.Count);
        }

        [Test]
        public void FindDeployStateListForMachine_BadEnvironmentID_ReturnsEmptyList()
        {
            var sut = this.GetRepository();

            string environmentId = this.Fixture.Create<string>();
            string machineId = this.Fixture.Create<string>();
            string buildId = this.Fixture.Create<string>();
            var deployStateList = new List<DeployState>();
            for (int i = 0; i < 5; i++)
            {
                var deployState = this.CreateTestDeployState(sut, buildId: buildId, environmentId: environmentId, machineId: machineId);
                deployStateList.Add(deployState);
            }

            var resultList = sut.FindDeployStateListForMachine(buildId, Guid.NewGuid().ToString(), machineId);

            Assert.IsNotNull(resultList);
            Assert.AreEqual(0, resultList.Count);
        }


        [Test]
        public void FindDeployStateListForMachine_BadMachineID_ReturnsEmptyList()
        {
            var sut = this.GetRepository();

            string environmentId = this.Fixture.Create<string>();
            string machineId = this.Fixture.Create<string>();
            string buildId = this.Fixture.Create<string>();
            var deployStateList = new List<DeployState>();
            for (int i = 0; i < 5; i++)
            {
                var deployState = this.CreateTestDeployState(sut, buildId: buildId, environmentId: environmentId, machineId: machineId);
                deployStateList.Add(deployState);
            }

            var resultList = sut.FindDeployStateListForMachine(buildId, environmentId, Guid.NewGuid().ToString());

            Assert.IsNotNull(resultList);
            Assert.AreEqual(0, resultList.Count);
        }

        [Test]
        public void TryGetDeployState_GetsDeployState()
        {
            var sut = this.GetRepository();

            string environmentId = this.Fixture.Create<string>();
            string machineId = this.Fixture.Create<string>();
            var deployState = this.CreateTestDeployState(sut, environmentId: environmentId, machineId: machineId);

            var result = sut.TryGetDeployState(deployState.ProjectId, deployState.Build.Id, environmentId, machineId, deployState.DeployBatchRequestItemId);

            Assert.IsNotNull(result);
            AssertDeployState(deployState, result);
        }

        [Test]
        public void TryGetDeployState_MissingProjectID_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            string environmentId = this.Fixture.Create<string>();
            string machineId = this.Fixture.Create<string>();
            var deployState = this.CreateTestDeployState(sut, environmentId: environmentId, machineId: machineId);

            Assert.Throws<ArgumentNullException>(() => sut.TryGetDeployState(null, deployState.Build.Id, environmentId, machineId, deployState.DeployBatchRequestItemId));
        }

        [Test]
        public void TryGetDeployState_MissingBuildID_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            string environmentId = this.Fixture.Create<string>();
            string machineId = this.Fixture.Create<string>();
            var deployState = this.CreateTestDeployState(sut, environmentId: environmentId, machineId: machineId);

            Assert.Throws<ArgumentNullException>(() => sut.TryGetDeployState(deployState.ProjectId, null, environmentId, machineId, deployState.DeployBatchRequestItemId));
        }

        [Test]
        public void TryGetDeployState_MissingEnvironmentID_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            string environmentId = this.Fixture.Create<string>();
            string machineId = this.Fixture.Create<string>();
            var deployState = this.CreateTestDeployState(sut, environmentId: environmentId, machineId: machineId);

            Assert.Throws<ArgumentNullException>(() => sut.TryGetDeployState(deployState.ProjectId, deployState.Build.Id, null, machineId, deployState.DeployBatchRequestItemId));
        }

        [Test]
        public void TryGetDeployState_MissingMachineID_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            string environmentId = this.Fixture.Create<string>();
            string machineId = this.Fixture.Create<string>();
            var deployState = this.CreateTestDeployState(sut, environmentId: environmentId, machineId: machineId);

            Assert.Throws<ArgumentNullException>(() => sut.TryGetDeployState(deployState.ProjectId, deployState.Build.Id, environmentId, null, deployState.DeployBatchRequestItemId));
        }

        [Test]
        public void TryGetDeployState_MissingDeploybatchRequestItemID_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            string environmentId = this.Fixture.Create<string>();
            string machineId = this.Fixture.Create<string>();
            var deployState = this.CreateTestDeployState(sut, environmentId: environmentId, machineId: machineId);

            Assert.Throws<ArgumentNullException>(() => sut.TryGetDeployState(deployState.ProjectId, deployState.Build.Id, environmentId, machineId, null));
        }

        [Test]
        public void TryGetDeployState_BadProjectID_ReturnsNull()
        {
            var sut = this.GetRepository();

            string environmentId = this.Fixture.Create<string>();
            string machineId = this.Fixture.Create<string>();
            var deployState = this.CreateTestDeployState(sut, environmentId: environmentId, machineId: machineId);

            var result = sut.TryGetDeployState(Guid.NewGuid().ToString(), deployState.Build.Id, environmentId, machineId, deployState.DeployBatchRequestItemId);
            
            Assert.IsNull(result);
        }

        [Test]
        public void TryGetDeployState_BadBuildID_ReturnsNull()
        {
            var sut = this.GetRepository();

            string environmentId = this.Fixture.Create<string>();
            string machineId = this.Fixture.Create<string>();
            var deployState = this.CreateTestDeployState(sut, environmentId: environmentId, machineId: machineId);

            var result = sut.TryGetDeployState(deployState.ProjectId, Guid.NewGuid().ToString(), environmentId, machineId, deployState.DeployBatchRequestItemId);
            
            Assert.IsNull(result);
        }


        [Test]
        public void TryGetDeployState_BadMachineID_ReturnsNull()
        {
            var sut = this.GetRepository();

            string environmentId = this.Fixture.Create<string>();
            string machineId = this.Fixture.Create<string>();
            var deployState = this.CreateTestDeployState(sut, environmentId: environmentId, machineId: machineId);

            var result = sut.TryGetDeployState(deployState.ProjectId, deployState.Build.Id, environmentId, Guid.NewGuid().ToString(), deployState.DeployBatchRequestItemId);
            
            Assert.IsNull(result);
        }

        [Test]
        public void TryGetDeployState_BadDeployBatchRequestItemID_ReturnsNull()
        {
            var sut = this.GetRepository();

            string environmentId = this.Fixture.Create<string>();
            string machineId = this.Fixture.Create<string>();
            var deployState = this.CreateTestDeployState(sut, environmentId: environmentId, machineId: machineId);

            var result = sut.TryGetDeployState(deployState.ProjectId, deployState.Build.Id, environmentId, machineId, Guid.NewGuid().ToString());
            
            Assert.IsNull(result);
        }

        [Test]
        public void UpdateDeploymentStatus_NoError_UpdatesStatus()
        {
            var sut = this.GetRepository();

            var deployState = this.CreateTestDeployState(sut);
            var newStatus = EnumDeployStatus.Success;
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i=>i.UserName).Returns(newUserName);

            var result = sut.UpdateDeploymentStatus(deployState.Id, newStatus);

            Assert.IsNotNull(result);
            Assert.AreEqual(result.Status, newStatus);
            Assert.IsNull(result.ErrorDetails);
            Assert.AreEqual(deployState.CreatedByUserName, result.CreatedByUserName);
            AssertDateEqual(deployState.CreatedDateTimeUtc, result.CreatedDateTimeUtc);
            Assert.AreEqual(newUserName, result.UpdatedByUserName);
            AssertIsRecent(result.UpdatedDateTimeUtc);
        }

        [Test]
        public void UpdateDeploymentStatus_WithError_UpdatesStatus()
        {
            var sut = this.GetRepository();

            var deployState = this.CreateTestDeployState(sut);
            var newStatus = EnumDeployStatus.Error;
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);
            var ex = new Exception("This is a test");

            var result = sut.UpdateDeploymentStatus(deployState.Id, newStatus, ex);

            Assert.IsNotNull(result);
            Assert.AreEqual(result.Status, newStatus);
            Assert.AreEqual(ex.ToString(), result.ErrorDetails);
            Assert.AreEqual(deployState.CreatedByUserName, result.CreatedByUserName);
            AssertDateEqual(deployState.CreatedDateTimeUtc, result.CreatedDateTimeUtc);
            Assert.AreEqual(newUserName, result.UpdatedByUserName);
            AssertIsRecent(result.UpdatedDateTimeUtc);
        }

        [Test]
        public void UpdateDeploymentStatus_MissingDeployStateId_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            var deployState = this.CreateTestDeployState(sut);
            var newStatus = EnumDeployStatus.Success;
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            Assert.Throws<ArgumentNullException>(() => sut.UpdateDeploymentStatus(null, newStatus));
        }

        [Test]
        public void UpdateDeploymentStatus_BadDeployStateId_ThrowsRecordNotFoundException()
        {
            var sut = this.GetRepository();

            var deployState = this.CreateTestDeployState(sut);
            var newStatus = EnumDeployStatus.Success;
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            Assert.Throws<RecordNotFoundException>(() => sut.UpdateDeploymentStatus(Guid.NewGuid().ToString(), newStatus));
        }

        [Test]
        public void AddDeploymentMessage_AddsMessage()
        {
            var sut = this.GetRepository();

            var deployState = this.CreateTestDeployState(sut);
            string message = this.Fixture.Create<string>("Message");
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            var result = sut.AddDeploymentMessage(deployState.Id, message);

            Assert.IsNotNull(result);
            Assert.IsNotNullOrEmpty(result.Id);
            Assert.AreEqual(deployState.Id, result.DeployStateId);
            Assert.AreEqual(message, result.Message);
            Assert.AreEqual(newUserName, result.MessageUserName);
            AssertIsRecent(result.DateTimeUtc);

            var dbState = sut.GetDeployState(deployState.Id);
            Assert.IsNotNull(dbState.MessageList);
            Assert.AreEqual(1, dbState.MessageList.Count);
            AssertMessage(result, dbState.MessageList[0]);
            Assert.AreEqual(deployState.CreatedByUserName, dbState.CreatedByUserName);
            AssertDateEqual(deployState.CreatedDateTimeUtc, dbState.CreatedDateTimeUtc);
            Assert.AreEqual(newUserName, dbState.UpdatedByUserName);
            AssertIsRecent(deployState.UpdatedDateTimeUtc);
        }

        [Test]
        public void AddDeploymentMessage_SecondMessage()
        {
            var sut = this.GetRepository();

            var deployState = this.CreateTestDeployState(sut);
            var firstMessage = sut.AddDeploymentMessage(deployState.Id, Guid.NewGuid().ToString());
            string message = this.Fixture.Create<string>("Message");
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            var result = sut.AddDeploymentMessage(deployState.Id, message);

            Assert.IsNotNull(result);
            Assert.IsNotNullOrEmpty(result.Id);
            Assert.AreEqual(deployState.Id, result.DeployStateId);
            Assert.AreEqual(message, result.Message);
            Assert.AreEqual(newUserName, result.MessageUserName);
            AssertIsRecent(result.DateTimeUtc);

            var dbState = sut.GetDeployState(deployState.Id);
            Assert.IsNotNull(dbState.MessageList);
            Assert.AreEqual(2, dbState.MessageList.Count);
            Assert.AreEqual(deployState.CreatedByUserName, dbState.CreatedByUserName);
            AssertDateEqual(deployState.CreatedDateTimeUtc, dbState.CreatedDateTimeUtc);
            Assert.AreEqual(newUserName, dbState.UpdatedByUserName);
            AssertIsRecent(deployState.UpdatedDateTimeUtc);
            var expectedList = new List<DeployStateMessage> { firstMessage, result };
            AssertMessageList(expectedList, dbState.MessageList);
        }

        [Test]
        public void AddDeploymentMessage_MissingDeployStateID_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            var deployState = this.CreateTestDeployState(sut);
            string message = this.Fixture.Create<string>("Message");
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            Assert.Throws<ArgumentNullException>(()=>sut.AddDeploymentMessage(null, message));
        }

        [Test]
        public void AddDeploymentMessage_BadDeployStateID_ThrowsRecordNotFoundException()
        {
            var sut = this.GetRepository();

            string message = this.Fixture.Create<string>("Message");

            Assert.Throws<RecordNotFoundException>(() => sut.AddDeploymentMessage(Guid.NewGuid().ToString(), message));
        }


        [Test]
        public void AddDeploymentMessage_MissingMessage_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            var deployState = this.CreateTestDeployState(sut);
            string message = this.Fixture.Create<string>("Message");
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            Assert.Throws<ArgumentNullException>(() => sut.AddDeploymentMessage(deployState.Id, null));
        }
    }
}
