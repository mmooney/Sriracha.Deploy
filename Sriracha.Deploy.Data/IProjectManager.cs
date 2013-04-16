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
		DeployProjectBranch CreateBranch(string projectId, string branchName);
		void DeleteProject(string projectId);

		IEnumerable<DeployComponent> GetComponentList(string projectId);
		DeployComponent CreateComponent(string projectId, string componentName);
		DeployComponent GetComponent(string componentId);
		DeployComponent UpdateComponent(string componentId, string projectId, string componentName);
		void DeleteComponent(string componentId);

		DeployComponentDeploymentStep CreateDeploymentStep(string projectId, string componentId, string stepName, string taskTypeName, dynamic taskOptions);
		DeployComponentDeploymentStep UpdateDeploymentStep(string projectId, string componentId, string deploymentStepId, string stepName, string taskTypeName, dynamic taskOptions);

	}
}
