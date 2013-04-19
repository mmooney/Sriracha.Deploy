using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Dto;

namespace Sriracha.Deploy.Data.Repository
{
	public interface IBuildRepository
	{
		DeployBuild StoreBuild(string projectId, string projectName, string projectComponentId, string projectComponentName, string projectBranchId, string projectBranchName, string fileId, Version version);

		IEnumerable<DeployBuild> GetBuildList();
	}
}
