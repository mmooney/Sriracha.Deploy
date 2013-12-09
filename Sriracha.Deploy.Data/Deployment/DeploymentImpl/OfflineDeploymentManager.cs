using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Dto.Deployment;
using Sriracha.Deploy.Data.Repository;

namespace Sriracha.Deploy.Data.Deployment.DeploymentImpl
{
	public class OfflineDeploymentManager : IOfflineDeploymentManager
	{
		private readonly IDeployRepository _deployRepository;
		private readonly IOfflineDeploymentRepository _offlineDeploymentRepository;

		public OfflineDeploymentManager(IDeployRepository deployRepository, IOfflineDeploymentRepository offlineDeploymentRepository)
		{
			_deployRepository = DIHelper.VerifyParameter(deployRepository);
			_offlineDeploymentRepository = DIHelper.VerifyParameter(offlineDeploymentRepository);
		}
		public OfflineDeployment BeginCreateOfflineDeployment(List<DeployBatchRequestItem> itemList, string deploymentLabel)
		{
			var batchRequest = _deployRepository.CreateBatchRequest(itemList, DateTime.UtcNow, EnumDeployStatus.OfflineRequested, deploymentLabel);
			return _offlineDeploymentRepository.CreateOfflineDeployment(batchRequest.Id, EnumOfflineDeploymentStatus.CreateRequested);
		}
	}
}
