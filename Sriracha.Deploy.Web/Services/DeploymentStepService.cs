using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MMDB.Shared;
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

		public object Get(DeployStep request)
		{
			if(request == null)
			{
				throw new ArgumentNullException("request is null");
			} 
			else if (string.IsNullOrEmpty(request.ParentId))
			{ 
				throw new ArgumentNullException("request.parentId is null");
			}
			else
			{
				if(!string.IsNullOrEmpty(request.Id))
				{
					switch(request.ParentType)
					{
						case EnumDeployStepParentType.Component:
							return _projectManager.GetComponentDeploymentStep(request.Id);
						case EnumDeployStepParentType.Configuration:
							return _projectManager.GetConfigurationDeploymentStepList(request.Id);
						default:
							throw new UnknownEnumValueException(request.ParentType);
					}
				}
				else
				{
					switch(request.ParentType)
					{
						case EnumDeployStepParentType.Component:
							return _projectManager.GetComponentDeploymentStepList(request.ParentId);
						case EnumDeployStepParentType.Configuration:
							return _projectManager.GetConfigurationDeploymentStepList(request.ParentId);
						default:
							throw new UnknownEnumValueException(request.ParentType);
					}
				}
			}
		}

		public object Post(DeployStep deploymentStep)
		{
			if (string.IsNullOrEmpty(deploymentStep.Id))
			{
				switch(deploymentStep.ParentType)
				{
					case EnumDeployStepParentType.Component:
						return _projectManager.CreateComponentDeploymentStep(deploymentStep.ProjectId, deploymentStep.ParentId, deploymentStep.StepName, deploymentStep.TaskTypeName, deploymentStep.TaskOptionsJson);
					case EnumDeployStepParentType.Configuration:
						return _projectManager.CreateConfigurationDeploymentStep(deploymentStep.ProjectId, deploymentStep.ParentId, deploymentStep.StepName, deploymentStep.TaskTypeName, deploymentStep.TaskOptionsJson);
					default:
						throw new UnknownEnumValueException(deploymentStep.ParentType);
				}
			}
			else
			{
				switch(deploymentStep.ParentType)
				{
					case EnumDeployStepParentType.Component:
						return _projectManager.UpdateComponentDeploymentStep(deploymentStep.Id, deploymentStep.ProjectId, deploymentStep.ParentId, deploymentStep.StepName, deploymentStep.TaskTypeName, deploymentStep.TaskOptionsJson);
					case EnumDeployStepParentType.Configuration:
						return _projectManager.UpdateConfigurationDeploymentStep(deploymentStep.Id, deploymentStep.ProjectId, deploymentStep.ParentId, deploymentStep.StepName, deploymentStep.TaskTypeName, deploymentStep.TaskOptionsJson);
					default:
						throw new UnknownEnumValueException(deploymentStep.ParentType);
				}
			}
		}

		public void Delete(DeployStep deploymentStep)
		{
			if(deploymentStep == null)
			{
				throw new ArgumentNullException("request is null");
			}
			else if (string.IsNullOrEmpty(deploymentStep.Id))
			{
				throw new ArgumentNullException("request.id is null");
			}
			switch(deploymentStep.ParentType)
			{ 
				case EnumDeployStepParentType.Component:
					_projectManager.DeleteComponentDeploymentStep(deploymentStep.Id);
					break;
				case EnumDeployStepParentType.Configuration:
					_projectManager.DeleteConfigurationDeploymentStep(deploymentStep.Id);
					break;
				default:
					throw new UnknownEnumValueException(deploymentStep.ParentType);
			}
		}

	}
}