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
        DeployConfiguration GetConfiguration(string configurationId, string projectId);
		DeployConfiguration UpdateConfiguration(string configurationId, string projectId, string configurationName, EnumDeploymentIsolationType isolationType);
        void DeleteConfiguration(string configurationI, string projectIdd);

		IEnumerable<DeployComponent> GetComponentList(string projectId);
        DeployComponent CreateComponent(string projectId, string componentName, bool useConfigurationGroup, string configurationId, EnumDeploymentIsolationType isolationType);
		DeployComponent GetComponent(string componentId, string projectId);
        DeployComponent UpdateComponent(string componentId, string projectId, string componentName, bool useConfigurationGroup, string configurationId, EnumDeploymentIsolationType isolationType);
		void DeleteComponent(string projectId, string componentId);

        List<DeployStep> GetComponentDeploymentStepList(string componentId, string projectId);
        List<DeployStep> GetConfigurationDeploymentStepList(string configurationId, string projectId);
		DeployStep CreateComponentDeploymentStep(string projectId, string componentId, string stepName, string taskTypeName, string taskOptionsJSON);
		DeployStep CreateConfigurationDeploymentStep(string projectId, string configurationId, string stepName, string taskTypeName, string taskOptionsJSON);
        DeployStep GetComponentDeploymentStep(string deploymentStepId, string projectId);
        DeployStep GetConfigurationDeploymentStep(string deploymentStepId, string projectId);
		DeployStep UpdateComponentDeploymentStep(string deploymentStepId, string projectId, string componentId, string stepName, string taskTypeName, string taskOptionsJSON, int? orderNumber=null);
		DeployStep UpdateConfigurationDeploymentStep(string deploymentStepId, string projectId, string configuration, string stepName, string taskTypeName, string taskOptionsJSON, int? orderNumber=null);
        void DeleteComponentDeploymentStep(string deploymentStepId, string projectId);
        void DeleteConfigurationDeploymentStep(string deploymentStepId, string projectId);

        DeployStep MoveDeploymentStepUp(string projectId, EnumDeployStepParentType parentType, string parentId, string deployStepId);
        DeployStep MoveDeploymentStepDown(string projectId, EnumDeployStepParentType parentType, string parentId, string deployStepId);


		IEnumerable<DeployProjectBranch> GetBranchList(string projectId);
		DeployProjectBranch CreateBranch(string projectId, string branchName);
		DeployProjectBranch GetBranch(string branchId, string projectId);
		DeployProjectBranch UpdateBranch(string branchId, string projectId, string branchName);
		void DeleteBranch(string branchId, string projectId);

		IEnumerable<DeployEnvironment> GetEnvironmentList(string projectId);
		DeployEnvironment CreateEnvironment(string projectId, string environmentName, IEnumerable<DeployEnvironmentConfiguration> componentList, IEnumerable<DeployEnvironmentConfiguration> configurationList);
        DeployEnvironment GetEnvironment(string environmentId, string projectId);
		DeployEnvironment UpdateEnvironment(string environmentId, string projectId, string environmentName, IEnumerable<DeployEnvironmentConfiguration> componentList, IEnumerable<DeployEnvironmentConfiguration> configurationList);
        void DeleteEnvironment(string environmentId, string projectId);

        //void UpdateMachineConfig(string machineId, string configName, string configValue);
        //void UpdateEnvironmentComponentConfig(string environmentId, string componentId, string configName, string configValue);

        EnumDeploymentIsolationType GetComponentIsolationType(string projectId, string componentId);

    }
}
