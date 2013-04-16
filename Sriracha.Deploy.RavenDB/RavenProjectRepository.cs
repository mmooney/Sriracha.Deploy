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

		public DeployProjectBranch CreateBranch(string projectId, string branchName)
		{
			if(string.IsNullOrEmpty(branchName))
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

		public DeployComponentDeploymentStep CreateDeploymentStep(string projectId, string componentId, string stepName, string taskTypeName, dynamic taskOptions) 
		{
			var project = GetProject(projectId);
			var component = project.ComponentList.Single(i => i.Id == componentId);
			var item = new DeployComponentDeploymentStep
			{
				Id = Guid.NewGuid().ToString(),
				StepName = stepName,
				TaskTypeName = taskTypeName,
				TaskOptionJSON = JsonConvert.SerializeObject(taskOptions)
			};
		
			component.DeploymentStepList.Add(item);
			this._documentSession.SaveChanges();
			return item;
		}

		public DeployComponentDeploymentStep UpdateDeploymentStep(string projectId, string componentId, string deploymentStepId, string stepName, string taskTypeName, dynamic taskOptions) 
		{
			var project = GetProject(projectId);
			var component = project.ComponentList.Single(i => i.Id == componentId);
			var item = component.DeploymentStepList.Single(i=>i.Id == deploymentStepId);
			item.StepName = stepName;
			item.TaskTypeName = taskTypeName;
			item.TaskOptionJSON = JsonConvert.SerializeObject(taskOptions);
			this._documentSession.SaveChanges();
			return item;
		}
	}
}
