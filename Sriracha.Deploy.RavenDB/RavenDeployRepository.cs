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

		public DeployState CreateDeployment(DeployBuild build, DeployProjectBranch branch, DeployEnvironment environment, DeployComponent component, IEnumerable<DeployMachine> machineList, string deployBatchRequestItemId)
		{
			var deployState = new DeployState
			{
				Id = Guid.NewGuid().ToString(),
				ProjectId = environment.ProjectId,
				Build = build,
				Branch = branch,
				Environment = environment,
				Component = component,
				MachineList = machineList.ToList(),
				Status = EnumDeployStatus.NotStarted,
				SubmittedDateTimeUtc = DateTime.UtcNow,
				DeployBatchRequestItemId = deployBatchRequestItemId,
				CreatedDateTimeUtc = DateTime.UtcNow,
				CreatedByUserName = _userIdentity.UserName,
				UpdatedDateTimeUtc = DateTime.UtcNow,
				UpdatedByUserName = _userIdentity.UserName
			};
			_documentSession.Store(deployState);
			_documentSession.SaveChanges();
			return deployState;
		}

		public DeployState GetDeployState(string deployStateId)
		{
			if (string.IsNullOrWhiteSpace(deployStateId))
			{
				throw new ArgumentNullException("Missing Deploy State ID");
			}
			var returnValue = _documentSession.Load<DeployState>(deployStateId);
			if (returnValue == null)
			{
				throw new RecordNotFoundException(typeof(DeployState), "Id", deployStateId);
			}
			return returnValue;
		}

		public DeployState PopNextDeployment()
		{
			using(var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable }))
			{
				this._logger.Trace("Checking for next deployment");
				var tempItem = this._documentSession.Query<DeployState>()
										.Customize(i=>i.WaitForNonStaleResultsAsOfNow(TimeSpan.FromSeconds(30)))
										.OrderBy(i=>i.SubmittedDateTimeUtc)
										.Where(i=>i.Status == EnumDeployStatus.NotStarted)
										.FirstOrDefault();
				if(tempItem == null)
				{
					this._logger.Trace("No pending deployment found");
					return null;
				}
				
				var reloadedItem = this._documentSession.Load<DeployState>(tempItem.Id);
				if(reloadedItem.Status != EnumDeployStatus.NotStarted)
				{
					this._logger.Warn("Stale pending deployment found, actual status: " + reloadedItem.Status.ToString());
					return null;
				}

				reloadedItem.Status = EnumDeployStatus.InProcess;
				reloadedItem.DeploymentStartedDateTimeUtc = DateTime.UtcNow;
				this._documentSession.SaveChanges();

				transaction.Complete();

				return reloadedItem;
			}
		}

		public List<DeployState> FindDeployStateListForEnvironment(string buildId, string environmentId)
		{
			return _documentSession.Query<DeployState>().Where(i=>i.Build.Id == buildId && i.Environment.Id == environmentId).ToList();
		}


		public List<DeployState> FindDeployStateListForMachine(string buildId, string environmentId, string machineId)
		{
			var tempList = _documentSession.Query<DeployState>().Where(i => i.Build.Id == buildId && i.Environment.Id == environmentId).ToList();
			return tempList.Where(i=>i.MachineList.Any(j=>j.Id == machineId)).ToList();;
		}

		public DeployBatchRequest PopNextBatchDeployment()
		{
			string itemId = null;
			using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.Serializable }))
			{
				this._logger.Trace("Checking for next deployment");
				var tempItem = this._documentSession.Query<DeployBatchRequest>()
										.Customize(i => i.WaitForNonStaleResultsAsOfNow(TimeSpan.FromSeconds(30)))
										.OrderBy(i => i.SubmittedDateTimeUtc)
										.Where(i => i.Status == EnumDeployStatus.NotStarted)
										.FirstOrDefault();
				if (tempItem == null)
				{
					this._logger.Trace("No pending deployment found");
					return null;
				}

				var reloadedItem = this._documentSession.Load<DeployBatchRequest>(tempItem.Id);
				if (reloadedItem.Status != EnumDeployStatus.NotStarted)
				{
					this._logger.Warn("Stale pending deployment found, actual status: " + reloadedItem.Status.ToString());
					return null;
				}

				reloadedItem.Status = EnumDeployStatus.InProcess;
				reloadedItem.StartedDateTimeUtc = DateTime.UtcNow;
				itemId = reloadedItem.Id;
				this._documentSession.SaveChanges();

				transaction.Complete();
			}
			if(string.IsNullOrEmpty(itemId))
			{
				return null;
			}
			else 
			{
				return _documentSession.Load<DeployBatchRequest>(itemId);
			}
		}

		public DeployStateMessage AddDeploymentMessage(string deployStateId, string message)
		{
			var deployStateMessage = CreateDeploymentMessage(deployStateId, message);
			var state = GetDeployState(deployStateId);
			state.MessageList.Add(deployStateMessage);
			this._documentSession.SaveChanges();
			return deployStateMessage;
		}

		private DeployStateMessage CreateDeploymentMessage(string deployStateId, string message)
		{
			var deployStateMessage = new DeployStateMessage
			{
				Id = Guid.NewGuid().ToString(),
				DeployStateId = deployStateId,
				Message = message,
				DateTimeUtc = DateTime.UtcNow,
				MessageUserName = _userIdentity.UserName
			};
			return deployStateMessage;
		}


		public DeployState UpdateDeploymentStatus(string deployStateId, EnumDeployStatus status, Exception err = null)
		{
			var state = GetDeployState(deployStateId);
			if(state.MessageList == null || state.MessageList.Count == 0)
			{
				_documentSession.Advanced.Evict(state);
				state = GetDeployState(deployStateId);
			}
			state.Status = status;
			switch(status)
			{
				case EnumDeployStatus.Success:
				case EnumDeployStatus.Error:
					state.DeploymentCompleteDateTimeUtc = DateTime.UtcNow;
					break;
			}
			if(err != null)
			{
				state.ErrorDetails = err.ToString();
			}
			state.UpdatedDateTimeUtc = DateTime.UtcNow;
			state.UpdatedByUserName = _userIdentity.UserName;
			this._documentSession.SaveChanges();
			return state;
		}


		public List<DeployBatchRequest> GetBatchRequestList()
		{
			return _documentSession.Query<DeployBatchRequest>().ToList();
		}

		public DeployBatchRequest GetBatchRequest(string id)
		{
			return _documentSession.Load<DeployBatchRequest>(id);
		}

		public DeployBatchRequest CreateBatchRequest(List<DeployBatchRequestItem> itemList, DateTime submittedDateTimeUtc, EnumDeployStatus status)
		{
			switch(status)
			{
				case EnumDeployStatus.Unknown:
					status = EnumDeployStatus.NotStarted;
					break;
				case EnumDeployStatus.NotStarted:
				case EnumDeployStatus.Requested:
					//OK
					break;
				case EnumDeployStatus.Error:
				case EnumDeployStatus.InProcess:
				case EnumDeployStatus.Success:
				case EnumDeployStatus.Warning:
					throw new ArgumentException(EnumHelper.GetDisplayValue(status) + " is not a valid initial status for a batch deployment request");
				default:
					throw new UnknownEnumValueException(status);
			}
			foreach(var item in itemList)
			{
				item.Id = Guid.NewGuid().ToString();
			}
			var request = new DeployBatchRequest
			{
				Id = Guid.NewGuid().ToString(),
				SubmittedDateTimeUtc = submittedDateTimeUtc,
				ItemList = itemList,
				Status = status,
				CreatedDateTimeUtc = DateTime.UtcNow,
				CreatedByUserName = _userIdentity.UserName,
				UpdatedDateTimeUtc = DateTime.UtcNow,
				UpdatedByUserName = _userIdentity.UserName

			};
			_documentSession.Store(request);
			_documentSession.SaveChanges();
			return request;
		}


		public List<DeployStateSummary> GetDeployStateSummaryListByDeployBatchRequestItemId(string deployBatchRequestItemId)
		{
			var list = (from i in _documentSession.Query<DeployState>()
							where i.DeployBatchRequestItemId == deployBatchRequestItemId
							select i).ToList();
			var returnList = new List<DeployStateSummary>();
			foreach(var dbItem in list)
			{
				var returnItem = Mapper.Map(dbItem, new DeployStateSummary());
				returnList.Add(returnItem);
			}
			return returnList;
		}


		public PagedSortedList<DeployBatchStatus> GetDeployBatchStatusList(ListOptions listOptions)
		{
			var requestList = _documentSession.QueryPageAndSort<DeployBatchRequest>(listOptions, "SubmittedDateTimeUtc", false);
			var returnListItems = requestList.Select(i=>BuildDeployBatchStatus(i)).ToList();
			var pagedList = new StaticPagedList<DeployBatchStatus>(returnListItems, requestList.PageNumber, requestList.PageSize, requestList.TotalItemCount);
			return new PagedSortedList<DeployBatchStatus>(pagedList, listOptions.SortField, listOptions.SortAscending.Value);
		}

		private DeployBatchStatus BuildDeployBatchStatus(DeployBatchRequest deployBatchRequest)
		{
			var status = new DeployBatchStatus
			{
				Request = deployBatchRequest,
				DeployBatchRequestId = deployBatchRequest.Id,
				DeployStateList = this.GetDeployStateSummaryListByDeployBatchRequestItemId(deployBatchRequest.Id)
			};
			return status;
		}


		public DeployBatchRequest UpdateBatchDeploymentStatus(string deployBatchRequestId, EnumDeployStatus status, Exception err = null, string statusMessage=null)
		{
			var batchRequest = GetBatchRequest(deployBatchRequestId);
			var oldStatus = batchRequest.Status;
			batchRequest.Status = status;
			switch (status)
			{
				case EnumDeployStatus.Success:
				case EnumDeployStatus.Error:
					batchRequest.CompleteDateTimeUtc = DateTime.UtcNow;
					break;
			}
			if (err != null)
			{
				batchRequest.ErrorDetails = err.ToString();
			}
			string message = string.Format("{0} changed status from {1} to {2} at {3} UTC.", _userIdentity.UserName, EnumHelper.GetDisplayValue(oldStatus), EnumHelper.GetDisplayValue(status), DateTime.UtcNow);
			if(!string.IsNullOrEmpty(statusMessage))
			{
				message += "  Notes: " + statusMessage;
			}
			batchRequest.MessageList.Add(message);
			batchRequest.UpdatedDateTimeUtc = DateTime.UtcNow;
			batchRequest.UpdatedByUserName = _userIdentity.UserName;
			this._documentSession.SaveChanges();
			return batchRequest;
		}
	}
}
