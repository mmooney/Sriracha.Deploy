using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;
using Raven.Client;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Exceptions;
using Sriracha.Deploy.Data.Repository;
using MMDB.Shared;
using PagedList;
using Raven.Client.Linq;
using Sriracha.Deploy.Data.Dto.Build;
using Raven.Client.Indexes;

namespace Sriracha.Deploy.RavenDB
{
	public class RavenBuildRepository : IBuildRepository
	{
		private readonly IDocumentSession _documentSession;
		private readonly IUserIdentity _userIdentity;
		private readonly Logger _logger;

        public class DeployBuildListIndex : AbstractIndexCreationTask<DeployBuild>
        {
            public DeployBuildListIndex()
            {
                Map = builds=> from i in builds
                                  select new { i.ProjectId, i.ProjectName, i.ProjectComponentId, i.ProjectComponentName, i.ProjectBranchName, i.ProjectBranchId, i.UpdatedDateTimeUtc };
                Index(x => x.ProjectId, Raven.Abstractions.Indexing.FieldIndexing.Analyzed);
                Index(x => x.ProjectName, Raven.Abstractions.Indexing.FieldIndexing.Analyzed);
                Index(x => x.ProjectComponentId, Raven.Abstractions.Indexing.FieldIndexing.Analyzed);
                Index(x => x.ProjectComponentName, Raven.Abstractions.Indexing.FieldIndexing.Analyzed);
                Index(x => x.ProjectBranchId, Raven.Abstractions.Indexing.FieldIndexing.Analyzed);
                Index(x => x.ProjectBranchName, Raven.Abstractions.Indexing.FieldIndexing.Analyzed);
            }
        }

		public RavenBuildRepository(IDocumentSession documentSession, IUserIdentity userIdentity, Logger logger)
		{
			_documentSession = DIHelper.VerifyParameter(documentSession);
			_userIdentity = DIHelper.VerifyParameter(userIdentity);
			_logger = DIHelper.VerifyParameter(logger);
		}

		public PagedSortedList<DeployBuild> GetBuildList(ListOptions listOptions, string projectId = null, string branchId = null, string componentId = null)
		{
            var query = this._documentSession.Query<DeployBuild, DeployBuildListIndex>().Customize(i => i.NoCaching()).Customize(i => i.NoTracking());
			if(!string.IsNullOrEmpty(projectId))
			{
				query = (IRavenQueryable<DeployBuild>)query.Where(i=>i.ProjectId == projectId);
			}
			if(!string.IsNullOrEmpty(branchId)) 
			{
				query = (IRavenQueryable<DeployBuild>)query.Where(i => i.ProjectBranchId == branchId);
			}
			if(!string.IsNullOrEmpty(componentId))
			{
				query = (IRavenQueryable<DeployBuild>)query.Where(i => i.ProjectComponentId == componentId);
			}
			IPagedList<DeployBuild> pagedList;
			listOptions = ListOptions.SetDefaults(listOptions, 20, 1, "UpdatedDateTimeUtc", false);
			switch(listOptions.SortField.ToLower())
			{
				case "updateddatetimeutc":
					pagedList = query.PageAndSort(listOptions, i=>i.UpdatedDateTimeUtc);
					break;
				case "branchname":
				case "projectbranchname":
					pagedList = query.PageAndSort(listOptions, i => i.ProjectBranchName);
					break;
				case "projectname":
					pagedList = query.PageAndSort(listOptions, i=>i.ProjectName);
					break;
				default:
					throw new Exception("Unrecognized sort field " + listOptions.SortField);
			}
			foreach(var item in pagedList)
			{
				_documentSession.Advanced.Evict(item);
			}
			return new PagedSortedList<DeployBuild>(pagedList, listOptions.SortField, listOptions.SortAscending.Value);
		}

		public DeployBuild CreateBuild(string projectId, string projectName, string projectComponentId, string projectComponentName, string projectBranchId, string projectBranchName, string fileId, string version)
		{
            if(string.IsNullOrEmpty(projectId))
            {
                throw new ArgumentNullException("Missing project ID");
            }
            if(string.IsNullOrEmpty(projectName))
            {
                throw new ArgumentNullException("Missing project name");
            }
            if(string.IsNullOrEmpty(projectComponentId))
            {
                throw new ArgumentNullException("Missing component ID");
            }
            if(string.IsNullOrEmpty(projectComponentName))
            {
                throw new ArgumentNullException("Missing component name");
            }
            if(string.IsNullOrEmpty(projectBranchId))
            {
                throw new ArgumentNullException("Missing branch ID");
            }
            if(string.IsNullOrEmpty(projectBranchName))
            {
                throw new ArgumentNullException("Missing branch name");
            }
            if(string.IsNullOrEmpty(fileId))
            {
                throw new ArgumentNullException("Missing file ID");
            }
            if(string.IsNullOrEmpty(version))
            {
                throw new ArgumentNullException("Missing version");
            }
            var existingItem = (from i in this._documentSession.QueryNoCacheNotStale<DeployBuild>()
                                where i.ProjectId == projectId
                                    && i.ProjectBranchId == projectBranchId
                                    && i.ProjectComponentId == projectComponentId
                                    //&& i.FileId == fileId
                                    && i.Version == version
                                select i).FirstOrDefault();
            if (existingItem != null)
            {
                throw new DuplicateObjectException<DeployBuild>(existingItem);
            }
            var item = new DeployBuild
			{
				Id = Guid.NewGuid().ToString(),
				ProjectId = projectId,
				ProjectName = projectName,
				ProjectComponentId = projectComponentId,
				ProjectComponentName = projectComponentName,
				ProjectBranchId = projectBranchId,
				ProjectBranchName = projectBranchName,
				FileId = fileId,
				Version = version,
				CreatedDateTimeUtc = DateTime.UtcNow,
				CreatedByUserName = _userIdentity.UserName,
				UpdatedDateTimeUtc = DateTime.UtcNow,
				UpdatedByUserName = _userIdentity.UserName
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
			return this._documentSession.LoadEnsure<DeployBuild>(buildId);
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
			build.UpdatedDateTimeUtc = DateTime.UtcNow;
			build.UpdatedByUserName = _userIdentity.UserName;
			this._documentSession.SaveChanges();
			return build;
		}

		public void DeleteBuild(string buildId)
		{
			_logger.Info("User {0} deleting build {1}", _userIdentity.UserName, buildId);
			var build = GetBuild(buildId);
			this._documentSession.Delete(build);
			this._documentSession.SaveChanges();

		}

		public Raven.Client.Linq.IRavenQueryable<DeployBuild> IRavenQueryable { get; set; }
	}
}
