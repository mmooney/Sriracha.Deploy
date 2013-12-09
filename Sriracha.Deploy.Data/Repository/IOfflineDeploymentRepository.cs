using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Dto.Deployment;

namespace Sriracha.Deploy.Data.Repository
{
	public interface IOfflineDeploymentRepository
	{
		Dto.Deployment.OfflineDeployment CreateOfflineDeployment(string deployBatchRequestId, EnumOfflineDeploymentStatus initialStatus);
	}
}
