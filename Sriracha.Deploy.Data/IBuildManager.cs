using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Dto;

namespace Sriracha.Deploy.Data
{
	public interface IBuildManager
	{
		IEnumerable<DeployBuild> GetBuildList(string projectId = null, string branchId = null, string componentId = null);
		PagedSortedList<DeployBuild> GetBuildList(ListOptions listOptions);
		DeployBuild CreateBuild(string projectId, string componentId, string branchId, string fileName, byte[] fileData, string version);
		DeployBuild GetBuild(string buildId);
		DeployBuild CreateBuild(string projectId, string componentId, string branchId, string fileId, string version);
		DeployBuild UpdateBuild(string buildId, string projectId, string componentId, string branchId, string fileId, string version);
		void DeleteBuild(string buildId);
	}
}
