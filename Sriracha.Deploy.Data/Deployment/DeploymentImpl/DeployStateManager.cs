using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Notifications;
using Sriracha.Deploy.Data.Repository;
using Sriracha.Deploy.Data.Dto.Project;
using Sriracha.Deploy.Data.Dto.Deployment;
using Sriracha.Deploy.Data.Dto.Deployment.Plan;

namespace Sriracha.Deploy.Data.Deployment.DeploymentImpl
{
	public class DeployStateManager : IDeployStateManager
	{
        private readonly IDeployStateRepository _deployStateRepository;
		private readonly IDeployRepository _deployRepository;
		private readonly IBuildRepository _buildRepository;
		private readonly IProjectRepository _projectRepository;
		private readonly IDeploymentValidator _validator;
		private readonly IProjectNotifier _projectNotifier;

		public DeployStateManager(IDeployRepository deployRepository, IDeployStateRepository deployStateRepository, IBuildRepository buildRepository, IProjectRepository projectRepository, IDeploymentValidator deploymentValidator, IProjectNotifier projectNotifier)
		{
			_deployRepository = DIHelper.VerifyParameter(deployRepository);
            _deployStateRepository = DIHelper.VerifyParameter(deployStateRepository);
			_buildRepository = DIHelper.VerifyParameter(buildRepository);
			_projectRepository = DIHelper.VerifyParameter(projectRepository);
			_validator = DIHelper.VerifyParameter(deploymentValidator);
			_projectNotifier = DIHelper.VerifyParameter(projectNotifier);
		}

		public DeployState GetDeployState(string deployStateId)
		{
			return _deployStateRepository.GetDeployState(deployStateId);
		}

		public DeployState CreateDeployState(string projectId, string buildId, string environmentId, string machineId, string deployBatchRequestItemId)
		{
			var build = _buildRepository.GetBuild(buildId);
			var project = _projectRepository.GetProject(projectId);
			var environment = project.GetEnvironment(environmentId);
			var component = project.GetComponent(build.ProjectComponentId);
			var branch = project.GetBranch(build.ProjectBranchId);
			var validationResult = _validator.ValidateDeployment(project, component, environment);
			var machineList = new List<DeployMachine>()
			{
				project.GetMachine(machineId)
			};
			return _deployStateRepository.CreateDeployment(build, branch, environment, component, machineList, deployBatchRequestItemId);
		}

		public DeployState GetOrCreateDeployState(string projectId, string buildId, string environmentId, string machineId, string deployBatchRequestItemId)
		{
			var state = _deployStateRepository.TryGetDeployState(projectId, buildId, environmentId, machineId, deployBatchRequestItemId);
			if(state != null)
			{
				return state;
			}
			else 
			{
				return this.CreateDeployState(projectId, buildId, environmentId, machineId, deployBatchRequestItemId);
			}
		}

		public DeployStateMessage AddDeploymentMessage(string deployStateId, string message)
		{
			return _deployStateRepository.AddDeploymentMessage(deployStateId, message);
		}

		public void MarkDeploymentInProcess(string deployStateId)
		{
			_deployStateRepository.UpdateDeploymentStatus(deployStateId, EnumDeployStatus.InProcess);
		}

		public void MarkDeploymentSuccess(string deployStateId)
		{
            _deployStateRepository.UpdateDeploymentStatus(deployStateId, EnumDeployStatus.Success);
		}

		public void MarkDeploymentFailed(string deployStateId, Exception err)
		{
            _deployStateRepository.UpdateDeploymentStatus(deployStateId, EnumDeployStatus.Error, err);
		}

		public void MarkBatchDeploymentSuccess(string deployBatchRequestId)
		{
			var deployRequest = _deployRepository.UpdateBatchDeploymentStatus(deployBatchRequestId, EnumDeployStatus.Success, statusMessage:"Deployment completed successfully");
			_projectNotifier.SendDeploySuccessNotification(deployRequest);
		}

		public void MarkBatchDeploymentFailed(string deployBatchRequestId, Exception err)
		{
			var deployRequest = _deployRepository.UpdateBatchDeploymentStatus(deployBatchRequestId, EnumDeployStatus.Error, err, "Deployment error");
			_projectNotifier.SendDeployFailedNotification(deployRequest);
		}

		public void MarkBatchDeploymentCancelled(string deployBatchRequestId, string cancelMessage)
		{
			string statusMessage = string.Format("Deployment was cancelled at {0} UTC", DateTime.UtcNow);
			if(!string.IsNullOrEmpty(cancelMessage) && cancelMessage  != "null")
			{
				statusMessage += ", Notes: " + cancelMessage;
			}
			var deployRequest = _deployRepository.UpdateBatchDeploymentStatus(deployBatchRequestId, EnumDeployStatus.Cancelled, statusMessage: statusMessage);
			_projectNotifier.SendDeployCancelledNotification(deployRequest);
		}


		public void MarkBatchDeploymentResumed(string deployBatchRequestId, string resumeMessage)
		{
			string statusMessage = string.Format("Deployment was resumed at {0} UTC", DateTime.UtcNow);
			if (!string.IsNullOrEmpty(resumeMessage) && resumeMessage != "null")
			{
				statusMessage += ", Notes: " + resumeMessage;
			}
			var deployRequest = _deployRepository.UpdateBatchDeploymentStatus(deployBatchRequestId, EnumDeployStatus.InProcess, statusMessage: statusMessage);
		}


		public DeploymentPlan SaveDeploymentPlan(DeploymentPlan plan)
		{
			return _deployRepository.SaveDeploymentPlan(plan);
		}
	}
}
