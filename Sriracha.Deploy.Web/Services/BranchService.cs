using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack.ServiceInterface;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Project;
using Sriracha.Deploy.Data.Dto.Project;

namespace Sriracha.Deploy.Web.Services
{
	public class BranchService : Service
	{
		private readonly IProjectManager _projectManager;

		public BranchService(IProjectManager projectManager)
		{
			_projectManager = DIHelper.VerifyParameter(projectManager);
		}

		public object Get(DeployProjectBranch request)
		{
			if(request == null || 
				(string.IsNullOrEmpty(request.Id) && string.IsNullOrEmpty(request.ProjectId)))
			{
				throw new ArgumentNullException();
			}
			if (string.IsNullOrEmpty(request.Id))
			{
				return _projectManager.GetBranch(request.Id);
			}
			else 
			{
				return _projectManager.GetBranchList(request.ProjectId);
			}
		}

		public object Post(DeployProjectBranch branch)
		{
			if (string.IsNullOrEmpty(branch.Id))
			{
				return _projectManager.CreateBranch(branch.ProjectId, branch.BranchName);
			}
			else
			{
				return _projectManager.UpdateBranch(branch.Id, branch.ProjectId, branch.BranchName);
			}
		}

		public void Delete(DeployProjectBranch branch)
		{
			_projectManager.DeleteBranch(branch.Id, branch.ProjectId);
		}
	}
}