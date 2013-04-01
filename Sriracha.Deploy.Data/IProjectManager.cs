using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Dto;

namespace Sriracha.Deploy.Data
{
	public interface IProjectManager
	{
		DeployProject CreateProject(string projectName);
		DeployProject GetProject(string projectId);
		void UpdateProject(string projectId, string projectName);
		DeployProjectBranch CreateBranch(string projectId, string branchName);
		IEnumerable<DeployProject> GetProjectList();
		void DeleteProject(string projectId);
	}
}
