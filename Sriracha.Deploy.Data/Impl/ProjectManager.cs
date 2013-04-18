using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Repository;

namespace Sriracha.Deploy.Data.Impl
{
	public class ProjectManager : IProjectManager
	{
		private readonly IProjectRepository _projectRepository;

		public ProjectManager(IProjectRepository projectRepository)
		{
			this._projectRepository = DIHelper.VerifyParameter(projectRepository);
		}

		public DeployProject CreateProject(string projectName)
		{
			if(string.IsNullOrEmpty(projectName))
			{
				throw new ArgumentNullException("Missing Project Name");
			}
			var project = this._projectRepository.CreateProject(projectName);
			this._projectRepository.CreateBranch(project.Id, "Trunk");
			return project;
		}

		public DeployProject GetProject(string projectId)
		{
			if(string.IsNullOrEmpty(projectId))
			{
				throw new ArgumentNullException("Missing Project ID");
			}
			var item = this._projectRepository.GetProject(projectId);
			if(item == null)
			{
				throw new KeyNotFoundException("No project found for ID: " + projectId);
			}
			return item;
		}

		public IEnumerable<DeployProject> GetProjectList()
		{
			return this._projectRepository.GetProjectList();
		}


		public void DeleteProject(string projectId)
		{
			if(string.IsNullOrEmpty(projectId))
			{
				throw new ArgumentNullException("Missing Project ID");
			}
			this._projectRepository.DeleteProject(projectId);
		}


		public DeployProject UpdateProject(string projectId, string projectName)
		{
			if(string.IsNullOrEmpty(projectId))
			{
				throw new ArgumentNullException("Missing Project ID");
			}
			if(string.IsNullOrEmpty(projectName)) 
			{
				throw new ArgumentNullException("Missing Project Name");
			}
			return this._projectRepository.UpdateProject(projectId, projectName);
		}

		public IEnumerable<DeployComponent> GetComponentList(string projectId)
		{
			return _projectRepository.GetComponentList(projectId);
		}

		public DeployComponent CreateComponent(string projectId, string componentName)
		{
			return this._projectRepository.CreateComponent(projectId, componentName);
		}

		public DeployComponent GetComponent(string componentId)
		{
			return _projectRepository.GetComponent(componentId);
		}

		public void DeleteComponent(string componentId)
		{
			_projectRepository.DeleteComponent(componentId);
		}

		public DeployComponent UpdateComponent(string componentId, string projectId, string componentName)
		{
			return this._projectRepository.UpdateComponent(componentId, projectId, componentName);
		}

		public List<DeployComponentDeploymentStep> GetDeploymentStepList(string componentId)
		{
			return this._projectRepository.GetDeploymentStepList(componentId);
		}

		public DeployComponentDeploymentStep CreateDeploymentStep(string projectId, string componentId, string stepName, string taskTypeName, string taskOptionsJson)
		{
			if(string.IsNullOrEmpty(projectId))
			{
				throw new ArgumentNullException("Missing Project Id");
			}
			if(string.IsNullOrEmpty(componentId))
			{
				throw new ArgumentNullException("Missing Component Id");
			}
			if(string.IsNullOrEmpty(stepName))
			{
				throw new ArgumentNullException("Missing Step Name");
			}
			if(string.IsNullOrEmpty(taskTypeName))
			{
				throw new ArgumentNullException("Missing Task Type Name");
			}
			if(string.IsNullOrEmpty(taskOptionsJson))
			{
				throw new ArgumentNullException("Missing Task Options");
			}
			return this._projectRepository.CreateDeploymentStep(projectId, componentId, stepName, taskTypeName, taskOptionsJson);
		}


		public DeployComponentDeploymentStep GetDeploymentStep(string deploymentStepId)
		{
			return this._projectRepository.GetDeploymentStep(deploymentStepId);
		}

		public DeployComponentDeploymentStep UpdateDeploymentStep(string deploymentStepId, string projectId, string componentId, string stepName, string taskTypeName, string taskOptionsJson)
		{
			if (string.IsNullOrEmpty(projectId))
			{
				throw new ArgumentNullException("Missing Project Id");
			}
			if (string.IsNullOrEmpty(componentId))
			{
				throw new ArgumentNullException("Missing Component Id");
			}
			if(string.IsNullOrEmpty(deploymentStepId))
			{
				throw new ArgumentNullException("Missing Deployment Step Id");
			}
			if (string.IsNullOrEmpty(stepName))
			{
				throw new ArgumentNullException("Missing Step Name");
			}
			if (string.IsNullOrEmpty(taskTypeName))
			{
				throw new ArgumentNullException("Missing Task Type Name");
			}
			if (string.IsNullOrEmpty(taskOptionsJson))
			{
				throw new ArgumentNullException("Missing Task Options");
			}
			return this._projectRepository.UpdateDeploymentStep(deploymentStepId, projectId, componentId, stepName, taskTypeName, taskOptionsJson);
		}

		public void DeleteDeploymentStep(string deploymentStepId)
		{
			this._projectRepository.DeleteDeploymentStep(deploymentStepId);
		}

		public IEnumerable<DeployProjectBranch> GetBranchList(string projectId)
		{
			return this._projectRepository.GetBranchList(projectId);
		}


		public DeployProjectBranch CreateBranch(string projectId, string branchName)
		{
			if (string.IsNullOrEmpty(projectId))
			{
				throw new ArgumentNullException("Missing Project ID");
			}
			if (string.IsNullOrEmpty(branchName))
			{
				throw new ArgumentNullException("Missing Branch Name");
			}
			return this._projectRepository.CreateBranch(projectId, branchName);
		}

		public DeployProjectBranch GetBranch(string branchId)
		{
			return this._projectRepository.GetBranch(branchId);
		}

		public DeployProjectBranch UpdateBranch(string branchId, string projectId, string branchName)
		{
			return this._projectRepository.UpdateBranch(branchId, projectId, branchName);
		}

		public void DeleteBranch(string branchId)
		{
			this._projectRepository.DeleteBranch(branchId);
		}
	}
}
