using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Dto.Project;

namespace Sriracha.Deploy.Data.Project
{
	public interface IProjectManager
	{
		IEnumerable<DeployProject> GetProjectList();
		DeployProject CreateProject(string projectName, bool usesSharedComponentConfiguration);
		DeployProject GetProject(string projectId);
		DeployProject UpdateProject(string projectId, string projectName, bool usesSharedComponentConfiguration);
		void DeleteProject(string projectId);

		List<DeployConfiguration> GetConfigurationList(string projectId);
        DeployConfiguration CreateConfiguration(string projectId, string configurationName, EnumDeploymentIsolationType isolationType);
		DeployConfiguration GetConfiguration(string configurationId);
		DeployConfiguration UpdateConfiguration(string configurationId, string projectId, string configurationName, EnumDeploymentIsolationType isolationType);
		void DeleteConfiguration(string configurationId);

		IEnumerable<DeployComponent> GetComponentList(string projectId);
        DeployComponent CreateComponent(string projectId, string componentName, bool useConfigurationGroup, string configurationId, EnumDeploymentIsolationType isolationType);
		DeployComponent GetComponent(string componentId);
        DeployComponent UpdateComponent(string componentId, string projectId, string componentName, bool useConfigurationGroup, string configurationId, EnumDeploymentIsolationType isolationType);
		void DeleteComponent(string projectId, string componentId);

		List<DeployStep> GetComponentDeploymentStepList(string componentId);
		List<DeployStep> GetConfigurationDeploymentStepList(string configurationId);
		DeployStep CreateComponentDeploymentStep(string projectId, string componentId, string stepName, string taskTypeName, string taskOptionsJSON);
		DeployStep CreateConfigurationDeploymentStep(string projectId, string configurationId, string stepName, string taskTypeName, string taskOptionsJSON);
		DeployStep GetComponentDeploymentStep(string deploymentStepId);
		DeployStep GetConfigurationDeploymentStep(string deploymentStepId);
		DeployStep UpdateComponentDeploymentStep(string deploymentStepId, string projectId, string componentId, string stepName, string taskTypeName, string taskOptionsJSON);
		DeployStep UpdateConfigurationDeploymentStep(string deploymentStepId, string projectId, string configuration, string stepName, string taskTypeName, string taskOptionsJSON);
		void DeleteComponentDeploymentStep(string deploymentStepId);
		void DeleteConfigurationDeploymentStep(string deploymentStepId);


		IEnumerable<DeployProjectBranch> GetBranchList(string projectId);
		DeployProjectBranch CreateBranch(string projectId, string branchName);
		DeployProjectBranch GetBranch(string branchId);
		DeployProjectBranch UpdateBranch(string branchId, string projectId, string branchName);
		void DeleteBranch(string branchId, string projectId);

		IEnumerable<DeployEnvironment> GetEnvironmentList(string projectId);
		DeployEnvironment CreateEnvironment(string projectId, string environmentName, IEnumerable<DeployEnvironmentConfiguration> componentList, IEnumerable<DeployEnvironmentConfiguration> configurationList);
		DeployEnvironment GetEnvironment(string environmentId);
		DeployEnvironment UpdateEnvironment(string environmentId, string projectId, string environmentName, IEnumerable<DeployEnvironmentConfiguration> componentList, IEnumerable<DeployEnvironmentConfiguration> configurationList);
		void DeleteEnvironment(string environmentId);

        //void UpdateMachineConfig(string machineId, string configName, string configValue);
        //void UpdateEnvironmentComponentConfig(string environmentId, string componentId, string configName, string configValue);

        EnumDeploymentIsolationType GetComponentIsolationType(string projectId, string componentId);
    }
}
