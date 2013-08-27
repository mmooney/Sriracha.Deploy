using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack.ServiceInterface;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Dto;

namespace Sriracha.Deploy.Web.Services.Deployment
{
	public class BatchDeployStatusService : Service
	{
		private readonly IDeployRequestManager _deployRequestManager;

		public BatchDeployStatusService(IDeployRequestManager deployRequestManager)
		{
			_deployRequestManager = DIHelper.VerifyParameter(deployRequestManager);
		}

		public object Get(DeployBatchStatus deployBatchStatus)
		{
			if(deployBatchStatus == null || string.IsNullOrEmpty(deployBatchStatus.DeployBatchRequestId))
			{
				throw new ArgumentNullException();
			}
			return _deployRequestManager.GetBatchDeployStatus(deployBatchStatus.DeployBatchRequestId);
		}
	}
}