using NUnit.Framework;
using Sriracha.Deploy.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ploeh.AutoFixture;
using Sriracha.Deploy.Data.Dto.Deployment.Offline;
using MMDB.Shared;

namespace Sriracha.Deploy.Data.Tests.Repository.Deploy
{
    public abstract class OfflineDeploymentRepositoryTests : RepositoryTestBase<IOfflineDeploymentRepository>
    {
        public OfflineDeploymentRepositoryTests()
        {

        }

        private void AssertCreatedDeployment(string deployBatchRequestId, EnumOfflineDeploymentStatus initialStatus, OfflineDeployment actual)
        {
            AssertHelpers.AssertCreatedBaseDto(actual, this.UserName);
            Assert.AreEqual(deployBatchRequestId, actual.DeployBatchRequestId);
            Assert.AreEqual(initialStatus, actual.Status);
        }

        private void AssertDeployment(OfflineDeployment expected, OfflineDeployment actual)
        {
            AssertHelpers.AssertBaseDto(expected, actual);
            Assert.AreEqual(expected.DeployBatchRequestId, actual.DeployBatchRequestId);
            Assert.AreEqual(expected.Status, actual.Status);
            Assert.AreEqual(expected.FileId, actual.FileId);
            Assert.AreEqual(expected.CreateErrorDetails, actual.CreateErrorDetails);
            Assert.AreEqual(expected.ResultFileId, actual.ResultFileId);
        }

        private void AssertUpdatedDeployment(OfflineDeployment original, EnumOfflineDeploymentStatus newStatus, Exception err, OfflineDeployment actual)
        {
            AssertHelpers.AssertUpdatedBaseDto(original, actual, this.UserName);
            Assert.AreEqual(original.DeployBatchRequestId, actual.DeployBatchRequestId);
            Assert.AreEqual(newStatus, actual.Status);
            Assert.AreEqual(original.FileId, actual.FileId);
            if(err != null)
            {  
                Assert.AreEqual(err.ToString(), actual.CreateErrorDetails);
            }
            Assert.AreEqual(original.ResultFileId, actual.ResultFileId);
        }

        [Test]
        public void CreateOfflineDeployment_CreatesOfflineDeployment()
        {
            var sut = this.GetRepository();
            string deployBatchRequestId = this.Fixture.Create<string>("deployBatchRequestId");
            var initialStatus = this.Fixture.Create<EnumOfflineDeploymentStatus>();

            var result = sut.CreateOfflineDeployment(deployBatchRequestId, initialStatus);

            Assert.IsNotNull(result);
            AssertCreatedDeployment(deployBatchRequestId, initialStatus, result);
            var dbItem = sut.GetOfflineDeployment(result.Id);
            AssertDeployment(result, dbItem);
        }

        [Test]
        public void CreateOfflineDeployment_MissingDeployRequestBatchID_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();
            string deployBatchRequestId = null;
            var initialStatus = this.Fixture.Create<EnumOfflineDeploymentStatus>();

            Assert.Throws<ArgumentNullException>(()=>sut.CreateOfflineDeployment(deployBatchRequestId, initialStatus));
        }

        [Test]
        public void GetOfflineDeployment_GetsOfflineDeployment()
        {
            var sut = this.GetRepository();
            string deployBatchRequestId = this.Fixture.Create<string>("deployBatchRequestId");
            var initialStatus = this.Fixture.Create<EnumOfflineDeploymentStatus>();
            var deployment = sut.CreateOfflineDeployment(deployBatchRequestId, initialStatus);

            var result = sut.GetOfflineDeployment(deployment.Id);

            AssertDeployment(deployment, result);
        }

