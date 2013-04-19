using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Exceptions;
using Sriracha.Deploy.Data.Repository;

namespace Sriracha.Deploy.RavenDB
{
	public class RavenBuildRepository : IBuildRepository
	{
		private readonly IDocumentSession _documentSession;

		public RavenBuildRepository(IDocumentSession documentSession)
		{
			this._documentSession = DIHelper.VerifyParameter(documentSession);
		}

		public DeployBuild StoreBuild(string projectId, string projectName, string projectComponentId, string projectComponentName, string projectBranchId, string projectBranchName, string fileId, Version version)
		{
			var existingItem = (from i in this._documentSession.Query<DeployBuild>()
											.Customize(x=>x.WaitForNonStaleResultsAsOfLastWrite())
								where i.ProjectId == projectId
									&& i.ProjectBranchId == projectBranchId
									&& i.ProjectComponentId == projectComponentId
									&& i.FileId == fileId
									&& i.Version == version
								select i).FirstOrDefault();
			if(existingItem != null)
			{
				throw new DuplicateObjectException<DeployBuild>(existingItem);
			}
			var item = new DeployBuild
			{
				ProjectId = projectId,
				ProjectName = projectName,
				ProjectComponentId = projectComponentId,
				ProjectComponentName = projectComponentName,
				ProjectBranchId = projectBranchId,
				ProjectBranchName = projectBranchName,
				FileId = fileId,
				Version = version
			};
			this._documentSession.Store(item);
			this._documentSession.SaveChanges();
			return item;
		}


		public IEnumerable<DeployBuild> GetBuildList()
		{
			return this._documentSession.Query<DeployBuild>();
		}
	}
}
