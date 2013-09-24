using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Dto;

namespace Sriracha.Deploy.Data
{
	public interface IProjectManager
	{
		IEnumerable<DeployProject> GetProjectList();
		DeployProject CreateProject(string projectName, bool usesSharedComponentConfiguration);
		DeployProject GetProject(string projectId);
		DeployProject UpdateProject(string projectId, string projectName, bool usesSharedComponentConfiguration);
		void DeleteProject(string projectId);

		IEnumerable<DeployComponent> GetComponentList(string projectId);
		DeployComponent CreateComponent(string projectId, string componentName);
		DeployComponent GetComponent(string componentId);
		DeployComponent UpdateComponent(string componentId, string projectId, string componentName);
		void DeleteComponent(string componentId);

		List<DeployComponentDeploymentStep> GetDeploymentStepList(string componentId);
		DeployComponentDeploymentStep CreateDeploymentStep(string projectId, string componentId, string stepName, string taskTypeName, string taskOptionsJSON);
		DeployComponentDeploymentStep GetDeploymentStep(string deploymentStepId);
		DeployComponentDeploymentStep UpdateDeploymentStep(string deploymentStepId, string projectId, string componentId, string stepName, string taskTypeName, string taskOptionsJSON);
		void DeleteDeploymentStep(string deploymentStepId);


		IEnumerable<DeployProjectBranch> GetBranchList(string projectId);
		DeployProjectBranch CreateBranch(string projectId, string branchName);
		DeployProjectBranch GetBranch(string branchId);
		DeployProjectBranch UpdateBranch(string branchId, string projectId, string branchName);
		void DeleteBranch(string branchId);

		IEnumerable<DeployEnvironment> GetEnvironmentList(string projectId);
		DeployEnvironment CreateEnvironment(string projectId, string environmentName, IEnumerable<DeployEnvironmentComponent> componentList);
		DeployEnvironment GetEnvironment(string environmentId);
		DeployEnvironment UpdateEnvironment(string environmentId, string projectId, string environmentName, IEnumerable<DeployEnvironmentComponent> componentList);
		void DeleteEnvironment(string environmentId);

		void UpdateMachineConfig(string machineId, string configName, string configValue);
		void UpdateEnvironmentComponentConfig(string environmentId, string componentId, string configName, string configValue);
	}
}
