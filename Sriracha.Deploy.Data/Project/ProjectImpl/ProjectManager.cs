﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Repository;
using Sriracha.Deploy.Data.Dto.Project;
using Sriracha.Deploy.Data.Tasks;
using MMDB.Shared;

namespace Sriracha.Deploy.Data.Project.ProjectImpl
{
	public class ProjectManager : IProjectManager
	{
		private readonly IProjectRepository _projectRepository;
        private readonly IDeployTaskFactory _deployTaskFactory;

		public ProjectManager(IProjectRepository projectRepository, IDeployTaskFactory deployTaskFactory)
		{
			_projectRepository = DIHelper.VerifyParameter(projectRepository);
            _deployTaskFactory = DIHelper.VerifyParameter(deployTaskFactory);
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

        public DeployComponent CreateComponent(string projectId, string componentName, bool useConfigurationGroup, string configurationId, EnumDeploymentIsolationType isolationType)
		{
			var project = this._projectRepository.GetProject(projectId);
			var returnValue = this._projectRepository.CreateComponent(projectId, componentName, useConfigurationGroup, configurationId, isolationType);
			if(project.UsesSharedComponentConfiguration)
			{
				var someOtherComponent = project.ComponentList.FirstOrDefault(i=>i.Id != returnValue.Id);
				if(someOtherComponent != null)
				{
					foreach(var deploymentStep in someOtherComponent.DeploymentStepList)
					{
						this._projectRepository.CreateComponentDeploymentStep(project.Id, returnValue.Id, deploymentStep.StepName, deploymentStep.TaskTypeName, deploymentStep.TaskOptionsJson, deploymentStep.SharedDeploymentStepId);
					}
				}
			}
			return returnValue;
		}

		public DeployComponent GetComponent(string componentId, string projectId)
		{
			return _projectRepository.GetComponent(componentId, projectId);
		}

		public void DeleteComponent(string projectId, string componentId)
		{
			_projectRepository.DeleteComponent(projectId, componentId);
		}

        public DeployComponent UpdateComponent(string componentId, string projectId, string componentName, bool useConfigurationGroup, string configurationId, EnumDeploymentIsolationType isolationType)
		{
			return this._projectRepository.UpdateComponent(componentId, projectId, componentName, useConfigurationGroup, configurationId, isolationType);
		}

		public List<DeployStep> GetComponentDeploymentStepList(string componentId, string projectId)
		{
			return this._projectRepository.GetComponentDeploymentStepList(componentId, projectId);
		}
        public List<DeployStep> GetConfigurationDeploymentStepList(string configurationId, string projectId)
		{
			return this._projectRepository.GetConfigurationDeploymentStepList(configurationId, projectId);
		}

		public DeployStep CreateComponentDeploymentStep(string projectId, string componentId, string stepName, string taskTypeName, string taskOptionsJson)
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
                var taskDefinition = _deployTaskFactory.CreateTaskDefinition(taskTypeName, "{}");
                taskOptionsJson = taskDefinition.DeployTaskOptions.ToJson();
			}
			var project = this._projectRepository.GetProject(projectId);
			var returnValue = this._projectRepository.CreateComponentDeploymentStep(project.Id, componentId, stepName, taskTypeName, taskOptionsJson, null);
			if(project.UsesSharedComponentConfiguration)
			{
				foreach(var component in project.ComponentList)
				{
					if(component.Id != componentId)
					{
						this._projectRepository.CreateComponentDeploymentStep(project.Id, component.Id, stepName, taskTypeName, taskOptionsJson, returnValue.SharedDeploymentStepId);
					}
				}
			}
			return returnValue;
		}

