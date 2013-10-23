using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PagedList;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Notifications;
using Sriracha.Deploy.Data.Repository;
using Sriracha.Deploy.Data.Tasks;

namespace Sriracha.Deploy.Data.Impl
{
	public class DeployRequestManager : IDeployRequestManager
	{
		private readonly IBuildRepository _buildRepository;
		private readonly IProjectRepository _projectRepository;
		private readonly IDeployRepository _deployRepository;
		private readonly IDeploymentValidator _validator;
		private readonly IProjectNotifier _projectNotifier;

		public DeployRequestManager(IBuildRepository buildRepository, IProjectRepository projectRepository, IDeployRepository deployRepository, IDeploymentValidator validator, IProjectNotifier projectNotifier)
		{
			_buildRepository = DIHelper.VerifyParameter(buildRepository);
			_projectRepository = DIHelper.VerifyParameter(projectRepository);
			_deployRepository = DIHelper.VerifyParameter(deployRepository);
			_validator = DIHelper.VerifyParameter(validator);
			_projectNotifier = DIHelper.VerifyParameter(projectNotifier);
		}

		public DeployRequestTemplate InitializeDeployRequest(string buildId, string environmentId)
		{
			var build = _buildRepository.GetBuild(buildId);
			var project = _projectRepository.GetProject(build.ProjectId);
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


		public DeployState SubmitDeployRequest(string projectId, string buildId, string environmentId, IEnumerable<string> machineIdList)
		{
			var build = _buildRepository.GetBuild(buildId);
			var project = _projectRepository.GetProject(projectId);
			var environment = project.GetEnvironment(environmentId);
			var component = project.GetComponent(build.ProjectComponentId);
			var branch = project.GetBranch(build.ProjectBranchId);
			var validationResult = _validator.ValidateDeployment(project, component, environment);
			var machineList = new List<DeployMachine>();
			foreach(string id in machineIdList)
			{
				var machine = _projectRepository.GetMachine(id);
				machineList.Add(machine);
			}
			return  _deployRepository.CreateDeployment(build, branch, environment, component, machineList, null);
		}


		public List<DeployBatchRequest> GetDeployBatchRequestList()
		{
			return _deployRepository.GetBatchRequestList();
		}

		public DeployBatchRequest GetDeployBatchRequest(string id)
		{
			return _deployRepository.GetBatchRequest(id);
		}


		public DeployBatchRequest CreateDeployBatchRequest(List<DeployBatchRequestItem> itemList, EnumDeployStatus initialStatus, string deploymentLabel)
		{
			var request = _deployRepository.CreateBatchRequest(itemList, DateTime.UtcNow, initialStatus, deploymentLabel);
			_projectNotifier.SendDeployRequestedNotification(request);
			return request;
		}

		public PagedSortedList<DeployBatchStatus> GetDeployBatchStatusList(ListOptions listOptions)
		{
			return _deployRepository.GetDeployBatchStatusList(listOptions);
		}

		public DeployBatchStatus GetDeployBatchStatus(string deployBatchRequestId)
		{
			var status = new DeployBatchStatus
			{
				DeployBatchRequestId = deployBatchRequestId,
				Request = _deployRepository.GetBatchRequest(deployBatchRequestId),
				DeployStateList = _deployRepository.GetDeployStateSummaryListByDeployBatchRequestItemId(deployBatchRequestId)
			};
			//foreach(var requestItem in status.Request.ItemList)
			//{
			//	var state = _deployRepository.GetDeployStateSummaryListByDeployBatchRequestItemId(deployBatchRequestId);
			//	if(state != null)
			//	{
			//		status.DeployStateList.Add(state);
			//	}
			//}
			return status;
		}


		public DeployBatchRequest UpdateDeployBatchStatus(string deployBatchRequestId, EnumDeployStatus newStatus, string statusMessage)
		{
			var item = _deployRepository.GetBatchRequest(deployBatchRequestId);
			_validator.ValidateStatusTransition(item.Status, newStatus);
			switch(newStatus)
			{
				case EnumDeployStatus.Approved:
					_projectNotifier.SendDeployApprovedNotification(item);
					break;
				case EnumDeployStatus.Rejected:
					_projectNotifier.SendDeployRejectedNotification(item);
					break;
			}
			return _deployRepository.UpdateBatchDeploymentStatus(deployBatchRequestId, newStatus, statusMessage:statusMessage);
		}
	}
}
