using MMDB.Shared;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Dto.Build;
using Sriracha.Deploy.Data.Exceptions;
using Sriracha.Deploy.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.SqlServer
{
    public class SqlServerBuildRepository : IBuildRepository
    {
        private readonly ISqlConnectionInfo _sqlConnectionInfo;
        private readonly IUserIdentity _userIdentity;

        public SqlServerBuildRepository(ISqlConnectionInfo sqlConnectionInfo, IUserIdentity userIdentity)
        {
            _sqlConnectionInfo = DIHelper.VerifyParameter(sqlConnectionInfo);
            _userIdentity = DIHelper.VerifyParameter(userIdentity);
        }

        private void VerifyBuildExists(string buildId)
        {
            if(string.IsNullOrEmpty(buildId))
            {
                throw new ArgumentNullException("Missing build ID");
            }
            using(var db = _sqlConnectionInfo.GetDB())
            {
                var sql = PetaPoco.Sql.Builder.Append("SELECT COUNT(*) FROM DeployBuild WHERE ID=@0", buildId);
                var count = db.ExecuteScalar<int>(sql);
                if(count == 0)
                {
                    throw new RecordNotFoundException(typeof(DeployBuild), "Id", buildId);
                }
            }
        }

        public PagedSortedList<DeployBuild> GetBuildList(ListOptions listOptions, string projectId = null, string branchId = null, string componentId = null)
        {
            listOptions = ListOptions.SetDefaults(listOptions, 20, 1, "UpdatedDateTimeUtc", false);
            using(var db = _sqlConnectionInfo.GetDB())
            {
                var sql = GetBaseQuery();
                sql = sql.Append("WHERE 1=1");
                if(!string.IsNullOrEmpty(projectId))
                {
                    sql = sql.Append("AND ProjectID=@0", projectId);
                }
                if(!string.IsNullOrEmpty(branchId))
                {
                    sql = sql.Append("AND ProjectBranchID=@0", branchId);
                }
                if(!string.IsNullOrEmpty(componentId))
                {
                    sql = sql.Append("AND ProjectComponentID=@0", componentId);
                }
                return db.PageAndSort<DeployBuild>(listOptions, sql);
            }
        }

        public DeployBuild CreateBuild(string projectId, string projectName, string projectComponentId, string projectComponentName, string projectBranchId, string projectBranchName, string fileId, string version)
        {
            if (string.IsNullOrEmpty(projectId))
            {
                throw new ArgumentNullException("Missing project ID");
            }
            if (string.IsNullOrEmpty(projectName))
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
            using(var db = _sqlConnectionInfo.GetDB())
            {
                var sql = GetBaseQuery()
                            .Append("WHERE ProjectID=@0", projectId)
                            .Append("AND ProjectBranchID=@0", projectBranchId)
                            .Append("AND ProjectComponentID=@0", projectComponentId)
                            .Append("AND Version=@0", version);
                var existingItem = db.SingleOrDefault<DeployBuild>(sql);
                if(existingItem != null)
                {
                    throw new DuplicateObjectException<DeployBuild>(existingItem);
                }
            }
            var build = new DeployBuild
            {
                Id = Guid.NewGuid().ToString(),
                ProjectId = projectId,
                ProjectName = projectName,
                ProjectBranchId = projectBranchId,
                ProjectBranchName = projectBranchName,
                ProjectComponentId = projectComponentId,
                ProjectComponentName = projectComponentName,
                FileId = fileId,
                Version = version,
                CreatedDateTimeUtc = DateTime.UtcNow,
                CreatedByUserName = _userIdentity.UserName,
                UpdatedDateTimeUtc = DateTime.UtcNow,
                UpdatedByUserName = _userIdentity.UserName
            };
            using(var db = _sqlConnectionInfo.GetDB())
            {                 
                var sql = PetaPoco.Sql.Builder
                            .Append("INSERT INTO DeployBuild(ID, ProjectID, ProjectName, ProjectBranchID, ProjectBranchName, ProjectComponentID, ProjectComponentName, FileID, Version, CreatedDateTimeUtc, CreatedByUserName, UpdatedDateTimeUtc, UpdatedByUserName)")
                            .Append("VALUES (@Id, @ProjectId, @ProjectName, @ProjectBranchId, @ProjectBranchName, @ProjectComponentId, @ProjectComponentName, @FileId, @Version, @CreatedDateTimeUtc, @CreatedByUserName, @UpdatedDateTimeUtc, @UpdatedByUserName)",  build);
                db.Execute(sql);
            }
            return build;
        }

        public DeployBuild GetBuild(string buildId)
        {
            VerifyBuildExists(buildId);
            using(var db = _sqlConnectionInfo.GetDB())
            {
                var sql = GetBaseQuery().Append("WHERE ID=@0", buildId);
                return db.Single<DeployBuild>(sql);
            }
        }

        private PetaPoco.Sql GetBaseQuery()
        {
            return PetaPoco.Sql.Builder
                    .Append("SELECT ID, ProjectID, ProjectName, ProjectBranchID, ProjectBranchName, ProjectComponentID, ProjectComponentName, FileID, Version, CreatedDateTimeUtc, CreatedByUserName, UpdatedDateTimeUtc, UpdatedByUserName")
                    .Append("FROM DeployBuild");
        }

        public DeployBuild UpdateBuild(string buildId, string projectId, string projectName, string projectComponentId, string projectComponentName, string projectBranchId, string projectBranchName, string fileId, string version)
        {
            throw new NotImplementedException();
        }

        public void DeleteBuild(string buildId)
        {
            throw new NotImplementedException();
        }
    }
}
