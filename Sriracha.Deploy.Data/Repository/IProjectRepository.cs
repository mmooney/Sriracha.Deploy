using System;
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
        DeployConfiguration GetConfiguration(string configurationId, string projectId);
        DeployConfiguration TryGetConfiguration(string configurationId, string projectId);
        DeployConfiguration UpdateConfiguration(string configurationId, string projectId, string configurationName, EnumDeploymentIsolationType isolationType);
        void DeleteConfiguration(string configurationId, string projectId);
		
		List<DeployComponent> GetComponentList(string projectId);
        DeployComponent CreateComponent(string projectId, string componentName, bool useConfigurationGroup, string configurationId, EnumDeploymentIsolationType isolationType);
		DeployComponent GetComponent(string componentId, string projectId);
		DeployComponent TryGetComponent(string componentId, string projectId);
		DeployComponent GetOrCreateComponent(string projectId, string componentIdOrName);
        DeployComponent UpdateComponent(string componentId, string projectId, string componentName, bool useConfigurationGroup, string configurationId, EnumDeploymentIsolationType isolationType);
		void DeleteComponent(string projectId, string componentId);

		List<DeployStep> GetComponentDeploymentStepList(string componentId, string projectId);
		List<DeployStep> GetConfigurationDeploymentStepList(string configurationId, string projectId);
		DeployStep CreateComponentDeploymentStep(string projectId, string componentId, string stepName, string taskTypeName, string taskOptionsJson, string sharedDeploymentStepId);
		DeployStep CreateConfigurationDeploymentStep(string projectId, string configurationId, string stepName, string taskTypeName, string taskOptionsJson);
        DeployStep GetComponentDeploymentStep(string deploymentStepId, string projectId);
        DeployStep GetConfigurationDeploymentStep(string deploymentStepId, string projectId);
		DeployStep UpdateComponentDeploymentStep(string deploymentStepId, string projectId, string componentId, string stepName, string taskTypeName, string taskOptionsJson, string sharedDeploymentStepId, int? orderNumber=null);
        DeployStep UpdateConfigurationDeploymentStep(string deploymentStepId, string projectId, string configurationId, string stepName, string taskTypeName, string taskOptionsJson, int? orderNumber=null);
        void DeleteComponentDeploymentStep(string deploymentStepId, string projectId);
        void DeleteConfigurationDeploymentStep(string deploymentStepId, string projectId);

		List<DeployProjectBranch> GetBranchList(string projectId);
		DeployProjectBranch CreateBranch(string projectId, string branchName);
		DeployProjectBranch GetBranch(string branchId, string projectId);
		DeployProjectBranch TryGetBranch(string branchId, string projectId);
		DeployProjectBranch GetBranchByName(string projectId, string branchName);
		DeployProjectBranch TryGetBranchByName(string projectId, string branchName);
		DeployProjectBranch GetOrCreateBranch(string projectId, string branchIdOrName);
		DeployProjectBranch UpdateBranch(string branchId, string projectId, string branchName);
		void DeleteBranch(string branchId, string projectId);

		List<DeployEnvironment> GetEnvironmentList(string projectId);
		DeployEnvironment CreateEnvironment(string projectId, string enviornmentName, IEnumerable<DeployEnvironmentConfiguration> componentList, IEnumerable<DeployEnvironmentConfiguration> configurationList);
        DeployEnvironment GetEnvironment(string environmentId, string projectId);
		DeployEnvironment UpdateEnvironment(string environmentId, string projectId, string environmentName, IEnumerable<DeployEnvironmentConfiguration> componentList, IEnumerable<DeployEnvironmentConfiguration> configurationList);
        void DeleteEnvironment(string environmentId, string projectId);

        //DeployMachine GetMachine(string machineId);
        //DeployMachine UpdateMachine(string machineId, string projectId, string environmentId, string enviromentComponentId, string machineName, Dictionary<string, string> configurationList);
	}
}
