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
	public class EnvironmentService : Service 
	{
		private readonly IProjectManager _projectManager;
        private readonly IParameterMasker _parameterMasker;

		public EnvironmentService(IProjectManager projectManager, IParameterMasker parameterMasker)
		{
			_projectManager = DIHelper.VerifyParameter(projectManager);
            _parameterMasker = DIHelper.VerifyParameter(parameterMasker);
		}

		public object Get(DeployEnvironment request)
		{
			if(request == null || 
				(string.IsNullOrEmpty(request.Id) && string.IsNullOrEmpty(request.ProjectId)))
			{
				throw new ArgumentNullException();
			}
            var project = _projectManager.GetProject(request.ProjectId);
			if (!string.IsNullOrEmpty(request.Id))
			{
				var environment = _projectManager.GetEnvironment(request.Id, request.ProjectId);
                return _parameterMasker.Mask(project, environment);
			}
			else 
			{
				var environmentList = _projectManager.GetEnvironmentList(request.ProjectId);
                return _parameterMasker.Mask(project, environmentList);
			}
		}

		public object Post(DeployEnvironment environment)
		{
			if (string.IsNullOrEmpty(environment.Id))
			{
				return _projectManager.CreateEnvironment(environment.ProjectId, environment.EnvironmentName, environment.ComponentList, environment.ConfigurationList);
			}
			else
			{
                var project = _projectManager.GetProject(environment.ProjectId);
                var originalEnvironment = _projectManager.GetEnvironment(environment.Id, environment.ProjectId);
                var unmaskedEnvironment = _parameterMasker.Unmask(project, environment, originalEnvironment);
                return _projectManager.UpdateEnvironment(unmaskedEnvironment.Id, unmaskedEnvironment.ProjectId, unmaskedEnvironment.EnvironmentName, unmaskedEnvironment.ComponentList, unmaskedEnvironment.ConfigurationList);
			}
		}

		public void Delete(DeployEnvironment environment)
		{
			_projectManager.DeleteEnvironment(environment.Id, environment.ProjectId);
		}
	}
}