using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack.ServiceInterface;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Dto;

namespace Sriracha.Deploy.Web.Services
{
	public class BuildService : Service
	{
		private readonly IBuildManager _buildManager;

		public BuildService(IBuildManager buildManager)
		{
			this._buildManager = buildManager;
		}

		public object Get(DeployBuild request)
		{
			if(request != null && !string.IsNullOrEmpty(request.Id))
			{
				return _buildManager.GetBuild(request.Id);
			}
			else 
			{
				return _buildManager.GetBuildList();
			}
		}

		public object Post(DeployBuild build)
		{
			if(string.IsNullOrEmpty(build.Id))
			{
				return this._buildManager.CreateBuild(build.ProjectId, build.ProjectComponentId, build.ProjectBranchId, build.FileId, build.Version);
			}
			else 
			{
				return this._buildManager.UpdateBuild(build.Id, build.ProjectId, build.ProjectComponentId, build.ProjectBranchId, build.FileId, build.Version);
			}
		}

		public void Delete(DeployBuild build)  
		{
			this._buildManager.DeleteBuild(build.Id);
		}

	}
}