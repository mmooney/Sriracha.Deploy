using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PagedList;
using Sriracha.Deploy.Data.Dto;

namespace Sriracha.Deploy.Data.Repository
{
	public interface IDeployRepository
	{
		DeployState CreateDeployment(DeployBuild build, DeployProjectBranch branch, DeployEnvironment environment, DeployComponent component, IEnumerable<DeployMachine> machineList, string deployBatchRequestItemId);
		DeployState GetDeployState(string deployStateId);
		DeployState PopNextDeployment();
		DeployBatchRequest PopNextBatchDeployment();
		DeployState UpdateDeploymentStatus(string deployStateId, EnumDeployStatus enumDeployStatus, Exception err = null);
		DeployBatchRequest UpdateBatchDeploymentStatus(string deployBatchRequestId, EnumDeployStatus status, Exception err = null);

		DeployStateMessage AddDeploymentMessage(string deployStateId, string message);

		List<DeployBatchRequest> GetBatchRequestList();
		DeployBatchRequest GetBatchRequest(string id);

		DeployBatchRequest CreateBatchRequest(List<DeployBatchRequestItem> itemList, DateTime submittedDateTimeUtc);

		PagedSortedList<DeployBatchStatus> GetDeployBatchStatusList(ListOptions listOptions);
		DeployStateSummary TryGetDeployStateSummaryByDeployBatchRequestItemId(string deployBatchRequestItemId);


	}
}
