using MMDB.Shared;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Dto.Project;
using Sriracha.Deploy.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.SqlServer
{
    public class SqlServerProjectRepository : IProjectRepository
    {
        private readonly ISqlConnectionInfo _sqlConnectionInfo;
        private readonly IUserIdentity _userIdentity;

        public SqlServerProjectRepository(ISqlConnectionInfo sqlConnectionInfo, IUserIdentity userIdentity)
        {
            _sqlConnectionInfo = DIHelper.VerifyParameter(sqlConnectionInfo);
            _userIdentity = DIHelper.VerifyParameter(userIdentity);
        }

        private void VerifyProjectExists(string projectId)
        {
            using (var db = _sqlConnectionInfo.GetDB())
            {
                var projectExistsSql = PetaPoco.Sql.Builder.Append("SELECT COUNT(*) FROM Project WHERE ID=@0", projectId);
                if (db.ExecuteScalar<int>(projectExistsSql) == 0)
                {
                    throw new RecordNotFoundException(typeof(DeployProject), "Id", projectId);
                }
            }
        }

        public IEnumerable<DeployProject> GetProjectList()
        {
            using(var db = _sqlConnectionInfo.GetDB())
            {
                var sql = PetaPoco.Sql.Builder
                            .Append("SELECT ID, ProjectName, UsesSharedComponentConfiguration, CreatedByUserName, CreatedDateTimeUtc, UpdatedByUserName, UpdatedDateTimeUtc")
                            .Append("FROM Project");
                return db.Query<DeployProject>(sql);
            }
        }

        public DeployProject CreateProject(string projectName, bool usesSharedComponentConfiguration)
        {
            if(string.IsNullOrEmpty(projectName))
            {
                throw new ArgumentNullException("Missing project name");
            }
            var project = new DeployProject
            {
                Id = Guid.NewGuid().ToString(),
                ProjectName = projectName,
                UsesSharedComponentConfiguration = usesSharedComponentConfiguration,
                CreatedByUserName = _userIdentity.UserName,
                CreatedDateTimeUtc = DateTime.UtcNow,
                UpdatedByUserName = _userIdentity.UserName,
                UpdatedDateTimeUtc = DateTime.UtcNow
            };

            using(var db = _sqlConnectionInfo.GetDB())
            {
                var sql = PetaPoco.Sql.Builder
                    .Append("INSERT INTO Project (ID, ProjectName, UsesSharedComponentConfiguration, CreatedByUserName, CreatedDateTimeUtc, UpdatedByUserName, UpdatedDateTimeUtc)")
                    .Append("VALUES (@Id, @ProjectName, @UsesSharedComponentConfiguration, @CreatedByUserName, @CreatedDateTimeUtc, @UpdatedByUserName, @UpdatedDateTimeUtc)", project);
                db.Execute(sql);
            }
            return project;
        }

        public DeployProject TryGetProject(string projectId)
        {
            DeployProject project;
            if(string.IsNullOrEmpty(projectId))
            {
                throw new ArgumentNullException("Missing project ID");
            }
            using(var db = _sqlConnectionInfo.GetDB())
            {
                var sql = PetaPoco.Sql.Builder
                            .Append("SELECT ID, ProjectName, UsesSharedComponentConfiguration, CreatedByUserName, CreatedDateTimeUtc, UpdatedByUserName, UpdatedDateTimeUtc")
                            .Append("FROM Project")
                            .Append("WHERE ID = @0", projectId);
                project = db.SingleOrDefault<DeployProject>(sql);
            }
            if(project != null)
            {
                project.BranchList = GetBranchList(projectId).ToList();
            }
            return project;
        }

        public DeployProject GetProject(string projectId)
        {
            var project = TryGetProject(projectId);
            if(project == null)
            {
                throw new RecordNotFoundException(typeof(DeployProject), "Id", projectId);
            }
            return project;
        }

        public DeployProject GetOrCreateProject(string projectId, string projectName)
        {
            throw new NotImplementedException();
        }

        public DeployProject TryGetProjectByName(string projectName)
        {
            throw new NotImplementedException();
        }

        public DeployProject GetProjectByName(string projectName)
        {
            throw new NotImplementedException();
        }

        public DeployProject UpdateProject(string projectId, string projectName, bool usesSharedComponentConfiguration)
        {
            if(string.IsNullOrEmpty(projectId))
            {
                throw new ArgumentNullException("Missing project ID");
            }
            if(string.IsNullOrEmpty(projectName))
            {
                throw new ArgumentNullException("Missing project name");
            }
            VerifyProjectExists(projectId);
            using(var db = _sqlConnectionInfo.GetDB())
            {
                var sql = PetaPoco.Sql.Builder.Append("UPDATE Project")
                                        .Append("SET ProjectName=@0, UsesSharedComponentConfiguration=@1, UpdatedByUserName=@2, UpdatedDateTimeUtc=@3", projectName, usesSharedComponentConfiguration, _userIdentity.UserName, DateTime.UtcNow)
                                        .Append("WHERE ID=@0", projectId);
                db.Execute(sql);
            }
            return GetProject(projectId);
        }

        public void DeleteProject(string projectId)
        {
            if(string.IsNullOrEmpty(projectId))
            {
                throw new ArgumentNullException("Missing project ID");
            }
            VerifyProjectExists(projectId);

            using(var db = _sqlConnectionInfo.GetDB())
            {
                var sql = PetaPoco.Sql.Builder
                                .Append("DELETE FROM Branch WHERE ProjectID=@0;", projectId)
                                .Append("DELETE FROM Project WHERE ID=@0", projectId);
                db.Execute(sql);
            }
        }


        public List<DeployConfiguration> GetConfigurationList(string projectId)
        {
            throw new NotImplementedException();
        }

        public DeployConfiguration CreateConfiguration(string projectId, string configurationName, Data.EnumDeploymentIsolationType isolationType)
        {
            throw new NotImplementedException();
        }

        public DeployConfiguration GetConfiguration(string configurationId, string projectId = null)
        {
            throw new NotImplementedException();
        }

        public DeployConfiguration TryGetConfiguration(string configurationId, string projectId = null)
        {
            throw new NotImplementedException();
        }

        public DeployConfiguration UpdateConfiguration(string configurationId, string projectId, string configurationName, Data.EnumDeploymentIsolationType isolationType)
        {
            throw new NotImplementedException();
        }

        public void DeleteConfiguration(string configurationId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DeployComponent> GetComponentList(string projectId)
        {
            throw new NotImplementedException();
        }

        public DeployComponent CreateComponent(string projectId, string componentName, bool useConfigurationGroup, string configurationId, Data.EnumDeploymentIsolationType isolationType)
        {
            throw new NotImplementedException();
        }

        public DeployComponent GetComponent(string componentId, string projectId = null)
        {
            throw new NotImplementedException();
        }

        public DeployComponent TryGetComponent(string componentId, string projectId = null)
        {
            throw new NotImplementedException();
        }

        public DeployComponent GetComponent(DeployProject project, string componentId)
        {
            throw new NotImplementedException();
        }

        public DeployComponent TryGetComponent(DeployProject project, string componentId)
        {
            throw new NotImplementedException();
        }

        public DeployComponent GetComponentByName(DeployProject project, string componentName)
        {
            throw new NotImplementedException();
        }

        public DeployComponent TryGetComponentByName(DeployProject project, string componentName)
        {
            throw new NotImplementedException();
        }

        public DeployComponent GetOrCreateComponent(string projectId, string componentId, string componentName)
        {
            throw new NotImplementedException();
        }

        public DeployComponent UpdateComponent(string projectId, string componentId, string componentName, bool useConfigurationGroup, string configurationId, Data.EnumDeploymentIsolationType isolationType)
        {
            throw new NotImplementedException();
        }

        public void DeleteComponent(string projectId, string componentId)
        {
            throw new NotImplementedException();
        }

        public List<DeployStep> GetComponentDeploymentStepList(string componentId)
        {
            throw new NotImplementedException();
        }

        public List<DeployStep> GetConfigurationDeploymentStepList(string configurationId)
        {
            throw new NotImplementedException();
        }

        public DeployStep CreateComponentDeploymentStep(string projectId, string componentId, string stepName, string taskTypeName, string taskOptionsJson, string sharedDeploymentStepId)
        {
            throw new NotImplementedException();
        }

        public DeployStep CreateConfigurationDeploymentStep(string projectId, string configurationId, string stepName, string taskTypeName, string taskOptionsJson)
        {
            throw new NotImplementedException();
        }

        public DeployStep GetComponentDeploymentStep(string deploymentStepId)
        {
            throw new NotImplementedException();
        }

        public DeployStep GetConfigurationDeploymentStep(string deploymentStepId)
        {
            throw new NotImplementedException();
        }

        public DeployStep UpdateComponentDeploymentStep(string deploymentStepId, string projectId, string componentId, string stepName, string taskTypeName, string taskOptionsJson, string sharedDeploymentStepId)
        {
            throw new NotImplementedException();
        }

        public DeployStep UpdateConfigurationDeploymentStep(string deploymentStepId, string projectId, string configurationId, string stepName, string taskTypeName, string taskOptionsJson)
        {
            throw new NotImplementedException();
        }

        public void DeleteComponentDeploymentStep(string deploymentStepId)
        {
            throw new NotImplementedException();
        }

        public void DeleteConfigurationDeploymentStep(string deploymentStepId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DeployProjectBranch> GetBranchList(string projectId)
        {
            using(var db = _sqlConnectionInfo.GetDB())
            {
                var sql = PetaPoco.Sql.Builder
                    .Append("SELECT ID, BranchName, ProjectID, CreatedByUserName, CreatedDateTimeUtc, UpdatedByUserName, UpdatedDateTimeUtc")
                    .Append("FROM Branch")
                    .Append("WHERE ProjectID=@0", projectId);
                return db.Fetch<DeployProjectBranch>(sql);
            }
        }

        public DeployProjectBranch CreateBranch(string projectId, string branchName)
        {
            if(string.IsNullOrEmpty(projectId))
            {
                throw new ArgumentNullException("Missing project ID");
            }
            if(string.IsNullOrEmpty(branchName))
            {
                throw new ArgumentNullException("Missing branch name");
            }
            var branch = new DeployProjectBranch
            {
                Id = Guid.NewGuid().ToString(),
                ProjectId = projectId,
                BranchName = branchName,
                CreatedByUserName = _userIdentity.UserName,
                CreatedDateTimeUtc = DateTime.UtcNow,
                UpdatedByUserName = _userIdentity.UserName,
                UpdatedDateTimeUtc = DateTime.UtcNow
            };
            using(var db = _sqlConnectionInfo.GetDB())
            {  
                VerifyProjectExists(projectId);
                var sql = PetaPoco.Sql.Builder
                            .Append("INSERT INTO Branch (ID, ProjectID, BranchName, CreatedByUserName, CreatedDateTimeUtc, UpdatedByUserName, UpdatedDateTimeUtc)")
                            .Append("VALUES (@Id, @ProjectId, @BranchName, @CreatedByUserName, @CreatedDateTimeUtc, @UpdatedByUserName, @UpdatedDateTimeUtc)", branch);
                db.Execute(sql);
            }
            return branch;
        }

        public DeployProjectBranch GetBranch(string branchId, string projectId = null)
        {
            throw new NotImplementedException();
        }

        public DeployProjectBranch TryGetBranch(string branchId, string projectId = null)
        {
            throw new NotImplementedException();
        }

        public DeployProjectBranch GetBranch(DeployProject project, string branchId)
        {
            throw new NotImplementedException();
        }

        public DeployProjectBranch TryGetBranch(DeployProject project, string branchId)
        {
            throw new NotImplementedException();
        }

        public DeployProjectBranch GetBranchByName(string projectId, string branchName)
        {
            throw new NotImplementedException();
        }

        public DeployProjectBranch TryGetBranchByName(string projectId, string branchName)
        {
            throw new NotImplementedException();
        }

        public DeployProjectBranch GetBranchByName(DeployProject project, string branchName)
        {
            throw new NotImplementedException();
        }

        public DeployProjectBranch TryGetBranchByName(DeployProject project, string branchName)
        {
            throw new NotImplementedException();
        }

        public DeployProjectBranch GetOrCreateBranch(string projectId, string branchId, string branchName)
        {
            throw new NotImplementedException();
        }

        public DeployProjectBranch UpdateBranch(string branchId, string projectId, string branchName)
        {
            throw new NotImplementedException();
        }

        public void DeleteBranch(string branchId, string projectId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DeployEnvironment> GetEnvironmentList(string projectId)
        {
            throw new NotImplementedException();
        }

        public DeployEnvironment CreateEnvironment(string projectId, string enviornmentName, IEnumerable<DeployEnvironmentConfiguration> componentList, IEnumerable<DeployEnvironmentConfiguration> configurationList)
        {
            throw new NotImplementedException();
        }

        public DeployEnvironment GetEnvironment(string environmentId)
        {
            throw new NotImplementedException();
        }

        public DeployEnvironment UpdateEnvironment(string environmentId, string projectId, string environmentName, IEnumerable<DeployEnvironmentConfiguration> componentList, IEnumerable<DeployEnvironmentConfiguration> configurationList)
        {
            throw new NotImplementedException();
        }

        public void DeleteEnvironment(string environmentId)
        {
            throw new NotImplementedException();
        }

        public DeployMachine GetMachine(string machineId)
        {
            throw new NotImplementedException();
        }

        public DeployMachine UpdateMachine(string machineId, string projectId, string environmentId, string enviromentComponentId, string machineName, Dictionary<string, string> configurationList)
        {
            throw new NotImplementedException();
        }
    }
}
