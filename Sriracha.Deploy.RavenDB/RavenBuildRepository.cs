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

		public IEnumerable<DeployBuild> GetBuildList()
		{
			return this._documentSession.Query<DeployBuild>();
		}

		public DeployBuild CreateBuild(string projectId, string projectName, string projectComponentId, string projectComponentName, string projectBranchId, string projectBranchName, string fileId, string version)
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

		public DeployBuild GetBuild(string buildId)
		{
			if(string.IsNullOrEmpty(buildId))
			{
				throw new ArgumentNullException("Missing build ID");
			}
			return this._documentSession.Load<DeployBuild>(buildId);
		}

		public DeployBuild UpdateBuild(string buildId, string projectId, string projectName, string projectComponentId, string projectComponentName, string projectBranchId, string projectBranchName, string fileId, string version)
		{
			if (string.IsNullOrEmpty(buildId))
			{
				throw new ArgumentNullException("Missing build ID");
			}
			if (string.IsNullOrEmpty(projectId))
			{
				throw new ArgumentNullException("Missing project ID");
			}
			if(string.IsNullOrEmpty(projectName))
			{
				throw new ArgumentNullException("Missing project name");
			}
			if (string.IsNullOrEmpty(projectComponentId))
			{
				throw new ArgumentNullException("Missing component ID");
			}
			if (string.IsNullOrEmpty(projectComponentName)) 
			{
				throw new ArgumentNullException("Missing component name");
			}
			if (string.IsNullOrEmpty(projectBranchId))
			{
				throw new ArgumentNullException("Missing branch ID");
			}
			if (string.IsNullOrEmpty(projectBranchName))
			{
				throw new ArgumentNullException("Missing branch name");
			}
			if (string.IsNullOrEmpty(fileId))
			{
				throw new ArgumentNullException("Missing file ID");
			}
			if (string.IsNullOrEmpty(version))
			{
				throw new ArgumentNullException("Missing version");
			}
			var build = this.GetBuild(buildId);
			build.ProjectId = projectId;
			build.ProjectComponentId = projectComponentId;
			build.ProjectComponentName = projectComponentName;
			build.ProjectBranchId = projectBranchId;
			build.ProjectBranchName = projectBranchName;
			build.FileId = fileId;
			build.Version = version;
			this._documentSession.SaveChanges();
			return build;
		}

		public void DeleteBuild(string buildId)
		{
			var build = GetBuild(buildId);
			this._documentSession.Delete(build);
			this._documentSession.SaveChanges();

		}
	}
}
