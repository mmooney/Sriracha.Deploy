using ServiceStack.ServiceInterface;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Deployment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sriracha.Deploy.Web.Services.Deployment
{
	public class DeployBatchActionService : Service
	{
		private readonly IDeployRequestManager _deployRequestManager;

		public DeployBatchActionService(IDeployRequestManager deployRequestManager)
		{
			_deployRequestManager = DIHelper.VerifyParameter(deployRequestManager);
		}

		public object Post(DeployBatchActionRequest request)
		{
			if(request == null)
			{
				throw new ArgumentNullException("request is null");
			}
			if(string.IsNullOrEmpty(request.Id))
			{
				throw new ArgumentNullException("request.id is null");
			}
			return _deployRequestManager.PerformAction(request.Id, request.Action, request.UserMessage);
		}
	}
}