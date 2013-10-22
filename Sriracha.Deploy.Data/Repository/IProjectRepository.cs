using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Dto;

namespace Sriracha.Deploy.Data.Repository
{
	public interface IProjectRepository
	{
		IEnumerable<DeployProject> GetProjectList(string[] idList=null);
		DeployProject CreateProject(string projectName, bool usesSharedComponentConfiguration);
		DeployProject TryGetProject(string projectId);
		DeployProject GetProject(string projectId);
		DeployProject GetOrCreateProject(string projectId, string projectName);
		DeployProject TryGetProjectByName(string projectName);
		DeployProject GetProjectByName(string projectName);
		DeployProject UpdateProject(string projectId, string projectName, bool usesSharedComponentConfiguration);
		void DeleteProject(string projectId);

		List<DeployConfiguration> GetConfigurationList(string projectId);
		DeployConfiguration CreateConfiguration(string projectId, string configurationName);
		DeployConfiguration CreateConfiguration(DeployProject project, string configurationName);
		DeployConfiguration GetConfiguration(string configurationId);
		DeployConfiguration TryGetConfiguration(string configurationId);
		DeployConfiguration UpdateConfiguration(string configurationId, string projectId, string configurationName);
		void DeleteConfiguration(string configurationId);
		
		IEnumerable<DeployComponent> GetComponentList(string projectId);
		DeployComponent CreateComponent(string projectId, string componentName, bool useConfigurationGroup, string configurationId);
		DeployComponent CreateComponent(DeployProject project, string componentName, bool useConfigurationGroup, string configurationId);
		DeployComponent GetComponent(string componentId, string projectId=null);
		DeployComponent TryGetComponent(string componentId, string projectId = null);
		DeployComponent GetComponent(DeployProject project, string componentId);
		DeployComponent TryGetComponent(DeployProject project, string componentId);
		DeployComponent GetComponentByName(DeployProject project, string componentName);
		DeployComponent TryGetComponentByName(DeployProject project, string componentName);
		DeployComponent GetOrCreateComponent(string projectId, string componentId, string componentName);
		DeployComponent UpdateComponent(string projectId, string componentId, string componentName, bool useConfigurationGroup, string configurationId);
		void DeleteComponent(string componentId);

		List<DeployStep> GetComponentDeploymentStepList(string componentId);
		List<DeployStep> GetConfigurationDeploymentStepList(string configurationId);
		DeployStep CreateComponentDeploymentStep(string projectId, string componentId, string stepName, string taskTypeName, string taskOptionsJson, string sharedDeploymentStepId);
		DeployStep CreateConfigurationDeploymentStep(string projectId, string configurationId, string stepName, string taskTypeName, string taskOptionsJson);
		DeployStep CreateComponentDeploymentStep(DeployProject project, string componentId, string stepName, string taskTypeName, string taskOptionsJson, string sharedDeploymentStepId);
		DeployStep CreateConfigurationDeploymentStep(DeployProject project, string configurationId, string stepName, string taskTypeName, string taskOptionsJson);
		DeployStep GetComponentDeploymentStep(string deploymentStepId);
		DeployStep GetConfigurationDeploymentStep(string deploymentStepId);
		DeployStep UpdateComponentDeploymentStep(string deploymentStepId, string projectId, string componentId, string stepName, string taskTypeName, string taskOptionsJson, string sharedDeploymentStepId);
		DeployStep UpdateConfigurationDeploymentStep(string deploymentStepId, string projectId, string configurationId, string stepName, string taskTypeName, string taskOptionsJson);
		DeployStep UpdateComponentDeploymentStep(string deploymentStepId, DeployProject project, string componentId, string stepName, string taskTypeName, string taskOptionsJson, string sharedDeploymentStepId);
		DeployStep UpdateConfigurationDeploymentStep(string deploymentStepId, DeployProject project, string configurationId, string stepName, string taskTypeName, string taskOptionsJson);
		void DeleteComponentDeploymentStep(string deploymentStepId);
		void DeleteConfigurationDeploymentStep(string deploymentStepId);

		IEnumerable<DeployProjectBranch> GetBranchList(string projectId);
		DeployProjectBranch CreateBranch(string projectId, string branchName);
		DeployProjectBranch GetBranch(string branchId, string projectId=null);
		DeployProjectBranch TryGetBranch(string branchId, string projectId = null);
		DeployProjectBranch GetBranch(DeployProject project, string branchId);
		DeployProjectBranch TryGetBranch(DeployProject project, string branchId);
		DeployProjectBranch GetBranchByName(string projectId, string branchName);
		DeployProjectBranch TryGetBranchByName(string projectId, string branchName);
		DeployProjectBranch GetBranchByName(DeployProject project, string branchName);
		DeployProjectBranch TryGetBranchByName(DeployProject project, string branchName);
		DeployProjectBranch GetOrCreateBranch(string projectId, string branchId, string branchName);
		DeployProjectBranch UpdateBranch(string branchId, string projectId, string branchName);
		void DeleteBranch(string branchId);

		IEnumerable<DeployEnvironment> GetEnvironmentList(string projectId);
		DeployEnvironment CreateEnvironment(string projectId, string enviornmentName, IEnumerable<DeployEnvironmentConfiguration> componentList, IEnumerable<DeployEnvironmentConfiguration> configurationList);
		DeployEnvironment GetEnvironment(string environmentId);
		DeployEnvironment UpdateEnvironment(string environmentId, string projectId, string environmentName, IEnumerable<DeployEnvironmentConfiguration> componentList, IEnumerable<DeployEnvironmentConfiguration> configurationList);
		void DeleteEnvironment(string environmentId);

		DeployMachine GetMachine(string machineId);
		DeployMachine UpdateMachine(string machineId, string projectId, string environmentId, string enviromentComponentId, string machineName, Dictionary<string, string> configurationList);
	}
}
