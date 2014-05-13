using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using AutoMapper;
using MMDB.Shared;
using NLog;
using PagedList;
using Raven.Client;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Repository;
using Raven.Client.Linq;
using Sriracha.Deploy.Data.Dto.Project;
using Sriracha.Deploy.Data.Dto.Deployment;
using Sriracha.Deploy.Data.Dto.Build;
using Sriracha.Deploy.Data.Dto.Deployment.Plan;

namespace Sriracha.Deploy.RavenDB
{
	public class RavenDeployRepository : IDeployRepository
	{
		private readonly IDocumentSession _documentSession;
		private readonly Logger _logger;
		private readonly IUserIdentity _userIdentity;

		public RavenDeployRepository(IDocumentSession documentSession, Logger logger, IUserIdentity userIdentity)
		{
			_documentSession = DIHelper.VerifyParameter(documentSession);
			_logger = DIHelper.VerifyParameter(logger);
			_userIdentity = DIHelper.VerifyParameter(userIdentity);
		}

        public PagedSortedList<DeployBatchRequest> GetBatchRequestList(ListOptions listOptions)
        {
            listOptions = ListOptions.SetDefaults(listOptions, 10, 1, "SubmittedDateTimeUtc", false);
            var pagedList = _documentSession.QueryPageAndSort<DeployBatchRequest>(listOptions, "SubmittedDateTimeUtc", false);
            return new PagedSortedList<DeployBatchRequest>(pagedList, listOptions.SortField, listOptions.SortAscending.Value);
        }

        public DeployBatchRequest CreateBatchRequest(List<DeployBatchRequestItem> itemList, EnumDeployStatus status, string deploymentLabel)
        {
            if(itemList == null || itemList.Count == 0)
            {
                throw new ArgumentNullException("itemList");
            }
            foreach (var item in itemList)
            {
                if(string.IsNullOrEmpty(item.Id))
                {
                    item.Id = Guid.NewGuid().ToString();
                }
            }
            string message = string.Format("{0} created deployment request with status of {1} at {2} UTC.", _userIdentity.UserName, EnumHelper.GetDisplayValue(status), DateTime.UtcNow);
            var request = new DeployBatchRequest
            {
                Id = Guid.NewGuid().ToString(),
                SubmittedDateTimeUtc = DateTime.UtcNow,
                SubmittedByUserName = _userIdentity.UserName,
                DeploymentLabel = deploymentLabel,
                ItemList = itemList,
                LastStatusMessage = message,
                Status = status,
                CreatedDateTimeUtc = DateTime.UtcNow,
                CreatedByUserName = _userIdentity.UserName,
                UpdatedDateTimeUtc = DateTime.UtcNow,
                UpdatedByUserName = _userIdentity.UserName

            };
            request.MessageList.Add(message);
            _documentSession.StoreSaveEvict(request);
            return request;
        }

