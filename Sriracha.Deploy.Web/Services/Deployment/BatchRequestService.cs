using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack.ServiceInterface;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Deployment;
using Sriracha.Deploy.Data.Dto.Deployment;

namespace Sriracha.Deploy.Web.Services.Deployment
{
	public class BatchRequestService : Service
	{
		private readonly IDeployRequestManager _deployRequestManager;

		public BatchRequestService(IDeployRequestManager deployRequestManager)
		{
			_deployRequestManager = DIHelper.VerifyParameter(deployRequestManager);
		}

		public object Get(BatchRequestRequest request)
		{
			if(request == null || string.IsNullOrWhiteSpace(request.Id))
			{
				return _deployRequestManager.GetDeployBatchRequestList(request.BuildListOptions());
			}
			else 
			{
				return _deployRequestManager.GetDeployBatchRequest(request.Id);
			}
		}

		public object Post(BatchRequestRequest request)
		{
			var returnValue = Save(request);
			return returnValue;
		}

		public object Put(BatchRequestRequest request)
		{
			return this.Save(request);
		}


		private DeployBatchRequest Save(BatchRequestRequest request)
		{
			var item = _deployRequestManager.CreateDeployBatchRequest(request.ItemList, request.Status, request.DeploymentLabel);
			return item;
		}
	}
}