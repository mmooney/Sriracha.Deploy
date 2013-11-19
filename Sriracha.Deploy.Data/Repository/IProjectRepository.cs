﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Dto.Project;

namespace Sriracha.Deploy.Data.Repository
{
	public interface IProjectRepository
	{
		IEnumerable<DeployProject> GetProjectList();
		DeployProject CreateProject(string projectName, bool usesSharedComponentConfiguration);
		DeployProject TryGetProject(string projectId);
		DeployProject GetProject(string projectId);
		DeployProject GetOrCreateProject(string projectIdOrName);
		DeployProject TryGetProjectByName(string projectName);
		DeployProject GetProjectByName(string projectName);
		DeployProject UpdateProject(string projectId, string projectName, bool usesSharedComponentConfiguration);
		void DeleteProject(string projectId);

		List<DeployConfiguration> GetConfigurationList(string projectId);
        DeployConfiguration CreateConfiguration(string projectId, string configurationName, EnumDeploymentIsolationType isolationType);
        DeployConfiguration GetConfiguration(string configurationId, string projectId = null);
        DeployConfiguration TryGetConfiguration(string configurationId, string projectId = null);
        DeployConfiguration UpdateConfiguration(string configurationId, string projectId, string configurationName, EnumDeploymentIsolationType isolationType);
		void DeleteConfiguration(string configurationId);
		
		List<DeployComponent> GetComponentList(string projectId);
        DeployComponent CreateComponent(string projectId, string componentName, bool useConfigurationGroup, string configurationId, EnumDeploymentIsolationType isolationType);
		DeployComponent GetComponent(string componentId, string projectId=null);
		DeployComponent TryGetComponent(string componentId, string projectId = null);
		DeployComponent GetOrCreateComponent(string projectId, string componentId, string componentName);
        DeployComponent UpdateComponent(string componentId, string projectId, string componentName, bool useConfigurationGroup, string configurationId, EnumDeploymentIsolationType isolationType);
		void DeleteComponent(string projectId, string componentId);

		List<DeployStep> GetComponentDeploymentStepList(string componentId);
		List<DeployStep> GetConfigurationDeploymentStepList(string configurationId);
		DeployStep CreateComponentDeploymentStep(string projectId, string componentId, string stepName, string taskTypeName, string taskOptionsJson, string sharedDeploymentStepId);
		DeployStep CreateConfigurationDeploymentStep(string projectId, string configurationId, string stepName, string taskTypeName, string taskOptionsJson);
		DeployStep GetComponentDeploymentStep(string deploymentStepId);
		DeployStep GetConfigurationDeploymentStep(string deploymentStepId);
		DeployStep UpdateComponentDeploymentStep(string deploymentStepId, string projectId, string componentId, string stepName, string taskTypeName, string taskOptionsJson, string sharedDeploymentStepId);
		DeployStep UpdateConfigurationDeploymentStep(string deploymentStepId, string projectId, string configurationId, string stepName, string taskTypeName, string taskOptionsJson);
		void DeleteComponentDeploymentStep(string deploymentStepId);
		void DeleteConfigurationDeploymentStep(string deploymentStepId);

		IEnumerable<DeployProjectBranch> GetBranchList(string projectId);
		DeployProjectBranch CreateBranch(string projectId, string branchName);
		DeployProjectBranch GetBranch(string branchId, string projectId=null);
		DeployProjectBranch TryGetBranch(string branchId, string projectId = null);
		DeployProjectBranch GetBranchByName(string projectId, string branchName);
		DeployProjectBranch TryGetBranchByName(string projectId, string branchName);
		DeployProjectBranch GetOrCreateBranch(string projectId, string branchId, string branchName);
		DeployProjectBranch UpdateBranch(string branchId, string projectId, string branchName);
		void DeleteBranch(string branchId, string projectId);

		IEnumerable<DeployEnvironment> GetEnvironmentList(string projectId);
		DeployEnvironment CreateEnvironment(string projectId, string enviornmentName, IEnumerable<DeployEnvironmentConfiguration> componentList, IEnumerable<DeployEnvironmentConfiguration> configurationList);
		DeployEnvironment GetEnvironment(string environmentId);
		DeployEnvironment UpdateEnvironment(string environmentId, string projectId, string environmentName, IEnumerable<DeployEnvironmentConfiguration> componentList, IEnumerable<DeployEnvironmentConfiguration> configurationList);
		void DeleteEnvironment(string environmentId);

		DeployMachine GetMachine(string machineId);
		DeployMachine UpdateMachine(string machineId, string projectId, string environmentId, string enviromentComponentId, string machineName, Dictionary<string, string> configurationList);
	}
}
