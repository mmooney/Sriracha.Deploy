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
using Sriracha.Deploy.Data.Dto;

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

        private DeployState CreateTestDeployState(IDeployStateRepository sut, string buildId=null, string environmentId=null, string environmentName=null, 
                                                        string machineId=null, string machineName=null, string projectId=null, string branchId=null,
                                                        string deployBatchRequestItemId=null, string componentId = null, bool similateRun = false)
        {
            var testData = this.GetCreateTestData(buildId: buildId, environmentId: environmentId, environmentName: environmentName, machineId: machineId, machineName: machineName, projectId: projectId, branchId: branchId, componentId: componentId, deployBatchRequestItemId: deployBatchRequestItemId);
            var returnValue = sut.CreateDeployState(testData.Build, testData.Branch, testData.Environment, testData.Component, testData.MachineList, testData.DeployBatchRequestId);
            if(similateRun)
            {
                SimulateRun(sut, returnValue);
            }
            return returnValue;
        }

        private void SimulateRun(IDeployStateRepository sut, DeployState deployState)
        {
            sut.UpdateDeploymentStatus(deployState.Id, EnumDeployStatus.InProcess);
            var newItem = sut.UpdateDeploymentStatus(deployState.Id, EnumDeployStatus.Success);
            deployState.DeploymentCompleteDateTimeUtc = newItem.DeploymentCompleteDateTimeUtc;
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
            AssertDeployStateSummary(expected, actual);
            AssertMessageList(expected.MessageList, actual.MessageList);
        }

        private void AssertDeployStateSummary(DeployStateSummary expected, DeployStateSummary actual)
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
            Assert.AreEqual(expected.ProjectId, actual.ProjectId);
            AssertDateEqual(expected.SubmittedDateTimeUtc, actual.SubmittedDateTimeUtc);

            Assert.AreEqual(expected.CreatedByUserName, actual.CreatedByUserName);
            AssertDateEqual(expected.CreatedDateTimeUtc, actual.CreatedDateTimeUtc);
            Assert.AreEqual(expected.UpdatedByUserName, actual.UpdatedByUserName);
            AssertDateEqual(expected.UpdatedDateTimeUtc, actual.UpdatedDateTimeUtc);
        }

        private void AssertDeployStateSummaryList(List<DeployState> expectedList, List<DeployStateSummary> actualList)
        {
            Assert.IsNotNull(actualList);
            Assert.AreEqual(expectedList.Count, actualList.Count);
            foreach(var expectedItem in expectedList)
            {
                var actualItem = actualList.SingleOrDefault<DeployStateSummary>(i=>i.Id == expectedItem.Id);
                AssertDeployStateSummary(expectedItem, actualItem);
            }
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

        private CreateTestData GetCreateTestData(string buildId = null, string environmentId = null, string environmentName=null, string machineId = null,
                                                    string machineName = null, string projectId = null, string branchId = null, string componentId = null, string deployBatchRequestItemId=null)
        {
            string projectName = this.Fixture.Create<string>("ProjectName");
            projectId = StringHelper.IsNullOrEmpty(projectId, this.Fixture.Create<string>());
            environmentId = StringHelper.IsNullOrEmpty(environmentId, this.Fixture.Create<string>());
            machineId = StringHelper.IsNullOrEmpty(machineId, this.Fixture.Create<string>());
            machineName = StringHelper.IsNullOrEmpty(machineName, this.Fixture.Create<string>("MachineName"));
            buildId = StringHelper.IsNullOrEmpty(buildId, this.Fixture.Create<string>());
            branchId = StringHelper.IsNullOrEmpty(branchId, this.Fixture.Create<string>());
            componentId = StringHelper.IsNullOrEmpty(componentId, this.Fixture.Create<string>());
            environmentName = StringHelper.IsNullOrEmpty(environmentName, this.Fixture.Create<string>("EnvironmentName"));
            deployBatchRequestItemId = StringHelper.IsNullOrEmpty(deployBatchRequestItemId, this.Fixture.Create<string>());
            var returnValue = new CreateTestData()
            {
                ProjectId = projectId,
                Branch = this.Fixture.Build<DeployProjectBranch>()
                                .With(i=>i.ProjectId, projectId)
                                .With(i=>i.Id, branchId)
                                .Create(),
                Environment = this.Fixture.Build<DeployEnvironment>()
                                .With(i=>i.ProjectId, projectId)
                                .With(i=>i.Id, environmentId)
                                .With(i=>i.EnvironmentName, environmentName)
                                .Create(),
                Component = this.Fixture.Build<DeployComponent>()
                                .With(i=>i.ProjectId, projectId)
                                .With(i=>i.Id, componentId)
                                .Create(),
                DeployBatchRequestId = deployBatchRequestItemId
            };
            returnValue.Environment.ComponentList[0].Id = componentId;
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
            returnValue.Environment.ComponentList[0].MachineList[0].MachineName = machineName;
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

        private void AssertComponentHistoryList(List<DeployState> expectedList, PagedSortedList<ComponentDeployHistory> actualList)
        {
            Assert.IsNotNull(actualList);
            Assert.IsNotNull(actualList.Items);
            Assert.AreEqual(expectedList.Count, actualList.Items.Count);
            Assert.AreEqual(expectedList.Count, actualList.TotalItemCount);
            foreach (var expectedItem in expectedList)
            {
                var actualItem = actualList.Items.SingleOrDefault(i => i.DeployStateId == expectedItem.Id);
                AssertComponentHistory(expectedItem, actualItem);
            }
        }

        private void AssertComponentHistory(DeployState expectedItem, ComponentDeployHistory actualItem)
        {
            Assert.IsNotNull(actualItem);
            Assert.AreEqual(expectedItem.Id, actualItem.DeployStateId);
            Assert.AreEqual(expectedItem.DeployBatchRequestItemId, actualItem.DeployBatchRequestItemId);
            Assert.AreEqual(expectedItem.ProjectId, actualItem.ProjectId);
            Assert.AreEqual(expectedItem.Build.ProjectName, actualItem.ProjectName);
            Assert.AreEqual(expectedItem.Branch.Id, actualItem.ProjectBranchId);
            Assert.AreEqual(expectedItem.Branch.BranchName, actualItem.ProjectBranchName);
            Assert.AreEqual(expectedItem.Component.Id, actualItem.ProjectComponentId);
            Assert.AreEqual(expectedItem.Component.ComponentName, actualItem.ProjectComponentName);

            Assert.AreEqual(expectedItem.Build.Id, actualItem.BuildId);
            Assert.AreEqual(expectedItem.Build.FileId, actualItem.FileId);
            Assert.AreEqual(expectedItem.Build.Version, actualItem.Version);
            Assert.AreEqual(expectedItem.Build.SortableVersion, actualItem.SortableVersion);
            Assert.AreEqual(expectedItem.DeploymentStartedDateTimeUtc, actualItem.DeploymentStartedDateTimeUtc);
            Assert.AreEqual(expectedItem.DeploymentCompleteDateTimeUtc, actualItem.DeploymentCompleteDateTimeUtc);
            Assert.AreEqual(expectedItem.ErrorDetails, actualItem.ErrorDetails);

            Assert.AreEqual(expectedItem.Environment.Id, actualItem.EnvironmentId);
            Assert.AreEqual(expectedItem.Environment.EnvironmentName, actualItem.EnvironmentName);
            AssertHelpers.AssertMachineList(expectedItem.MachineList, actualItem.MachineList);
        }

        [Test]
        public void CreateDeployment_CreatesDeployment()
        {
            var sut = this.GetRepository();

            var testData = this.GetCreateTestData();

            var result = sut.CreateDeployState(testData.Build, testData.Branch, testData.Environment, testData.Component, testData.MachineList, testData.DeployBatchRequestId);

            AssertCreatedDeployState(sut, result, testData);
        }

        [Test]
        public void CreateDeployment_MissingBuild_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            var testData = this.GetCreateTestData();

            Assert.Throws<ArgumentNullException>(() => sut.CreateDeployState(null, testData.Branch, testData.Environment, testData.Component, testData.MachineList, testData.DeployBatchRequestId));
        }

        [Test]
        public void CreateDeployment_MissingBranch_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            var testData = this.GetCreateTestData();

            Assert.Throws<ArgumentNullException>(() => sut.CreateDeployState(testData.Build, null, testData.Environment, testData.Component, testData.MachineList, testData.DeployBatchRequestId));
        }

        [Test]
        public void CreateDeployment_MissingEnvironment_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            var testData = this.GetCreateTestData();

            Assert.Throws<ArgumentNullException>(() => sut.CreateDeployState(testData.Build, testData.Branch, null, testData.Component, testData.MachineList, testData.DeployBatchRequestId));
        }

        [Test]
        public void CreateDeployment_MissingComponent_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            var testData = this.GetCreateTestData();

            Assert.Throws<ArgumentNullException>(() => sut.CreateDeployState(testData.Build, testData.Branch, testData.Environment, null, testData.MachineList, testData.DeployBatchRequestId));
        }

        [Test]
        public void CreateDeployment_MissingMachineList_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            var testData = this.GetCreateTestData();

            Assert.Throws<ArgumentNullException>(() => sut.CreateDeployState(testData.Build, testData.Branch, testData.Environment, testData.Component, null, testData.DeployBatchRequestId));
        }

        [Test]
        public void CreateDeployment_MissingDeployBatchRequestItemID_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            var testData = this.GetCreateTestData();

            Assert.Throws<ArgumentNullException>(() => sut.CreateDeployState(testData.Build, testData.Branch, testData.Environment, testData.Component, testData.MachineList, null));
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
        public void UpdateDeploymentStatus_Success_NullCompleteDate_UpdatesDeployCompleteDate()
        {
            var sut = this.GetRepository();

            var deployState = this.CreateTestDeployState(sut);
            var newStatus = EnumDeployStatus.Success;
            //string newUserName = this.Fixture.Create<string>("UserName");
            //this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            var result = sut.UpdateDeploymentStatus(deployState.Id, newStatus);

            Assert.IsNotNull(result);
            Assert.AreEqual(result.Status, newStatus);
            Assert.IsNotNull(result.DeploymentCompleteDateTimeUtc);
            AssertIsRecent(result.DeploymentCompleteDateTimeUtc.Value);
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
            var resultMessage = result.MessageList.First();

            Assert.IsNotNull(resultMessage);
            Assert.IsNotNullOrEmpty(resultMessage.Id);
            Assert.AreEqual(deployState.Id, resultMessage.DeployStateId);
            Assert.AreEqual(message, resultMessage.Message);
            Assert.AreEqual(newUserName, resultMessage.MessageUserName);
            AssertIsRecent(resultMessage.DateTimeUtc);

            var dbState = sut.GetDeployState(deployState.Id);
            Assert.IsNotNull(dbState.MessageList);
            Assert.AreEqual(1, dbState.MessageList.Count);
            AssertMessage(resultMessage, dbState.MessageList[0]);
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
            var firstMessage = sut.AddDeploymentMessage(deployState.Id, Guid.NewGuid().ToString()).MessageList.First();
            string message = this.Fixture.Create<string>("Message");
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            var result = sut.AddDeploymentMessage(deployState.Id, message);
            var resultMessage = result.MessageList.Skip(1).First();

            Assert.IsNotNull(result);
            Assert.IsNotNullOrEmpty(result.Id);
            Assert.AreEqual(deployState.Id, resultMessage.DeployStateId);
            Assert.AreEqual(message, resultMessage.Message);
            Assert.AreEqual(newUserName, resultMessage.MessageUserName);
            AssertIsRecent(resultMessage.DateTimeUtc);

            var dbState = sut.GetDeployState(deployState.Id);
            Assert.IsNotNull(dbState.MessageList);
            Assert.AreEqual(2, dbState.MessageList.Count);
            Assert.AreEqual(deployState.CreatedByUserName, dbState.CreatedByUserName);
            AssertDateEqual(deployState.CreatedDateTimeUtc, dbState.CreatedDateTimeUtc);
            Assert.AreEqual(newUserName, dbState.UpdatedByUserName);
            AssertIsRecent(deployState.UpdatedDateTimeUtc);
            var expectedList = new List<DeployStateMessage> { firstMessage, resultMessage };
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

        [Test]
        public void GetComponentDeployHistory_GetsHistoryList()
        {
            var sut = this.GetRepository();

            var deployStateList = new List<DeployState>();
            for (int i = 0; i < 5; i++)
            {
                var deployState = this.CreateTestDeployState(sut);
                deployStateList.Add(deployState);
            }

            var result = sut.GetComponentDeployHistory(null);

            Assert.IsNotNull(result);
            Assert.AreNotEqual(0, result.Items.Count);
            Assert.AreNotEqual(0, result.TotalItemCount);
        }

        [Test]
        public void GetComponentDeployHistory_Defaults()
        {
            var sut = this.GetRepository();

            var deployStateList = new List<DeployState>();
            for (int i = 0; i < 5; i++)
            {
                var deployState = this.CreateTestDeployState(sut);
                deployStateList.Add(deployState);
            }

            var result = sut.GetComponentDeployHistory(null);

            Assert.IsNotNull(result);
            Assert.AreEqual(20, result.PageSize);
            Assert.AreEqual(1, result.PageNumber);
            Assert.AreEqual("DeploymentStartedDateTimeUtc", result.SortField);
            Assert.IsFalse(result.SortAscending);
        }

        [Test]
        public void GetComponentDeployHistory_PageSize()
        {
            var sut = this.GetRepository();

            for (int i = 0; i < 30; i++)
            {
                this.CreateTestDeployState(sut);
            }

            var result = sut.GetComponentDeployHistory(new ListOptions { PageSize = 5 });

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Items);
            Assert.AreEqual(5, result.Items.Count);
            Assert.AreEqual(5, result.PageSize);
            Assert.IsTrue(result.HasNextPage);
            Assert.IsFalse(result.HasPreviousPage);
            Assert.IsTrue(result.IsFirstPage);
            Assert.IsFalse(result.IsLastPage);
            Assert.LessOrEqual(6, result.PageCount);
            Assert.AreEqual(1, result.PageNumber);
            Assert.IsFalse(result.SortAscending);
            Assert.AreEqual("DeploymentStartedDateTimeUtc", result.SortField);
            Assert.LessOrEqual(30, result.TotalItemCount);
        }

        [Test]
        public void GetComponentDeployHistory_PageNumber()
        {
            var sut = this.GetRepository();

            for (int i = 0; i < 30; i++)
            {
                this.CreateTestDeployState(sut, similateRun:true);
            }

            var result = sut.GetComponentDeployHistory(new ListOptions { PageNumber = 2 });

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Items);
            Assert.LessOrEqual(10, result.Items.Count);
            Assert.AreEqual(20, result.PageSize);
            //Assert.IsTrue(buildList.HasNextPage);
            Assert.IsTrue(result.HasPreviousPage);
            //Assert.IsTrue(buildList.IsFirstPage);
            //Assert.IsFalse(buildList.IsLastPage);
            Assert.LessOrEqual(2, result.PageCount);
            Assert.AreEqual(2, result.PageNumber);
            Assert.IsFalse(result.SortAscending);
            Assert.AreEqual("DeploymentStartedDateTimeUtc", result.SortField);
            Assert.LessOrEqual(30, result.TotalItemCount);
        }

        [Test]
        public void GetComponentDeployHistory_DeploymentStartedDateTimeUtcAsc()
        {
            var sut = this.GetRepository();

            for (int i = 0; i < 30; i++)
            {
                this.CreateTestDeployState(sut, similateRun: true);
            }

            var result = sut.GetComponentDeployHistory(new ListOptions { SortField = "DeploymentStartedDateTimeUtc", SortAscending = true });

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Items);
            Assert.IsTrue(result.SortAscending);
            ComponentDeployHistory lastItem = null;
            foreach (var item in result.Items)
            {
                if (lastItem != null)
                {
                    Assert.LessOrEqual(lastItem.DeploymentStartedDateTimeUtc.GetValueOrDefault(), item.DeploymentStartedDateTimeUtc.GetValueOrDefault());
                }
                lastItem = item;
            }
        }

        [Test]
        public void GetComponentDeployHistory_DeploymentStartedDateTimeUtcDesc()
        {
            var sut = this.GetRepository();

            for (int i = 0; i < 30; i++)
            {
                this.CreateTestDeployState(sut, similateRun: true);
            }

            var result = sut.GetComponentDeployHistory(new ListOptions { SortField = "DeploymentStartedDateTimeUtc", SortAscending = false });

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Items);
            Assert.IsFalse(result.SortAscending);
            ComponentDeployHistory lastItem = null;
            foreach (var item in result.Items)
            {
                if (lastItem != null)
                {
                    Assert.GreaterOrEqual(lastItem.DeploymentStartedDateTimeUtc.GetValueOrDefault(), item.DeploymentStartedDateTimeUtc.GetValueOrDefault());
                }
                lastItem = item;
            }
        }

        [Test]
        public void GetComponentDeployHistory_VersionAsc()
        {
            var sut = this.GetRepository();

            for (int i = 0; i < 30; i++)
            {
                this.CreateTestDeployState(sut, similateRun: true);
            }

            //var result = sut.GetComponentDeployHistory(new ListOptions { SortField = "Version", SortAscending = false });
            var result = sut.GetComponentDeployHistory(new ListOptions { SortField = "Version" });
            //var result = sut.GetComponentDeployHistory(new ListOptions { SortField = "DeploymentStartedDateTimeUtc", SortAscending = false });


            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Items);
            Assert.GreaterOrEqual(result.Items.Count, 20);
            Assert.IsTrue(result.SortAscending);
            ComponentDeployHistory lastItem = null;
            foreach (var item in result.Items)
            {
                if (lastItem != null)
                {
                    Assert.LessOrEqual(lastItem.SortableVersion, item.SortableVersion);
                }
                lastItem = item;
            }
        }

        [Test]
        public void GetComponentDeployHistory_VersionDesc()
        {
            var sut = this.GetRepository();

            for (int i = 0; i < 30; i++)
            {
                this.CreateTestDeployState(sut, similateRun: true);
            }

            var result = sut.GetComponentDeployHistory(new ListOptions { SortField = "Version", SortAscending = false });

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Items);
            Assert.IsFalse(result.SortAscending);
            ComponentDeployHistory lastItem = null;
            foreach (var item in result.Items)
            {
                if (lastItem != null)
                {
                    Assert.GreaterOrEqual(lastItem.SortableVersion, item.SortableVersion);
                }
                lastItem = item;
            }
        }

        [Test]
        public void GetComponentDeployHistory_ProjectID_GetsHistoryList()
        {
            var sut = this.GetRepository();

            string projectId = this.Fixture.Create<string>();
            var deployStateList = new List<DeployState>();
            for (int i = 0; i < 5; i++)
            {
                var deployState = this.CreateTestDeployState(sut, projectId: projectId);
                deployStateList.Add(deployState);
            }

            var result = sut.GetComponentDeployHistory(null, projectIdList: projectId.ListMe());

            AssertComponentHistoryList(deployStateList, result);
        }

        [Test]
        public void GetComponentDeployHistory_TwoProjectIDs_GetsHistoryList()
        {
            var sut = this.GetRepository();

            var deployStateList = new List<DeployState>();

            string projectId1 = this.Fixture.Create<string>();
            for (int i = 0; i < 5; i++)
            {
                var deployState = this.CreateTestDeployState(sut, projectId: projectId1);
                deployStateList.Add(deployState);
            }
            string projectId2 = this.Fixture.Create<string>();
            for (int i = 0; i < 5; i++)
            {
                var deployState = this.CreateTestDeployState(sut, projectId: projectId2);
                deployStateList.Add(deployState);
            }

            var projectIdList = new List<string> { projectId1, projectId2 };
            var result = sut.GetComponentDeployHistory(null, projectIdList: projectIdList);

            AssertComponentHistoryList(deployStateList, result);
        }

        [Test]
        public void GetComponentDeployHistory_BranchID_GetsHistoryList()
        {
            var sut = this.GetRepository();

            string branchId = this.Fixture.Create<string>();
            var deployStateList = new List<DeployState>();
            for (int i = 0; i < 5; i++)
            {
                var deployState = this.CreateTestDeployState(sut, branchId: branchId);
                deployStateList.Add(deployState);
            }

            var result = sut.GetComponentDeployHistory(null, branchIdList: branchId.ListMe());

            AssertComponentHistoryList(deployStateList, result);
        }


        [Test]
        public void GetComponentDeployHistory_TwoBranchIDs_GetsHistoryList()
        {
            var sut = this.GetRepository();

            var deployStateList = new List<DeployState>();

            string branchId1 = this.Fixture.Create<string>();
            for (int i = 0; i < 5; i++)
            {
                var deployState = this.CreateTestDeployState(sut, branchId: branchId1);
                deployStateList.Add(deployState);
            }
            string branchId2 = this.Fixture.Create<string>();
            for (int i = 0; i < 5; i++)
            {
                var deployState = this.CreateTestDeployState(sut, branchId: branchId2);
                deployStateList.Add(deployState);
            }

            var branchIdList = new List<string> { branchId1, branchId2 };
            var result = sut.GetComponentDeployHistory(null, branchIdList: branchIdList);

            AssertComponentHistoryList(deployStateList, result);
        }

        [Test]
        public void GetComponentDeployHistory_ComponentID_GetsHistoryList()
        {
            var sut = this.GetRepository();

            string componentId = this.Fixture.Create<string>();
            var deployStateList = new List<DeployState>();
            for (int i = 0; i < 5; i++)
            {
                var deployState = this.CreateTestDeployState(sut, componentId: componentId);
                deployStateList.Add(deployState);
            }

            var result = sut.GetComponentDeployHistory(null,  componentIdList: componentId.ListMe());

            AssertComponentHistoryList(deployStateList, result);
        }

        [Test]
        public void GetComponentDeployHistory_TwoComponentIDs_GetsHistoryList()
        {
            var sut = this.GetRepository();

            var deployStateList = new List<DeployState>();

            string componentId1 = this.Fixture.Create<string>();
            for (int i = 0; i < 5; i++)
            {
                var deployState = this.CreateTestDeployState(sut, componentId: componentId1);
                deployStateList.Add(deployState);
            }
            string componentId2 = this.Fixture.Create<string>();
            for (int i = 0; i < 5; i++)
            {
                var deployState = this.CreateTestDeployState(sut, componentId: componentId2);
                deployStateList.Add(deployState);
            }

            var componentIdList = new List<string> { componentId1, componentId2 };
            var result = sut.GetComponentDeployHistory(null, componentIdList: componentIdList);

            AssertComponentHistoryList(deployStateList, result);
        }

        [Test]
        public void GetComponentDeployHistory_BuildID_GetsHistoryList()
        {
            var sut = this.GetRepository();

            string buildId = this.Fixture.Create<string>();
            var deployStateList = new List<DeployState>();
            for (int i = 0; i < 5; i++)
            {
                var deployState = this.CreateTestDeployState(sut, buildId: buildId);
                deployStateList.Add(deployState);
            }

            var result = sut.GetComponentDeployHistory(null, buildIdList: buildId.ListMe());

            AssertComponentHistoryList(deployStateList, result);
        }

        [Test]
        public void GetComponentDeployHistory_TwoBuildIDs_GetsHistoryList()
        {
            var sut = this.GetRepository();

            var deployStateList = new List<DeployState>();

            string buildIdId1 = this.Fixture.Create<string>();
            for (int i = 0; i < 5; i++)
            {
                var deployState = this.CreateTestDeployState(sut, buildId: buildIdId1);
                deployStateList.Add(deployState);
            }
            string buildId2 = this.Fixture.Create<string>();
            for (int i = 0; i < 5; i++)
            {
                var deployState = this.CreateTestDeployState(sut, buildId: buildId2);
                deployStateList.Add(deployState);
            }

            var buildIdList = new List<string> { buildIdId1, buildId2 };
            var result = sut.GetComponentDeployHistory(null, buildIdList: buildIdList);

            AssertComponentHistoryList(deployStateList, result);
        }

        [Test]
        public void GetComponentDeployHistory_EnvironmentID_GetsHistoryList()
        {
            var sut = this.GetRepository();

            string environmentId = this.Fixture.Create<string>();
            var deployStateList = new List<DeployState>();
            for (int i = 0; i < 5; i++)
            {
                var deployState = this.CreateTestDeployState(sut, environmentId: environmentId);
                deployStateList.Add(deployState);
            }

            var result = sut.GetComponentDeployHistory(null, environmentIdList: environmentId.ListMe());

            AssertComponentHistoryList(deployStateList, result);
        }

        [Test]
        public void GetComponentDeployHistory_TwoEnvironmentIDs_GetsHistoryList()
        {
            var sut = this.GetRepository();

            var deployStateList = new List<DeployState>();

            string environmentId1 = this.Fixture.Create<string>();
            for (int i = 0; i < 5; i++)
            {
                var deployState = this.CreateTestDeployState(sut, environmentId: environmentId1);
                deployStateList.Add(deployState);
            }
            string environmentId2 = this.Fixture.Create<string>();
            for (int i = 0; i < 5; i++)
            {
                var deployState = this.CreateTestDeployState(sut, environmentId: environmentId2);
                deployStateList.Add(deployState);
            }

            var environmentIdList = new List<string> { environmentId1, environmentId2 };
            var result = sut.GetComponentDeployHistory(null, environmentIdList: environmentIdList);

            AssertComponentHistoryList(deployStateList, result);
        }

        [Test]
        public void GetComponentDeployHistory_EnvironmentName_GetsHistoryList()
        {
            var sut = this.GetRepository();

            string environmentName = this.Fixture.Create<string>("EnvironmentName");
            var deployStateList = new List<DeployState>();
            for (int i = 0; i < 5; i++)
            {
                var deployState = this.CreateTestDeployState(sut, environmentName: environmentName);
                deployStateList.Add(deployState);
            }

            var result = sut.GetComponentDeployHistory(null, environmentNameList: environmentName.ListMe());

            AssertComponentHistoryList(deployStateList, result);
        }

        [Test]
        public void GetComponentDeployHistory_TwoEnvironmentNames_GetsHistoryList()
        {
            var sut = this.GetRepository();

            var deployStateList = new List<DeployState>();

            string environmentName1 = this.Fixture.Create<string>("EnvironmentName");
            for (int i = 0; i < 5; i++)
            {
                var deployState = this.CreateTestDeployState(sut, environmentName: environmentName1);
                deployStateList.Add(deployState);
            }
            string environmentName2 = this.Fixture.Create<string>();
            for (int i = 0; i < 5; i++)
            {
                var deployState = this.CreateTestDeployState(sut, environmentName: environmentName2);
                deployStateList.Add(deployState);
            }

            var environmentNameList = new List<string> { environmentName1, environmentName2 };
            var result = sut.GetComponentDeployHistory(null, environmentNameList: environmentNameList);

            AssertComponentHistoryList(deployStateList, result);
        }

        [Test]
        public void GetComponentDeployHistory_MachineID_GetsHistoryList()
        {
            var sut = this.GetRepository();

            string machineId = this.Fixture.Create<string>();
            var deployStateList = new List<DeployState>();
            for (int i = 0; i < 5; i++)
            {
                var deployState = this.CreateTestDeployState(sut, machineId: machineId);
                deployStateList.Add(deployState);
            }

            var result = sut.GetComponentDeployHistory(null, machineIdList: machineId.ListMe());

            AssertComponentHistoryList(deployStateList, result);
        }

        [Test]
        public void GetComponentDeployHistory_TwoMachineIDs_GetsHistoryList()
        {
            var sut = this.GetRepository();

            var deployStateList = new List<DeployState>();

            string machineId1 = this.Fixture.Create<string>();
            for (int i = 0; i < 5; i++)
            {
                var deployState = this.CreateTestDeployState(sut, machineId: machineId1);
                deployStateList.Add(deployState);
            }
            string machineId2 = this.Fixture.Create<string>();
            for (int i = 0; i < 5; i++)
            {
                var deployState = this.CreateTestDeployState(sut, machineId: machineId2);
                deployStateList.Add(deployState);
            }

            var machineIdList = new List<string> { machineId1, machineId2 };
            var result = sut.GetComponentDeployHistory(null, machineIdList: machineIdList);

            AssertComponentHistoryList(deployStateList, result);
        }

        [Test]
        public void GetComponentDeployHistory_MachineName_GetsHistoryList()
        {
            var sut = this.GetRepository();

            string machineName = this.Fixture.Create<string>("MachineName");
            var deployStateList = new List<DeployState>();
            for (int i = 0; i < 5; i++)
            {
                var deployState = this.CreateTestDeployState(sut, machineName: machineName);
                deployStateList.Add(deployState);
            }

            var result = sut.GetComponentDeployHistory(null, machineNameList: machineName.ListMe());

            AssertComponentHistoryList(deployStateList, result);
        }

        [Test]
        public void GetComponentDeployHistory_TwoMachineNames_GetsHistoryList()
        {
            var sut = this.GetRepository();

            var deployStateList = new List<DeployState>();

            string machineName1 = this.Fixture.Create<string>("MachineName");
            for (int i = 0; i < 5; i++)
            {
                var deployState = this.CreateTestDeployState(sut, machineName: machineName1);
                deployStateList.Add(deployState);
            }
            string machineName2 = this.Fixture.Create<string>();
            for (int i = 0; i < 5; i++)
            {
                var deployState = this.CreateTestDeployState(sut, machineName: machineName2);
                deployStateList.Add(deployState);
            }

            var machineNameList = new List<string> { machineName1, machineName2 };
            var result = sut.GetComponentDeployHistory(null, machineNameList: machineNameList);

            AssertComponentHistoryList(deployStateList, result);
        }

        [Test]
        public void GetComponentDeployHistory_ProjectAndStatus_GetsHistoryList()
        {
            var sut = this.GetRepository();

            string projectId = this.Fixture.Create<string>();
            var deployStateList = new List<DeployState>();
            for (int i = 0; i < 5; i++)
            {
                var deployState = this.CreateTestDeployState(sut, projectId: projectId, similateRun:true);
                deployStateList.Add(deployState);
            }

            var result = sut.GetComponentDeployHistory(null, projectIdList:projectId.ListMe(), statusList: EnumDeployStatus.Success.ToString().ListMe());

            AssertComponentHistoryList(deployStateList, result);
        }

        [Test]
        public void GetComponentDeployHistory_ProjectAndTwoStatuses_GetsHistoryList()
        {
            var sut = this.GetRepository();

            var deployStateList = new List<DeployState>();

            string projectId = this.Fixture.Create<string>();
            for (int i = 0; i < 5; i++)
            {
                var deployState = this.CreateTestDeployState(sut, projectId:projectId, similateRun:true);
                deployStateList.Add(deployState);
            }
            for (int i = 0; i < 5; i++)
            {
                var deployState = this.CreateTestDeployState(sut, projectId:projectId, similateRun:true);
                deployStateList.Add(deployState);
            }

            var statusList = new List<string> { EnumDeployStatus.NotStarted.ToString(), EnumDeployStatus.Success.ToString() };
            var result = sut.GetComponentDeployHistory(null, projectIdList:projectId.ListMe(), statusList:statusList);

            AssertComponentHistoryList(deployStateList, result);
        }

        [Test]
        public void GetDeployStateSummaryListByDeployBatchRequestItemId_GetsHistoryList()
        {
            var sut = this.GetRepository();

            var deployStateList = new List<DeployState>();
            string deployBatchRequestItemId = this.Fixture.Create<string>();
            for(int i = 0; i < 5; i++)
            {
                var deployState = this.CreateTestDeployState(sut, deployBatchRequestItemId: deployBatchRequestItemId);
                deployStateList.Add(deployState);
            }
            
            var result = sut.GetDeployStateSummaryListByDeployBatchRequestItemId(deployBatchRequestItemId);

            Assert.IsNotNull(result);
            Assert.AreEqual(deployStateList.Count, result.Count);
            AssertDeployStateSummaryList(deployStateList, result);
        }

        [Test]
        public void GetDeployStateSummaryListByDeployBatchRequestItemId_MissingDeployBatchRequestItemId_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            Assert.Throws<ArgumentNullException>(()=>sut.GetDeployStateSummaryListByDeployBatchRequestItemId(null));
        }

        [Test]
        public void GetDeployStateSummaryListByDeployBatchRequestItemId_BadDeployBatchRequestItemId_ReturnsEmptyList()
        {
            var sut = this.GetRepository();

            var result = sut.GetDeployStateSummaryListByDeployBatchRequestItemId(Guid.NewGuid().ToString());

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

    }
}
