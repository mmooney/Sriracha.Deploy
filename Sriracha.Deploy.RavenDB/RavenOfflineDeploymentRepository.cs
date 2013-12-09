using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Dto.Deployment;
using Sriracha.Deploy.Data.Repository;

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
	}
}
