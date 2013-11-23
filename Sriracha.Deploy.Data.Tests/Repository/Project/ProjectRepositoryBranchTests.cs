using MMDB.Shared;
using NUnit.Framework;
using Sriracha.Deploy.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ploeh.AutoFixture;
using Sriracha.Deploy.Data.Dto.Project;

namespace Sriracha.Deploy.Data.Tests.Repository.Project
{
    [TestFixture]
    public abstract class ProjectRepositoryBranchTests : ProjectRepositoryTestBase
    {
        private DeployProjectBranch CreateTestBranch(IProjectRepository sut, string projectId)
        {
            return sut.CreateBranch(projectId, this.Fixture.Create<string>("BranchName"));
        }

        private void AssertBranch(DeployProjectBranch expected, DeployProjectBranch actual)
        {
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected.Id, actual.Id);
            Assert.AreEqual(expected.ProjectId, actual.ProjectId);
            Assert.AreEqual(expected.BranchName, actual.BranchName);
            Assert.AreEqual(expected.CreatedByUserName, actual.CreatedByUserName);
            AssertDateEqual(expected.CreatedDateTimeUtc, actual.CreatedDateTimeUtc);
            Assert.AreEqual(expected.UpdatedByUserName, actual.UpdatedByUserName);
            AssertDateEqual(expected.UpdatedDateTimeUtc, actual.UpdatedDateTimeUtc);
        }

        private void AssertCreatedBranch(DeployProjectBranch result, string projectId, string branchName, IProjectRepository sut)
        {
            Assert.IsNotNull(result);
            Assert.IsNotNullOrEmpty(result.Id);
            Assert.AreEqual(projectId, result.ProjectId);

            var dbProject = sut.GetProject(result.ProjectId);
            Assert.IsNotNull(dbProject);
            Assert.AreEqual(1, dbProject.BranchList.Count);
            Assert.AreEqual(result.Id, dbProject.BranchList[0].Id);
            Assert.AreEqual(projectId, dbProject.BranchList[0].ProjectId);
            Assert.AreEqual(branchName, dbProject.BranchList[0].BranchName);
        }

        private void AssertUpdated(DeployProjectBranch result, DeployProjectBranch original, string newBranchName, string newUserName, IProjectRepository sut)
        {
            Assert.IsNotNull(result);
            Assert.AreEqual(original.Id, result.Id);
            Assert.AreEqual(original.ProjectId, result.ProjectId);
            Assert.AreEqual(newBranchName, result.BranchName);
            Assert.AreEqual(original.CreatedByUserName, result.CreatedByUserName);
            AssertDateEqual(original.CreatedDateTimeUtc, result.CreatedDateTimeUtc);
            Assert.AreEqual(newUserName, result.UpdatedByUserName);
            AssertIsRecent(result.UpdatedDateTimeUtc);

            var dbItem = sut.GetBranch(result.Id, result.ProjectId);
            AssertBranch(result, dbItem);
        }

