using NUnit.Framework;
using Sriracha.Deploy.Data.Dto.Deployment;
using Sriracha.Deploy.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ploeh.AutoFixture;
using Sriracha.Deploy.Data.Dto;
using MMDB.Shared;

namespace Sriracha.Deploy.Data.Tests.Repository.Deploy
{
    public abstract class DeployRepositoryTests : RepositoryTestBase<IDeployRepository>
    {
        private class TestData
        {
            public List<DeployBatchRequest> DeployList { get; set; }
            public IDeployRepository Sut { get; set; }

            public static TestData Create(DeployRepositoryTests tester, int existingCount=0)
            {
                var testData = new TestData
                {
                    DeployList = new List<DeployBatchRequest>(),
                    Sut = tester.GetRepository()
                };
                for(var i = 0; i < existingCount; i++)
                {
                    var data = tester.Fixture.Create<DeployBatchRequest>();
                    var item = testData.Sut.CreateBatchRequest(data.ItemList, data.Status, data.DeploymentLabel);
                    testData.DeployList.Add(item);
                }
                return testData;
            }
        }

        private void AssertRequest(DeployBatchRequest expected, DeployBatchRequest actual)
        {
            if (expected == null)
            {
                Assert.IsNull(actual);
            }
            else
            {
                Assert.IsNotNull(actual);
                Assert.AreEqual(expected.Id, actual.Id);
                AssertDateEqual(expected.SubmittedDateTimeUtc, actual.SubmittedDateTimeUtc);
                Assert.AreEqual(expected.SubmittedByUserName, actual.SubmittedByUserName);
                AssertItemList(expected.ItemList, actual.ItemList);
                Assert.AreEqual(expected.Status, actual.Status);
                AssertDateEqual(expected.StartedDateTimeUtc, actual.StartedDateTimeUtc);
                AssertDateEqual(expected.CompleteDateTimeUtc, actual.CompleteDateTimeUtc);
                Assert.AreEqual(expected.ErrorDetails, actual.ErrorDetails);
                AssertDateEqual(expected.CreatedDateTimeUtc, actual.CreatedDateTimeUtc);
                Assert.AreEqual(expected.CreatedByUserName, actual.CreatedByUserName);
                AssertDateEqual(expected.UpdatedDateTimeUtc, actual.UpdatedDateTimeUtc);
                Assert.AreEqual(expected.UpdatedByUserName, actual.UpdatedByUserName);
                Assert.AreEqual(expected.LastStatusMessage, actual.LastStatusMessage);
                Assert.AreEqual(expected.DeploymentLabel, actual.DeploymentLabel);
                Assert.AreEqual(expected.CancelRequested, actual.CancelRequested);
                Assert.AreEqual(expected.CancelMessage, actual.CancelMessage);
                AssertHelpers.AssertStringList(expected.MessageList, actual.MessageList);
                Assert.AreEqual(expected.ResumeRequested, actual.ResumeRequested);
                Assert.AreEqual(expected.ResumeMessage, actual.ResumeMessage);
            }
        }

        private void AssertCreatedRequest(DeployBatchRequest expected, DeployBatchRequest actual)
        {
            if (expected == null)
            {
                Assert.IsNull(actual);
            }
            else
            {
                if(expected.Status == EnumDeployStatus.Unknown)
                {
                    expected.Status = EnumDeployStatus.NotStarted;
                }
                Assert.IsNotNull(actual);
                Assert.IsNotNullOrEmpty(actual.Id);
                AssertIsRecent(actual.SubmittedDateTimeUtc);
                Assert.AreEqual(this.UserName, actual.SubmittedByUserName);
                AssertItemList(expected.ItemList, actual.ItemList);
                Assert.AreEqual(expected.Status, actual.Status);
                Assert.IsNull(actual.StartedDateTimeUtc);
                Assert.IsNull(actual.CompleteDateTimeUtc);
                Assert.IsNullOrEmpty(actual.ErrorDetails);
                AssertIsRecent(actual.CreatedDateTimeUtc);
                Assert.AreEqual(this.UserName, actual.CreatedByUserName);
                AssertIsRecent(actual.UpdatedDateTimeUtc);
                Assert.AreEqual(this.UserName, actual.UpdatedByUserName);
                Assert.IsNotNullOrEmpty(actual.LastStatusMessage);
                Assert.AreEqual(expected.DeploymentLabel, actual.DeploymentLabel);
                Assert.IsFalse(actual.CancelRequested);
                Assert.IsNullOrEmpty(actual.CancelMessage);
                Assert.AreEqual(1, actual.MessageList.Count);
                Assert.AreEqual(actual.LastStatusMessage, actual.MessageList[0]);
                Assert.IsFalse(actual.ResumeRequested);
                Assert.IsNullOrEmpty(actual.ResumeMessage);
            }
        }

