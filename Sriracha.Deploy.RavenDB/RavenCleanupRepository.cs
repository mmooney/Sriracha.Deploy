using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Repository;

namespace Sriracha.Deploy.RavenDB
{
	public class RavenCleanupRepository : ICleanupRepository
	{
		private readonly IDocumentSession _documentSession;
		private readonly IUserIdentity _userIdentity;

		public RavenCleanupRepository(IDocumentSession documentSession, IUserIdentity userIdentity)
		{
			_documentSession = DIHelper.VerifyParameter(documentSession);
			_userIdentity = DIHelper.VerifyParameter(userIdentity);
		}

		public CleanupTaskData CreateCleanupTask(EnumCleanupTaskType taskType, string folderPath, int ageMinutes)
		{
			var task = new CleanupTaskData
			{
				Id = Guid.NewGuid().ToString(),
				TaskType = taskType,
				FolderPath = folderPath,
				AgeMinutes = ageMinutes,
				CreatedByUserName = _userIdentity.UserName,
				CreatedDateTimeUtc = DateTime.UtcNow,
				UpdatedByUserName = _userIdentity.UserName,
				UpdatedDateTimeUtc = DateTime.UtcNow
			};
			return _documentSession.StoreSaveEvict(task);
		}
	}
}
