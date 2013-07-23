using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack.ServiceInterface;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Dto;

namespace Sriracha.Deploy.Web.Services
{
	public class ComponentConfigurationService : Service
	{
		private readonly IDeploymentValidator _deploymentValidator;
		private readonly IProjectManager _projectManager;

		public ComponentConfigurationService(IDeploymentValidator deploymentValidator, IProjectManager projectManager)
		{
			_deploymentValidator = DIHelper.VerifyParameter(deploymentValidator);
			_projectManager = DIHelper.VerifyParameter(projectManager);
		}

		public object Get(ComponentConfigurationDefinition request)
		{	
			if(request == null || string.IsNullOrWhiteSpace(request.ComponentId))
			{
				throw new ArgumentNullException();
			}	
			var component = _projectManager.GetComponent(request.ComponentId);
			return _deploymentValidator.GetComponentConfigurationDefinition(component);
		}
	}
}