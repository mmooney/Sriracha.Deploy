using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MMDB.Shared;
using ServiceStack.ServiceInterface;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Deployment;
using Sriracha.Deploy.Data.Project;
using Sriracha.Deploy.Data.Dto.Project;

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
			if(request == null)
			{
				throw new ArgumentNullException("request is null");
			}
			else if(string.IsNullOrWhiteSpace(request.ParentId))
			{
				throw new ArgumentNullException("request.parentId is null");
			}	
			switch(request.ParentType)
			{
				case EnumDeployStepParentType.Component:
					var component = _projectManager.GetComponent(request.ParentId, request.ProjectId);
					return _deploymentValidator.GetComponentConfigurationDefinition(component.DeploymentStepList);
				case EnumDeployStepParentType.Configuration:
					var configuration = _projectManager.GetConfiguration(request.ParentId, request.ProjectId);
					return _deploymentValidator.GetComponentConfigurationDefinition(configuration.DeploymentStepList);
				default:
					throw new UnknownEnumValueException(request.ParentType);
			}
		}
	}
}