﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Dto.Build;

namespace Sriracha.Deploy.Data.Repository
{
	public interface IBuildRepository
	{
		PagedSortedList<DeployBuild> GetBuildList(ListOptions listOptions, string projectId = null, string branchId = null, string componentId = null);
		DeployBuild CreateBuild(string projectId, string projectName, string projectComponentId, string projectComponentName, string projectBranchId, string projectBranchName, string fileId, string version);
		DeployBuild GetBuild(string buildId);
		DeployBuild UpdateBuild(string buildId, string projectId, string projectName, string projectComponentId, string projectComponentName, string projectBranchId, string projectBranchName, string fileId, string version);
		void DeleteBuild(string buildId);
	}
}
