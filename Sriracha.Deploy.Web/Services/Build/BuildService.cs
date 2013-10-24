using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack.ServiceInterface;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Build;

namespace Sriracha.Deploy.Web.Services.Build
{
	public class BuildService : Service
	{
		private readonly IBuildManager _buildManager;

		public BuildService(IBuildManager buildManager)
		{
			this._buildManager = buildManager;
		}

		public object Get(BuildRequest request)
		{
			if(request != null && !string.IsNullOrEmpty(request.Id))
			{
				return _buildManager.GetBuild(request.Id);
			}
			else 
			{
				return _buildManager.GetBuildList(request.BuildListOptions(), request.ProjectId, request.ProjectBranchId, request.ProjectComponentId);
			}
		}

		public object Post(BuildRequest request)
		{
			if(string.IsNullOrEmpty(request.Id))
			{
				return this._buildManager.CreateBuild(request.ProjectId, request.ProjectComponentId, request.ProjectBranchId, request.FileId, request.Version);
			}
			else 
			{
				return this._buildManager.UpdateBuild(request.Id, request.ProjectId, request.ProjectComponentId, request.ProjectBranchId, request.FileId, request.Version);
			}
		}

		public void Delete(BuildRequest build)  
		{
			this._buildManager.DeleteBuild(build.Id);
		}

	}
}