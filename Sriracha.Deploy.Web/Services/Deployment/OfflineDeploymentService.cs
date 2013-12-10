using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack.ServiceInterface;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Deployment;
using Sriracha.Deploy.Data.Dto.Deployment;

namespace Sriracha.Deploy.Web.Services.Deployment
{
	public class OfflineDeploymentService : Service
	{
		private readonly IOfflineDeploymentManager _offlineDeploymentManager;

		public OfflineDeploymentService(IOfflineDeploymentManager offlineDeploymentManager)
		{
			_offlineDeploymentManager = DIHelper.VerifyParameter(offlineDeploymentManager);
		}

		public object Get(OfflineDeploymentRequest request)
		{
            if(request == null)
            {
                throw new ArgumentNullException("request is null");
            }
            if(string.IsNullOrEmpty(request.Id))
            {
                throw new ArgumentNullException("request.id is null");
            }
            return _offlineDeploymentManager.GetOfflineDeployment(request.Id);
		}

		public object Put(OfflineDeploymentRequest data)
		{
			return Save(data);
		}

		public object Post(OfflineDeploymentRequest data)
		{
			return Save(data);
		}

		private OfflineDeployment Save(OfflineDeploymentRequest data)
		{
			return _offlineDeploymentManager.BeginCreateOfflineDeployment(data.BatchRequest.ItemList, data.BatchRequest.DeploymentLabel);
		}
	}
}