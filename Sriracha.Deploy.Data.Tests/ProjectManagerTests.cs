using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Repository;

namespace Sriracha.Deploy.Data.Tests
{
	public class ProjectManagerTests
    {
		[Test]
		public void CanCreateProject()
		{
			var repository = new Mock<IProjectRepository>();
			var sut = new ProjectManager(repository.Object);
			var project = new DeployProject
			{
				ProjectName = Guid.NewGuid().ToString(),
				Id = Guid.NewGuid().ToString()
			};
			repository.Setup(i=>i.CreateProject(project.ProjectName)).Returns(project);
			DeployProject result = sut.CreateProject(project.ProjectName);
			Assert.AreEqual(project, result);
			repository.Verify(i => i.CreateProject(project.ProjectName), Times.Once());
		}

		[Test]
		public void CanRetrieveProjectByID()
		{
			var repository = new Mock<IProjectRepository>();
			var project = new DeployProject
			{
				ProjectName = Guid.NewGuid().ToString(),
				Id = Guid.NewGuid().ToString()
			};
			repository.Setup(i=>i.GetProject(project.Id)).Returns(project);
			var sut = new ProjectManager(repository.Object);
			DeployProject result = sut.GetProject(project.Id);
			Assert.AreEqual(project, result);
			repository.Verify(i=>i.GetProject(project.Id), Times.Once());
		}

		[Test]
		public void CanCreateProjectBranch()
		{
			var repository = new Mock<IProjectRepository>();
			string projectId = Guid.NewGuid().ToString();
			var branch = new DeployProjectBranch
			{
				Id = Guid.NewGuid().ToString(),
				ProjectId = Guid.NewGuid().ToString(),
				BranchName = Guid.NewGuid().ToString()
			};
			repository.Setup(i=>i.CreateBranch(branch.ProjectId, branch.BranchName)).Returns(branch);
			var sut = new ProjectManager(repository.Object);
			var result = sut.CreateBranch(branch.ProjectId, branch.BranchName);
			Assert.AreEqual(branch, result);
			repository.Verify(i=>i.CreateBranch(branch.ProjectId, branch.BranchName));
		}
    }
}
