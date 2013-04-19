using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Dto;

namespace Sriracha.Deploy.Data
{
	public interface IBuildManager
	{
		IEnumerable<DeployBuild> GetBuildList();
		DeployBuild SubmitBuild(string projectId, string branchId, string fileName, byte[] fileData, Version version);
	}
}
