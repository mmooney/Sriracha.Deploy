using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Exceptions;
using Sriracha.Deploy.Data.Repository;

namespace Sriracha.Deploy.RavenDB
{
	public class RavenDBBuildRepository : IBuildRepository
	{
		private IDocumentSession DocumentSession { get; set; }

		public RavenDBBuildRepository(IDocumentSession documentSession)
		{
			this.DocumentSession = documentSession;
		}

		public DeployBuild StoreBuild(string projectId, string projectBranchId, string fileId, Version version)
		{
			var existingItem = (from i in this.DocumentSession.Query<DeployBuild>()
											.Customize(x=>x.WaitForNonStaleResultsAsOfLastWrite())
								where i.ProjectId == projectId
									&& i.ProjectBranchId == projectBranchId
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
				ProjectBranchId = projectBranchId,
				FileId = fileId,
				Version = version
			};
			this.DocumentSession.Store(item);
			this.DocumentSession.SaveChanges();
			return item;
		}
	}
}
