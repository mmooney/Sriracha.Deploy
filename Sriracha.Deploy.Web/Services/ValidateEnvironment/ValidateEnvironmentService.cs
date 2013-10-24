using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack.ServiceInterface;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Build;
using Sriracha.Deploy.Data.Deployment;
using Sriracha.Deploy.Data.Project;

namespace Sriracha.Deploy.Web.Services.ValidateEnvironment
{
	public class ValidateEnvironmentService : Service 
	{
		private readonly IDeployRequestManager _deployRequestManager;
		private readonly IBuildManager _buildManager;
		private readonly IProjectManager _projectManager;
		private readonly IDeploymentValidator _validator;

		public ValidateEnvironmentService(IDeployRequestManager deployRequestManager, IBuildManager buildManager, IProjectManager projectManager, IDeploymentValidator validator)
		{
			_deployRequestManager = DIHelper.VerifyParameter(deployRequestManager);
			_buildManager = DIHelper.VerifyParameter(buildManager);
			_projectManager = DIHelper.VerifyParameter(projectManager);
			_validator = DIHelper.VerifyParameter(validator);
		}

		public object Get(ValidateEnvironmentRequest request)
		{
			if(request == null || string.IsNullOrWhiteSpace(request.BuildId) || string.IsNullOrWhiteSpace(request.EnvironmentId))
			{
				throw new ArgumentNullException();
			}
			var deployRequest = this.InitializeDeployRequest(request.BuildId, request.EnvironmentId);
			return new ValidateEnvironmentResponse
			{
				Environment = deployRequest.Environment,
				Build = deployRequest.Build,
				Component = deployRequest.Component,
				ValidationResult = deployRequest.ValidationResult
			};
		}

		private DeployRequestTemplate InitializeDeployRequest(string buildId, string environmentId)
		{
			var build = _buildManager.GetBuild(buildId);
			var project = _projectManager.GetProject(build.ProjectId);
			var environment = project.GetEnvironment(environmentId);
			var component = project.GetComponent(build.ProjectComponentId);
			var returnValue = new DeployRequestTemplate
			{
				Build = build,
				Environment = environment,
				Component = component,
				ValidationResult = _validator.ValidateDeployment(project, component, environment)
			};
			return returnValue;
		}

	}
}