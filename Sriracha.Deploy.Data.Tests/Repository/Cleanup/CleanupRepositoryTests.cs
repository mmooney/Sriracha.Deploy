using MMDB.Shared;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Tests.Repository.Cleanup
{
    public abstract class CleanupRepositoryTests : RepositoryTestBase<ICleanupRepository>
    {
        private class TestData
        {
            public Fixture Fixture { get; set; }
            public CleanupTaskData CleanupTaskData { get; set; }
            public ICleanupRepository Sut { get; set; }

            public static TestData Create(CleanupRepositoryTests tester, bool existing)
            {
                var fixture = new Fixture();
                var testData = new TestData
                {
                    Fixture = fixture,
                    CleanupTaskData = fixture.Create<CleanupTaskData>()
                };
                if(!existing)
                {  
                    testData.CleanupTaskData.CompletedDateTimeUtc = null;
                    testData.CleanupTaskData.StartedDateTimeUtc = null;
                    testData.CleanupTaskData.ErrorDetails = null;
                }
                testData.Sut = tester.GetRepository();

                return testData;
            }
        }

        private void AssertCreatedTask(CleanupTaskData expected, CleanupTaskData actual)
        {
            if(expected == null)
            {
                Assert.IsNull(actual);
            }
            else
            {
                Assert.IsNotNull(actual);
                Assert.IsNotNullOrEmpty(actual.Id);
                Assert.AreEqual(expected.MachineName, actual.MachineName);
                Assert.AreEqual(expected.FolderPath, actual.FolderPath);
                Assert.AreEqual(expected.AgeMinutes, actual.AgeMinutes);
                AssertDateEqual(DateTime.UtcNow.AddMinutes(expected.AgeMinutes), actual.TargetCleanupDateTimeUtc);
                Assert.AreEqual(expected.Status, actual.Status);
                AssertDateEqual(expected.StartedDateTimeUtc, actual.StartedDateTimeUtc);
                AssertDateEqual(expected.CompletedDateTimeUtc, actual.CompletedDateTimeUtc);
                Assert.AreEqual(expected.ErrorDetails, actual.ErrorDetails);

                AssertIsRecent(actual.CreatedDateTimeUtc);
                Assert.AreEqual(this.UserName, actual.CreatedByUserName);
                AssertIsRecent(actual.UpdatedDateTimeUtc);
                Assert.AreEqual(this.UserName, actual.UpdatedByUserName);
            }
        }

        private void AssertTask(CleanupTaskData expected, CleanupTaskData actual)
        {
            if (expected == null)
            {
                Assert.IsNull(actual);
            }
            else
            {
                Assert.IsNotNull(actual);
                Assert.AreEqual(expected.Id, actual.Id);
                Assert.AreEqual(expected.MachineName, actual.MachineName);
                Assert.AreEqual(expected.FolderPath, actual.FolderPath);
                Assert.AreEqual(expected.AgeMinutes, actual.AgeMinutes);
                AssertDateEqual(expected.TargetCleanupDateTimeUtc, actual.TargetCleanupDateTimeUtc);
                AssertDateEqual(expected.CreatedDateTimeUtc, actual.CreatedDateTimeUtc);
                Assert.AreEqual(expected.CreatedByUserName, actual.CreatedByUserName);
                AssertDateEqual(expected.UpdatedDateTimeUtc, actual.UpdatedDateTimeUtc);
                Assert.AreEqual(expected.UpdatedByUserName, actual.UpdatedByUserName);
                Assert.AreEqual(expected.Status, actual.Status);
                AssertDateEqual(expected.StartedDateTimeUtc, actual.StartedDateTimeUtc);
                AssertDateEqual(expected.CompletedDateTimeUtc, actual.CompletedDateTimeUtc);
                Assert.AreEqual(expected.ErrorDetails, actual.ErrorDetails);
            }
        }

        [Test]
        public void CreateCleanupTask_CreatesTask()
        {
            var testData = TestData.Create(this, false);

            var result = testData.Sut.CreateCleanupTask(testData.CleanupTaskData.MachineName, testData.CleanupTaskData.TaskType, testData.CleanupTaskData.FolderPath, testData.CleanupTaskData.AgeMinutes);

            AssertCreatedTask(testData.CleanupTaskData, result);
            var dbItem = testData.Sut.GetCleanupTask(result.Id);
            AssertTask(result, dbItem);
        }

        [Test]
        public void CreateCleanupTask_MissingMachineName_ThrowsArgumentNullException()
        {
            var testData = TestData.Create(this, false);

            Assert.Throws<ArgumentNullException>(()=>testData.Sut.CreateCleanupTask(null, testData.CleanupTaskData.TaskType, testData.CleanupTaskData.FolderPath, testData.CleanupTaskData.AgeMinutes));
        }

        [Test]
        public void CreateCleanupTask_MissingFolderName_ThrowsArgumentNullException()
        {
            var testData = TestData.Create(this, false);

            Assert.Throws<ArgumentNullException>(() => testData.Sut.CreateCleanupTask(testData.CleanupTaskData.MachineName, testData.CleanupTaskData.TaskType, null, testData.CleanupTaskData.AgeMinutes));
        }

        //[Test]
        //public void CreateCleanupTask_MissingAgeMinutes_ThrowsArgumentNullException()
        //{
        //    var testData = TestData.Create(this, false);

        //    Assert.Throws<ArgumentNullException>(() => testData.Sut.CreateCleanupTask(testData.CleanupTaskData.MachineName, testData.CleanupTaskData.TaskType, testData.CleanupTaskData.FolderPath, 0));
        //}

        [Test]
        public void PopNextFolderCleanupTask_PopsTask()
        {
            var testData = TestData.Create(this, false);
            var newTaskData = testData.Sut.CreateCleanupTask(testData.CleanupTaskData.MachineName, EnumCleanupTaskType.Folder, testData.CleanupTaskData.FolderPath, -1);
            var newUserName = this.Fixture.Create("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            var result = testData.Sut.PopNextFolderCleanupTask(testData.CleanupTaskData.MachineName);

            Assert.IsNotNull(result);
            Assert.AreEqual(newTaskData.Id, result.Id);
            Assert.AreEqual(newTaskData.TaskType, result.TaskType);
            Assert.AreEqual(newTaskData.FolderPath, result.FolderPath);
            Assert.AreEqual(newTaskData.AgeMinutes, result.AgeMinutes);
            AssertDateEqual(newTaskData.TargetCleanupDateTimeUtc, result.TargetCleanupDateTimeUtc);
            AssertDateEqual(newTaskData.CreatedDateTimeUtc, result.CreatedDateTimeUtc);
            Assert.AreEqual(newTaskData.CreatedByUserName, result.CreatedByUserName);
            AssertIsRecent(result.UpdatedDateTimeUtc);
            Assert.AreEqual(newUserName, result.UpdatedByUserName);
            AssertIsRecent(result.StartedDateTimeUtc);
            Assert.IsNull(result.CompletedDateTimeUtc);
            Assert.AreEqual(EnumQueueStatus.InProcess, result.Status);

            var dbItem = testData.Sut.GetCleanupTask(result.Id);
            AssertTask(result, dbItem);
        }

        [Test]
        public void PopNextFolderCleanupTask_TaskAlreadyInProcess_ReturnsNull()
        {
            var testData = TestData.Create(this, false);
            var newTaskData = testData.Sut.CreateCleanupTask(testData.CleanupTaskData.MachineName, EnumCleanupTaskType.Folder, testData.CleanupTaskData.FolderPath, -1);
            var firstResult = testData.Sut.PopNextFolderCleanupTask(testData.CleanupTaskData.MachineName);
            Assert.IsNotNull(firstResult);
            Assert.AreEqual(newTaskData.Id, firstResult.Id);
            Assert.AreEqual(EnumQueueStatus.InProcess, firstResult.Status);

            var result = testData.Sut.PopNextFolderCleanupTask(testData.CleanupTaskData.MachineName);
            
            Assert.IsNull(result);
        }

        [Test]
        public void PopNextFolderCleanupTask_NoCurrentTask_ReturnsNull()
        {
            var testData = TestData.Create(this, false);
            var newTaskData = testData.Sut.CreateCleanupTask(testData.CleanupTaskData.MachineName, EnumCleanupTaskType.Folder, testData.CleanupTaskData.FolderPath, 100);
            var newUserName = this.Fixture.Create("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            var result = testData.Sut.PopNextFolderCleanupTask(testData.CleanupTaskData.MachineName);

            Assert.IsNull(result);
        }

        [Test]
        public void PopNextFolderCleanupTask_NullMachineName_ThrowsArgumentNullException()
        {
            var testData = TestData.Create(this, false);
            Assert.Throws<ArgumentNullException>(()=>testData.Sut.PopNextFolderCleanupTask(null));
        }

        [Test]
        public void PopNextFolderCleanupTask_BadMachineName_ReturnsNull()
        {
            var testData = TestData.Create(this, false);
            var newTaskData = testData.Sut.CreateCleanupTask(testData.CleanupTaskData.MachineName, EnumCleanupTaskType.Folder, testData.CleanupTaskData.FolderPath, -1);

            var result = testData.Sut.PopNextFolderCleanupTask(Guid.NewGuid().ToString());

            Assert.IsNull(result);
        }

        [Test]
        public void GetCleanupTask_GetsTask()
        {
            var testData = TestData.Create(this, false);
            var newTaskData = testData.Sut.CreateCleanupTask(testData.CleanupTaskData.MachineName, EnumCleanupTaskType.Folder, testData.CleanupTaskData.FolderPath, testData.CleanupTaskData.AgeMinutes);

            var result = testData.Sut.GetCleanupTask(newTaskData.Id);

            AssertTask(newTaskData, result);
        }

        [Test]
        public void GetCleanupTask_MissingTaskId_ThrowsArgumentNullException()
        {
            var testData = TestData.Create(this, false);

            Assert.Throws<ArgumentNullException>(()=>testData.Sut.GetCleanupTask(null));
        }

        [Test]
        public void GetCleanupTask_BadTaskId_ThrowsRecordNotFoundException()
        {
            var testData = TestData.Create(this, false);
            Assert.Throws<RecordNotFoundException>(() => testData.Sut.GetCleanupTask(Guid.NewGuid().ToString()));
        }

        [Test]
        public void MarkItemSuccessful_MarksItemSuccessful()
        {
            var testData = TestData.Create(this, false);
            var newTaskData = testData.Sut.CreateCleanupTask(testData.CleanupTaskData.MachineName, EnumCleanupTaskType.Folder, testData.CleanupTaskData.FolderPath, -1);
            var firstResult = testData.Sut.PopNextFolderCleanupTask(testData.CleanupTaskData.MachineName);
            Assert.IsNotNull(firstResult);
            Assert.AreEqual(newTaskData.Id, firstResult.Id);
            Assert.AreEqual(EnumQueueStatus.InProcess, firstResult.Status);
            var newUserName = this.Fixture.Create("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            var result = testData.Sut.MarkItemSuccessful(newTaskData.Id);

            Assert.IsNotNull(result);
            Assert.AreEqual(newTaskData.Id, result.Id);
            Assert.AreEqual(EnumQueueStatus.Completed, result.Status);
            AssertIsRecent(result.CompletedDateTimeUtc);
            Assert.AreEqual(newTaskData.CreatedByUserName, result.CreatedByUserName);
            AssertDateEqual(newTaskData.CreatedDateTimeUtc, result.CreatedDateTimeUtc);
            AssertIsRecent(result.UpdatedDateTimeUtc);
            Assert.AreEqual(newUserName, result.UpdatedByUserName);

            var dbItem = testData.Sut.GetCleanupTask(result.Id);
            AssertTask(result, dbItem);
        }

        [Test]
        public void MarkItemSuccessful_MissingTaskID_ThrowsArgumentNullException()
        {
            var testData = TestData.Create(this, false);

            Assert.Throws<ArgumentNullException>(()=>testData.Sut.MarkItemSuccessful(null));
        }

        [Test]
        public void MarkItemSuccessful_BadTaskID_ThrowsRecordNotFoundException()
        {
            var testData = TestData.Create(this, false);

            Assert.Throws<RecordNotFoundException>(() => testData.Sut.MarkItemSuccessful(Guid.NewGuid().ToString()));
        }

        [Test]
        public void MarkItemFailed_MarksItemFailed()
        {
            var testData = TestData.Create(this, false);
            var newTaskData = testData.Sut.CreateCleanupTask(testData.CleanupTaskData.MachineName, EnumCleanupTaskType.Folder, testData.CleanupTaskData.FolderPath, -1);
            var firstResult = testData.Sut.PopNextFolderCleanupTask(testData.CleanupTaskData.MachineName);
            Assert.IsNotNull(firstResult);
            Assert.AreEqual(newTaskData.Id, firstResult.Id);
            Assert.AreEqual(EnumQueueStatus.InProcess, firstResult.Status);
            var newUserName = this.Fixture.Create("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);
            var err = testData.Fixture.Create<Exception>();

            var result = testData.Sut.MarkItemFailed(newTaskData.Id, err);

            Assert.IsNotNull(result);
            Assert.AreEqual(newTaskData.Id, result.Id);
            Assert.AreEqual(EnumQueueStatus.Error, result.Status);
            Assert.AreEqual(err.ToString(), result.ErrorDetails);
            AssertIsRecent(result.CompletedDateTimeUtc);
            Assert.AreEqual(newTaskData.CreatedByUserName, result.CreatedByUserName);
            AssertDateEqual(newTaskData.CreatedDateTimeUtc, result.CreatedDateTimeUtc);
            AssertIsRecent(result.UpdatedDateTimeUtc);
            Assert.AreEqual(newUserName, result.UpdatedByUserName);

            var dbItem = testData.Sut.GetCleanupTask(result.Id);
            AssertTask(result, dbItem);
        }

        [Test]
        public void MarkItemFailed_MissingTaskID_ThrowsArgumentNullException()
        {
            var testData = TestData.Create(this, false);
            var err = testData.Fixture.Create<Exception>();

            Assert.Throws<ArgumentNullException>(() => testData.Sut.MarkItemFailed(null, err));
        }

        [Test]
        public void MarkItemFailed_BadTaskID_ThrowsRecordNotFoundException()
        {
            var testData = TestData.Create(this, false);
            var err = testData.Fixture.Create<Exception>();

            Assert.Throws<RecordNotFoundException>(() => testData.Sut.MarkItemFailed(Guid.NewGuid().ToString(), err));
        }

        [Test]
        public void MarkItemFailed_MissingException_MarksItemFailed()
        {
            var testData = TestData.Create(this, false);
            var newTaskData = testData.Sut.CreateCleanupTask(testData.CleanupTaskData.MachineName, EnumCleanupTaskType.Folder, testData.CleanupTaskData.FolderPath, -1);
            var firstResult = testData.Sut.PopNextFolderCleanupTask(testData.CleanupTaskData.MachineName);
            Assert.IsNotNull(firstResult);
            Assert.AreEqual(newTaskData.Id, firstResult.Id);
            Assert.AreEqual(EnumQueueStatus.InProcess, firstResult.Status);
            var newUserName = this.Fixture.Create("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            var result = testData.Sut.MarkItemFailed(newTaskData.Id, null);

            Assert.IsNotNull(result);
            Assert.AreEqual(newTaskData.Id, result.Id);
            Assert.AreEqual(EnumQueueStatus.Error, result.Status);
            Assert.IsNullOrEmpty(result.ErrorDetails);
            AssertIsRecent(result.CompletedDateTimeUtc);
            Assert.AreEqual(newTaskData.CreatedByUserName, result.CreatedByUserName);
            AssertDateEqual(newTaskData.CreatedDateTimeUtc, result.CreatedDateTimeUtc);
            AssertIsRecent(result.UpdatedDateTimeUtc);
            Assert.AreEqual(newUserName, result.UpdatedByUserName);

            var dbItem = testData.Sut.GetCleanupTask(result.Id);
            AssertTask(result, dbItem);
        }
    }
}
