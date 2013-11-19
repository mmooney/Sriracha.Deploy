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
            if(string.IsNullOrEmpty(projectId))
            {
                throw new ArgumentNullException("Missing project ID");
            }
            using (var db = _sqlConnectionInfo.GetDB())
            {
                var projectExistsSql = PetaPoco.Sql.Builder.Append("SELECT COUNT(*) FROM Project WHERE ID=@0", projectId);
                if (db.ExecuteScalar<int>(projectExistsSql) == 0)
                {
                    throw new RecordNotFoundException(typeof(DeployProject), "Id", projectId);
                }
            }
        }

        private void VerifyBranchExists(string branchId, string projectId)
        {
            if (string.IsNullOrEmpty(branchId))
            {
                throw new ArgumentNullException("Missing branch ID");
            }
            using (var db = _sqlConnectionInfo.GetDB())
            {
                var branchExistsSql = PetaPoco.Sql.Builder.Append("SELECT COUNT(*) FROM Branch WHERE ID=@0", branchId);
                if (!string.IsNullOrEmpty(projectId))
                {
                    branchExistsSql = branchExistsSql.Append("AND ProjectID=@0", projectId);
                }
                if (db.ExecuteScalar<int>(branchExistsSql) == 0)
                {
                    throw new RecordNotFoundException(typeof(DeployProject), "Id", branchId);
                }
            }
        }

        private void VerifyConfigurationExists(string configurationId, string projectId)
        {
            if(string.IsNullOrEmpty(configurationId))
            {
                throw new ArgumentNullException("Missing configuration ID");
            }
            using (var db = _sqlConnectionInfo.GetDB())
            {
                var configurationExistsSql = PetaPoco.Sql.Builder.Append("SELECT COUNT(*) FROM Configuration WHERE ID=@0", configurationId);
                if(!string.IsNullOrEmpty(projectId))
                {
                    configurationExistsSql = configurationExistsSql.Append("AND ProjectID=@0", projectId);
                }
                if (db.ExecuteScalar<int>(configurationExistsSql) == 0)
                {
                    throw new RecordNotFoundException(typeof(DeployConfiguration), "Id", configurationId);
                }
            }
        }


        private void VerifyComponentExists(string componentId, string projectId)
        {
            if (string.IsNullOrEmpty(componentId))
            {
                throw new ArgumentNullException("Missing component Id");
            }
            using (var db = _sqlConnectionInfo.GetDB())
            {
                var configurationExistsSql = PetaPoco.Sql.Builder.Append("SELECT COUNT(*) FROM Component WHERE ID=@0", componentId);
                if (!string.IsNullOrEmpty(projectId))
                {
                    configurationExistsSql = configurationExistsSql.Append("AND ProjectID=@0", projectId);
                }
                if (db.ExecuteScalar<int>(configurationExistsSql) == 0)
                {
                    throw new RecordNotFoundException(typeof(DeployComponent), "Id", componentId);
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
                var list = db.Query<DeployProject>(sql).ToList();
                foreach(var project in list)
                {
                    LoadProjectChildren(project);
                }
                return list;
            }
        }

        private void LoadProjectChildren(DeployProject project)
        {
            project.BranchList = GetBranchList(project.Id).ToList();
            project.ConfigurationList = GetConfigurationList(project.Id);
            project.ComponentList = GetComponentList(project.Id);
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
                LoadProjectChildren(project);
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

        public DeployProject GetOrCreateProject(string projectIdOrName)
        {
            if(string.IsNullOrEmpty(projectIdOrName))
            {
                throw new ArgumentNullException("Missing project ID or name");
            }
            var project = TryGetProject(projectIdOrName);
            if(project == null)
            {
                project = TryGetProjectByName(projectIdOrName);
            }
            if(project == null)
            {
                project = CreateProject(projectIdOrName, false);
            }
            return project;
        }

        public DeployProject TryGetProjectByName(string projectName)
        {
            DeployProject project;
            if (string.IsNullOrEmpty(projectName))
            {
                throw new ArgumentNullException("Missing project name");
            }
            using (var db = _sqlConnectionInfo.GetDB())
            {
                var sql = PetaPoco.Sql.Builder
                            .Append("SELECT ID, ProjectName, UsesSharedComponentConfiguration, CreatedByUserName, CreatedDateTimeUtc, UpdatedByUserName, UpdatedDateTimeUtc")
                            .Append("FROM Project")
                            .Append("WHERE ProjectName = @0", projectName);
                var list = db.Fetch<DeployProject>(sql);
                if(list.Count == 0)
                {
                    project = null;
                }
                else  if(list.Count > 1)
                {
                    throw new ArgumentException("Multiple projects found with name " + projectName);
                }
                else 
                {
                    project = list[0];
                }
            }
            if (project != null)
            {
                LoadProjectChildren(project);
            }
            return project;
        }

        public DeployProject GetProjectByName(string projectName)
        {
            if(string.IsNullOrEmpty(projectName))
            {
                throw new ArgumentNullException("Missing project name");
            }
            var item = TryGetProjectByName(projectName);
            if(item == null)
            {
                throw new RecordNotFoundException(typeof(DeployProject), "ProjectName", projectName);
            }
            return item;
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
                                .Append("DELETE FROM Component WHERE ProjectID=@0;", projectId)
                                .Append("DELETE FROM Configuration WHERE ProjectID=@0;", projectId)
                                .Append("DELETE FROM Project WHERE ID=@0", projectId);
                db.Execute(sql);
            }
        }


        public List<DeployConfiguration> GetConfigurationList(string projectId)
        {
            if(string.IsNullOrEmpty(projectId))
            {
                throw new ArgumentNullException("Missing Project ID");
            }
            VerifyProjectExists(projectId);
            using(var db = _sqlConnectionInfo.GetDB())
            {
                var sql = GetConfigurationBaseQuery()
                            .Append("WHERE ProjectID=@0", projectId);
                return db.Fetch<DeployConfiguration>(sql).ToList();
            }
        }

        public DeployConfiguration CreateConfiguration(string projectId, string configurationName, EnumDeploymentIsolationType isolationType)
        {
            if(string.IsNullOrEmpty(projectId))
            {
                throw new ArgumentNullException("Missing project name");
            }
            if(string.IsNullOrEmpty(configurationName))
            {
                throw new ArgumentNullException("Missing configuration name");
            }
            VerifyProjectExists(projectId);
            var configuration = new DeployConfiguration
            {
                Id = Guid.NewGuid().ToString(),
                ProjectId = projectId,
                ConfigurationName = configurationName,
                IsolationType = isolationType,
                CreatedByUserName = _userIdentity.UserName,
                CreatedDateTimeUtc = DateTime.UtcNow,
                UpdatedByUserName = _userIdentity.UserName,
                UpdatedDateTimeUtc = DateTime.UtcNow
            };

            using(var db = _sqlConnectionInfo.GetDB())
            {
                var sql = PetaPoco.Sql.Builder
                            .Append("INSERT INTO Configuration (ID, ProjectID, ConfigurationName, EnumDeploymentIsolationTypeID, CreatedByUserName, CreatedDateTimeUtc, UpdatedByUserName, UpdatedDateTimeUtc)")
                            .Append("VALUES (@Id, @ProjectId, @ConfigurationName, @IsolationType, @CreatedByUserName, @CreatedDateTimeUtc, @UpdatedByUserName, @UpdatedDateTimeUtc)", configuration);
                db.Execute(sql);
            }
            return configuration;
        }

        public DeployConfiguration GetConfiguration(string configurationId, string projectId = null)
        {
            var item = this.TryGetConfiguration(configurationId, projectId);
            if(item == null)
            {
                throw new RecordNotFoundException(typeof(DeployConfiguration), "Id", configurationId);
            }
            return item;
        }

        public DeployConfiguration TryGetConfiguration(string configurationId, string projectId = null)
        {
            if(string.IsNullOrEmpty(configurationId))
            {
                throw new ArgumentNullException("Missing configurationID");
            }
            if(!string.IsNullOrEmpty(projectId))
            {
                VerifyProjectExists(projectId);
            }
            using(var db = _sqlConnectionInfo.GetDB())
            {
                var sql = GetConfigurationBaseQuery()
                            .Append("WHERE ID=@0", configurationId);
                if(!string.IsNullOrEmpty(projectId))
                {
                    sql = sql.Append("AND ProjectID=@0", projectId);
                }
                return db.SingleOrDefault<DeployConfiguration>(sql);
            }
        }

        private PetaPoco.Sql GetConfigurationBaseQuery()
        {
            return PetaPoco.Sql.Builder
                            .Append("SELECT ID, ProjectID, ConfigurationName, EnumDeploymentIsolationTypeID AS IsolationType, CreatedByUserName, CreatedDateTimeUtc, UpdatedByUserName, UpdatedDateTimeUtc")
                            .Append("FROM Configuration");
        }

        public DeployConfiguration UpdateConfiguration(string configurationId, string projectId, string configurationName, EnumDeploymentIsolationType isolationType)
        {
            VerifyProjectExists(projectId);
            VerifyConfigurationExists(configurationId, projectId);
            using(var db = _sqlConnectionInfo.GetDB())
            {
                var sql = PetaPoco.Sql.Builder
                                .Append("UPDATE Configuration")
                                .Append("SET ConfigurationName=@0, EnumDeploymentIsolationTypeID=@1, UpdatedByUserName=@2, UpdatedDateTimeUtc=@3", configurationName, isolationType, _userIdentity.UserName, DateTime.UtcNow)
                                .Append("WHERE ID=@0", configurationId);
                db.Execute(sql);
            }
            return GetConfiguration(configurationId, projectId);
        }

        public void DeleteConfiguration(string configurationId)
        {
            if(string.IsNullOrEmpty(configurationId))
            {
                throw new ArgumentNullException("Missing Configuration ID");
            }
            VerifyConfigurationExists(configurationId, null);
            using(var db = _sqlConnectionInfo.GetDB())
            {
                var sql = PetaPoco.Sql.Builder
                                .Append("DELETE FROM Configuration WHERE ID=@0", configurationId);
                db.Execute(sql);
            }
        }

        public List<DeployComponent> GetComponentList(string projectId)
        {
            if(string.IsNullOrEmpty(projectId))
            {
                throw new ArgumentNullException("Missing project ID");
            }
            VerifyProjectExists(projectId);
            using(var db = _sqlConnectionInfo.GetDB())
            {
                var sql = GetComponentBaseQuery().Where("ProjectID=@0", projectId);
                return db.Fetch<DeployComponent>(sql).ToList();
            }
        }

        public DeployComponent CreateComponent(string projectId, string componentName, bool useConfigurationGroup, string configurationId, EnumDeploymentIsolationType isolationType)
        {
            if(string.IsNullOrEmpty(projectId))
            {
                throw new ArgumentNullException("Missing project ID");
            }
            if(string.IsNullOrEmpty(componentName))
            {
                throw new ArgumentNullException("Missing component name");
            }
            VerifyProjectExists(projectId);
            var component = new DeployComponent
            {
                Id = Guid.NewGuid().ToString(),
                ComponentName = componentName,
                ProjectId = projectId, 
                UseConfigurationGroup = useConfigurationGroup,
                ConfigurationId = configurationId,
                IsolationType = isolationType,
                CreatedByUserName = _userIdentity.UserName,
                CreatedDateTimeUtc = DateTime.UtcNow,
                UpdatedByUserName = _userIdentity.UserName,
                UpdatedDateTimeUtc = DateTime.UtcNow
            };
            using(var db = _sqlConnectionInfo.GetDB())
            {
                var sql = PetaPoco.Sql.Builder
                            .Append("INSERT INTO Component (ID, ProjectID, ComponentName, UseConfigurationGroup, ConfigurationID, EnumDeploymentIsolationTypeID, CreatedByUserName, CreatedDateTimeUtc, UpdatedByUserName, UpdatedDateTimeUtc)")
                            .Append("VALUES (@Id, @ProjectId, @ComponentName, @UseConfigurationGroup, @ConfigurationId, @IsolationType, @CreatedByUserName, @CreatedDateTimeUtc, @UpdatedByUserName, @UpdatedDateTimeUtc)", component);
                db.Execute(sql);
            }
            return component;
        }

        public DeployComponent GetComponent(string componentId, string projectId = null)
        {
            var item = this.TryGetComponent(componentId, projectId);
            if(item == null)
            {
                throw new RecordNotFoundException(typeof(DeployComponent), "Id", componentId);
            }
            return item;
        }

        public DeployComponent TryGetComponent(string componentId, string projectId = null)
        {
            if (string.IsNullOrEmpty(componentId))
            {
                throw new ArgumentNullException("Missing component ID");
            }
            if (!string.IsNullOrEmpty(projectId))
            {
                VerifyProjectExists(projectId);
            }
            using (var db = _sqlConnectionInfo.GetDB())
            {
                var sql = GetComponentBaseQuery()
                            .Append("WHERE ID=@0", componentId);
                if (!string.IsNullOrEmpty(projectId))
                {
                    sql = sql.Append("AND ProjectID=@0", projectId);
                }
                return db.SingleOrDefault<DeployComponent>(sql);
            }
        }

        private PetaPoco.Sql GetComponentBaseQuery()
        {
            return PetaPoco.Sql.Builder
                    .Append("SELECT ID, ProjectID, ComponentName, UseConfigurationGroup, ConfigurationID, EnumDeploymentIsolationTypeID AS IsolationType, CreatedByUserName, CreatedDateTimeUtc, UpdatedByUserName, UpdatedDateTimeUtc")
                    .Append("FROM Component");
        }

        public DeployComponent GetOrCreateComponent(string projectId, string componentId, string componentName)
        {
            throw new NotImplementedException();
        }

        public DeployComponent UpdateComponent(string componentId, string projectId, string componentName, bool useConfigurationGroup, string configurationId, EnumDeploymentIsolationType isolationType)
        {
            if(string.IsNullOrEmpty(projectId))
            {
                throw new ArgumentNullException("Missing projectID");
            }
            if(string.IsNullOrEmpty(componentId))
            {
                throw new ArgumentNullException("Component ID");
            }
            if(string.IsNullOrEmpty(componentName))
            {
                throw new ArgumentNullException("Missing component name");   
            }
            VerifyProjectExists(projectId);
            VerifyComponentExists(componentId, projectId);
            using(var db = _sqlConnectionInfo.GetDB())
            {
                var sql = PetaPoco.Sql.Builder
                                .Append("UPDATE Component")
                                .Append("SET ComponentName=@0, UseConfigurationGroup=@1, ConfigurationID=@2, EnumDeploymentIsolationTypeID=@3, UpdatedByUserName=@4, UpdatedDateTimeUtc=@5", 
                                        componentName, useConfigurationGroup, configurationId, isolationType, _userIdentity.UserName, DateTime.UtcNow)
                                .Append("WHERE ID=@0", componentId);
                db.Execute(sql);
            }
            return this.GetComponent(componentId, projectId);
        }

        public void DeleteComponent(string projectId, string componentId)
        {
            VerifyProjectExists(projectId);
            VerifyComponentExists(componentId, projectId);
            using(var db = _sqlConnectionInfo.GetDB())
            {
                var sql = PetaPoco.Sql.Builder
                                .Append("DELETE FROM Component")
                                .Append("WHERE ID=@0", componentId);
                db.Execute(sql);
            }
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

        public List<DeployProjectBranch> GetBranchList(string projectId)
        {
            if(string.IsNullOrEmpty(projectId))
            {
                throw new ArgumentNullException("Missing project ID");
            }
            VerifyProjectExists(projectId);
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
            var item = this.TryGetBranch(branchId, projectId);
            if(item == null)
            {
                throw new RecordNotFoundException(typeof(DeployProjectBranch), "Id", branchId);
            }
            return item;
        }

        public DeployProjectBranch TryGetBranch(string branchId, string projectId = null)
        {
            if(string.IsNullOrEmpty(branchId))
            {
                throw new ArgumentNullException("Missing branch ID");
            }
            using(var db = _sqlConnectionInfo.GetDB())
            {
                var sql = GetBranchBaseQuery().Append("WHERE ID=@0", branchId);
                if(!string.IsNullOrEmpty(projectId))
                {
                    sql = sql.Append("AND ProjectID=@0", projectId);
                }
                return db.SingleOrDefault<DeployProjectBranch>(sql);
            }
        }

        private PetaPoco.Sql GetBranchBaseQuery()
        {
            return PetaPoco.Sql.Builder
                .Append("SELECT ID, ProjectID, BranchName, CreatedByUserName, CreatedDateTimeUtc, UpdatedByUserName, UpdatedDateTimeUtc")
                .Append("FROM Branch");
        }

        public DeployProjectBranch GetBranchByName(string projectId, string branchName)
        {
            var item = TryGetBranchByName(projectId, branchName);
            if(item == null)
            {
                throw new RecordNotFoundException(typeof(DeployProjectBranch), "BranchName", branchName);
            }
            return item;
        }

        public DeployProjectBranch TryGetBranchByName(string projectId, string branchName)
        {
            if (string.IsNullOrEmpty(projectId))
            {
                throw new ArgumentNullException("Missing project ID");
            }
            if (string.IsNullOrEmpty(branchName))
            {
                throw new ArgumentNullException("Missing brandh name");
            }
            VerifyProjectExists(projectId);
            using (var db = _sqlConnectionInfo.GetDB())
            {
                var sql = GetBranchBaseQuery().Where("ProjectID=@0 AND BranchName=@1", projectId, branchName);
                return db.SingleOrDefault<DeployProjectBranch>(sql);
            }
        }

        public DeployProjectBranch GetOrCreateBranch(string projectId, string branchIdOrName)
        {
            if(string.IsNullOrEmpty(projectId))
            {
                throw new ArgumentNullException("Missing project ID");
            }
            if(string.IsNullOrEmpty(branchIdOrName))
            {
                throw new ArgumentNullException("Missing branch ID or name");
            }
            VerifyProjectExists(projectId);
            var item = TryGetBranch(branchIdOrName, projectId);
            if(item == null)
            {
                item = TryGetBranchByName(projectId, branchIdOrName);
            }
            if(item == null)
            {
                item = CreateBranch(projectId, branchIdOrName);
            }
            return item;
        }

        public DeployProjectBranch UpdateBranch(string branchId, string projectId, string branchName)
        {
            if(string.IsNullOrEmpty(branchId))
            {
                throw new ArgumentNullException("Missing branch ID");
            }
            if(string.IsNullOrEmpty(projectId))
            {
                throw new ArgumentNullException("Missing project ID");
            }
            if(string.IsNullOrEmpty(branchName))
            {
                throw new ArgumentNullException("Missing branch name");
            }
            VerifyProjectExists(projectId);
            VerifyBranchExists(branchId, projectId);
            using(var db = _sqlConnectionInfo.GetDB())
            {
                var sql = PetaPoco.Sql.Builder
                            .Append("UPDATE Branch")
                            .Append("SET BranchName=@0, UpdatedByUserName=@1, UpdatedDateTimeUtc=@2", branchName, _userIdentity.UserName, DateTime.UtcNow)
                            .Append("WHERE ID=@0", branchId);
                if(!string.IsNullOrEmpty(projectId))
                {
                    sql = sql.Append("AND ProjectID=@0", projectId);
                }
                db.Execute(sql);
            }
            return GetBranch(branchId, projectId);
        }

        public void DeleteBranch(string branchId, string projectId)
        {
            if(string.IsNullOrEmpty(branchId))
            {
                throw new ArgumentNullException("Missing project ID");
            }
            if(!string.IsNullOrEmpty(projectId))
            {
                VerifyProjectExists(projectId);
            }
            VerifyBranchExists(branchId, projectId);
            using(var db = _sqlConnectionInfo.GetDB())
            {
                var sql = PetaPoco.Sql.Builder
                                .Append("DELETE FROM Branch WHERE ID=@0", branchId);
                if(!string.IsNullOrEmpty(projectId))
                {
                    sql = sql.Append("AND ProjectID=@0", projectId);
                }
                db.Execute(sql);
            }
        }

        public List<DeployEnvironment> GetEnvironmentList(string projectId)
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
