using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Dto.Deployment;

namespace Sriracha.Deploy.Data.Deployment
{
	public interface IOfflineDeploymentManager
	{
		OfflineDeployment BeginCreateOfflineDeployment(List<DeployBatchRequestItem> itemList, string deploymentLabel);
	}
}
