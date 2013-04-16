using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack.ServiceInterface;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Dto;

namespace Sriracha.Deploy.Web.Services
{
	public class DeploymentStepService : Service
	{
		private readonly IProjectManager _projectManager;

		public DeploymentStepService(IProjectManager projectManager)
		{
			_projectManager = DIHelper.VerifyParameter(projectManager);
		}

		public object Get(DeployComponentDeploymentStep request)
		{
			if (request == null ||
				(string.IsNullOrEmpty(request.Id) && string.IsNullOrEmpty(request.ComponentId)))
			{
				throw new ArgumentNullException();
			}
			if (string.IsNullOrEmpty(request.Id))
			{
				return _projectManager.GetDeploymentStep(request.Id);
			}
			else
			{
				return _projectManager.GetDeploymentStepList(request.ComponentId);
			}
		}

		public object Post(DeployComponentDeploymentStep deploymentStep)
		{
			if (string.IsNullOrEmpty(deploymentStep.Id))
			{
				return _projectManager.CreateDeploymentStep(deploymentStep.ProjectId, deploymentStep.ComponentId, deploymentStep.StepName, deploymentStep.TaskTypeName, deploymentStep.TaskOptionsJson);
			}
			else
			{
				return _projectManager.UpdateDeploymentStep(deploymentStep.Id, deploymentStep.ProjectId, deploymentStep.ComponentId, deploymentStep.StepName, deploymentStep.TaskTypeName, deploymentStep.TaskOptionsJson);
			}
		}

		public void Delete(DeployComponentDeploymentStep deploymentStep)
		{
			_projectManager.DeleteDeploymentStep(deploymentStep.Id);
		}

	}
}