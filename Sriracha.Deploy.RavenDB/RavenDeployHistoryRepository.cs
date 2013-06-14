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
	public class RavenDeployHistoryRepository : IDeployHistoryRepository
	{
		private readonly IDocumentSession _documentSession;

		public RavenDeployHistoryRepository(IDocumentSession documentSession)
		{
			_documentSession = DIHelper.VerifyParameter(documentSession);
		}

		public IEnumerable<DeployHistory> GetDeployHistoryList(string buildId, string environmentId)
		{
			var list = _documentSession.Query<DeployHistory>().AsQueryable();
			if(!string.IsNullOrEmpty(buildId))
			{
				list = list.Where(i=>i.BuildId == buildId);
			}
			if(!string.IsNullOrEmpty(environmentId))
			{
				list = list.Where(i=>i.EnvironmentId == environmentId);
			}
			return list;
		}

		public DeployHistory GetDeployHistory(string deployHistoryId)
		{
			throw new NotImplementedException();
		}
	}
}
