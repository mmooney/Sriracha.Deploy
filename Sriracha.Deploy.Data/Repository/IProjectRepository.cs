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
		DeployProject TryGetProjectByName(string projectName);
		DeployProject GetProjectByName(string projectName);
		DeployProject UpdateProject(string projectId, string projectName, bool usesSharedComponentConfiguration);
		void DeleteProject(string projectId);

		IEnumerable<DeployComponent> GetComponentList(string projectId);
		DeployComponent CreateComponent(string projectId, string componentName);
		DeployComponent GetComponent(string componentId);
		DeployComponent TryGetComponent(string componentId);
		DeployComponent GetComponent(DeployProject project, string componentId);
		DeployComponent TryGetComponent(DeployProject project, string componentId);
		DeployComponent GetComponentByName(DeployProject project, string componentName);
		DeployComponent TryGetComponentByName(DeployProject project, string componentName);
		DeployComponent UpdateComponent(string projectId, string componentId, string componentName);
		void DeleteComponent(string componentId);

		List<DeployComponentDeploymentStep> GetDeploymentStepList(string componentId);
		DeployComponentDeploymentStep CreateDeploymentStep(string projectId, string componentId, string stepName, string taskTypeName, string taskOptionsJson, string sharedDeploymentStepId);
		DeployComponentDeploymentStep CreateDeploymentStep(DeployProject project, string componentId, string stepName, string taskTypeName, string taskOptionsJson, string sharedDeploymentStepId);
		DeployComponentDeploymentStep GetDeploymentStep(string deploymentStepId);
		DeployComponentDeploymentStep UpdateDeploymentStep(string deploymentStepId, string projectId, string componentId, string stepName, string taskTypeName, string taskOptionsJson, string sharedDeploymentStepId);
		DeployComponentDeploymentStep UpdateDeploymentStep(string deploymentStepId, DeployProject project, string componentId, string stepName, string taskTypeName, string taskOptionsJson, string sharedDeploymentStepId);
		void DeleteDeploymentStep(string deploymentStepId);

		IEnumerable<DeployProjectBranch> GetBranchList(string projectId);
		DeployProjectBranch CreateBranch(string projectId, string branchName);
		DeployProjectBranch GetBranch(string branchId);
		DeployProjectBranch TryGetBranch(string branchId);
		DeployProjectBranch GetBranch(DeployProject project, string branchId);
		DeployProjectBranch TryGetBranch(DeployProject project, string branchId);
		DeployProjectBranch GetBranchByName(string projectId, string branchName);
		DeployProjectBranch TryGetBranchByName(string projectId, string branchName);
		DeployProjectBranch GetBranchByName(DeployProject project, string branchName);
		DeployProjectBranch TryGetBranchByName(DeployProject project, string branchName);
		DeployProjectBranch UpdateBranch(string branchId, string projectId, string branchName);
		void DeleteBranch(string branchId);

		IEnumerable<DeployEnvironment> GetEnvironmentList(string projectId);
		DeployEnvironment CreateEnvironment(string projectId, string enviornmentName, IEnumerable<DeployEnvironmentComponent> componentList);
		DeployEnvironment GetEnvironment(string environmentId);
		DeployEnvironment UpdateEnvironment(string environmentId, string projectId, string environmentName, IEnumerable<DeployEnvironmentComponent> componentList);
		void DeleteEnvironment(string environmentId);

		DeployMachine GetMachine(string machineId);
		DeployMachine UpdateMachine(string machineId, string projectId, string environmentId, string enviromentComponentId, string machineName, Dictionary<string, string> configurationList);
	}
}
