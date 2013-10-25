using Sriracha.Deploy.Data.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Deployment
{
	public interface IDeployQueueManager
	{
		PagedSortedList<DeployBatchRequest> GetQueue(ListOptions listOptions, List<EnumDeployStatus> statusList = null, List<string> environmentIds = null, bool includeResumeRequested = true);

		DeployBatchRequest PopNextBatchDeployment();

		DeployBatchRequest RequeueDeployment(string deployBatchRequestId, string message);
	}
}
