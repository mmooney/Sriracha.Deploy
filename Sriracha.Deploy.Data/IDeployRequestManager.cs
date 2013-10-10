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
		DeployRequestTemplate InitializeDeployRequest(string buildId, string environmentId);
		DeployState SubmitDeployRequest(string projectId, string buildId, string environmentId, IEnumerable<string> machineIdList);

		List<DeployBatchRequest> GetDeployBatchRequestList();
		DeployBatchRequest CreateDeployBatchRequest(List<DeployBatchRequestItem> itemList, EnumDeployStatus initialStatus);
		DeployBatchRequest GetDeployBatchRequest(string id);

		PagedSortedList<DeployBatchStatus> GetDeployBatchStatusList(ListOptions listOptions);
		DeployBatchStatus GetDeployBatchStatus(string deployBatchRequestId);
		DeployBatchRequest UpdateDeployBatchStatus(string deployBatchRequestId, EnumDeployStatus newStatus, string statusMessage);
	}
}
