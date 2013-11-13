using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PagedList;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Dto.Project;
using Sriracha.Deploy.Data.Dto.Deployment;
using Sriracha.Deploy.Data.Dto.Build;
using Sriracha.Deploy.Data.Dto.Deployment.Plan;

namespace Sriracha.Deploy.Data.Repository
{
	public interface IDeployRepository
	{
		DeployState CreateDeployment(DeployBuild build, DeployProjectBranch branch, DeployEnvironment environment, DeployComponent component, IEnumerable<DeployMachine> machineList, string deployBatchRequestItemId);
		DeployState TryGetDeployState(string projectId, string buildId, string environmentId, string machineId, string deployBatchRequestItemId);
		DeployBatchRequest PopNextBatchDeployment();
		DeployState UpdateDeploymentStatus(string deployStateId, EnumDeployStatus enumDeployStatus, Exception err = null);

		DeployState GetDeployState(string deployStateId);
		List<DeployState> FindDeployStateListForEnvironment(string buildId, string environmentId);
		List<DeployState> FindDeployStateListForMachine(string buildId, string environmentId, string machineId);
		DeployStateMessage AddDeploymentMessage(string deployStateId, string message);

		PagedSortedList<DeployBatchRequest> GetBatchRequestList(ListOptions listOptions);
		DeployBatchRequest CreateBatchRequest(List<DeployBatchRequestItem> itemList, DateTime submittedDateTimeUtc, EnumDeployStatus status, string deploymentLabel);
		DeployBatchRequest GetBatchRequest(string id);
		DeployBatchRequest UpdateBatchDeploymentStatus(string deployBatchRequestId, EnumDeployStatus status, Exception err = null, string statusMessage = null, bool addToMessageHistory=true);

		PagedSortedList<DeployBatchStatus> GetDeployBatchStatusList(ListOptions listOptions);
		List<DeployStateSummary> GetDeployStateSummaryListByDeployBatchRequestItemId(string deployBatchRequestItemId);
		PagedSortedList<DeployBatchRequest> GetDeployQueue(ListOptions listOptions, List<EnumDeployStatus> statusList = null, List<string> environmentIds = null, bool includeResumeRequested=true);
		DeployBatchRequest RequeueDeployment(string deployBatchRequestId, EnumDeployStatus enumDeployStatus, string statusMessage);

		DeployBatchRequest SetCancelRequested(string deployBatchRequestId, string userMessage);
		bool HasCancelRequested(string deployBatchRequestId);
		DeployBatchRequest SetResumeRequested(string deployBatchRequestId, string userMessage);

		bool IsStopped(string deployBatchRequestId);
		bool IsCancelled(string deployBatchRequestId);

		DeploymentPlan SaveDeploymentPlan(DeploymentPlan plan);
	}
}
