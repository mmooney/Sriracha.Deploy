using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Dto;

namespace Sriracha.Deploy.Data
{
	public interface IDeployRequestManager
	{
		DeployRequestTemplate InitializeDeployRequest(string buildId, string environmentId);
		DeployState SubmitDeployRequest(string projectId, string buildId, string environmentId, IEnumerable<string> machineIdList);

		List<DeployBatchRequest> GetBatchRequestList();
		DeployBatchRequest GetBatchRequest(string id);
	}
}
