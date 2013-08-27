using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack.ServiceInterface;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Dto;

namespace Sriracha.Deploy.Web.Services.Deployment
{
	public class BatchRequestService : Service
	{
		private readonly IDeployRequestManager _deployRequestManager;

		public BatchRequestService(IDeployRequestManager deployRequestManager)
		{
			_deployRequestManager = DIHelper.VerifyParameter(deployRequestManager);
		}

		public object Get(DeployBatchRequest request)
		{
			if(request == null || string.IsNullOrWhiteSpace(request.Id))
			{
				return _deployRequestManager.GetBatchRequestList();
			}
			else 
			{
				return _deployRequestManager.GetBatchRequest(request.Id);
			}
		}

		public object Post(DeployBatchRequest request)
		{
			var returnValue = Save(request);
			return returnValue;
		}

		public object Put(DeployBatchRequest request)
		{
			return this.Save(request);
		}


		private DeployBatchRequest Save(DeployBatchRequest request)
		{
			var item = _deployRequestManager.CreateBatchRequest(request.ItemList);
			return item;
		}
	}
}