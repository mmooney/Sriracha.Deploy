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
		DeployProject CreateProject(string projectName);
		DeployProject GetProject(string projectId);
		DeployProject UpdateProject(string projectId, string projectName);
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

	}
}
