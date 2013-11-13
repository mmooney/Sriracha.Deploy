using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Repository;
using Sriracha.Deploy.Data.Project;
using Sriracha.Deploy.Data.Dto.Project;

namespace Sriracha.Deploy.Web.Services
{
	public class ProjectService : Service
	{
		private readonly IProjectManager _projectManager;

		public ProjectService(IProjectManager projectRepository)
		{
			_projectManager = DIHelper.VerifyParameter(projectRepository);
		}

		public object Get(DeployProject request)
		{
			if(request != null && !string.IsNullOrEmpty(request.Id))
			{
				return _projectManager.GetProject(request.Id);				
			}
			else 
			{
				return _projectManager.GetProjectList();
			}
		}

		public object Post(DeployProject project)
		{
			if(string.IsNullOrEmpty(project.Id))
			{
				return _projectManager.CreateProject(project.ProjectName, project.UsesSharedComponentConfiguration);
			}
			else 
			{
				return _projectManager.UpdateProject(project.Id, project.ProjectName, project.UsesSharedComponentConfiguration);
			}
		}

		public void Delete(DeployProject project) 
		{
			_projectManager.DeleteProject(project.Id);
		}
	}
}