        private void AssertItemList(List<DeployBatchRequestItem> expectedList, List<DeployBatchRequestItem> actualList)
        {
            if (expectedList == null)
            {
                Assert.IsNull(actualList);
            }
            else
            {
                Assert.IsNotNull(actualList);
                Assert.AreEqual(expectedList.Count, actualList.Count);
                foreach (var expectedItem in expectedList)
                {
                    var actualItem = actualList.FirstOrDefault(i => i.Id == expectedItem.Id);
                    Assert.IsNotNull(actualItem);
                    Assert.AreEqual(expectedItem.Id, actualItem.Id);
                    AssertHelpers.AssertBuild(expectedItem.Build, actualItem.Build);
                    AssertHelpers.AssertMachineList(expectedItem.MachineList, actualItem.MachineList);
                }
            }
        }

        [Test]
        public void GetBatchRequestList_GetsList()
        {
            var testData = TestData.Create(this, 20);

            var result = testData.Sut.GetBatchRequestList(null);

            Assert.IsNotNull(result);
            Assert.Greater(result.Items.Count, 0);
        }

        [Test]
        public void GetBatchRequestList_DefaultsSubmittedDateDescAndTenItems()
        {
            var testData = TestData.Create(this, 20);

            var result = testData.Sut.GetBatchRequestList(null);

            Assert.IsNotNull(result);
            Assert.AreEqual(false, result.SortAscending);
            Assert.AreEqual("SubmittedDateTimeUtc", result.SortField);
            Assert.AreEqual(result.Items.Count, 10);
            
            DateTime? lastDate = null;
            foreach(var item in result.Items)
            {
                if(!lastDate.HasValue)
                {
                    lastDate = item.SubmittedDateTimeUtc;
                }
                else 
                {
                    Assert.GreaterOrEqual(lastDate, item.SubmittedDateTimeUtc);
                    lastDate = item.SubmittedDateTimeUtc;
                }
            }
        }

        [Test]
        public void GetBatchRequestList_SortSubmittedDateAscAnd15Items()
        {
            var testData = TestData.Create(this, 15);

            var result = testData.Sut.GetBatchRequestList(new ListOptions { SortField="SubmittedDateTimeUtc", SortAscending=true, PageSize=15 });

            Assert.IsNotNull(result);
            Assert.AreEqual(true, result.SortAscending);
            Assert.AreEqual("SubmittedDateTimeUtc", result.SortField);
            Assert.AreEqual(result.Items.Count, 15);

            DateTime? lastDate = null;
            foreach (var item in result.Items)
            {
                if (!lastDate.HasValue)
                {
                    lastDate = item.SubmittedDateTimeUtc;
                }
                else
                {
                    Assert.LessOrEqual(lastDate, item.SubmittedDateTimeUtc);
                    lastDate = item.SubmittedDateTimeUtc;
                }
            }
        }

        [Test]
        public void GetBatchRequestList_SortDeploymentLabelDescAnd5Items()
        {
            var testData = TestData.Create(this, 20);

            var result = testData.Sut.GetBatchRequestList(new ListOptions { SortField = "DeploymentLabel", SortAscending = false, PageSize = 5 });

            Assert.IsNotNull(result);
            Assert.AreEqual(false, result.SortAscending);
            Assert.AreEqual("DeploymentLabel", result.SortField);
            Assert.AreEqual(result.Items.Count, 5);

            string lastLabel = null;
            foreach (var item in result.Items)
            {
                if (string.IsNullOrEmpty(lastLabel))
                {
                    lastLabel = item.DeploymentLabel;
                }
                else
                {
                    Assert.GreaterOrEqual(lastLabel, item.DeploymentLabel);
                    lastLabel = item.DeploymentLabel;
                }
            }
        }

