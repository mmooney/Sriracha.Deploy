using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Impl;
using Sriracha.Deploy.Data.Repository;
using Sriracha.Deploy.Data.Tasks;

namespace Sriracha.Deploy.Data.Tests
{
	public class ProjectManagerTests
    {
		public class CreateProject
		{
			[Test]
			public void CanCreateProject()
			{
				var repository = new Mock<IProjectRepository>();
				IProjectManager sut = new ProjectManager(repository.Object);
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
			public void MissingProjectName_ThrowsError()
			{
				var repository = new Mock<IProjectRepository>();
				IProjectManager sut = new ProjectManager(repository.Object);
				Assert.Throws<ArgumentNullException>(delegate { sut.CreateProject(string.Empty); });
				repository.Verify(i => i.CreateProject(It.IsAny<string>()), Times.Never());
			}
		}

		public class GetProject
		{
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
				IProjectManager sut = new ProjectManager(repository.Object);
				DeployProject result = sut.GetProject(project.Id);
				Assert.AreEqual(project, result);
				repository.Verify(i=>i.GetProject(project.Id), Times.Once());
			}

			[Test]
			public void MissingId_ThrowsError()
			{
				var repository = new Mock<IProjectRepository>();
				IProjectManager sut = new ProjectManager(repository.Object);
				Assert.Throws<ArgumentNullException>(delegate { sut.GetProject(string.Empty); });
				repository.Verify(i => i.GetProject(It.IsAny<string>()), Times.Never());
			}

			[Test]
			public void BadId_ThrowsError()
			{
				var repository = new Mock<IProjectRepository>();
				var project = new DeployProject
				{
					ProjectName = Guid.NewGuid().ToString(),
					Id = Guid.NewGuid().ToString()
				};
				repository.Setup(i => i.GetProject(project.Id)).Returns((DeployProject)null);
				IProjectManager sut = new ProjectManager(repository.Object);
				Assert.Throws<KeyNotFoundException>(delegate { sut.GetProject(project.Id); });
				repository.Verify(i => i.GetProject(It.IsAny<string>()), Times.Once());
			}
		}

		public class CreateBranch
		{
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
				IProjectManager sut = new ProjectManager(repository.Object);
				var result = sut.CreateBranch(branch.ProjectId, branch.BranchName);
				Assert.AreEqual(branch, result);
				repository.Verify(i=>i.CreateBranch(branch.ProjectId, branch.BranchName), Times.Once());
			}

