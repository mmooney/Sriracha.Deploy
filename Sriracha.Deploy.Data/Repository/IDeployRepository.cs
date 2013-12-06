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
    	PagedSortedList<DeployBatchRequest> GetBatchRequestList(ListOptions listOptions);
		DeployBatchRequest CreateBatchRequest(List<DeployBatchRequestItem> itemList, DateTime submittedDateTimeUtc, EnumDeployStatus status, string deploymentLabel);
        DeployBatchRequest PopNextBatchDeployment();
        DeployBatchRequest GetBatchRequest(string id);
		DeployBatchRequest UpdateBatchDeploymentStatus(string deployBatchRequestId, EnumDeployStatus status, Exception err = null, string statusMessage = null, bool addToMessageHistory=true);

        //PagedSortedList<DeployBatchStatus> GetDeployBatchStatusList(ListOptions listOptions);
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
