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

namespace Sriracha.Deploy.Data.Tests.Repository.Project
{
	[TestFixture]
	public abstract class ProjectRepositoryBaseProjectTests : ProjectRepositoryTestBase
	{
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

        [Test]
        public void GetOrCreateProject_CreatesNewProject()
        {
            var sut = this.GetRepository();

            string projectName = this.Fixture.Create<string>("ProjectName");

            var result = sut.GetOrCreateProject(projectName);
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
        public void GetOrCreateProject_GetsProjectById()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);

            var result = sut.GetOrCreateProject(project.Id);
            Assert.IsNotNull(result);
            Assert.IsNotNullOrEmpty(result.Id);
            Assert.AreEqual(project.Id, result.Id);
            Assert.AreEqual(project.ProjectName, result.ProjectName);
            Assert.AreEqual(project.UsesSharedComponentConfiguration, result.UsesSharedComponentConfiguration);
            Assert.AreEqual(project.CreatedByUserName, result.CreatedByUserName);
            AssertDateEqual(project.CreatedDateTimeUtc, result.CreatedDateTimeUtc);
            Assert.AreEqual(project.UpdatedByUserName, result.UpdatedByUserName);
            AssertDateEqual(project.UpdatedDateTimeUtc, result.UpdatedDateTimeUtc);
       }

        [Test]
        public void GetOrCreateProject_GetsProjectByName()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);

            var result = sut.GetOrCreateProject(project.ProjectName);
            AssertProject(project, result);
        }


        [Test]
        public void GetOrCreateProject_MissingProjectIdOrName_ThrowsNullException()
        {
            var sut = this.GetRepository();

            Assert.Throws<ArgumentNullException>(()=>sut.GetOrCreateProject(null));
        }

        [Test]
        public void TryGetProject_ReturnsProject()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);

            var result = sut.TryGetProject(project.Id);
            AssertProject(project, result);
        }

        [Test]
        public void TryGetProject_MissingProjectID_ThrowsNullException()
        {
            var sut = this.GetRepository();

            Assert.Throws<ArgumentNullException>(() => sut.TryGetProject(null));
        }

        [Test]
        public void TryGetProjectByName_ReturnsProject()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);

            var result = sut.TryGetProjectByName(project.ProjectName);
            AssertProject(project, result);
        }

        [Test]
        public void TryGetProjectByName_MissingProjectName_ThrowsNullException()
        {
            var sut = this.GetRepository();

            Assert.Throws<ArgumentNullException>(() => sut.TryGetProjectByName(null));
        }

        [Test]
        public void TryGetProjectByName_InvalidProjectName_ReturnsNull()
        {
            var sut = this.GetRepository();

            var result = sut.TryGetProjectByName(Guid.NewGuid().ToString());
            Assert.IsNull(result);
        }

        [Test]
        public void TryGetProjectByName_DuplicateName_ThrowsArgumentException()
        {
            var sut = this.GetRepository();

            string projectName = this.Fixture.Create<string>("ProjectName");
            var project1 = sut.CreateProject(projectName, false);
            var project2 = sut.CreateProject(projectName, false);

            Assert.Throws<ArgumentException>(()=>sut.TryGetProjectByName(projectName));
        }

        [Test]
        public void GetProjectByName_GetsProject()
        {
            var sut = this.GetRepository();

            var project = this.CreateTestProject(sut);

            var result = sut.GetProjectByName(project.ProjectName);
            AssertProject(project, result);
        }

        [Test]
        public void GetProjectByName_MissingProjectName_ThrowsNullException()
        {
            var sut = this.GetRepository();

            Assert.Throws<ArgumentNullException>(()=>sut.GetProjectByName(null));
        }

        [Test]
        public void GetProjectByName_InvalidProjectName_ThrowsRecordNotFoundException()
        {
            var sut = this.GetRepository();

            Assert.Throws<RecordNotFoundException>(() => sut.GetProjectByName(Guid.NewGuid().ToString()));
        }
    }
}
