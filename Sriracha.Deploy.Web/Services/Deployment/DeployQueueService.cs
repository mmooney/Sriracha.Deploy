using ServiceStack.ServiceInterface;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sriracha.Deploy.Web.Services.Deployment
{
	public class DeployQueueService : Service
	{
		private readonly IDeployQueueManager _deployQueueManager;

		public DeployQueueService(IDeployQueueManager deployQueueManager)
		{
			_deployQueueManager = DIHelper.VerifyParameter(deployQueueManager);
		}

		public object Get(DeployQueueRequest request)
		{
			var listOptions = new ListOptions();
			if(request != null)
			{
				listOptions = request.BuildListOptions();
			}
			return _deployQueueManager.GetQueue(listOptions);
		}
	}
}