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
	public class RavenSystemLogRepository : ISystemLogRepository
	{
		private readonly IDocumentSession _documentSession;

		public RavenSystemLogRepository(IDocumentSession documentSession)
		{
			_documentSession = DIHelper.VerifyParameter(documentSession);
		}

		public SystemLog LogMessage(EnumSystemLogType logType, string userName, DateTime messageDateTime, string message)
		{
			var systemLog = new SystemLog
			{
				Id = Guid.NewGuid().ToString(),
				EnumSystemLogTypeID = logType,
				UserName = userName,
				MessageDateTimeUtc = messageDateTime.ToUniversalTime(),
				MessageText = message
			};
			this._documentSession.Store(systemLog);
			this._documentSession.SaveChanges();
			return systemLog;
		}
	}
}
