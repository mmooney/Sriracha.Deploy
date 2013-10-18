using ServiceStack.ServiceInterface;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Dto.Project.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sriracha.Deploy.Web.Services
{
	public class ProjectRoleService : Service
	{
		private IProjectRoleManager _projectRoleManager;

		public ProjectRoleService(IProjectRoleManager projectRoleManager)
		{
			_projectRoleManager = DIHelper.VerifyParameter(projectRoleManager);
		}
	
		public object Get(DeployProjectRole request)
		{
			if(request == null)
			{
				throw new ArgumentNullException("request is null");
			}
			if(!string.IsNullOrEmpty(request.Id))
			{
				return _projectRoleManager.GetProjectRole(request.Id);
			}
			else if (!string.IsNullOrEmpty(request.ProjectId))
			{
				return _projectRoleManager.GetProjectRoleList(request.ProjectId);
			}
			else
			{
				throw new ArgumentNullException("request.Id and request.ProjectId are null");
			}
		}

		public object Post(DeployProjectRole role)
		{
			if(role == null)
			{
				throw new ArgumentNullException("role is null");
			}
			if(!string.IsNullOrEmpty(role.Id))
			{
				return _projectRoleManager.UpdateRole(role.Id, role.ProjectId, role.RoleName, role.Permissions);
			}
			else if(!string.IsNullOrEmpty(role.ProjectId))
			{
				return _projectRoleManager.CreateRole(role.ProjectId, role.RoleName, role.Permissions);
			}
			else 
			{
				throw new ArgumentNullException("role.id and role.projectId are null");
			}
		}
	}
}