﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PagedList;
using Sriracha.Deploy.Data.Dto;

namespace Sriracha.Deploy.Data.Repository
{
	public interface IDeployRepository
	{
		[Obsolete] DeployState CreateDeployment(DeployBuild build, DeployProjectBranch branch, DeployEnvironment environment, DeployComponent component, IEnumerable<DeployMachine> machineList, string deployBatchRequestItemId);
		[Obsolete] DeployState PopNextDeployment();
		[Obsolete] DeployBatchRequest PopNextBatchDeployment();
		[Obsolete] DeployState UpdateDeploymentStatus(string deployStateId, EnumDeployStatus enumDeployStatus, Exception err = null);

		DeployState GetDeployState(string deployStateId);
		List<DeployState> FindDeployStateListForEnvironment(string buildId, string environmentId);
		List<DeployState> FindDeployStateListForMachine(string buildId, string environmentId, string machineId);
		DeployStateMessage AddDeploymentMessage(string deployStateId, string message);

		List<DeployBatchRequest> GetBatchRequestList();
		DeployBatchRequest CreateBatchRequest(List<DeployBatchRequestItem> itemList, DateTime submittedDateTimeUtc, EnumDeployStatus status, string deploymentLabel);
		DeployBatchRequest GetBatchRequest(string id);
		DeployBatchRequest UpdateBatchDeploymentStatus(string deployBatchRequestId, EnumDeployStatus status, Exception err = null, string statusMessage = null);

		DeployBatchRequest SetCancelRequested(string deployBatchRequestId, string userMessage);

		PagedSortedList<DeployBatchStatus> GetDeployBatchStatusList(ListOptions listOptions);
		List<DeployStateSummary> GetDeployStateSummaryListByDeployBatchRequestItemId(string deployBatchRequestItemId);
		PagedSortedList<DeployBatchRequest> GetDeployQueue(ListOptions listOptions);
	}
}
