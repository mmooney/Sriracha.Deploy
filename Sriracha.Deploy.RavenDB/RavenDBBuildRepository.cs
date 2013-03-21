using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client;
using Sriracha.Deploy.Data.Dto;
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
