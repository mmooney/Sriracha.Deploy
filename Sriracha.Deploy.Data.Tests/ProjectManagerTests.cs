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
using Sriracha.Deploy.Data.Project;
using Sriracha.Deploy.Data.Project.ProjectImpl;
using Sriracha.Deploy.Data.Dto.Project;

namespace Sriracha.Deploy.Data.Tests
{
	public class ProjectManagerTests
    {
        private class TestTaskType : IDeployTaskDefinition
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

            public void Execute(DeployEnvironmentConfiguration deployEnvironmentComponent, RuntimeSystemSettings runtimeSystemSettings)
            {
                throw new NotImplementedException();
            }


            public Type GetTaskExecutorType()
            {
                throw new NotImplementedException();
            }

            public string TaskDefintionName
            {
                get { throw new NotImplementedException(); }
            }

            public Type GetTaskOptionType()
            {
                throw new NotImplementedException();
            }

            public object DeployTaskOptions { get; set; }

            public IList<TaskParameter> GetBuildTaskParameterList()
            {
                throw new NotImplementedException();
            }


            public IList<TaskParameter> GetDeployTaskParameterList()
            {
                throw new NotImplementedException();
            }
        }
        
        public class CreateProject
		{
			[Test]
			public void CanCreateProject()
			{
				var repository = new Mock<IProjectRepository>();
				IProjectManager sut = new ProjectManager(repository.Object, new Mock<IDeployTaskFactory>().Object);
				var project = new DeployProject
				{
					ProjectName = Guid.NewGuid().ToString(),
					Id = Guid.NewGuid().ToString(),
					UsesSharedComponentConfiguration = false
				};
				repository.Setup(i=>i.CreateProject(project.ProjectName, project.UsesSharedComponentConfiguration)).Returns(project);
				DeployProject result = sut.CreateProject(project.ProjectName, project.UsesSharedComponentConfiguration);
				Assert.AreEqual(project, result);
				repository.Verify(i => i.CreateProject(project.ProjectName, project.UsesSharedComponentConfiguration), Times.Once());
			}

			[Test]
			public void MissingProjectName_ThrowsError()
			{
				var repository = new Mock<IProjectRepository>();
                IProjectManager sut = new ProjectManager(repository.Object, new Mock<IDeployTaskFactory>().Object);
				Assert.Throws<ArgumentNullException>(delegate { sut.CreateProject(string.Empty, false); });
				repository.Verify(i => i.CreateProject(It.IsAny<string>(), It.IsAny<bool>()), Times.Never());
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
                IProjectManager sut = new ProjectManager(repository.Object, new Mock<IDeployTaskFactory>().Object);
				DeployProject result = sut.GetProject(project.Id);
				Assert.AreEqual(project, result);
				repository.Verify(i=>i.GetProject(project.Id), Times.Once());
			}

