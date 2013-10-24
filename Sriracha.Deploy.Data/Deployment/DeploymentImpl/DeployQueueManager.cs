using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Deployment.DeploymentImpl
{
	public class DeployQueueManager : IDeployQueueManager
	{
		private readonly IDeployRepository _deployRepository;

		public DeployQueueManager(IDeployRepository deployRepository)
		{
			_deployRepository = DIHelper.VerifyParameter(deployRepository);
		}

		public PagedSortedList<DeployBatchRequest> GetQueue(ListOptions listOptions)
		{
			return _deployRepository.GetDeployQueue(listOptions);
		}
	}
}
