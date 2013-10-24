using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PagedList;
using Sriracha.Deploy.Data.Dto;

namespace Sriracha.Deploy.Data
{
	public interface IDeployRequestManager
	{
		List<DeployBatchRequest> GetDeployBatchRequestList();
		DeployBatchRequest CreateDeployBatchRequest(List<DeployBatchRequestItem> itemList, EnumDeployStatus initialStatus, string deploymentLabel);
		DeployBatchRequest GetDeployBatchRequest(string id);

		PagedSortedList<DeployBatchStatus> GetDeployBatchStatusList(ListOptions listOptions);
		DeployBatchStatus GetDeployBatchStatus(string deployBatchRequestId);
		DeployBatchRequest UpdateDeployBatchStatus(string deployBatchRequestId, EnumDeployStatus newStatus, string statusMessage);

		DeployBatchRequest PerformAction(string deployBatchRequestId, EnumDeployBatchAction action, string userMessage);
	}
}