        [Test]
        public void CreateBatchRequest_CreatesRequest()
        {
            var testData = TestData.Create(this);
            var data = this.Fixture.Create<DeployBatchRequest>();

            var result = testData.Sut.CreateBatchRequest(data.ItemList, data.Status, data.DeploymentLabel);

            AssertCreatedRequest(data, result);
            var dbItem = testData.Sut.GetBatchRequest(result.Id);
            AssertRequest(result, dbItem);
        }

        [Test]
        public void CreateBatchRequest_MissingItemList_ThrowsArgumentNullException()
        {
            var testData = TestData.Create(this);
            var data = this.Fixture.Create<DeployBatchRequest>();

            Assert.Throws<ArgumentNullException>(()=>testData.Sut.CreateBatchRequest(null, data.Status, data.DeploymentLabel));
        }

        [Test]
        public void CreateBatchRequest_EmptyItemList_ThrowsArgumentNullException()
        {
            var testData = TestData.Create(this);
            var data = this.Fixture.Create<DeployBatchRequest>();

            Assert.Throws<ArgumentNullException>(()=>testData.Sut.CreateBatchRequest(new List<DeployBatchRequestItem>(), data.Status, data.DeploymentLabel));
        }

        [Test]
        public void CreateBatchRequest_MissingItemIDs_GeneratesIDs()
        {
            var testData = TestData.Create(this);
            var data = this.Fixture.Create<DeployBatchRequest>();
            foreach(var item in data.ItemList)
            {
                item.Id = null;
            }

            var result = testData.Sut.CreateBatchRequest(data.ItemList, data.Status, data.DeploymentLabel);
            
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.ItemList);
            foreach(var item in result.ItemList)
            {
                Assert.IsNotNullOrEmpty(item.Id);
            }
        }

        [Test]
        public void PopNextBatchDeployment_PopsDeployment()
        {
            var testData = TestData.Create(this);
            var data = this.Fixture.Create<DeployBatchRequest>();
            var existingItem = testData.Sut.CreateBatchRequest(data.ItemList, EnumDeployStatus.NotStarted, data.DeploymentLabel);
            var newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i=>i.UserName).Returns(newUserName);

            var result = testData.Sut.PopNextBatchDeployment();

