using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack.ServiceInterface;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Dto;

namespace Sriracha.Deploy.Web.Services.Deployment
{
	public class DeployBatchStatusService : Service
	{
		private readonly IDeployRequestManager _deployRequestManager;

		public DeployBatchStatusService(IDeployRequestManager deployRequestManager)
		{
			_deployRequestManager = DIHelper.VerifyParameter(deployRequestManager);
		}

		public object Get(DeployBatchStatusRequest request)
		{
			if(request == null)
			{
				throw new ArgumentNullException();
			}
			if(string.IsNullOrEmpty(request.Id))
			{
				return _deployRequestManager.GetDeployBatchStatusList(request.BuildListOptions());
			}
			else 
			{
				return _deployRequestManager.GetDeployBatchStatus(request.Id);
			}
		}
	}
}