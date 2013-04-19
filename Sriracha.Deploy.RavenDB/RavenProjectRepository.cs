using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client;
using Raven.Imports.Newtonsoft.Json;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Repository;

namespace Sriracha.Deploy.RavenDB
{
	public class RavenProjectRepository : IProjectRepository
	{
		private readonly IDocumentSession _documentSession;

		public RavenProjectRepository(IDocumentSession documentSession)
		{
			_documentSession = DIHelper.VerifyParameter(documentSession);
		}

		public IEnumerable<DeployProject> GetProjectList(string[] idList = null)
		{
			if(idList != null && idList.Any())
			{
				return _documentSession.Query<DeployProject>().Where(i=>idList.Contains(i.Id));
			}
			else 
			{
				return _documentSession.Query<DeployProject>();
			}
		}

		public DeployProject CreateProject(string projectName)
		{
			if(string.IsNullOrEmpty(projectName))
			{
				throw new ArgumentNullException("Missing Project Name");
			}
			var project = new DeployProject
			{
				Id = Guid.NewGuid().ToString(),
				ProjectName = projectName
			};
			_documentSession.Store(project);
			_documentSession.SaveChanges();
			return project;
		}

		public DeployProject GetProject(string projectId)
		{
			if(string.IsNullOrEmpty(projectId))
			{
				throw new ArgumentNullException("Missing Project ID");
			}
			var item = _documentSession.Load<DeployProject>(projectId);
			if(item == null)
			{
				throw new KeyNotFoundException("Project not found: " + projectId);
			}
			return item;
		}

		public void DeleteProject(string projectId)
		{
			var item = this.GetProject(projectId);
			this._documentSession.Delete(item);
			this._documentSession.SaveChanges();
		}


		public DeployProject UpdateProject(string projectId, string projectName)
		{
			if(string.IsNullOrEmpty(projectName))
			{
				throw new ArgumentNullException("Missing Project Name");
			}
			var item = this.GetProject(projectId);
			item.ProjectName = projectName;
			this._documentSession.SaveChanges();
			return item;
		}


		public IEnumerable<DeployComponent> GetComponentList(string projectId)
		{
			var project = GetProject(projectId);
			return project.ComponentList;
		}

		public DeployComponent CreateComponent(string projectId, string componentName)
		{
			var project = GetProject(projectId);
			var item = new DeployComponent
			{
				Id = Guid.NewGuid().ToString(),
				ProjectId = projectId,
				ComponentName = componentName
			};
			project.ComponentList.Add(item);
			this._documentSession.SaveChanges();
			return item;
		}

		public DeployComponent GetComponent(string componentId)
		{
			if(string.IsNullOrEmpty(componentId))
			{
				throw new ArgumentNullException("Missing Component ID");
			}
			var project = this._documentSession.Query<DeployProject>().SingleOrDefault(i=>i.ComponentList.Any(j=>j.Id == componentId));
			if(project == null)
			{
				throw new KeyNotFoundException("No project found for component ID " + componentId);
			}
			return project.ComponentList.First(i=>i.Id == componentId);
		}

		public DeployComponent UpdateComponent(string componentId, string projectId, string componentName)
		{
			var project = GetProject(projectId);
			var item = project.ComponentList.Single(i=>i.Id == componentId);
			item.ComponentName = componentName;
			this._documentSession.SaveChanges();
			return item;
		}

		public void DeleteComponent(string componentId)
		{
			if (string.IsNullOrEmpty(componentId))
			{
				throw new ArgumentNullException("Missing Component ID");
			}
			var project = this._documentSession.Query<DeployProject>().SingleOrDefault(i => i.ComponentList.Any(j => j.Id == componentId));
			if (project == null)
			{
				throw new KeyNotFoundException("No project found for component ID " + componentId);
			}
			var component = project.ComponentList.First(i => i.Id == componentId);
			project.ComponentList.Remove(component);
			this._documentSession.SaveChanges();
		}

		public List<DeployComponentDeploymentStep> GetDeploymentStepList(string componentId)
		{
			var component = GetComponent(componentId);
			return component.DeploymentStepList;
		}

