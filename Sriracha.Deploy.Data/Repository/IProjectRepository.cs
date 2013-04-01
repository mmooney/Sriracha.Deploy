using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Dto;

namespace Sriracha.Deploy.Data.Repository
{
	public interface IProjectRepository
	{
		IEnumerable<DeployProject> GetProjectList();
		DeployProject CreateProject(string projectName);
		DeployProject GetProject(string projectId);
		void UpdateProject(string projectId, string projectName);
		DeployProjectBranch CreateBranch(string projectId, string branchName);
		void DeleteProject(string projectId);
	}
}
