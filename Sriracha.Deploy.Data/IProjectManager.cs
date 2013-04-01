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
		DeployProjectBranch CreateBranch(string projectId, string branchName);
	}
}