		public DeployComponentDeploymentStep CreateDeploymentStep(string projectId, string componentId, string stepName, string taskTypeName, string taskOptionsJson) 
		{
			var project = GetProject(projectId);
			var component = project.ComponentList.Single(i => i.Id == componentId);
			var item = new DeployComponentDeploymentStep
			{
				Id = Guid.NewGuid().ToString(),
				ProjectId = projectId,
				ComponentId = componentId,
				StepName = stepName,
				TaskTypeName = taskTypeName,
				TaskOptionsJson = taskOptionsJson
			};
			
			if(component.DeploymentStepList == null)
			{
				component.DeploymentStepList = new List<DeployComponentDeploymentStep>();
			}
			component.DeploymentStepList.Add(item);
			this._documentSession.SaveChanges();
			return item;
		}

		public DeployComponentDeploymentStep GetDeploymentStep(string deploymentStepId)
		{
			var project = this._documentSession.Query<DeployProject>().SingleOrDefault(i=>i.ComponentList.Any(j=>j.DeploymentStepList.Any(k=>k.Id == deploymentStepId)));
			if(project == null)
			{
				throw new KeyNotFoundException("Could not find project for deployment step " + deploymentStepId);
			}
			var item = project.ComponentList.Single(i=>i.DeploymentStepList.Any(j=>j.Id == deploymentStepId)).DeploymentStepList.First(i=>i.Id == deploymentStepId);
			return item;
		}

		public DeployComponentDeploymentStep UpdateDeploymentStep(string deploymentStepId, string projectId, string componentId, string stepName, string taskTypeName, string taskOptionsJson) 
		{
			var project = GetProject(projectId);
			var component = project.ComponentList.Single(i => i.Id == componentId);
			var item = component.DeploymentStepList.Single(i=>i.Id == deploymentStepId);
			item.StepName = stepName;
			item.TaskTypeName = taskTypeName;
			item.TaskOptionsJson = taskOptionsJson;
			this._documentSession.SaveChanges();
			return item;
		}


		public void DeleteDeploymentStep(string deploymentStepId)
		{
			var project = this._documentSession.Query<DeployProject>().SingleOrDefault(i => i.ComponentList.Any(j => j.DeploymentStepList.Any(k => k.Id == deploymentStepId)));
			if (project == null)
			{
				throw new KeyNotFoundException("Could not find project for deployment step " + deploymentStepId);
			}
			var component = project.ComponentList.Single(i => i.DeploymentStepList.Any(j => j.Id == deploymentStepId));
			var item = component.DeploymentStepList.First(i => i.Id == deploymentStepId);
			component.DeploymentStepList.Remove(item);
			this._documentSession.SaveChanges();

		}


		public IEnumerable<DeployProjectBranch> GetBranchList(string projectId)
		{
			if(string.IsNullOrEmpty(projectId))
			{
				throw new ArgumentNullException("Missing project ID");
			}
			var project = GetProject(projectId);
			return project.BranchList;
		}

		public DeployProjectBranch CreateBranch(string projectId, string branchName)
		{
			if (string.IsNullOrEmpty(branchName))
			{
				throw new ArgumentNullException("Missing Branch Name");
			}
			var project = this.GetProject(projectId);
			var branch = new DeployProjectBranch
			{
				Id = Guid.NewGuid().ToString(),
				BranchName = branchName,
				ProjectId = projectId
			};
			project.BranchList.Add(branch);
			this._documentSession.SaveChanges();
			return branch;
		}


		public DeployProjectBranch GetBranch(string branchId)
		{
			if(string.IsNullOrEmpty(branchId))
			{
				throw new ArgumentNullException("Missing branch ID");
			}
			var project = this._documentSession.Query<DeployProject>().FirstOrDefault(i=>i.BranchList.Any(j=>j.Id == branchId));
			if(project == null)
			{
				throw new ArgumentException("Unable to find project for branch ID " + branchId);
			}
			return project.BranchList.First(i=>i.Id == branchId);
		}

		public DeployProjectBranch GetBranch(DeployProject project, string branchId)
		{
			if(string.IsNullOrEmpty(branchId))
			{
				throw new ArgumentNullException("Missing branch ID");
			}
			if(project == null)
			{
				throw new ArgumentNullException("Project is null");
			}
			var branch = project.BranchList.FirstOrDefault(i => i.Id == branchId);
			if (branchId == null)
			{
				throw new ArgumentException("Unable to find branch " + branchId + " in project " + project.Id);
			}
			return branch;	
		}

