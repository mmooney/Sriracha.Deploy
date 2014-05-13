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
        private readonly IDeployStatusNotifier _deployStatusNotifier;

		public DeployStateManager(IDeployRepository deployRepository, IDeployStateRepository deployStateRepository, IBuildRepository buildRepository, IProjectRepository projectRepository, IDeploymentValidator deploymentValidator, IProjectNotifier projectNotifier, IDeployStatusNotifier deployStatusNotifier)
		{
			_deployRepository = DIHelper.VerifyParameter(deployRepository);
            _deployStateRepository = DIHelper.VerifyParameter(deployStateRepository);
			_buildRepository = DIHelper.VerifyParameter(buildRepository);
			_projectRepository = DIHelper.VerifyParameter(projectRepository);
			_validator = DIHelper.VerifyParameter(deploymentValidator);
			_projectNotifier = DIHelper.VerifyParameter(projectNotifier);
            _deployStatusNotifier = DIHelper.VerifyParameter(deployStatusNotifier);
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
			var returnValue = _deployStateRepository.CreateDeployState(build, branch, environment, component, machineList, deployBatchRequestItemId);
            _deployStatusNotifier.Notify(returnValue);
            return returnValue;
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

		public DeployState AddDeploymentMessage(string deployStateId, string message)
		{
			var state = _deployStateRepository.AddDeploymentMessage(deployStateId, message);
            _deployStatusNotifier.Notify(state);
            return state;
        }

		public void MarkDeploymentInProcess(string deployStateId)
		{
			var state = _deployStateRepository.UpdateDeploymentStatus(deployStateId, EnumDeployStatus.InProcess);
            _deployStatusNotifier.Notify(state);
        }

		public void MarkDeploymentSuccess(string deployStateId)
		{
            var state = _deployStateRepository.UpdateDeploymentStatus(deployStateId, EnumDeployStatus.Success);
            _deployStatusNotifier.Notify(state);
        }

		public void MarkDeploymentFailed(string deployStateId, Exception err)
		{
            var state = _deployStateRepository.UpdateDeploymentStatus(deployStateId, EnumDeployStatus.Error, err);
            _deployStatusNotifier.Notify(state);
        }

		public void MarkBatchDeploymentSuccess(string deployBatchRequestId)
		{
			var deployRequest = _deployRepository.UpdateBatchDeploymentStatus(deployBatchRequestId, EnumDeployStatus.Success, statusMessage:"Deployment completed successfully");
            _deployStatusNotifier.Notify(deployRequest);
			_projectNotifier.SendDeploySuccessNotification(deployRequest);
		}

		public void MarkBatchDeploymentFailed(string deployBatchRequestId, Exception err)
		{
			var deployRequest = _deployRepository.UpdateBatchDeploymentStatus(deployBatchRequestId, EnumDeployStatus.Error, err, "Deployment error");
            _deployStatusNotifier.Notify(deployRequest);
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
            _deployStatusNotifier.Notify(deployRequest);
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
            _deployStatusNotifier.Notify(deployRequest);
        }


        //public DeploymentPlan SaveDeploymentPlan(DeploymentPlan plan)
        //{
        //    return _deployRepository.SaveDeploymentPlan(plan);
        //}


        public void MarkBatchDeploymentInProcess(string deployBatchRequestId)
        {
            string statusMessage = string.Format("Deployment was started at {0} UTC", DateTime.UtcNow);
            var deployRequest = _deployRepository.UpdateBatchDeploymentStatus(deployBatchRequestId, EnumDeployStatus.InProcess, statusMessage: statusMessage);
            _deployStatusNotifier.Notify(deployRequest);
        }
    }
}
