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
		DeployProjectBranch CreateBranch(string projectId, string branchName);
		void DeleteProject(string projectId);

		IEnumerable<DeployComponent> GetComponentList(string projectId);
		DeployComponent CreateComponent(string projectId, string componentName);
		DeployComponent GetComponent(string componentId);
		DeployComponent UpdateComponent(string projectId, string componentId, string componentName);
		void DeleteComponent(string componentId);

		List<DeployComponentDeploymentStep> GetDeploymentStepList(string componentId);
		DeployComponentDeploymentStep CreateDeploymentStep(string projectId, string componentId, string stepName, string taskTypeName, string taskOptionsJson);
		DeployComponentDeploymentStep GetDeploymentStep(string deploymentStepId);
		DeployComponentDeploymentStep UpdateDeploymentStep(string deploymentStepId, string projectId, string componentId, string stepName, string taskTypeName, string taskOptionsJson);
		void DeleteDeploymentStep(string deploymentStepId);
	}
}
