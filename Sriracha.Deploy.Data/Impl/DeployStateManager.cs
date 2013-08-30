using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Repository;

namespace Sriracha.Deploy.Data.Impl
{
	public class DeployStateManager : IDeployStateManager
	{
		private readonly IDeployRepository _deployRepository;
		private readonly IBuildRepository _buildRepository;
		private readonly IProjectRepository _projectRepository;
		private readonly IDeploymentValidator _validator;

		public DeployStateManager(IDeployRepository deployRepository, IBuildRepository buildRepository, IProjectRepository projectRepository, IDeploymentValidator deploymentValidator)
		{
			_deployRepository = DIHelper.VerifyParameter(deployRepository);
			_buildRepository = DIHelper.VerifyParameter(buildRepository);
			_projectRepository = DIHelper.VerifyParameter(projectRepository);
			_validator = DIHelper.VerifyParameter(deploymentValidator);
		}

		public DeployState GetDeployState(string deployStateId)
		{
			return _deployRepository.GetDeployState(deployStateId);
		}

		public DeployState CreateDeployState(string projectId, string buildId, string environmentId, string machineId, string deployBatchRequestItemId)
		{
			var build = _buildRepository.GetBuild(buildId);
			var environment = _projectRepository.GetEnvironment(environmentId);
			var component = _projectRepository.GetComponent(build.ProjectComponentId);
			var branch = _projectRepository.GetBranch(build.ProjectBranchId);
			var validationResult = _validator.ValidateDeployment(component, environment);
			var machineList = new List<DeployMachine>()
			{
				_projectRepository.GetMachine(machineId)
			};
			return _deployRepository.CreateDeployment(build, branch, environment, component, machineList, deployBatchRequestItemId);
		}


		public DeployState PopNextDeployment()
		{
			return _deployRepository.PopNextDeployment();
		}


		public DeployStateMessage AddDeploymentMessage(string deployStateId, string message)
		{
			return _deployRepository.AddDeploymentMessage(deployStateId, message);
		}

		public void MarkDeploymentInProcess(string deployStateId)
		{
			_deployRepository.UpdateDeploymentStatus(deployStateId, EnumDeployStatus.InProcess);
		}

		public void MarkDeploymentSuccess(string deployStateId)
		{
			_deployRepository.UpdateDeploymentStatus(deployStateId, EnumDeployStatus.Success);
		}

		public void MarkDeploymentFailed(string deployStateId, Exception err)
		{
			_deployRepository.UpdateDeploymentStatus(deployStateId, EnumDeployStatus.Error, err);
		}

		public DeployBatchRequest PopNextBatchDeployment()
		{
			return _deployRepository.PopNextBatchDeployment();
		}


		public void MarkBatchDeploymentSuccess(string deployBatchRequestId)
		{
			_deployRepository.UpdateBatchDeploymentStatus(deployBatchRequestId, EnumDeployStatus.Success);
		}

		public void MarkBatchDeploymentFailed(string deployBatchRequestId, Exception err)
		{
			_deployRepository.UpdateBatchDeploymentStatus(deployBatchRequestId, EnumDeployStatus.Error, err);
		}
	}
}
