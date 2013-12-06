using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PagedList;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Notifications;
using Sriracha.Deploy.Data.Repository;
using Sriracha.Deploy.Data.Tasks;
using MMDB.Shared;
using Sriracha.Deploy.Data.Dto.Deployment;

namespace Sriracha.Deploy.Data.Deployment.DeploymentImpl
{
	public class DeployRequestManager : IDeployRequestManager
	{
		private readonly IBuildRepository _buildRepository;
		private readonly IProjectRepository _projectRepository;
		private readonly IDeployRepository _deployRepository;
        private readonly IDeployStateRepository _deployStateRepository;
		private readonly IDeploymentValidator _validator;
		private readonly IProjectNotifier _projectNotifier;
		private readonly IUserIdentity _userIdentity;

        public DeployRequestManager(IBuildRepository buildRepository, IProjectRepository projectRepository, IDeployRepository deployRepository, IDeploymentValidator validator, IProjectNotifier projectNotifier, IUserIdentity userIdentity, IDeployStateRepository deployStateRepository)
		{
			_buildRepository = DIHelper.VerifyParameter(buildRepository);
			_projectRepository = DIHelper.VerifyParameter(projectRepository);
			_deployRepository = DIHelper.VerifyParameter(deployRepository);
            _deployStateRepository = DIHelper.VerifyParameter(deployStateRepository);
			_validator = DIHelper.VerifyParameter(validator);
			_projectNotifier = DIHelper.VerifyParameter(projectNotifier);
			_userIdentity = DIHelper.VerifyParameter(userIdentity);
		}

		public PagedSortedList<DeployBatchRequest> GetDeployBatchRequestList(ListOptions listOptions)
		{
			return _deployRepository.GetBatchRequestList(listOptions);
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
            var requestList = _deployRepository.GetBatchRequestList(listOptions);
            var returnListItems = requestList.Items.Select(i => BuildDeployBatchStatus(i)).ToList();
            var pagedList = new StaticPagedList<DeployBatchStatus>(returnListItems, requestList.PageNumber, requestList.PageSize, requestList.TotalItemCount);
            return new PagedSortedList<DeployBatchStatus>(pagedList, listOptions.SortField, listOptions.SortAscending.Value);
		}

        private DeployBatchStatus BuildDeployBatchStatus(DeployBatchRequest deployBatchRequest)
        {
            var status = new DeployBatchStatus
            {
                Request = deployBatchRequest,
                DeployBatchRequestId = deployBatchRequest.Id,
                DeployStateList = _deployStateRepository.GetDeployStateSummaryListByDeployBatchRequestItemId(deployBatchRequest.Id)
            };
            return status;
        }


		public DeployBatchStatus GetDeployBatchStatus(string deployBatchRequestId)
		{
			var status = new DeployBatchStatus
			{
				DeployBatchRequestId = deployBatchRequestId,
				Request = _deployRepository.GetBatchRequest(deployBatchRequestId),
				DeployStateList = _deployStateRepository.GetDeployStateSummaryListByDeployBatchRequestItemId(deployBatchRequestId)
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


		public DeployBatchRequest UpdateDeployBatchStatus(string deployBatchRequestId, EnumDeployStatus newStatus, string userMessage)
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
			string statusMessage = BuildStatusChangeMessage(newStatus, userMessage);
			return _deployRepository.UpdateBatchDeploymentStatus(deployBatchRequestId, newStatus, statusMessage:statusMessage);
		}

		private string BuildStatusChangeMessage(EnumDeployStatus newStatus, string userMessage)
		{
			string message = string.Format("{0} changed status to {1} at {2} UTC.", _userIdentity.UserName, EnumHelper.GetDisplayValue(newStatus), DateTime.UtcNow);
			if (!string.IsNullOrEmpty(userMessage) && userMessage != "null")
			{
				message += "  Notes: " + userMessage;
			}
			return message;
		}


		public DeployBatchRequest PerformAction(string deployBatchRequestId, EnumDeployBatchAction action, string userMessage)
		{
			switch(action)
			{
				case EnumDeployBatchAction.Cancel:
					return _deployRepository.SetCancelRequested(deployBatchRequestId, userMessage);
				case EnumDeployBatchAction.Resume:
					return _deployRepository.SetResumeRequested(deployBatchRequestId, userMessage);
				default:
					throw new UnknownEnumValueException(action);
			}
		}


		public bool HasCancelRequested(string deployBatchRequestId)
		{
			return _deployRepository.HasCancelRequested(deployBatchRequestId);
		}


		public bool IsCancelled(string deployBatchRequestId)
		{
			return _deployRepository.IsCancelled(deployBatchRequestId);
		}

		public bool IsStopped(string deployBatchRequestId)
		{
			return _deployRepository.IsStopped(deployBatchRequestId);
		}
	}
}