        [Test]
        public void CreateBranch_CanCreateBranch()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);

            string branchName = this.Fixture.Create<string>("BranchName");
            var result = sut.CreateBranch(project.Id, branchName);

            AssertCreatedBranch(result, project.Id, branchName, sut);
        }

        [Test]
        public void CreateBranch_MissingBranchName_ThrowsError()
        {
            var sut = this.GetRepository();
            string projectId = Guid.NewGuid().ToString();
            string branchName = null;
            Assert.Throws<ArgumentNullException>(() => sut.CreateBranch(projectId, branchName));
        }

        [Test]
        public void CreateBranch_MissingProjectId_ThrowsError()
        {
            var sut = this.GetRepository();
            string projectId = null;
            string branchName = Guid.NewGuid().ToString();
            Assert.Throws<ArgumentNullException>(() => sut.CreateBranch(projectId, branchName));
        }

        [Test]
        public void CreateBranch_BadProjectId_ThrowsError()
        {
            var sut = this.GetRepository();
            string projectId = Guid.NewGuid().ToString();
            string branchName = Guid.NewGuid().ToString();
            Assert.Throws<RecordNotFoundException>(() => sut.CreateBranch(projectId, branchName));
        }

        [Test]
        public void GetBranch_WithProjectID_GetsBranch()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);
            var branch = this.CreateTestBranch(sut, project.Id);

            var result = sut.GetBranch(branch.Id, project.Id);

            AssertBranch(branch, result);
        }

        [Test]
        public void GetBranch_WithoutProjectID_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);
            var branch = this.CreateTestBranch(sut, project.Id);

            Assert.Throws<ArgumentNullException>(()=>sut.GetBranch(branch.Id, null));
        }

        [Test]
        public void GetBranch_BadProjectID_ThrowsRecordNotFoundException()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);
            var branch = this.CreateTestBranch(sut, project.Id);

            Assert.Throws<RecordNotFoundException>(() => sut.GetBranch(branch.Id, Guid.NewGuid().ToString()));
        }

        [Test]
        public void GetBranch_MissingProjectID_ThrowsRecordNotFoundException()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);
            var branch = this.CreateTestBranch(sut, project.Id);

            Assert.Throws<ArgumentNullException>(() => sut.GetBranch(null, branch.ProjectId));
        }

        [Test]
        public void GetBranch_BadBranchID_ThrowsRecordNotFoundException()
        {
            var sut = this.GetRepository();
            var project = this.CreateTestProject(sut);
            var branch = this.CreateTestBranch(sut, project.Id);

            Assert.Throws<RecordNotFoundException>(() => sut.GetBranch(Guid.NewGuid().ToString(), branch.ProjectId));
        }

        [Test]
        public void GetBranchList_GetsBranchList()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);
            var branchList = new List<DeployProjectBranch>();
            for(int i = 0; i < 5; i++)
            {
                var branch = this.CreateTestBranch(sut, project.Id);
                branchList.Add(branch);
            }
            var otherProject = this.CreateTestProject(sut);
            for(int i = 0; i < 5; i++)
            {
                this.CreateTestBranch(sut, otherProject.Id);
            }

            var result = sut.GetBranchList(project.Id);

            Assert.IsNotNull(result);
            Assert.AreEqual(branchList.Count, result.Count);
            foreach(var branch in branchList)
            {
                var resultItem = result.SingleOrDefault(i=>i.Id == branch.Id);
                AssertBranch(branch, resultItem); 
            }
        }

        [Test]
        public void GetBranchList_MissingProjectID_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            Assert.Throws<ArgumentNullException>(() => sut.GetBranchList(null));
        }

        [Test]
        public void GetBranchList_BadProjectID_ThrowsRecordNotFoundException()
        {
            var sut = this.GetRepository();

            Assert.Throws<RecordNotFoundException>(() => sut.GetBranchList(Guid.NewGuid().ToString()));
        }

        [Test]
        public void TryGetBranch_WithProjectID_GetsBranch()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);
            var branch = this.CreateTestBranch(sut, project.Id);

            var result = sut.TryGetBranch(branch.Id, project.Id);

            AssertBranch(branch, result);
        }

        [Test]
        public void TryGetBranch_WithoutProjectID_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);
            var branch = this.CreateTestBranch(sut, project.Id);

            Assert.Throws<ArgumentNullException>(()=>sut.TryGetBranch(branch.Id, null));
        }

        [Test]
        public void TryGetBranch_BadProjectID_ReturnsNull()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);
            var branch = this.CreateTestBranch(sut, project.Id);

            var result = sut.TryGetBranch(branch.Id, Guid.NewGuid().ToString());

            Assert.IsNull(result);
        }

        [Test]
        public void TryGetBranch_BadBranchID_ReturnsNull()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);
            var branch = this.CreateTestBranch(sut, project.Id);

            var result = sut.TryGetBranch(Guid.NewGuid().ToString(), project.Id);

            Assert.IsNull(result);
        }

        [Test]
        public void TryGetBranch_MissingBranchID_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);
            var branch = this.CreateTestBranch(sut, project.Id);

            Assert.Throws<ArgumentNullException>(()=>sut.TryGetBranch(null, project.Id));
        }

        [Test]
        public void GetBranchByName_GetsBranch()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);
            var branch = this.CreateTestBranch(sut, project.Id);

            var result = sut.GetBranchByName(branch.ProjectId, branch.BranchName);

            AssertBranch(branch, result);
        }

        [Test]
        public void GetBranchByName_MissingBranchName_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);
            var branch = this.CreateTestBranch(sut, project.Id);

            Assert.Throws<ArgumentNullException>(()=>sut.GetBranchByName(branch.ProjectId, null));
        }

        [Test]
        public void GetBranchByName_MissingProjectID_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);
            var branch = this.CreateTestBranch(sut, project.Id);

            Assert.Throws<ArgumentNullException>(() => sut.GetBranchByName(null, branch.BranchName));
        }

        [Test]
        public void GetBranchByName_BadProjectID_ThrowsRecordNotFoundException()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);
            var branch = this.CreateTestBranch(sut, project.Id);

            Assert.Throws<RecordNotFoundException>(() => sut.GetBranchByName(Guid.NewGuid().ToString(), branch.BranchName));
        }

        [Test]
        public void GetBranchByName_BadBranchName_ThrowsRecordNotFoundException()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);
            var branch = this.CreateTestBranch(sut, project.Id);

            Assert.Throws<RecordNotFoundException>(() => sut.GetBranchByName(branch.ProjectId, Guid.NewGuid().ToString()));
        }

        [Test]
        public void GetOrCreateBranch_CreatesNewBranch()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);
            string branchName = this.Fixture.Create<string>("BranchName");

            var result = sut.GetOrCreateBranch(project.Id, branchName);

            AssertCreatedBranch(result, project.Id, branchName, sut);
        }

        [Test]
        public void GetOrCreateBranch_GetsBranchByID()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);
            var branch = this.CreateTestBranch(sut, project.Id);
            
            var result = sut.GetOrCreateBranch(project.Id, branch.Id);

            AssertBranch(branch, result);
        }

        [Test]
        public void GetOrCreateBranch_GetsBranchByName()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);
            var branch = this.CreateTestBranch(sut, project.Id);

            var result = sut.GetOrCreateBranch(project.Id, branch.BranchName);

            AssertBranch(branch, result);
        }

        [Test]
        public void GetOrCreateBranch_BadProjectID_ThrowsRecordNotFoundException()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);
            var branch = this.CreateTestBranch(sut, project.Id);

            Assert.Throws<RecordNotFoundException>(() => sut.GetOrCreateBranch(Guid.NewGuid().ToString(), branch.Id));
        }

        [Test]
        public void GetOrCreateBranch_MissingProjectID_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);
            var branch = this.CreateTestBranch(sut, project.Id);

            Assert.Throws<ArgumentNullException>(() => sut.GetOrCreateBranch(null, branch.Id));
        }

        [Test]
        public void GetOrCreateBranch_MissingBranchIdOrName_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);
            var branch = this.CreateTestBranch(sut, project.Id);

            Assert.Throws<ArgumentNullException>(() => sut.GetOrCreateBranch(project.Id, null));
        }

        [Test]
        public void UpdateBranch_UpdatesBranch()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);
            var branch = this.CreateTestBranch(sut, project.Id);
            string newBranchName = this.Fixture.Create<string>("BranchName");
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i=>i.UserName).Returns(newUserName);

            var result = sut.UpdateBranch(branch.Id, project.Id, newBranchName);

            AssertUpdated(result, branch, newBranchName, newUserName, sut);
        }

        [Test]
        public void UpdateBranch_MissingBranchID_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);
            var branch = this.CreateTestBranch(sut, project.Id);
            string newBranchName = this.Fixture.Create<string>("BranchName");
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            Assert.Throws<ArgumentNullException>(() => sut.UpdateBranch(null, project.Id, newBranchName));
        }

        [Test]
        public void UpdateBranch_BadBranchID_ThrowsRecordNotFoundException()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);
            var branch = this.CreateTestBranch(sut, project.Id);
            string newBranchName = this.Fixture.Create<string>("BranchName");
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            Assert.Throws<RecordNotFoundException>(() => sut.UpdateBranch(Guid.NewGuid().ToString(), project.Id, newBranchName));
        }

        [Test]
        public void UpdateBranch_MissingProjectID_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);
            var branch = this.CreateTestBranch(sut, project.Id);
            string newBranchName = this.Fixture.Create<string>("BranchName");
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            Assert.Throws<ArgumentNullException>(() => sut.UpdateBranch(branch.Id, null, newBranchName));
        }

        [Test]
        public void UpdateBranch_BadProjectID_ThrowsRecordNotException()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);
            var branch = this.CreateTestBranch(sut, project.Id);
            string newBranchName = this.Fixture.Create<string>("BranchName");
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            Assert.Throws<RecordNotFoundException>(() => sut.UpdateBranch(branch.Id, Guid.NewGuid().ToString(), newBranchName));
        }

        [Test]
        public void UpdateBranch_MissingBranchName_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);
            var branch = this.CreateTestBranch(sut, project.Id);
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i => i.UserName).Returns(newUserName);

            Assert.Throws<ArgumentNullException>(() => sut.UpdateBranch(branch.Id, project.Id, null));
        }

        [Test]
        public void DeleteProject_DeletesBranch()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);

            string branchName = this.Fixture.Create<string>("BranchName");
            var result = sut.CreateBranch(project.Id, branchName);

            sut.DeleteProject(project.Id);

            var dbItem = sut.TryGetBranch(result.Id, result.ProjectId);
            Assert.IsNull(dbItem);
        }

        [Test]
        public void DeleteBranch_WithProjectID_DeletesBranch()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);
            var branch = this.CreateTestBranch(sut, project.Id);

            sut.DeleteBranch(branch.Id, project.Id);

            var dbItem = sut.TryGetBranch(branch.Id, project.Id);
            Assert.IsNull(dbItem);

            var dbProject = sut.GetProject(project.Id);
            var dbProjectBranch = dbProject.BranchList.SingleOrDefault(i=>i.Id == branch.Id);
            Assert.IsNull(dbProjectBranch);
        }

        [Test]
        public void DeleteBranch_MissingProjectID_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);
            var branch = this.CreateTestBranch(sut, project.Id);

            Assert.Throws<ArgumentNullException>(()=>sut.DeleteBranch(branch.Id, null));
        }

        [Test]
        public void DeleteBranch_MissingBranchID_ThrowsArgumentNullException()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);
            var branch = this.CreateTestBranch(sut, project.Id);

            Assert.Throws<ArgumentNullException>(() => sut.DeleteBranch(null, project.Id));
        }

        [Test]
        public void DeleteBranch_BadBranchID_ThrowsRecordNotFoundException()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);
            var branch = this.CreateTestBranch(sut, project.Id);

            Assert.Throws<RecordNotFoundException>(() => sut.DeleteBranch(Guid.NewGuid().ToString(), project.Id));
        }

        [Test]
        public void DeleteBranch_BadProjectID_ThrowsRecordNotFoundException()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);
            var branch = this.CreateTestBranch(sut, project.Id);

            Assert.Throws<RecordNotFoundException>(() => sut.DeleteBranch(branch.Id, Guid.NewGuid().ToString()));
        }
    }
}