		public DeployStep CreateConfigurationDeploymentStep(string projectId, string configurationId, string stepName, string taskTypeName, string taskOptionsJson)
		{
			if(string.IsNullOrEmpty(projectId))
			{
				throw new ArgumentNullException("Missing Project Id");
			}
			if(string.IsNullOrEmpty(configurationId))
			{
				throw new ArgumentNullException("Missing Configuration Id");
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
			return this._projectRepository.CreateConfigurationDeploymentStep(projectId, configurationId, stepName, taskTypeName, taskOptionsJson);
		}

        public DeployStep GetComponentDeploymentStep(string deploymentStepId, string projectId)
		{
			return this._projectRepository.GetComponentDeploymentStep(deploymentStepId, projectId);
		}
        public DeployStep GetConfigurationDeploymentStep(string deploymentStepId, string projectId)
		{
			return this._projectRepository.GetConfigurationDeploymentStep(deploymentStepId, projectId);
		}

		public DeployStep UpdateComponentDeploymentStep(string deploymentStepId, string projectId, string componentId, string stepName, string taskTypeName, string taskOptionsJson, int? orderNumber=null)
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
			var returnValue = this._projectRepository.UpdateComponentDeploymentStep(deploymentStepId, projectId, componentId, stepName, taskTypeName, taskOptionsJson, null, orderNumber);
			if(project.UsesSharedComponentConfiguration)
			{ 
				foreach(var component in project.ComponentList)
				{
					this._projectRepository.UpdateComponentDeploymentStep(null, projectId, component.Id, stepName, taskTypeName, taskOptionsJson, returnValue.SharedDeploymentStepId);
				}
			}
			return returnValue;
		}
		public DeployStep UpdateConfigurationDeploymentStep(string deploymentStepId, string projectId, string configurationId, string stepName, string taskTypeName, string taskOptionsJson, int? orderNumber=null)
		{
			if (string.IsNullOrEmpty(projectId))
			{
				throw new ArgumentNullException("Missing Project Id");
			}
			if (string.IsNullOrEmpty(configurationId))
			{
				throw new ArgumentNullException("Missing Configuration Id");
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
			var returnValue = this._projectRepository.UpdateConfigurationDeploymentStep(deploymentStepId, projectId, configurationId, stepName, taskTypeName, taskOptionsJson, orderNumber);
			return returnValue;
		}

        public void DeleteComponentDeploymentStep(string deploymentStepId, string projectId)
		{
			this._projectRepository.DeleteComponentDeploymentStep(deploymentStepId, projectId);
		}
        public void DeleteConfigurationDeploymentStep(string deploymentStepId, string projectId)
		{
			this._projectRepository.DeleteConfigurationDeploymentStep(deploymentStepId, projectId);
		}

        public DeployStep MoveDeploymentStepUp(string projectId, EnumDeployStepParentType parentType, string parentId, string deployStepId)
        {
            List<DeployStep> stepList;
            switch(parentType)
            {
                case EnumDeployStepParentType.Component:
                    stepList = _projectRepository.GetComponentDeploymentStepList(parentId, projectId);
                    break;
                case EnumDeployStepParentType.Configuration:
                    stepList = _projectRepository.GetConfigurationDeploymentStepList(parentId, projectId);
                    break;
                default:
                    throw new UnknownEnumValueException(parentType);
            }
            var itemToMove = stepList.FirstOrDefault(i=>i.Id == deployStepId);
            if(itemToMove == null)
            {
                throw new ArgumentException("Step not found: " + deployStepId);
            }
            stepList = stepList.OrderBy(i=>i.OrderNumber).ToList();
            if(itemToMove == stepList.First())
            {
                return itemToMove;
            }
            for(int i = 0; i < stepList.Count; i++)
            {
                stepList[i].OrderNumber = i;
            }
            itemToMove.OrderNumber--;
            stepList[itemToMove.OrderNumber].OrderNumber++;
            foreach(var step in stepList)
            {
                switch(parentType)
                {
                    case EnumDeployStepParentType.Component:
                        _projectRepository.UpdateComponentDeploymentStep(step.Id, step.ProjectId, step.ParentId, step.StepName, step.TaskTypeName, step.TaskOptionsJson, step.SharedDeploymentStepId, step.OrderNumber);
                        break;
                    case EnumDeployStepParentType.Configuration:
                        _projectRepository.UpdateConfigurationDeploymentStep(step.Id, step.ProjectId, step.ParentId, step.StepName, step.TaskTypeName, step.TaskOptionsJson, step.OrderNumber);
                        break;
                    default:
                        throw new UnknownEnumValueException(parentType);
                }
            }
            return itemToMove;
        }

        public DeployStep MoveDeploymentStepDown(string projectId, EnumDeployStepParentType parentType, string parentId, string deployStepId)
        {
            List<DeployStep> stepList;
            switch (parentType)
            {
                case EnumDeployStepParentType.Component:
                    stepList = _projectRepository.GetComponentDeploymentStepList(parentId, projectId);
                    break;
                case EnumDeployStepParentType.Configuration:
                    stepList = _projectRepository.GetConfigurationDeploymentStepList(parentId, projectId);
                    break;
                default:
                    throw new UnknownEnumValueException(parentType);
            }
            var itemToMove = stepList.FirstOrDefault(i => i.Id == deployStepId);
            if (itemToMove == null)
            {
                throw new ArgumentException("Step not found: " + deployStepId);
            }
            stepList = stepList.OrderBy(i => i.OrderNumber).ToList();
            if (itemToMove == stepList.Last())
            {
                return itemToMove;
            }
            for (int i = 0; i < stepList.Count; i++)
            {
                stepList[i].OrderNumber = i;
            }
            itemToMove.OrderNumber++;
            stepList[itemToMove.OrderNumber].OrderNumber--;
            foreach (var step in stepList)
            {
                switch (parentType)
                {
                    case EnumDeployStepParentType.Component:
                        _projectRepository.UpdateComponentDeploymentStep(step.Id, step.ProjectId, step.ParentId, step.StepName, step.TaskTypeName, step.TaskOptionsJson, step.SharedDeploymentStepId, step.OrderNumber);
                        break;
                    case EnumDeployStepParentType.Configuration:
                        _projectRepository.UpdateConfigurationDeploymentStep(step.Id, step.ProjectId, step.ParentId, step.StepName, step.TaskTypeName, step.TaskOptionsJson, step.OrderNumber);
                        break;
                    default:
                        throw new UnknownEnumValueException(parentType);
                }
            }
            return itemToMove;
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

		public DeployProjectBranch GetBranch(string branchId, string projectId)
		{
			return this._projectRepository.GetBranch(branchId, projectId);
		}

		public DeployProjectBranch UpdateBranch(string branchId, string projectId, string branchName)
		{
			return this._projectRepository.UpdateBranch(branchId, projectId, branchName);
		}

		public void DeleteBranch(string branchId, string projectId)
		{
			this._projectRepository.DeleteBranch(branchId, projectId);
		}


		public IEnumerable<DeployEnvironment> GetEnvironmentList(string projectId)
		{
			return this._projectRepository.GetEnvironmentList(projectId);
		}

		public DeployEnvironment CreateEnvironment(string projectId, string environmentName, IEnumerable<DeployEnvironmentConfiguration> componentList, IEnumerable<DeployEnvironmentConfiguration> configurationList)
		{
			return this._projectRepository.CreateEnvironment(projectId, environmentName, componentList, configurationList);
		}

        public DeployEnvironment GetEnvironment(string environmentId, string projectId)
		{
			return this._projectRepository.GetEnvironment(environmentId, projectId);
		}

		public DeployEnvironment UpdateEnvironment(string environmentId, string projectId, string environmentName, IEnumerable<DeployEnvironmentConfiguration> componentList, IEnumerable<DeployEnvironmentConfiguration> configurationList)
		{
			return this._projectRepository.UpdateEnvironment(environmentId, projectId, environmentName, componentList, configurationList);
		}

		public void DeleteEnvironment(string environmentId, string projectId)
		{
			this._projectRepository.DeleteEnvironment(environmentId, projectId);
		}


        //public void UpdateMachineConfig(string machineId, string configName, string configValue)
        //{
        //    var machine = this._projectRepository.GetMachine(machineId);
        //    if(machine.ConfigurationValueList == null)
        //    {
        //        machine.ConfigurationValueList = new Dictionary<string,string>();
        //    }
        //    this.UpdateConfig(machine.ConfigurationValueList, configName, configValue);
        //    this._projectRepository.UpdateMachine(machine.Id, machine.ProjectId, machine.EnvironmentId, machine.ParentId, machine.MachineName, machine.ConfigurationValueList);
        //}

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

        //public void UpdateEnvironmentComponentConfig(string environmentId, string projectId, string componentId, string configName, string configValue)
        //{
        //    var environment = this._projectRepository.GetEnvironment(environmentId, string projectId);
        //    var component = environment.GetComponentItem(componentId);
        //    if (component.ConfigurationValueList == null)
        //    {
        //        component.ConfigurationValueList = new Dictionary<string, string>();
        //    }
        //    this.UpdateConfig(component.ConfigurationValueList, configName, configValue);
        //    this._projectRepository.UpdateEnvironment(environmentId, environment.ProjectId, environment.EnvironmentName, environment.ComponentList, environment.ConfigurationList);
        //}


		public List<DeployConfiguration> GetConfigurationList(string projectId)
		{
			return _projectRepository.GetConfigurationList(projectId);
		}

        public DeployConfiguration GetConfiguration(string configurationId, string projectId)
		{
			return _projectRepository.GetConfiguration(configurationId, projectId);
		}


        public DeployConfiguration CreateConfiguration(string projectId, string configurationName, EnumDeploymentIsolationType isolationType)
		{
			return _projectRepository.CreateConfiguration(projectId, configurationName, isolationType);
		}

        public DeployConfiguration UpdateConfiguration(string configurationId, string projectId, string configurationName, EnumDeploymentIsolationType isolationType)
		{
			return _projectRepository.UpdateConfiguration(configurationId, projectId, configurationName, isolationType);
		}

        public void DeleteConfiguration(string configurationId, string projectId)
		{
			_projectRepository.DeleteConfiguration(configurationId, projectId);
		}


        public EnumDeploymentIsolationType GetComponentIsolationType(string projectId, string componentId)
        {
			var component = _projectRepository.GetComponent(componentId, projectId);
            if(component.UseConfigurationGroup && !string.IsNullOrEmpty(component.ConfigurationId))
            {
				var configuration = _projectRepository.GetConfiguration(component.ConfigurationId, projectId);
                return configuration.IsolationType;
            }
            else 
            {
                return component.IsolationType;
            }
        }
    }
}
