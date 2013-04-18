using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack.ServiceInterface;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Dto;

namespace Sriracha.Deploy.Web.Services
{
	public class EnvironmentService : Service 
	{
		private readonly IProjectManager _projectManager;

		public EnvironmentService(IProjectManager projectManager)
		{
			_projectManager = DIHelper.VerifyParameter(projectManager);
		}

		public object Get(DeployEnvironment request)
		{
			if(request == null || 
				(string.IsNullOrEmpty(request.Id) && string.IsNullOrEmpty(request.ProjectId)))
			{
				throw new ArgumentNullException();
			}
			if (string.IsNullOrEmpty(request.Id))
			{
				return _projectManager.GetEnvironment(request.Id);
			}
			else 
			{
				return _projectManager.GetEnvironmentList(request.ProjectId);
			}
		}

		public object Post(DeployEnvironment environment)
		{
			if (string.IsNullOrEmpty(environment.Id))
			{
				return _projectManager.CreateEnvironment(environment.ProjectId, environment.EnvironmentName);
			}
			else
			{
				return _projectManager.UpdateEnvironment(environment.Id, environment.ProjectId, environment.EnvironmentName);
			}
		}

		public void Delete(DeployEnvironment environment)
		{
			_projectManager.DeleteEnvironment(environment.Id);
		}
	}
}