        [Test]
        public void GetOfflineDeployment_MissingDeploymentID_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            Assert.Throws<ArgumentNullException>(()=>sut.GetOfflineDeployment(null));
        }

        [Test]
        public void GetOfflineDeployment_BadDeploymentID_ThrowsRecordNotFoundException()
        {
            var sut = this.GetRepository();

            Assert.Throws<RecordNotFoundException>(() => sut.GetOfflineDeployment(Guid.NewGuid().ToString()));
        }

        [Test]
        public void GetOfflineDeploymentForDeploymentBatchRequestId_GetsOfflineDeployment()
        {
            var sut = this.GetRepository();
            string deployBatchRequestId = this.Fixture.Create<string>("deployBatchRequestId");
            var initialStatus = this.Fixture.Create<EnumOfflineDeploymentStatus>();
            var deployment = sut.CreateOfflineDeployment(deployBatchRequestId, initialStatus);

            var result = sut.GetOfflineDeploymentForDeploymentBatchRequestId(deployBatchRequestId);

            AssertDeployment(deployment, result);
        }

        [Test]
        public void GetOfflineDeploymentForDeploymentBatchRequestId_MutipleMatches_GetsOfflineDeployment()
        {
            var sut = this.GetRepository();
            string deployBatchRequestId = this.Fixture.Create<string>("deployBatchRequestId");
            var initialStatus = this.Fixture.Create<EnumOfflineDeploymentStatus>();
            var deployment1 = sut.CreateOfflineDeployment(deployBatchRequestId, initialStatus);
            var deployment2 = sut.CreateOfflineDeployment(deployBatchRequestId, initialStatus);

            var result = sut.GetOfflineDeploymentForDeploymentBatchRequestId(deployBatchRequestId);

            Assert.That(result.Id == deployment1.Id || result.Id == deployment2.Id);
            if (result.Id == deployment1.Id)
            {
                AssertDeployment(deployment1, result);
            }
            if (result.Id == deployment2.Id)
            {
                AssertDeployment(deployment2, result);
            }
        }

        [Test]
        public void GetOfflineDeploymentForDeploymentBatchRequestId_MissingDeployBatchRequestID_ThrosArgumentNullException()
        {
            var sut = this.GetRepository();

            Assert.Throws<ArgumentNullException>(()=>sut.GetOfflineDeploymentForDeploymentBatchRequestId(null));

        }

        [Test]
        public void UpdateStatus_WithoutError_UpdatesStatus()
        {
            var sut = this.GetRepository();
            string deployBatchRequestId = this.Fixture.Create<string>("deployBatchRequestId");
            var initialStatus = this.Fixture.Create<EnumOfflineDeploymentStatus>();
            var deployment = sut.CreateOfflineDeployment(deployBatchRequestId, initialStatus);
            var newStatus = this.Fixture.Create<EnumOfflineDeploymentStatus>();
            this.CreateNewUserName();

            var result = sut.UpdateStatus(deployment.Id, newStatus, null);

            AssertUpdatedDeployment(deployment, newStatus, null, result);
        }

        [Test]
        public void UpdateStatus_WithError_UpdatesStatusAndError()
        {
            var sut = this.GetRepository();
            string deployBatchRequestId = this.Fixture.Create<string>("deployBatchRequestId");
            var initialStatus = this.Fixture.Create<EnumOfflineDeploymentStatus>();
            var deployment = sut.CreateOfflineDeployment(deployBatchRequestId, initialStatus);
            var newStatus = this.Fixture.Create<EnumOfflineDeploymentStatus>();
            var err = this.Fixture.Create<Exception>();
            this.CreateNewUserName();

            var result = sut.UpdateStatus(deployment.Id, newStatus, err);

            AssertUpdatedDeployment(deployment, newStatus, err, result);
        }

        [Test]
        public void UpdateStatus_MissingID_ThrowArgumentNullException()
        {
            var sut = this.GetRepository();
            var newStatus = this.Fixture.Create<EnumOfflineDeploymentStatus>();

            Assert.Throws<ArgumentNullException>(()=>sut.UpdateStatus(null, newStatus, null));
        }

        [Test]
        public void PopNextOfflineDeploymentToCreate_PopsNextItem()
        {
            var sut = this.GetRepository();
            string deployBatchRequestId = this.Fixture.Create<string>("deployBatchRequestId");
            var initialStatus = EnumOfflineDeploymentStatus.CreateRequested;
            var deployment = sut.CreateOfflineDeployment(deployBatchRequestId, initialStatus);
            var newUserName = this.Fixture.Create("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            var result = sut.PopNextOfflineDeploymentToCreate();

            AssertUpdatedDeployment(deployment, EnumOfflineDeploymentStatus.CreateInProcess, null, result);
            var dbItem = sut.GetOfflineDeployment(deployment.Id);
            AssertDeployment(result, dbItem);

        }

        [Test]
        public void PopNextOfflineDeploymentToCreate_TaskAlreadyInProcess_ReturnsNull()
        {
            var sut = this.GetRepository();
            string deployBatchRequestId = this.Fixture.Create<string>("deployBatchRequestId");
            var initialStatus = EnumOfflineDeploymentStatus.CreateInProcess;
            var deployment = sut.CreateOfflineDeployment(deployBatchRequestId, initialStatus);

            var result = sut.PopNextOfflineDeploymentToCreate();

            Assert.IsNull(result);
        }

        [Test]
        public void PopNextOfflineDeploymentToCreate_NoCurrentTask_ReturnsNull()
        {
            var sut = this.GetRepository();
            
            var result = sut.PopNextOfflineDeploymentToCreate();

            Assert.IsNull(result);
        }

        [Test]
        public void SetReadyForDownload_SetsReadyForDownload()
        {
            var sut = this.GetRepository();
            string deployBatchRequestId = this.Fixture.Create<string>("deployBatchRequestId");
            var initialStatus = EnumOfflineDeploymentStatus.CreateInProcess;
            var deployment = sut.CreateOfflineDeployment(deployBatchRequestId, initialStatus);
            string fileId = this.Fixture.Create<string>("FileId");
            this.CreateNewUserName();

            var result = sut.SetReadyForDownload(deployment.Id, fileId);

            Assert.IsNotNull(result);
            AssertHelpers.AssertUpdatedBaseDto(deployment, result, this.UserName);
            Assert.AreEqual(fileId, result.FileId);
            Assert.AreEqual(EnumOfflineDeploymentStatus.ReadyForDownload, result.Status);
        }

        [Test]
        public void SetReadyForDownload_MissingID_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();
            string fileId = this.Fixture.Create<string>("FileId");

            Assert.Throws<ArgumentNullException>(()=>sut.SetReadyForDownload(null, fileId));
        }

        [Test]
        public void SetReadyForDownload_BadID_ThrowsRecordNotFoundException()
        {
            var sut = this.GetRepository();
            string fileId = this.Fixture.Create<string>("FileId");

            Assert.Throws<RecordNotFoundException>(() => sut.SetReadyForDownload(Guid.NewGuid().ToString(), fileId));
        }

        [Test]
        public void SetReadyForDownload_MissingFileID_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();
            string deployBatchRequestId = this.Fixture.Create<string>("deployBatchRequestId");
            var initialStatus = EnumOfflineDeploymentStatus.CreateInProcess;
            var deployment = sut.CreateOfflineDeployment(deployBatchRequestId, initialStatus);
            this.CreateNewUserName();

            Assert.Throws<ArgumentNullException>(()=>sut.SetReadyForDownload(deployment.Id, null));
        }
    }
}
