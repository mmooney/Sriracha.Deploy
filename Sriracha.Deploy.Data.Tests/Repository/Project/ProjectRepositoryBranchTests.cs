using MMDB.Shared;
using NUnit.Framework;
using Sriracha.Deploy.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ploeh.AutoFixture;

namespace Sriracha.Deploy.Data.Tests.Repository.Project
{
    [TestFixture]
    public abstract class ProjectRepositoryBaseBranchTests : ProjectRepositoryTestBase
    {
        [Test]
        public void CreateBranch_CanCreateBranch()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);

            string branchName = this.Fixture.Create<string>("BranchName");
            var result = sut.CreateBranch(project.Id, branchName);

            Assert.IsNotNull(result);
            Assert.IsNotNullOrEmpty(result.Id);
            Assert.AreEqual(project.Id, result.ProjectId);

            var dbProject = sut.GetProject(result.ProjectId);
            Assert.IsNotNull(dbProject);
            Assert.AreEqual(1, dbProject.BranchList.Count);
            Assert.AreEqual(result.Id, dbProject.BranchList[0].Id);
            Assert.AreEqual(project.Id, dbProject.BranchList[0].ProjectId);
            Assert.AreEqual(branchName, dbProject.BranchList[0].BranchName);
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
        public void DeleteProject_DeletesBranch()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);

            string branchName = this.Fixture.Create<string>("BranchName");
            var result = sut.CreateBranch(project.Id, branchName);

            sut.DeleteProject(project.Id);

            var dbItem = sut.TryGetBranch(result.Id);
            Assert.IsNull(dbItem);
        }
    }
}