        public DeployBatchRequest PopNextBatchDeployment()
		{
			string itemId = null;
			using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable }))
			{
				this._logger.Trace("Checking for next deployment");
				var tempItem = this._documentSession.QueryNoCache<DeployBatchRequest>()
										.Customize(i => i.WaitForNonStaleResultsAsOfNow(TimeSpan.FromSeconds(30)))
										.OrderBy(i => i.SubmittedDateTimeUtc)
										.Where(i => i.Status == EnumDeployStatus.NotStarted
														|| (i.ResumeRequested && (i.Status == EnumDeployStatus.Error || i.Status == EnumDeployStatus.Cancelled)))
										.FirstOrDefault();
				if (tempItem == null)
				{
					this._logger.Trace("No pending deployment found");
					return null;
				}

				var reloadedItem = this._documentSession.LoadEnsure<DeployBatchRequest>(tempItem.Id);
				if (reloadedItem.Status != EnumDeployStatus.NotStarted
						&& !(reloadedItem.ResumeRequested && (reloadedItem.Status == EnumDeployStatus.Error || reloadedItem.Status == EnumDeployStatus.Cancelled)))
				{
					this._logger.Warn("Stale pending deployment found, actual status: " + reloadedItem.Status.ToString());
					return null;
				}

				reloadedItem.Status = EnumDeployStatus.InProcess;
				reloadedItem.StartedDateTimeUtc = DateTime.UtcNow;
                reloadedItem.UpdatedByUserName = _userIdentity.UserName;
                reloadedItem.UpdatedDateTimeUtc = DateTime.UtcNow;
				itemId = reloadedItem.Id;
				this._documentSession.SaveEvict(reloadedItem);

				transaction.Complete();
			}
			if(string.IsNullOrEmpty(itemId))
			{
				return null;
			}
			else 
			{
				return _documentSession.LoadNoCache<DeployBatchRequest>(itemId);
			}
		}

		public DeployBatchRequest GetBatchRequest(string id)
		{
			return _documentSession.LoadEnsureNoCache<DeployBatchRequest>(id);
		}

		public DeployBatchRequest UpdateBatchDeploymentStatus(string deployBatchRequestId, EnumDeployStatus status, Exception err = null, string statusMessage=null, bool addToMessageHistory=true)
		{
			var batchRequest = _documentSession.LoadEnsure<DeployBatchRequest>(deployBatchRequestId);
			var oldStatus = batchRequest.Status;
			batchRequest.Status = status;
			switch (status)
			{
				case EnumDeployStatus.Success:
				case EnumDeployStatus.Error:
					batchRequest.CompleteDateTimeUtc = DateTime.UtcNow;
					break;
				case EnumDeployStatus.InProcess:
					batchRequest.ResumeRequested = false;
					break;
				case EnumDeployStatus.Cancelled:
					batchRequest.CancelRequested = false;
					break;
			}
			if (err != null)
			{
				batchRequest.ErrorDetails = err.ToString();
			}
			if(addToMessageHistory)
			{
				batchRequest.MessageList.Add(statusMessage);
			}
			batchRequest.LastStatusMessage = statusMessage;
			batchRequest.UpdatedDateTimeUtc = DateTime.UtcNow;
			batchRequest.UpdatedByUserName = _userIdentity.UserName;
			this._documentSession.SaveEvict(batchRequest);
			return batchRequest;
		}


		public PagedSortedList<DeployBatchRequest> GetDeployQueue(ListOptions listOptions, List<EnumDeployStatus> statusList = null, List<string> environmentIds = null, bool includeResumeRequested=true)
		{
			if(statusList == null || statusList.Count == 0)
			{
				statusList = new List<EnumDeployStatus> { EnumDeployStatus.NotStarted, EnumDeployStatus.InProcess };
			}
			var query = (IRavenQueryable<DeployBatchRequest>)_documentSession.QueryNoCache<DeployBatchRequest>()
										//.OrderBy(i => i.SubmittedDateTimeUtc)
										.Where(i => i.Status.In(statusList) 
												|| (i.ResumeRequested && (i.Status == EnumDeployStatus.Error || i.Status == EnumDeployStatus.Cancelled)));
			if(environmentIds != null && environmentIds.Count != 0)
			{
				query = (IRavenQueryable<DeployBatchRequest>)query
								.Where(i=>i.ItemList
											.Any(j=>j.MachineList
												.Any(k=>k.EnvironmentId.In(environmentIds))));
			}
            listOptions = ListOptions.SetDefaults(listOptions, 5, 1,  "SubmittedDateTimeUtc", false);
			return query.PageAndSort(listOptions, i=>i.SubmittedDateTimeUtc);
		}

		public DeployBatchRequest SetCancelRequested(string deployBatchRequestId, string userMessage)
		{
			var request = _documentSession.LoadEnsure<DeployBatchRequest>(deployBatchRequestId);
			request.CancelRequested = true;
            request.CancelMessage = userMessage;
			string statusMessage = string.Format("{0} requested deployment to be cancelled at {1} UTC", _userIdentity.UserName, DateTime.UtcNow);
			if(!string.IsNullOrEmpty(userMessage))
			{
				statusMessage += ". Notes: " + userMessage;
			}
			request.LastStatusMessage = statusMessage;
            request.UpdatedByUserName = _userIdentity.UserName;
            request.UpdatedDateTimeUtc = DateTime.UtcNow;
			request.MessageList.Add(request.LastStatusMessage);
			_documentSession.SaveEvict(request);
			return request;
		}


		public DeployBatchRequest RequeueDeployment(string deployBatchRequestId, EnumDeployStatus newStatus, string userMessage)
		{
			var batchRequest = _documentSession.LoadEnsure<DeployBatchRequest>(deployBatchRequestId);
            string statusMessage = string.Format("{0} requested deployment to be cancelled at {1} UTC", _userIdentity.UserName, DateTime.UtcNow);
            if (!string.IsNullOrEmpty(userMessage))
            {
                statusMessage += ". Notes: " + userMessage;
            }
            batchRequest.Status = newStatus;
			batchRequest.StartedDateTimeUtc = null;
			batchRequest.LastStatusMessage = statusMessage;
			batchRequest.UpdatedDateTimeUtc = DateTime.UtcNow;
			batchRequest.UpdatedByUserName = _userIdentity.UserName;
            batchRequest.MessageList.Add(batchRequest.LastStatusMessage);
			this._documentSession.SaveEvict(batchRequest);
			return batchRequest;
		}


        //public bool HasCancelRequested(string deployBatchRequestId)
        //{
        //    var item = _documentSession.LoadEnsureNoCache<DeployBatchRequest>(deployBatchRequestId);
        //    bool returnValue = item.CancelRequested;
        //    return returnValue;
        //}


		public DeployBatchRequest SetResumeRequested(string deployBatchRequestId, string userMessage)
		{
			var request = _documentSession.LoadEnsure<DeployBatchRequest>(deployBatchRequestId);
			request.ResumeRequested = true;
            request.ResumeMessage = userMessage;
			string statusMessage = string.Format("{0} requested deployment to be resumed at {1} UTC", _userIdentity.UserName, DateTime.UtcNow);
			if (!string.IsNullOrEmpty(userMessage))
			{
				statusMessage += ". Notes: " + userMessage;
			}
			request.LastStatusMessage = statusMessage;
            request.UpdatedByUserName = _userIdentity.UserName;
            request.UpdatedDateTimeUtc = DateTime.UtcNow;
			request.MessageList.Add(statusMessage);
			_documentSession.SaveEvict(request);
			return request;
		}

        //public bool IsCancelled(string deployBatchRequestId)
        //{
        //    var item = _documentSession.LoadEnsureNoCache<DeployBatchRequest>(deployBatchRequestId);
        //    return (item.Status == EnumDeployStatus.Cancelled);
        //}

        //public DeploymentPlan SaveDeploymentPlan(DeploymentPlan plan)
        //{
        //    var deployBatchRequest = _documentSession.LoadEnsure<DeployBatchRequest>(plan.DeployBatchRequestId);
        //    deployBatchRequest.Plan = plan;
        //    _documentSession.SaveEvict(deployBatchRequest);
        //    return plan;
        //}
	}
}
