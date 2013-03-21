using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Dto;

namespace Sriracha.Deploy.Data.Repository
{
	public interface IProjectRepository
	{
		IEnumerable<DeployProject> LoadProjectList();
		DeployProject CreateProject(string projectName);
		DeployProject GetProject(string id);
		DeployProjectBranch CreateBranch(string projectId, string branchName);
	}
}