			[Test]
			public void MissingProjectID_ThrowsError()
			{
				var repository = new Mock<IProjectRepository>();
				string projectId = string.Empty;
				string branchName = Guid.NewGuid().ToString();
				IProjectManager sut = new ProjectManager(repository.Object);
				Assert.Throws<ArgumentNullException>(delegate { sut.CreateBranch(projectId, branchName); });
				repository.Verify(i => i.CreateBranch(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
			}

			[Test]
			public void MissingBranchName_ThrowsError()
			{
				var repository = new Mock<IProjectRepository>();
				string projectId = Guid.NewGuid().ToString();
				string branchName = string.Empty;
				IProjectManager sut = new ProjectManager(repository.Object);
				Assert.Throws<ArgumentNullException>(delegate { sut.CreateBranch(projectId, branchName); });
				repository.Verify(i => i.CreateBranch(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
			}
		}

		public class GetProjectList
		{	
			[Test]
			public void CanGetProjectList()
			{
				var repository = new Mock<IProjectRepository>();
				var projectList = new List<DeployProject>
				{
					new DeployProject { Id = Guid.NewGuid().ToString(), ProjectName = Guid.NewGuid().ToString() },
					new DeployProject { Id = Guid.NewGuid().ToString(), ProjectName = Guid.NewGuid().ToString() },
					new DeployProject { Id = Guid.NewGuid().ToString(), ProjectName = Guid.NewGuid().ToString() }
				};
				repository.Setup(i=>i.GetProjectList(null)).Returns(projectList);
				IProjectManager sut = new ProjectManager(repository.Object);
				var result = sut.GetProjectList();
				Assert.AreEqual(projectList.Count, result.Count());
				repository.Verify(i=>i.GetProjectList(null), Times.Once());
			}
		}

		public class DeleteProject
		{
			[Test]
			public void CanDeleteProject()
			{
				var repository = new Mock<IProjectRepository>();
				IProjectManager sut = new ProjectManager(repository.Object);
				string projectId = Guid.NewGuid().ToString();
				sut.DeleteProject(projectId);
				repository.Verify(i => i.DeleteProject(projectId), Times.Once());
			}

			[Test]
			public void MissingId_ThrowsError()
			{
				var repository = new Mock<IProjectRepository>();
				IProjectManager sut = new ProjectManager(repository.Object);
				Assert.Throws<ArgumentNullException>(delegate{ sut.DeleteProject(string.Empty); });
				repository.Verify(i=>i.DeleteProject(It.IsAny<string>()), Times.Never());
			}
		}

		public class UpdateProject
		{
			[Test]
			public void CanUpdateProject()
			{
				var repository = new Mock<IProjectRepository>();
				string projectId = Guid.NewGuid().ToString();
				string projectName = Guid.NewGuid().ToString();
				IProjectManager sut = new ProjectManager(repository.Object);
				sut.UpdateProject(projectId, projectName);
				repository.Verify(i=>i.UpdateProject(projectId, projectName), Times.Once());
			}

			[Test]
			public void MissingProjectId_ThrowsError()
			{
				var repository = new Mock<IProjectRepository>();
				string projectId = string.Empty;
				string projectName = Guid.NewGuid().ToString();
				IProjectManager sut = new ProjectManager(repository.Object);
				Assert.Throws<ArgumentNullException>(delegate { sut.UpdateProject(projectId, projectName); });
				repository.Verify(i => i.UpdateProject(projectId, projectName), Times.Never());
			}

			[Test]
			public void MissingProjectName_ThrowsError()
			{
				var repository = new Mock<IProjectRepository>();
				string projectId = Guid.NewGuid().ToString();
				string projectName = string.Empty;
				IProjectManager sut = new ProjectManager(repository.Object);
				Assert.Throws<ArgumentNullException>(delegate { sut.UpdateProject(projectId, projectName); });
				repository.Verify(i => i.UpdateProject(projectId, projectName), Times.Never());
			}

		}

		public class CreateDeploymentStep
		{
			private class TestTaskType : IDeployTask 
			{
				public IList<TaskParameter> GetStaticTaskParameterList()
				{
					throw new NotImplementedException();
				}

				public IList<TaskParameter> GetEnvironmentTaskParameterList()
				{
					throw new NotImplementedException();
				}

				public IList<TaskParameter> GetMachineTaskParameterList()
				{
					throw new NotImplementedException();
				}
			}

			[Test]
			public void CanCreateDeploymentStep()
			{
				string projectId = Guid.NewGuid().ToString();
				string componentId = Guid.NewGuid().ToString();
				string stepName = Guid.NewGuid().ToString();
				string taskTypeName = typeof(TestTaskType).ToString();
				string taskOptionsJson = Guid.NewGuid().ToString();
				var projectRepository = new Mock<IProjectRepository>();
				projectRepository.Setup(i=>i.CreateDeploymentStep(projectId, componentId, stepName, taskTypeName, taskOptionsJson))
									.Returns(new DeployComponentDeploymentStep
										{
											Id = Guid.NewGuid().ToString(),
											StepName = stepName,
											TaskTypeName = taskTypeName,
											TaskOptionsJson = taskOptionsJson
										});
				IProjectManager sut = new ProjectManager(projectRepository.Object);

				var result = sut.CreateDeploymentStep(projectId, componentId, stepName, taskTypeName, taskOptionsJson);

				Assert.IsNotNull(result);
				Assert.AreEqual(stepName, result.StepName);
				projectRepository.Verify(i=>i.CreateDeploymentStep(projectId, componentId, stepName, taskTypeName, taskOptionsJson), Times.Once());
			}

			[Test]
			public void MissingProjectId_ThrowsError()
			{
				string projectId = null;
				string componentId = Guid.NewGuid().ToString();
				string stepName = Guid.NewGuid().ToString();
				string taskTypeName = typeof(TestTaskType).ToString();
				string taskOptionsJson = Guid.NewGuid().ToString();
				var projectRepository = new Mock<IProjectRepository>();
				IProjectManager sut = new ProjectManager(projectRepository.Object);

				Assert.Throws<ArgumentNullException>(()=>sut.CreateDeploymentStep(projectId, componentId, stepName, taskTypeName, taskOptionsJson));
				projectRepository.Verify(i => i.CreateDeploymentStep(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never());
			}

			[Test]
			public void MissingComponentId_ThrowsError()
			{
				string projectId = Guid.NewGuid().ToString();
				string componentId = null;
				string stepName = Guid.NewGuid().ToString();
				string taskTypeName = typeof(TestTaskType).ToString();
				string taskOptionsJson = Guid.NewGuid().ToString();
				var projectRepository = new Mock<IProjectRepository>();
				IProjectManager sut = new ProjectManager(projectRepository.Object);

				Assert.Throws<ArgumentNullException>(() => sut.CreateDeploymentStep(projectId, componentId, stepName, taskTypeName, taskOptionsJson));
				projectRepository.Verify(i => i.CreateDeploymentStep(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never());
			}

			[Test]
			public void MissingStepName_ThrowsError()
			{
				string projectId = Guid.NewGuid().ToString();
				string componentId = Guid.NewGuid().ToString();
				string stepName = null;
				string taskTypeName = typeof(TestTaskType).ToString();
				string taskOptionsJson = Guid.NewGuid().ToString();
				var projectRepository = new Mock<IProjectRepository>();
				IProjectManager sut = new ProjectManager(projectRepository.Object);

				Assert.Throws<ArgumentNullException>(() => sut.CreateDeploymentStep(projectId, componentId, stepName, taskTypeName, taskOptionsJson));
				projectRepository.Verify(i => i.CreateDeploymentStep(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never());
			}

			[Test]
			public void MissingTaskTypeName_ThrowsError()
			{
				string projectId = Guid.NewGuid().ToString();
				string componentId = Guid.NewGuid().ToString();
				string stepName = Guid.NewGuid().ToString();
				string taskTypeName = null;
				string taskOptionsJson = Guid.NewGuid().ToString();
				var projectRepository = new Mock<IProjectRepository>();
				IProjectManager sut = new ProjectManager(projectRepository.Object);

				Assert.Throws<ArgumentNullException>(() => sut.CreateDeploymentStep(projectId, componentId, stepName, taskTypeName, taskOptionsJson));
				projectRepository.Verify(i => i.CreateDeploymentStep(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never());
			}


			[Test]
			public void MissingTaskOptions_ThrowsError()
			{
				string projectId = Guid.NewGuid().ToString();
				string componentId = Guid.NewGuid().ToString();
				string stepName = Guid.NewGuid().ToString();
				string taskTypeName = typeof(TestTaskType).ToString();
				string taskOptionsJson = null;
				var projectRepository = new Mock<IProjectRepository>();
				IProjectManager sut = new ProjectManager(projectRepository.Object);

				Assert.Throws<ArgumentNullException>(() => sut.CreateDeploymentStep(projectId, componentId, stepName, taskTypeName, taskOptionsJson));
				projectRepository.Verify(i => i.CreateDeploymentStep(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never());
			}
		}

		public class UpdateDeploymentStep
		{
			private class TestTaskType : IDeployTask
			{
				public IList<TaskParameter> GetStaticTaskParameterList()
				{
					throw new NotImplementedException();
				}

				public IList<TaskParameter> GetEnvironmentTaskParameterList()
				{
					throw new NotImplementedException();
				}

				public IList<TaskParameter> GetMachineTaskParameterList()
				{
					throw new NotImplementedException();
				}
			}

			[Test]
			public void CanUpdateDeploymentStep()
			{
				string projectId = Guid.NewGuid().ToString();
				string componentId = Guid.NewGuid().ToString();
				string deploymentStepId = Guid.NewGuid().ToString();
				string stepName = Guid.NewGuid().ToString();
				string taskTypeName = typeof(TestTaskType).ToString();
				string taskOptionsJson = Guid.NewGuid().ToString();
				var projectRepository = new Mock<IProjectRepository>();
				projectRepository.Setup(i => i.UpdateDeploymentStep(projectId, componentId, deploymentStepId, stepName, taskTypeName, taskOptionsJson))
									.Returns(new DeployComponentDeploymentStep
									{
										Id = deploymentStepId,
										StepName = stepName,
										TaskTypeName = taskTypeName,
										TaskOptionsJson = Guid.NewGuid().ToString()
									});
				IProjectManager sut = new ProjectManager(projectRepository.Object);

				var result = sut.UpdateDeploymentStep(projectId, componentId, deploymentStepId, stepName, taskTypeName, taskOptionsJson);

				Assert.IsNotNull(result);
				Assert.AreEqual(stepName, result.StepName);

				projectRepository.Verify(i => i.UpdateDeploymentStep(projectId, componentId, deploymentStepId, stepName, taskTypeName, taskOptionsJson), Times.Once());
			}

			[Test]
			public void MissingProjectId_ThrowsError()
			{
				string projectId = null;
				string componentId = Guid.NewGuid().ToString();
				string deploymentStepId = Guid.NewGuid().ToString();
				string stepName = Guid.NewGuid().ToString();
				string taskTypeName = typeof(TestTaskType).ToString();
				string taskOptionsJson = Guid.NewGuid().ToString();
				var projectRepository = new Mock<IProjectRepository>();
				IProjectManager sut = new ProjectManager(projectRepository.Object);

				Assert.Throws<ArgumentNullException>(() => sut.UpdateDeploymentStep(projectId, componentId, deploymentStepId, stepName, taskTypeName, taskOptionsJson));
				projectRepository.Verify(i => i.UpdateDeploymentStep(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never());
			}

			[Test]
			public void MissingComponentId_ThrowsError()
			{
				string projectId = Guid.NewGuid().ToString();
				string componentId = null;
				string deploymentStepId = Guid.NewGuid().ToString();
				string stepName = Guid.NewGuid().ToString();
				string taskTypeName = typeof(TestTaskType).ToString();
				string taskOptionsJson = Guid.NewGuid().ToString();
				var projectRepository = new Mock<IProjectRepository>();
				IProjectManager sut = new ProjectManager(projectRepository.Object);

				Assert.Throws<ArgumentNullException>(() => sut.UpdateDeploymentStep(projectId, componentId, deploymentStepId, stepName, taskTypeName, taskOptionsJson));
				projectRepository.Verify(i => i.UpdateDeploymentStep(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never());
			}

			[Test]
			public void MissingDeploymentStepId_ThrowsError()
			{
				string projectId = Guid.NewGuid().ToString();
				string componentId = Guid.NewGuid().ToString();
				string deploymentStepId = null;
				string stepName = Guid.NewGuid().ToString();
				string taskTypeName = typeof(TestTaskType).ToString();
				string taskOptionsJson = Guid.NewGuid().ToString();
				var projectRepository = new Mock<IProjectRepository>();
				IProjectManager sut = new ProjectManager(projectRepository.Object);

				Assert.Throws<ArgumentNullException>(() => sut.UpdateDeploymentStep(projectId, componentId, deploymentStepId, stepName, taskTypeName, taskOptionsJson));
				projectRepository.Verify(i => i.UpdateDeploymentStep(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never());
			}

			[Test]
			public void MissingStepName_ThrowsError()
			{
				string projectId = Guid.NewGuid().ToString();
				string componentId = Guid.NewGuid().ToString();
				string deploymentStepId = Guid.NewGuid().ToString();
				string stepName = null;
				string taskTypeName = typeof(TestTaskType).ToString();
				string taskOptionsJson = Guid.NewGuid().ToString();
				var projectRepository = new Mock<IProjectRepository>();
				IProjectManager sut = new ProjectManager(projectRepository.Object);

				Assert.Throws<ArgumentNullException>(() => sut.UpdateDeploymentStep(projectId, componentId, deploymentStepId, stepName, taskTypeName, taskOptionsJson));
				projectRepository.Verify(i => i.UpdateDeploymentStep(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never());
			}

			[Test]
			public void MissingTaskTypeName_ThrowsError()
			{
				string projectId = Guid.NewGuid().ToString();
				string componentId = Guid.NewGuid().ToString();
				string deploymentStepId = Guid.NewGuid().ToString();
				string stepName = Guid.NewGuid().ToString();
				string taskTypeName = null;
				string taskOptionsJson = Guid.NewGuid().ToString();
				var projectRepository = new Mock<IProjectRepository>();
				IProjectManager sut = new ProjectManager(projectRepository.Object);

				Assert.Throws<ArgumentNullException>(() => sut.UpdateDeploymentStep(projectId, componentId, deploymentStepId, stepName, taskTypeName, taskOptionsJson));
				projectRepository.Verify(i => i.UpdateDeploymentStep(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never());
			}


			[Test]
			public void MissingTaskOptions_ThrowsError()
			{
				string projectId = Guid.NewGuid().ToString();
				string componentId = Guid.NewGuid().ToString();
				string deploymentStepId = Guid.NewGuid().ToString();
				string stepName = Guid.NewGuid().ToString();
				string taskTypeName = typeof(TestTaskType).ToString();
				string taskOptionsJson = null;
				var projectRepository = new Mock<IProjectRepository>();
				IProjectManager sut = new ProjectManager(projectRepository.Object);

				Assert.Throws<ArgumentNullException>(() => sut.UpdateDeploymentStep(projectId, componentId, deploymentStepId, stepName, taskTypeName, taskOptionsJson));
				projectRepository.Verify(i => i.UpdateDeploymentStep(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never());
			}
		}

    }
}
