using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Repository;

namespace Sriracha.Deploy.Web.Services
{
	public class ProjectService : Service
	{
		private readonly IProjectRepository _projectRepository;

		public ProjectService(IProjectRepository projectRepository)
		{
			_projectRepository = DIHelper.VerifyParameter(projectRepository);
		}

		public object Get(DeployProject request)
		{
			if(request != null && !string.IsNullOrEmpty(request.Id))
			{
				return _projectRepository.GetProject(request.Id);				
			}
			else 
			{
				return _projectRepository.GetProjectList();
			}
		}

		public object Post(DeployProject project)
		{
			if(string.IsNullOrEmpty(project.Id))
			{
				return _projectRepository.CreateProject(project.ProjectName);
			}
			else 
			{
				return _projectRepository.UpdateProject(project.Id, project.ProjectName);
			}
		}

		public void Delete(DeployProject project) 
		{
			_projectRepository.DeleteProject(project.Id);
		}
	}
}