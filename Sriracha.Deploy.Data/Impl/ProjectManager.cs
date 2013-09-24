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

		public DeployProject CreateProject(string projectName, bool usesSharedComponentConfiguration)
		{
			if(string.IsNullOrEmpty(projectName))
			{
				throw new ArgumentNullException("Missing Project Name");
			}
			var project = this._projectRepository.CreateProject(projectName, usesSharedComponentConfiguration);
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


		public DeployProject UpdateProject(string projectId, string projectName, bool usesSharedComponentConfiguration)
		{
			if(string.IsNullOrEmpty(projectId))
			{
				throw new ArgumentNullException("Missing Project ID");
			}
			if(string.IsNullOrEmpty(projectName)) 
			{
				throw new ArgumentNullException("Missing Project Name");
			}
			return this._projectRepository.UpdateProject(projectId, projectName, usesSharedComponentConfiguration);
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
			var project = this._projectRepository.GetProject(projectId);
			var returnValue = this._projectRepository.CreateDeploymentStep(project, componentId, stepName, taskTypeName, taskOptionsJson, null);
			if(project.UsesSharedComponentConfiguration)
			{
				foreach(var component in project.ComponentList)
				{
					if(component.Id != componentId)
					{
						this._projectRepository.CreateDeploymentStep(project, component.Id, stepName, taskTypeName, taskOptionsJson, returnValue.SharedDeploymentStepId);
					}
				}
			}
			return returnValue;
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
			var project = this._projectRepository.GetProject(projectId);
			var returnValue = this._projectRepository.UpdateDeploymentStep(deploymentStepId, project, componentId, stepName, taskTypeName, taskOptionsJson, null);
			if(project.UsesSharedComponentConfiguration)
			{ 
				foreach(var component in project.ComponentList)
				{
					this._projectRepository.UpdateDeploymentStep(null, projectId, component.Id, stepName, taskTypeName, taskOptionsJson, returnValue.SharedDeploymentStepId);
				}
			}
			return returnValue;
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


		public IEnumerable<DeployEnvironment> GetEnvironmentList(string projectId)
		{
			return this._projectRepository.GetEnvironmentList(projectId);
		}

		public DeployEnvironment CreateEnvironment(string projectId, string environmentName, IEnumerable<DeployEnvironmentComponent> componentList)
		{
			return this._projectRepository.CreateEnvironment(projectId, environmentName, componentList);
		}

		public DeployEnvironment GetEnvironment(string environmentId)
		{
			return this._projectRepository.GetEnvironment(environmentId);
		}

		public DeployEnvironment UpdateEnvironment(string environmentId, string projectId, string environmentName, IEnumerable<DeployEnvironmentComponent> componentList)
		{
			return this._projectRepository.UpdateEnvironment(environmentId, projectId, environmentName, componentList);
		}

		public void DeleteEnvironment(string environmentId)
		{
			this._projectRepository.DeleteEnvironment(environmentId);
		}


		public void UpdateMachineConfig(string machineId, string configName, string configValue)
		{
			var machine = this._projectRepository.GetMachine(machineId);
			if(machine.ConfigurationValueList == null)
			{
				machine.ConfigurationValueList = new Dictionary<string,string>();
			}
			this.UpdateConfig(machine.ConfigurationValueList, configName, configValue);
			this._projectRepository.UpdateMachine(machine.Id, machine.ProjectId, machine.EnvironmentId, machine.EnvironmentComponentId, machine.MachineName, machine.ConfigurationValueList);
		}

		private void UpdateConfig(Dictionary<string, string> configList, string configName, string configValue)
		{
			if (configList.ContainsKey(configName))
			{
				if (string.IsNullOrWhiteSpace(configValue))
				{
					configList.Remove(configName);
				}
				else
				{
					configList[configName] = configValue;
				}
			}
			else
			{
				if (!string.IsNullOrWhiteSpace(configValue))
				{
					configList.Add(configName, configValue);
				}
			}
		}

		public void UpdateEnvironmentComponentConfig(string environmentId, string componentId, string configName, string configValue)
		{
			var environment = this._projectRepository.GetEnvironment(environmentId);
			var component = environment.GetEnvironmentComponent(componentId);
			if (component.ConfigurationValueList == null)
			{
				component.ConfigurationValueList = new Dictionary<string, string>();
			}
			this.UpdateConfig(component.ConfigurationValueList, configName, configValue);
			this._projectRepository.UpdateEnvironment(environmentId, environment.ProjectId, environment.EnvironmentName, environment.ComponentList);
		}
	}
}
