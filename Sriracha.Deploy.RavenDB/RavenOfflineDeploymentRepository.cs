using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Dto.Deployment;
using Sriracha.Deploy.Data.Repository;
using System.Transactions;

namespace Sriracha.Deploy.RavenDB
{
	public class RavenOfflineDeploymentRepository : IOfflineDeploymentRepository
	{
		private readonly IDocumentSession _documentSession;
		private readonly IUserIdentity _userIdentity;

		public RavenOfflineDeploymentRepository(IDocumentSession documentSession, IUserIdentity userIdentity)
		{
			_documentSession = DIHelper.VerifyParameter(documentSession);
			_userIdentity = DIHelper.VerifyParameter(userIdentity);
		}

		public OfflineDeployment CreateOfflineDeployment(string deployBatchRequestId, EnumOfflineDeploymentStatus initialStatus)
		{
			var item = new OfflineDeployment
			{
				Id = Guid.NewGuid().ToString(),
				DeployBatchRequestId = deployBatchRequestId,
				Status = initialStatus,
				CreatedByUserName = _userIdentity.UserName,
				CreatedDateTimeUtc = DateTime.UtcNow,
				UpdatedByUserName = _userIdentity.UserName,
				UpdatedDateTimeUtc = DateTime.UtcNow
			};
			return _documentSession.StoreSaveEvict(item);
		}

        public OfflineDeployment GetOfflineDeployment(string offlineDeploymentId)
        {
            return _documentSession.LoadEnsureNoCache<OfflineDeployment>(offlineDeploymentId);
        }


        public OfflineDeployment UpdateStatus(string offlineDeploymentId, EnumOfflineDeploymentStatus status, Exception err = null)
        {
            var item = _documentSession.LoadEnsure<OfflineDeployment>(offlineDeploymentId);
            item.Status = status;
            item.UpdatedByUserName = _userIdentity.UserName;
            item.UpdatedDateTimeUtc = DateTime.UtcNow;
            if(err != null)
            {
                item.CreateErrorDetails = err.ToString();
            }
            return _documentSession.SaveEvict(item);
        }

        public OfflineDeployment PopNextOfflineDeploymentToCreate()
        {
            return _documentSession.Pop<OfflineDeployment, DateTime>
                        (i=>i.Status == EnumOfflineDeploymentStatus.CreateRequested, 
                        i=>i.CreatedDateTimeUtc, 
                        i=>{i.Status = EnumOfflineDeploymentStatus.CreateInProcess;});
        }


        public OfflineDeployment SetReadyForDownload(string offlineDeploymentId, string fileId)
        {
            var item = _documentSession.LoadEnsure<OfflineDeployment>(offlineDeploymentId);
            item.FileId = fileId;
            item.Status = EnumOfflineDeploymentStatus.ReadyForDownload;
            item.UpdatedByUserName = _userIdentity.UserName;
            item.UpdatedDateTimeUtc = DateTime.UtcNow;
            return _documentSession.SaveEvict(item);
        }
    }
}