		public DeployProjectBranch UpdateBranch(string branchId, string projectId, string branchName)
		{
			if(string.IsNullOrEmpty(branchId)) 
			{
				throw new ArgumentNullException("Missing branch ID");
			}
			if(string.IsNullOrEmpty(branchName)) 
			{
				throw new ArgumentNullException("Missing branch name");
			}
			var project = GetProject(projectId);
			var branch = project.BranchList.FirstOrDefault(i=>i.Id == branchId);
			if(branchId == null)
			{
				throw new ArgumentException("Unable to find branch " + branchId + " in project " + projectId);
			}
			branch.BranchName = branchName;
			this._documentSession.SaveChanges();
			return branch;
		}

		public void DeleteBranch(string branchId)
		{
			if(string.IsNullOrEmpty(branchId))
			{
				throw new ArgumentNullException("Missing branch ID");
			}
			var project = this._documentSession.Query<DeployProject>().FirstOrDefault(i=>i.BranchList.Any(j=>j.Id == branchId));
			if(project == null)
			{
				throw new ArgumentException("Unable to find project for branch ID " + branchId);
			}
			var branch = project.BranchList.First(i=>i.Id == branchId);
			project.BranchList.Remove(branch);
			this._documentSession.SaveChanges();
		}


		public IEnumerable<DeployEnvironment> GetEnvironmentList(string projectId)
		{
			if(string.IsNullOrEmpty(projectId))
			{
				throw new ArgumentNullException("Missing project ID");
			}
			var project = GetProject(projectId);
			return project.EnvironmentList;
		}

		public DeployEnvironment CreateEnvironment(string projectId, string environmentName)
		{
			if(string.IsNullOrEmpty(projectId))
			{
				throw new ArgumentNullException("Missing project ID");
			}
			if(string.IsNullOrEmpty(environmentName))
			{
				throw new ArgumentNullException("Missing environment name");
			}
			var project = GetProject(projectId);
			var environment = new DeployEnvironment
			{
				Id = Guid.NewGuid().ToString(),
				ProjectId = projectId,
				EnvironmentName = environmentName
			};
			if(project.EnvironmentList == null)
			{
				project.EnvironmentList = new List<DeployEnvironment>();
			}
			project.EnvironmentList.Add(environment);
			this._documentSession.SaveChanges();
			return environment;
		}

		public DeployEnvironment GetEnvironment(string environmentId)
		{
			if (string.IsNullOrEmpty(environmentId))
			{
				throw new ArgumentNullException("Missing environment ID");
			}
			var project = this._documentSession.Query<DeployProject>().FirstOrDefault(i=>i.EnvironmentList.Any(j=>j.Id == environmentId));
			if(project == null)
			{
				throw new ArgumentException("Unable to find project for environment ID " + environmentId);
			}
			var environment = project.EnvironmentList.First(i=>i.Id == environmentId);
			return environment;
		}

		public DeployEnvironment UpdateEnvironment(string environmentId, string projectId, string environmentName)
		{
			if (string.IsNullOrEmpty(projectId))
			{
				throw new ArgumentNullException("Missing project ID");
			}
			if (string.IsNullOrEmpty(environmentId))
			{
				throw new ArgumentNullException("Missing environment ID");
			}
			if (string.IsNullOrEmpty(environmentName))
			{
				throw new ArgumentNullException("Missing environment name");
			}
			var project = this._documentSession.Query<DeployProject>().FirstOrDefault(i => i.EnvironmentList.Any(j => j.Id == environmentId));
			if (project == null)
			{
				throw new ArgumentException("Unable to find project for environment ID " + environmentId);
			}
			var environment = project.EnvironmentList.First(i => i.Id == environmentId);
			environment.EnvironmentName = environmentName;
			this._documentSession.SaveChanges();
			return environment;
		}

		public void DeleteEnvironment(string environmentId)
		{
			if (string.IsNullOrEmpty(environmentId))
			{
				throw new ArgumentNullException("Missing environment ID");
			}
			var project = this._documentSession.Query<DeployProject>().FirstOrDefault(i => i.EnvironmentList.Any(j => j.Id == environmentId));
			if (project == null)
			{
				throw new ArgumentException("Unable to find project for environment ID " + environmentId);
			}
			var environment = project.EnvironmentList.First(i => i.Id == environmentId);
			project.EnvironmentList.Remove(environment);
			this._documentSession.SaveChanges();
		}

	}
}
