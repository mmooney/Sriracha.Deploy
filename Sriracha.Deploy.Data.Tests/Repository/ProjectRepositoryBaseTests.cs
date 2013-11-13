using MMDB.Shared;
using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Sriracha.Deploy.Data.Dto.Project;
using Sriracha.Deploy.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Tests.Repository
{
	[TestFixture]
	public abstract class ProjectRepositoryBaseTests : RepositoryTestBase<IProjectRepository>
	{
        protected Mock<IUserIdentity> UserIdentity { get; private set; }
        protected Mock<NLog.Logger> Logger { get; private set; }
        protected string UserName { get; private set; }
        protected Fixture Fixture { get; private set; }

        protected void AssertIsRecent(DateTime dateTime)
        {
            Assert.Greater(DateTime.UtcNow, dateTime);
            Assert.Less(DateTime.UtcNow.AddMinutes(-1), dateTime);
        }

        protected DeployProject CreateTestProject(IProjectRepository sut)
        {
 	        return sut.CreateProject(this.Fixture.Create<string>("ProjectName"), false);
        }

        private void AssertDateEqual(DateTime expected, DateTime actual)
        {
            Assert.AreEqual(expected.Date, actual.Date);
            Assert.AreEqual(expected.Hour, actual.Hour);
            Assert.AreEqual(expected.Minute, actual.Minute);
            Assert.AreEqual(expected.Second, actual.Second);
        }


        [SetUp]
        public void SetUp()
        {
            this.Fixture = new Fixture();
            this.UserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity = new Mock<IUserIdentity>();
            this.UserIdentity.Setup(i=>i.UserName).Returns(this.UserName);
            this.Logger = new Mock<NLog.Logger>();
        }

        [Test]
        public void CreateProject_StoresProject()
        {
            string projectName = Guid.NewGuid().ToString();
            
            var sut = this.GetRepository(); 

            var result = sut.CreateProject(projectName, false);
            Assert.IsNotNull(result);
            Assert.IsNotNullOrEmpty(result.Id);
            Assert.AreEqual(projectName, result.ProjectName);
            Assert.AreEqual(false, result.UsesSharedComponentConfiguration);
            Assert.AreEqual(this.UserName, result.CreatedByUserName);
            AssertIsRecent(result.CreatedDateTimeUtc);
            Assert.AreEqual(this.UserName, result.UpdatedByUserName);
            AssertIsRecent(result.UpdatedDateTimeUtc);

            var dbItem = sut.GetProject(result.Id);
            Assert.AreEqual(projectName, dbItem.ProjectName);
        }

        [Test]
        public void CreateProject_MissingProjectName_ThrowsError()
        {
            string projectName = string.Empty;
            var sut = this.GetRepository();
            Assert.Throws<ArgumentNullException>(() => sut.CreateProject(projectName, false));
        }

        [Test]
        public void GetProjectList_CanRetrieveProjectList()
        {
            var sut = this.GetRepository();

            var projectList = new List<DeployProject>();
            for(int i = 0; i < 3; i++)
            {
                var project = this.CreateTestProject(sut);
                projectList.Add(project);
            }

            var result = sut.GetProjectList().ToList();

            Assert.GreaterOrEqual(result.Count, projectList.Count);
            foreach (var project in projectList)
            {
                Assert.IsTrue(result.Any(i => i.Id == project.Id));
            }
        }

        [Test]
        public void GetProject_CanGetProject()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);

            var result = sut.GetProject(project.Id);

            Assert.IsNotNull(result);
            Assert.AreEqual(project.Id, result.Id);
            Assert.AreEqual(project.ProjectName, result.ProjectName);
        }

        [Test]
        public void GetProject_MissingProjectId_ThrowsError()
        {
            var sut = this.GetRepository();
            string projectId = string.Empty;
            Assert.Throws<ArgumentNullException>(() => sut.GetProject(projectId));
        }

        [Test]
        public void GetProject_BadProjectId_ThrowsError()
        {
            var sut = GetRepository();
            string projectId = Guid.NewGuid().ToString();
            Assert.Throws<RecordNotFoundException>(() => sut.GetProject(projectId));
        }

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
        public void DeleteProject_CanDeleteProject()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);

            sut.DeleteProject(project.Id);

            var dbItem = sut.TryGetProject(project.Id);
            Assert.IsNull(dbItem);
        }

        [Test]
        public void DeleteProject_MissingProjectId_ThrowsError()
        {
            var sut = this.GetRepository();
            string projectId = string.Empty;
            Assert.Throws<ArgumentNullException>(() => sut.DeleteProject(projectId));
        }

        [Test]
        public void DeleteProject_BadProjectId_ThrowsError()
        {
            var sut = this.GetRepository();
            string projectId = Guid.NewGuid().ToString();
            Assert.Throws<RecordNotFoundException>(() => sut.DeleteProject(projectId));
        }

        [Test]
        public void UpdateProject_CanUpdateProject()
        {
            var sut = this.GetRepository();
            var project = this.CreateTestProject(sut);

            string newProjectName = this.Fixture.Create<string>("ProjectName");
            string newUserName = this.Fixture.Create<string>("UserName");
            this.UserIdentity.Setup(i=>i.UserName).Returns(newUserName);
            sut.UpdateProject(project.Id, newProjectName, true);

            var dbItem = sut.GetProject(project.Id);
            Assert.IsNotNull(dbItem);
            Assert.AreEqual(newProjectName, dbItem.ProjectName);
            Assert.AreEqual(true, dbItem.UsesSharedComponentConfiguration);
            AssertDateEqual(project.CreatedDateTimeUtc, dbItem.CreatedDateTimeUtc);
            Assert.AreEqual(project.CreatedByUserName, dbItem.CreatedByUserName);
            Assert.AreEqual(newUserName, dbItem.UpdatedByUserName);
            AssertIsRecent(dbItem.UpdatedDateTimeUtc);
        }

        [Test]
        public void UpdateProject_MissingProjectId_ThrowsError()
        {
            var sut = this.GetRepository();
            string projectId = string.Empty;
            string projectName = Guid.NewGuid().ToString();
            Assert.Throws<ArgumentNullException>(() => sut.UpdateProject(projectId, projectName, false));
        }

        [Test]
        public void UpdateProject_MissingProjectName_ThrowsError()
        {
            var sut = this.GetRepository();
            string projectId = Guid.NewGuid().ToString();
            string projectName = string.Empty;
            Assert.Throws<ArgumentNullException>(() => sut.UpdateProject(projectId, projectName, false));
        }
    }
}