            Assert.IsNotNull(result);
            Assert.AreEqual(EnumDeployStatus.InProcess, result.Status);
            AssertIsRecent(result.StartedDateTimeUtc);
            //AssertDateEqual(existingItem.CreatedDateTimeUtc, result.CreatedDateTimeUtc);
            //Assert.AreEqual(existingItem.CreatedByUserName, result.CreatedByUserName);
            Assert.AreEqual(newUserName, result.UpdatedByUserName);
            AssertIsRecent(result.UpdatedDateTimeUtc);
        }

        [Test, Explicit]
        public void PopNextBatchDeployment_NoDeployments_ReturnsNothing()
        {
            var testData = TestData.Create(this);

            var result = testData.Sut.PopNextBatchDeployment();

            Assert.IsNull(result);
        }

        [Test]
        public void GetBatchRequest_GetsBatchRequest()
        {
            var testData = TestData.Create(this, 1);

            var result = testData.Sut.GetBatchRequest(testData.DeployList[0].Id);
            
            AssertRequest(testData.DeployList[0], result);
        }

        [Test]
        public void GetBatchRequest_MissingID_ThrowsArgumentNullException()
        {
            var testData = TestData.Create(this);

            Assert.Throws<ArgumentNullException>(() => testData.Sut.GetBatchRequest(null));
        }

        [Test]
        public void GetBatchRequest_BadID_ThrowsRecordNotFoundException()
        {
            var testData = TestData.Create(this);

            Assert.Throws<RecordNotFoundException>(() => testData.Sut.GetBatchRequest(Guid.NewGuid().ToString()));
        }

        [Test]
        public void UpdateBatchDeploymentStatus_UpdatesStatus()
        {
            var testData = TestData.Create(this, 1);
            var data = this.Fixture.Create<DeployBatchRequest>();
            data.Id = testData.DeployList[0].Id;
            data.ItemList = testData.DeployList[0].ItemList;

            var result = testData.Sut.UpdateBatchDeploymentStatus(data.Id, data.Status);

            Assert.IsNotNull(result);
            Assert.AreEqual(data.Id, result.Id);
            Assert.AreEqual(data.Status, result.Status);
            var dbItem = testData.Sut.GetBatchRequest(result.Id);
            AssertRequest(result, dbItem);
        }

        [Test]
        public void UpdateBatchDeploymentStatus_Success_SetsCompleteDate()
        {
            var testData = TestData.Create(this, 1);
            var data = this.Fixture.Create<DeployBatchRequest>();
            data.Id = testData.DeployList[0].Id;
            data.ItemList = testData.DeployList[0].ItemList;

            var result = testData.Sut.UpdateBatchDeploymentStatus(data.Id, EnumDeployStatus.Success);

            Assert.IsNotNull(result);
            Assert.AreEqual(data.Id, result.Id);
            Assert.AreEqual(EnumDeployStatus.Success, result.Status);
            AssertIsRecent(result.CompleteDateTimeUtc);
            var dbItem = testData.Sut.GetBatchRequest(result.Id);
            AssertRequest(result, dbItem);
        }

        [Test]
        public void UpdateBatchDeploymentStatus_Error_SetsCompleteDate()
        {
            var testData = TestData.Create(this, 1);
            var data = this.Fixture.Create<DeployBatchRequest>();
            data.Id = testData.DeployList[0].Id;
            data.ItemList = testData.DeployList[0].ItemList;

            var result = testData.Sut.UpdateBatchDeploymentStatus(data.Id, EnumDeployStatus.Error);

            Assert.IsNotNull(result);
            Assert.AreEqual(data.Id, result.Id);
            Assert.AreEqual(EnumDeployStatus.Error, result.Status);
            AssertIsRecent(result.CompleteDateTimeUtc);
            var dbItem = testData.Sut.GetBatchRequest(result.Id);
            AssertRequest(result, dbItem);
        }

        [Test]
        public void UpdateBatchDeploymentStatus_InProcess_ClearsResumeRequested()
        {
            var testData = TestData.Create(this, 1);
            var data = this.Fixture.Create<DeployBatchRequest>();
            data.Id = testData.DeployList[0].Id;
            data.ItemList = testData.DeployList[0].ItemList;
            testData.Sut.SetResumeRequested(data.Id, null);

            var result = testData.Sut.UpdateBatchDeploymentStatus(data.Id, EnumDeployStatus.InProcess);

            Assert.IsNotNull(result);
            Assert.AreEqual(data.Id, result.Id);
            Assert.AreEqual(EnumDeployStatus.InProcess, result.Status);
            Assert.IsFalse(result.ResumeRequested);
            var dbItem = testData.Sut.GetBatchRequest(result.Id);
            AssertRequest(result, dbItem);
        }

        [Test]
        public void UpdateBatchDeploymentStatus_Cancelled_ClearsCancelRequested()
        {
            var testData = TestData.Create(this, 1);
            var data = this.Fixture.Create<DeployBatchRequest>();
            data.Id = testData.DeployList[0].Id;
            data.ItemList = testData.DeployList[0].ItemList;
            testData.Sut.SetCancelRequested(data.Id, null);

            var result = testData.Sut.UpdateBatchDeploymentStatus(data.Id, EnumDeployStatus.Cancelled);

            Assert.IsNotNull(result);
            Assert.AreEqual(data.Id, result.Id);
            Assert.AreEqual(EnumDeployStatus.Cancelled, result.Status);
            Assert.IsFalse(result.CancelRequested);
            var dbItem = testData.Sut.GetBatchRequest(result.Id);
            AssertRequest(result, dbItem);
        }

        [Test]
        public void UpdateBatchDeploymentStatus_WithException_UpdatesStatus()
        {
            var testData = TestData.Create(this, 1);
            var data = this.Fixture.Create<DeployBatchRequest>();
            data.Id = testData.DeployList[0].Id;
            data.ItemList = testData.DeployList[0].ItemList;
            var err = this.Fixture.Create<Exception>();

            var result = testData.Sut.UpdateBatchDeploymentStatus(data.Id, data.Status, err);

            Assert.IsNotNull(result);
            Assert.AreEqual(data.Id, result.Id);
            Assert.AreEqual(data.Status, result.Status);
            Assert.AreEqual(err.ToString(), result.ErrorDetails);
            var dbItem = testData.Sut.GetBatchRequest(result.Id);
            AssertRequest(result, dbItem);
        }

        [Test]
        public void UpdateBatchDeploymentStatus_WithStatusMessage_AddsToMessageHistory()
        {
            var testData = TestData.Create(this, 1);
            var data = this.Fixture.Create<DeployBatchRequest>();
            data.Id = testData.DeployList[0].Id;
            data.ItemList = testData.DeployList[0].ItemList;
            var err = this.Fixture.Create<Exception>();
            string statusMessage = this.Fixture.Create<string>();

            var result = testData.Sut.UpdateBatchDeploymentStatus(data.Id, data.Status, statusMessage: statusMessage, addToMessageHistory: true);

            Assert.IsNotNull(result);
            Assert.AreEqual(data.Id, result.Id);
            Assert.AreEqual(data.Status, result.Status);
            Assert.AreEqual(statusMessage, result.LastStatusMessage);
            Assert.AreEqual(testData.DeployList[0].MessageList.Count + 1, result.MessageList.Count);
            Assert.AreEqual(statusMessage, result.MessageList.Last());
            var dbItem = testData.Sut.GetBatchRequest(result.Id);
            AssertRequest(result, dbItem);
        }

        [Test]
        public void UpdateBatchDeploymentStatus_WithoutAddToMessageHistory_DoesNotAddToMessageHistory()
        {
            var testData = TestData.Create(this, 1);
            var data = this.Fixture.Create<DeployBatchRequest>();
            data.Id = testData.DeployList[0].Id;
            data.ItemList = testData.DeployList[0].ItemList;
            var err = this.Fixture.Create<Exception>();
            string statusMessage = this.Fixture.Create<string>();

            var result = testData.Sut.UpdateBatchDeploymentStatus(data.Id, data.Status, statusMessage: statusMessage, addToMessageHistory: false);

            Assert.IsNotNull(result);
            Assert.AreEqual(data.Id, result.Id);
            Assert.AreEqual(data.Status, result.Status);
            Assert.AreEqual(statusMessage, result.LastStatusMessage);
            Assert.AreEqual(testData.DeployList[0].MessageList.Count, result.MessageList.Count);
            Assert.IsFalse(result.MessageList.Contains(statusMessage));
            var dbItem = testData.Sut.GetBatchRequest(result.Id);
            AssertRequest(result, dbItem);
        }

        [Test]
        public void GetDeployQueue_GetsDeployQueue_DefaultNotStartedAndInProcess_DefaultSortSubmittedDateDesc()
        {
            var testData = TestData.Create(this, 20);

            var result = testData.Sut.GetDeployQueue(null);

            Assert.IsNotNull(result);
            var invalidItems = result.Items.Where(i=>i.Status != EnumDeployStatus.InProcess && i.Status != EnumDeployStatus.NotStarted);
            Assert.IsEmpty(invalidItems);
            Assert.IsFalse(result.SortAscending);
            Assert.AreEqual("SubmittedDateTimeUtc", result.SortField);

            var temp = result.Items.OrderByDescending(i=>i.SubmittedDateTimeUtc);
            DateTime? lastDate = null;
            foreach(var item in result.Items)
            {
                if(lastDate.HasValue)
                {
                    Assert.GreaterOrEqual(lastDate.Value, item.SubmittedDateTimeUtc);
                }
                lastDate = item.SubmittedDateTimeUtc;
            }
        }

        [Test]
        public void GetDeployQueue_WithCompleteStatuses()
        {
            var testData = TestData.Create(this, 20);

            var result = testData.Sut.GetDeployQueue(null, new List<EnumDeployStatus> { EnumDeployStatus.Success, EnumDeployStatus.Error });

            Assert.IsNotNull(result);
            var invalidItems = result.Items.Where(i => i.Status != EnumDeployStatus.Success && i.Status != EnumDeployStatus.Error);
            Assert.IsEmpty(invalidItems);
        }

        [Test, Explicit]
        public void GetDeployQueue_WithEnvironmentIds()
        {
            var testData = TestData.Create(this, 20);
            string environmentId1 = testData.DeployList[0].ItemList[0].MachineList[0].EnvironmentId;
            string environmentId2 = testData.DeployList[1].ItemList[1].MachineList[1].EnvironmentId;

            var result = testData.Sut.GetDeployQueue(null, environmentIds: new List<string> { environmentId1, environmentId2 });

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Items.Count);
            Assert.IsTrue(result.Items.Any(i=>i.Id == testData.DeployList[0].Id));
            Assert.IsTrue(result.Items.Any(i=>i.Id == testData.DeployList[1].Id));
        }

        [Test]
        public void GetDeployQueue_WithResumeRequested()
        {
            var testData = TestData.Create(this, 20);
            var result = testData.Sut.GetDeployQueue(null, includeResumeRequested: true);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.Items.Any(i => i.ResumeRequested));
        }

        [Test]
        public void GetDeployQueue_WithoutResumeRequested()
        {
            var testData = TestData.Create(this, 20);
            var result = testData.Sut.GetDeployQueue(null, includeResumeRequested:false);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.Items.Any(i=>i.ResumeRequested));
        }

        [Test]
        public void SetCancelRequested_RequestsCancel()
        {
            var testData = TestData.Create(this, 1);
            var cancelMessage = this.Fixture.Create<string>("CancelMessage");
            var newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            var result = testData.Sut.SetCancelRequested(testData.DeployList[0].Id, cancelMessage);

            Assert.IsNotNull(result);
            Assert.AreEqual(testData.DeployList[0].Id, result.Id);
            Assert.IsTrue(result.CancelRequested);
            Assert.AreEqual(cancelMessage, result.CancelMessage);
            Assert.AreEqual(testData.DeployList[0].Status, result.Status);
            Assert.IsTrue(result.LastStatusMessage.Contains(cancelMessage));
            Assert.AreEqual(newUserName, result.UpdatedByUserName);
            AssertIsRecent(result.UpdatedDateTimeUtc);

            var dbItem = testData.Sut.GetBatchRequest(result.Id);
            AssertRequest(result, dbItem);
        }

        [Test]
        public void SetCancelRequested_WithoutMessage_RequestsCancel()
        {
            var testData = TestData.Create(this, 1);
            string cancelMessage = null;
            var newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            var result = testData.Sut.SetCancelRequested(testData.DeployList[0].Id, cancelMessage);

            Assert.IsNotNull(result);
            Assert.AreEqual(testData.DeployList[0].Id, result.Id);
            Assert.IsTrue(result.CancelRequested);
            Assert.AreEqual(cancelMessage, result.CancelMessage);
            Assert.AreEqual(testData.DeployList[0].Status, result.Status);
            //Assert.IsTrue(result.LastStatusMessage.Contains(cancelMessage));
            Assert.Contains(result.LastStatusMessage, result.MessageList);
            Assert.AreEqual(newUserName, result.UpdatedByUserName);
            AssertIsRecent(result.UpdatedDateTimeUtc);

            var dbItem = testData.Sut.GetBatchRequest(result.Id);
            AssertRequest(result, dbItem);
        }

        [Test]
        public void SetCancelRequested_NullRequestID_ThrowsArgumentNullException()
        {
            var testData = TestData.Create(this);

            Assert.Throws<ArgumentNullException>(()=>testData.Sut.SetCancelRequested(null, Guid.NewGuid().ToString()));
        }

        [Test]
        public void RequeueDeployment_RequeuesDeployment()
        {
            var testData = TestData.Create(this, 1);
            var statusMessage = this.Fixture.Create<string>("StatusMessage");
            var newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);
            var newStatus = EnumDeployStatus.NotStarted;

            var result = testData.Sut.RequeueDeployment(testData.DeployList[0].Id, newStatus, statusMessage);

            Assert.IsNotNull(result);
            Assert.AreEqual(testData.DeployList[0].Id, result.Id);
            Assert.IsNull(result.StartedDateTimeUtc);
            Assert.AreEqual(newStatus, result.Status);
            Assert.IsTrue(result.LastStatusMessage.Contains(statusMessage));
            Assert.AreEqual(newUserName, result.UpdatedByUserName);
            AssertIsRecent(result.UpdatedDateTimeUtc);

            var dbItem = testData.Sut.GetBatchRequest(result.Id);
            AssertRequest(result, dbItem);
        }

        [Test]
        public void RequeueDeployment_WithoutMessage_RequeuesDeployment()
        {
            var testData = TestData.Create(this, 1);
            string statusMessage = null;
            var newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);
            var newStatus = EnumDeployStatus.NotStarted;

            var result = testData.Sut.RequeueDeployment(testData.DeployList[0].Id, newStatus, statusMessage);

            Assert.IsNotNull(result);
            Assert.AreEqual(testData.DeployList[0].Id, result.Id);
            Assert.IsNull(result.StartedDateTimeUtc);
            Assert.AreEqual(newStatus, result.Status);
            //Assert.IsTrue(result.LastStatusMessage.Contains(statusMessage));
            Assert.AreEqual(newUserName, result.UpdatedByUserName);
            AssertIsRecent(result.UpdatedDateTimeUtc);

            var dbItem = testData.Sut.GetBatchRequest(result.Id);
            AssertRequest(result, dbItem);
        }

        [Test]
        public void RequeueDeployment_MissingRequestID_ThrowsArgumentNullException()
        {
            var testData = TestData.Create(this, 0);

            Assert.Throws<ArgumentNullException>(()=>testData.Sut.RequeueDeployment(null, EnumDeployStatus.NotStarted, Guid.NewGuid().ToString()));
        }

        [Test]
        public void SetResumeRequested_SetsResumeRequested()
        {
            var testData = TestData.Create(this, 1);
            var userMessage = this.Fixture.Create<string>("UserMessage");
            var newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            var result = testData.Sut.SetResumeRequested(testData.DeployList[0].Id, userMessage);

            Assert.IsNotNull(result);
            Assert.AreEqual(testData.DeployList[0].Id, result.Id);
            Assert.IsTrue(result.ResumeRequested);
            Assert.AreEqual(userMessage, result.ResumeMessage);
            Assert.AreEqual(testData.DeployList[0].Status, result.Status);
            Assert.IsTrue(result.LastStatusMessage.Contains(userMessage));
            Assert.AreEqual(newUserName, result.UpdatedByUserName);
            AssertIsRecent(result.UpdatedDateTimeUtc);

            var dbItem = testData.Sut.GetBatchRequest(result.Id);
            AssertRequest(result, dbItem);
        }

        [Test]
        public void SetResumeRequested_WithoutMessage_SetsResumeRequested()
        {
            var testData = TestData.Create(this, 1);
            string userMessage = null;
            var newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            var result = testData.Sut.SetResumeRequested(testData.DeployList[0].Id, userMessage);

            Assert.IsNotNull(result);
            Assert.AreEqual(testData.DeployList[0].Id, result.Id);
            Assert.IsTrue(result.ResumeRequested);
            Assert.AreEqual(userMessage, result.ResumeMessage);
            Assert.AreEqual(testData.DeployList[0].Status, result.Status);
            //Assert.IsTrue(result.LastStatusMessage.Contains(cancelMessage));
            Assert.Contains(result.LastStatusMessage, result.MessageList);
            Assert.AreEqual(newUserName, result.UpdatedByUserName);
            AssertIsRecent(result.UpdatedDateTimeUtc);

            var dbItem = testData.Sut.GetBatchRequest(result.Id);
            AssertRequest(result, dbItem);
        }

        [Test]
        public void SetResumeRequested_MissingID_ThrowsArgumentNullException()
        {
            var testData = TestData.Create(this);

            Assert.Throws<ArgumentNullException>(()=>testData.Sut.SetResumeRequested(null, Guid.NewGuid().ToString()));
        }
    }
}