			[Test]
			public void MissingId_ThrowsError()
			{
				var repository = new Mock<IProjectRepository>();
                IProjectManager sut = new ProjectManager(repository.Object, new Mock<IDeployTaskFactory>().Object);
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
                IProjectManager sut = new ProjectManager(repository.Object, new Mock<IDeployTaskFactory>().Object);
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
                IProjectManager sut = new ProjectManager(repository.Object, new Mock<IDeployTaskFactory>().Object);
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
                IProjectManager sut = new ProjectManager(repository.Object, new Mock<IDeployTaskFactory>().Object);
				Assert.Throws<ArgumentNullException>(delegate { sut.CreateBranch(projectId, branchName); });
				repository.Verify(i => i.CreateBranch(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
			}

			[Test]
			public void MissingBranchName_ThrowsError()
			{
				var repository = new Mock<IProjectRepository>();
				string projectId = Guid.NewGuid().ToString();
				string branchName = string.Empty;
                IProjectManager sut = new ProjectManager(repository.Object, new Mock<IDeployTaskFactory>().Object);
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
				repository.Setup(i=>i.GetProjectList()).Returns(projectList);
                IProjectManager sut = new ProjectManager(repository.Object, new Mock<IDeployTaskFactory>().Object);
				var result = sut.GetProjectList();
				Assert.AreEqual(projectList.Count, result.Count());
				repository.Verify(i=>i.GetProjectList(), Times.Once());
			}
		}

		public class DeleteProject
		{
			[Test]
			public void CanDeleteProject()
			{
				var repository = new Mock<IProjectRepository>();
                IProjectManager sut = new ProjectManager(repository.Object, new Mock<IDeployTaskFactory>().Object);
				string projectId = Guid.NewGuid().ToString();
				sut.DeleteProject(projectId);
				repository.Verify(i => i.DeleteProject(projectId), Times.Once());
			}

			[Test]
			public void MissingId_ThrowsError()
			{
				var repository = new Mock<IProjectRepository>();
                IProjectManager sut = new ProjectManager(repository.Object, new Mock<IDeployTaskFactory>().Object);
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
                IProjectManager sut = new ProjectManager(repository.Object, new Mock<IDeployTaskFactory>().Object);
				sut.UpdateProject(projectId, projectName, true);
				repository.Verify(i=>i.UpdateProject(projectId, projectName, true), Times.Once());
			}

			[Test]
			public void MissingProjectId_ThrowsError()
			{
				var repository = new Mock<IProjectRepository>();
				string projectId = string.Empty;
				string projectName = Guid.NewGuid().ToString();
                IProjectManager sut = new ProjectManager(repository.Object, new Mock<IDeployTaskFactory>().Object);
				Assert.Throws<ArgumentNullException>(delegate { sut.UpdateProject(projectId, projectName, true); });
				repository.Verify(i => i.UpdateProject(projectId, projectName, true), Times.Never());
			}

			[Test]
			public void MissingProjectName_ThrowsError()
			{
				var repository = new Mock<IProjectRepository>();
				string projectId = Guid.NewGuid().ToString();
				string projectName = string.Empty;
                IProjectManager sut = new ProjectManager(repository.Object, new Mock<IDeployTaskFactory>().Object);
				Assert.Throws<ArgumentNullException>(delegate { sut.UpdateProject(projectId, projectName, true); });
				repository.Verify(i => i.UpdateProject(projectId, projectName, true), Times.Never());
			}

		}

		public class CreateDeploymentStep
		{
			[Test]
			public void CanCreateDeploymentStep()
			{
				string projectId = Guid.NewGuid().ToString();
				string componentId = Guid.NewGuid().ToString();
				string stepName = Guid.NewGuid().ToString();
				string taskTypeName = typeof(TestTaskType).ToString();
				string taskOptionsJson = Guid.NewGuid().ToString();
				var projectRepository = new Mock<IProjectRepository>();
				var project = new DeployProject
									{
										Id = projectId,
										ComponentList = new List<DeployComponent>(),
										UsesSharedComponentConfiguration = false
									};
				projectRepository.Setup(i=>i.GetProject(projectId))
									.Returns(project);
				projectRepository.Setup(i=>i.CreateComponentDeploymentStep(project.Id, componentId, stepName, taskTypeName, taskOptionsJson, null))
									.Returns(new DeployStep
										{
											Id = Guid.NewGuid().ToString(),
											StepName = stepName,
											TaskTypeName = taskTypeName,
											TaskOptionsJson = taskOptionsJson,
											ProjectId = projectId,
											ParentId = componentId,
											SharedDeploymentStepId = Guid.NewGuid().ToString()
										});
                IProjectManager sut = new ProjectManager(projectRepository.Object, new Mock<IDeployTaskFactory>().Object);

				var result = sut.CreateComponentDeploymentStep(projectId, componentId, stepName, taskTypeName, taskOptionsJson);

				Assert.IsNotNull(result);
				Assert.AreEqual(stepName, result.StepName);
				projectRepository.Verify(i=>i.CreateComponentDeploymentStep(project.Id, componentId, stepName, taskTypeName, taskOptionsJson, null), Times.Once());
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
                IProjectManager sut = new ProjectManager(projectRepository.Object, new Mock<IDeployTaskFactory>().Object);

				Assert.Throws<ArgumentNullException>(()=>sut.CreateComponentDeploymentStep(projectId, componentId, stepName, taskTypeName, taskOptionsJson));
				projectRepository.Verify(i => i.CreateComponentDeploymentStep(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never());
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
                IProjectManager sut = new ProjectManager(projectRepository.Object, new Mock<IDeployTaskFactory>().Object);

				Assert.Throws<ArgumentNullException>(() => sut.CreateComponentDeploymentStep(projectId, componentId, stepName, taskTypeName, taskOptionsJson));
				projectRepository.Verify(i => i.CreateComponentDeploymentStep(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never());
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
                IProjectManager sut = new ProjectManager(projectRepository.Object, new Mock<IDeployTaskFactory>().Object);

				Assert.Throws<ArgumentNullException>(() => sut.CreateComponentDeploymentStep(projectId, componentId, stepName, taskTypeName, taskOptionsJson));
				projectRepository.Verify(i => i.CreateComponentDeploymentStep(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never());
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
                IProjectManager sut = new ProjectManager(projectRepository.Object, new Mock<IDeployTaskFactory>().Object);

				Assert.Throws<ArgumentNullException>(() => sut.CreateComponentDeploymentStep(projectId, componentId, stepName, taskTypeName, taskOptionsJson));
				projectRepository.Verify(i => i.CreateComponentDeploymentStep(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never());
			}


			[Test]
			public void MissingTaskOptions_CreatesTaskOptions()
			{
                //var project = new DeployProject
                //{
                //    ProjectName = Guid.NewGuid().ToString(),
                //    Id = Guid.NewGuid().ToString(),
                //    UsesSharedComponentConfiguration = false
                //};
                //string componentId = Guid.NewGuid().ToString();
                //string stepName = Guid.NewGuid().ToString();
                //string taskTypeName = typeof(TestTaskType).ToString();
                //string taskOptionsJson = null;
                //var projectRepository = new Mock<IProjectRepository>();
                //projectRepository.Setup(i => i.GetProject(project.Id)).Returns(project);
                //var deployTaskFactory = new Mock<IDeployTaskFactory>();
                //deployTaskFactory.Setup(i=>i.CreateTaskDefinition(taskTypeName, It.IsAny<string>())).Returns(new TestTaskType() { DeployTaskOptions = new object() });
                //IProjectManager sut = new ProjectManager(projectRepository.Object, deployTaskFactory.Object);
                
                //var result = sut.CreateComponentDeploymentStep(project.Id, componentId, stepName, taskTypeName, taskOptionsJson);

                //Assert.IsNotNull(result);
                //Assert.AreEqual(stepName, result.StepName);
                //projectRepository.Verify(i => i.CreateComponentDeploymentStep(project.Id, componentId, stepName, taskTypeName, It.IsAny<string>(), null), Times.Once());

                string projectId = Guid.NewGuid().ToString();
                string componentId = Guid.NewGuid().ToString();
                string stepName = Guid.NewGuid().ToString();
                string taskTypeName = typeof(TestTaskType).ToString();
                string taskOptionsJson = null;
                var projectRepository = new Mock<IProjectRepository>();
                var project = new DeployProject
                {
                    Id = projectId,
                    ComponentList = new List<DeployComponent>(),
                    UsesSharedComponentConfiguration = false
                };
                projectRepository.Setup(i => i.GetProject(projectId))
                                    .Returns(project);
                projectRepository.Setup(i => i.CreateComponentDeploymentStep(project.Id, componentId, stepName, taskTypeName, It.IsAny<string>(), null))
                                    .Returns(new DeployStep
                                    {
                                        Id = Guid.NewGuid().ToString(),
                                        StepName = stepName,
                                        TaskTypeName = taskTypeName,
                                        TaskOptionsJson = taskOptionsJson,
                                        ProjectId = projectId,
                                        ParentId = componentId,
                                        SharedDeploymentStepId = Guid.NewGuid().ToString()
                                    });
                var deployTaskFactory = new Mock<IDeployTaskFactory>();
                deployTaskFactory.Setup(i=>i.CreateTaskDefinition(taskTypeName, It.IsAny<string>())).Returns(new TestTaskType() { DeployTaskOptions = new object() });
                IProjectManager sut = new ProjectManager(projectRepository.Object, deployTaskFactory.Object);

                var result = sut.CreateComponentDeploymentStep(projectId, componentId, stepName, taskTypeName, taskOptionsJson);

                Assert.IsNotNull(result);
                Assert.AreEqual(stepName, result.StepName);
                projectRepository.Verify(i => i.CreateComponentDeploymentStep(project.Id, componentId, stepName, taskTypeName, It.IsAny<string>(), null), Times.Once());
            }
		}

		public class UpdateDeploymentStep
		{
			private class TestTaskType : IDeployTaskDefinition
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

				public Type GetTaskExecutorType()
				{
					throw new NotImplementedException();
				}

				public string TaskDefintionName
				{
					get { throw new NotImplementedException(); }
				}


				public Type GetTaskOptionType()
				{
					throw new NotImplementedException();
				}

				public object DeployTaskOptions
				{
					get { throw new NotImplementedException(); }
					set {  throw new NotImplementedException(); }
				}


				public IList<TaskParameter> GetBuildTaskParameterList()
				{
					throw new NotImplementedException();
				}


				public IList<TaskParameter> GetDeployTaskParameterList()
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
				var project = new DeployProject
				{
					Id = projectId,
					ComponentList = new List<DeployComponent>(),
					UsesSharedComponentConfiguration = false
				};
				projectRepository.Setup(i=>i.GetProject(projectId)).Returns(project);
				projectRepository.Setup(i => i.UpdateComponentDeploymentStep(deploymentStepId, project.Id, componentId, stepName, taskTypeName, taskOptionsJson, null, null))
									.Returns(new DeployStep
									{
										Id = deploymentStepId,
										StepName = stepName,
										TaskTypeName = taskTypeName,
										TaskOptionsJson = Guid.NewGuid().ToString(),
										ParentId = componentId,
										ProjectId = projectId,
										SharedDeploymentStepId = Guid.NewGuid().ToString(),
									});
                IProjectManager sut = new ProjectManager(projectRepository.Object, new Mock<IDeployTaskFactory>().Object);

				var result = sut.UpdateComponentDeploymentStep(deploymentStepId, projectId, componentId, stepName, taskTypeName, taskOptionsJson);

				Assert.IsNotNull(result);
				Assert.AreEqual(stepName, result.StepName);

				projectRepository.Verify(i => i.UpdateComponentDeploymentStep(deploymentStepId, project.Id, componentId, stepName, taskTypeName, taskOptionsJson,null, null), Times.Once());
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
                IProjectManager sut = new ProjectManager(projectRepository.Object, new Mock<IDeployTaskFactory>().Object);

				Assert.Throws<ArgumentNullException>(() => sut.UpdateComponentDeploymentStep(projectId, componentId, deploymentStepId, stepName, taskTypeName, taskOptionsJson));
				projectRepository.Verify(i => i.UpdateComponentDeploymentStep(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int?>()), Times.Never());
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
                IProjectManager sut = new ProjectManager(projectRepository.Object, new Mock<IDeployTaskFactory>().Object);

				Assert.Throws<ArgumentNullException>(() => sut.UpdateComponentDeploymentStep(projectId, componentId, deploymentStepId, stepName, taskTypeName, taskOptionsJson));
                projectRepository.Verify(i => i.UpdateComponentDeploymentStep(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int?>()), Times.Never());
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
                IProjectManager sut = new ProjectManager(projectRepository.Object, new Mock<IDeployTaskFactory>().Object);

				Assert.Throws<ArgumentNullException>(() => sut.UpdateComponentDeploymentStep(projectId, componentId, deploymentStepId, stepName, taskTypeName, taskOptionsJson));
                projectRepository.Verify(i => i.UpdateComponentDeploymentStep(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int?>()), Times.Never());
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
                IProjectManager sut = new ProjectManager(projectRepository.Object, new Mock<IDeployTaskFactory>().Object);

				Assert.Throws<ArgumentNullException>(() => sut.UpdateComponentDeploymentStep(projectId, componentId, deploymentStepId, stepName, taskTypeName, taskOptionsJson));
                projectRepository.Verify(i => i.UpdateComponentDeploymentStep(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int?>()), Times.Never());
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
                IProjectManager sut = new ProjectManager(projectRepository.Object, new Mock<IDeployTaskFactory>().Object);

				Assert.Throws<ArgumentNullException>(() => sut.UpdateComponentDeploymentStep(projectId, componentId, deploymentStepId, stepName, taskTypeName, taskOptionsJson));
                projectRepository.Verify(i => i.UpdateComponentDeploymentStep(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int?>()), Times.Never());
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
                IProjectManager sut = new ProjectManager(projectRepository.Object, new Mock<IDeployTaskFactory>().Object);

				Assert.Throws<ArgumentNullException>(() => sut.UpdateComponentDeploymentStep(projectId, componentId, deploymentStepId, stepName, taskTypeName, taskOptionsJson));
                projectRepository.Verify(i => i.UpdateComponentDeploymentStep(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int?>()), Times.Never());
			}
		}

    }
}
