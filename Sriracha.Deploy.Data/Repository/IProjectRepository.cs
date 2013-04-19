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
		DeployProject CreateProject(string projectName);
		DeployProject GetProject(string projectId);
		DeployProject UpdateProject(string projectId, string projectName);
		void DeleteProject(string projectId);

		IEnumerable<DeployComponent> GetComponentList(string projectId);
		DeployComponent CreateComponent(string projectId, string componentName);
		DeployComponent GetComponent(string componentId);
		DeployComponent UpdateComponent(string projectId, string componentId, string componentName);
		void DeleteComponent(string componentId);

		List<DeployComponentDeploymentStep> GetDeploymentStepList(string componentId);
		DeployComponentDeploymentStep CreateDeploymentStep(string projectId, string componentId, string stepName, string taskTypeName, string taskOptionsJson);
		DeployComponentDeploymentStep GetDeploymentStep(string deploymentStepId);
		DeployComponent GetComponent(DeployProject project, string componentId);
		DeployComponentDeploymentStep UpdateDeploymentStep(string deploymentStepId, string projectId, string componentId, string stepName, string taskTypeName, string taskOptionsJson);
		void DeleteDeploymentStep(string deploymentStepId);

		IEnumerable<DeployProjectBranch> GetBranchList(string projectId);
		DeployProjectBranch CreateBranch(string projectId, string branchName);
		DeployProjectBranch GetBranch(string branchId);
		DeployProjectBranch GetBranch(DeployProject project, string branchId);
		DeployProjectBranch UpdateBranch(string branchId, string projectId, string branchName);
		void DeleteBranch(string branchId);

		IEnumerable<DeployEnvironment> GetEnvironmentList(string projectId);
		DeployEnvironment CreateEnvironment(string projectId, string enviornmentName);
		DeployEnvironment GetEnvironment(string environmentId);
		DeployEnvironment UpdateEnvironment(string environmentId, string projectId, string environmentName);
		void DeleteEnvironment(string environmentId);
	}
}
