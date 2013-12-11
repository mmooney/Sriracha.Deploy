using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack.ServiceInterface;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Deployment;
using Sriracha.Deploy.Data.Dto.Deployment;
using Sriracha.Deploy.Data.Deployment.Offline;
using Sriracha.Deploy.Data.Dto.Deployment.Offline;

namespace Sriracha.Deploy.Web.Services.Deployment
{
	public class OfflineDeploymentService : Service
	{
		private readonly IOfflineDeploymentManager _offlineDeploymentManager;
        private readonly IDeployRequestManager _deployRequestManager;

		public OfflineDeploymentService(IOfflineDeploymentManager offlineDeploymentManager, IDeployRequestManager deployRequestManager)
		{
			_offlineDeploymentManager = DIHelper.VerifyParameter(offlineDeploymentManager);
            _deployRequestManager = DIHelper.VerifyParameter(deployRequestManager);
		}

		public object Get(OfflineDeploymentRequest request)
		{
            if(request == null)
            {
                throw new ArgumentNullException("request is null");
            }
            if(!string.IsNullOrEmpty(request.Id))
            {
                return _offlineDeploymentManager.GetOfflineDeployment(request.Id);
            }
            else if(!string.IsNullOrEmpty(request.DeployBatchRequestId))
            {
                return _offlineDeploymentManager.GetOfflineDeploymentForDeploymentBatchRequestId(request.DeployBatchRequestId);
            }
            else 
            {
                throw new ArgumentNullException("request.id and request.deployBatchRequestId are null");
            }
            
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
            if(data == null)
            {
                throw new ArgumentNullException("request is null");
            }
            if(!string.IsNullOrEmpty(data.Id))
            {
                if(string.IsNullOrEmpty(data.ResultFileId))
                {
                    throw new ArgumentNullException("request.resultFileId is null");
                }
                return _offlineDeploymentManager.ImportHistory(data.Id, data.ResultFileId);
            }
            else if (!string.IsNullOrEmpty(data.DeployBatchRequestId))
            {
                return _offlineDeploymentManager.BeginCreateOfflineDeployment(data.DeployBatchRequestId);
            }
            else 
            {
                throw new ArgumentNullException("request.id and request.deployBatchRequestId are null");
            }
		}
	}
}