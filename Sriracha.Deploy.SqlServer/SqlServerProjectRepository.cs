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

        private class ConfigurationPair
        {
            public string Id { get; set; }
            public string ConfigurationName { get; set; }
            public string ConfigurationValue { get; set; }
        }

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
                var projectExistsSql = PetaPoco.Sql.Builder.Append("SELECT COUNT(*) FROM DeployProject WHERE ID=@0", projectId);
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
                var branchExistsSql = PetaPoco.Sql.Builder.Append("SELECT COUNT(*) FROM DeployBranch WHERE ID=@0", branchId);
                if (!string.IsNullOrEmpty(projectId))
                {
                    branchExistsSql = branchExistsSql.Append("AND DeployProjectID=@0", projectId);
                }
                if (db.ExecuteScalar<int>(branchExistsSql) == 0)
                {
                    throw new RecordNotFoundException(typeof(DeployEnvironment), "Id", branchId);
                }
            }
        }

        private void VerifyEnvironmentExists(string environmentId, string projectId)
        {
            if (string.IsNullOrEmpty(environmentId))
            {
                throw new ArgumentNullException("Missing environment ID");
            }
            using (var db = _sqlConnectionInfo.GetDB())
            {
                var environmentExistsSql = PetaPoco.Sql.Builder.Append("SELECT COUNT(*) FROM DeployEnvironment WHERE ID=@0", environmentId);
                if (!string.IsNullOrEmpty(projectId))
                {
                    environmentExistsSql = environmentExistsSql.Append("AND DeployProjectID=@0", projectId);
                }
                if (db.ExecuteScalar<int>(environmentExistsSql) == 0)
                {
                    throw new RecordNotFoundException(typeof(DeployEnvironment), "Id", environmentId);
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
                var configurationExistsSql = PetaPoco.Sql.Builder.Append("SELECT COUNT(*) FROM DeployConfiguration WHERE ID=@0", configurationId);
                if(!string.IsNullOrEmpty(projectId))
                {
                    configurationExistsSql = configurationExistsSql.Append("AND DeployProjectID=@0", projectId);
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
                var componentExistsSql = PetaPoco.Sql.Builder.Append("SELECT COUNT(*) FROM DeployComponent WHERE ID=@0", componentId);
                if (!string.IsNullOrEmpty(projectId))
                {
                    componentExistsSql = componentExistsSql.Append("AND DeployProjectID=@0", projectId);
                }
                if (db.ExecuteScalar<int>(componentExistsSql) == 0)
                {
                    throw new RecordNotFoundException(typeof(DeployComponent), "Id", componentId);
                }
            }
        }

        private void VerifyComponentStepExists(string deploymentStepId, string componentId, string projectId)
        {
            if (string.IsNullOrEmpty(deploymentStepId))
            {
                throw new ArgumentNullException("Missing deployment step Id");
            }
            using (var db = _sqlConnectionInfo.GetDB())
            {
                var stepExistsSql = PetaPoco.Sql.Builder.Append("SELECT COUNT(*) FROM DeployComponentStep WHERE ID=@0", deploymentStepId);
                if (!string.IsNullOrEmpty(projectId))
                {
                    stepExistsSql = stepExistsSql.Append("AND DeployProjectID=@0", projectId);
                }
                if (!string.IsNullOrEmpty(componentId))
                {
                    stepExistsSql = stepExistsSql.Append("AND DeployComponentID=@0", componentId);
                }
                if (db.ExecuteScalar<int>(stepExistsSql) == 0)
                {
                    throw new RecordNotFoundException(typeof(DeployStep), "Id", deploymentStepId);
                }
            }
        }

        private void VerifyConfigurationStepExists(string deploymentStepId, string configurationId, string projectId)
        {
            if (string.IsNullOrEmpty(deploymentStepId))
            {
                throw new ArgumentNullException("Missing deployment step Id");
            }
            using (var db = _sqlConnectionInfo.GetDB())
            {
                var stepExistsSql = PetaPoco.Sql.Builder.Append("SELECT COUNT(*) FROM DeployConfigurationStep WHERE ID=@0", deploymentStepId);
                if (!string.IsNullOrEmpty(projectId))
                {
                    stepExistsSql = stepExistsSql.Append("AND DeployProjectID=@0", projectId);
                }
                if (!string.IsNullOrEmpty(configurationId))
                {
                    stepExistsSql = stepExistsSql.Append("AND DeployConfigurationID=@0", configurationId);
                }
                if (db.ExecuteScalar<int>(stepExistsSql) == 0)
                {
                    throw new RecordNotFoundException(typeof(DeployStep), "Id", deploymentStepId);
                }
            }
        }

        private string VerifySharedComponentStepExists(string sharedDeploymentStepId, string componentId, string projectId)
        {
            if (string.IsNullOrEmpty(sharedDeploymentStepId))
            {
                throw new ArgumentNullException("Missing deployment step Id");
            }
            if(string.IsNullOrEmpty(componentId))
            {
                throw new ArgumentNullException("Missing component ID");
            }
            using (var db = _sqlConnectionInfo.GetDB())
            {
                var stepExistsSql = PetaPoco.Sql.Builder.Append("SELECT ID FROM DeployComponentStep WHERE SharedDeploymentStepID=@0", sharedDeploymentStepId);
                if (!string.IsNullOrEmpty(projectId))
                {
                    stepExistsSql = stepExistsSql.Append("AND DeployProjectID=@0", projectId);
                }
                if (!string.IsNullOrEmpty(componentId))
                {
                    stepExistsSql = stepExistsSql.Append("AND DeployComponentID=@0", componentId);
                }
                string deploymentStepId = db.ExecuteScalar<string>(stepExistsSql);
                if (string.IsNullOrEmpty(deploymentStepId))
                {
                    throw new RecordNotFoundException(typeof(DeployComponent), "SharedDeploymentStepId", componentId);
                }
                return deploymentStepId;
            }
        }

        public IEnumerable<DeployProject> GetProjectList()
        {
            using(var db = _sqlConnectionInfo.GetDB())
            {
                var sql = PetaPoco.Sql.Builder
                            .Append("SELECT ID, ProjectName, UsesSharedComponentConfiguration, CreatedByUserName, CreatedDateTimeUtc, UpdatedByUserName, UpdatedDateTimeUtc")
                            .Append("FROM DeployProject");
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
            project.EnvironmentList = GetEnvironmentList(project.Id);
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
                    .Append("INSERT INTO DeployProject (ID, ProjectName, UsesSharedComponentConfiguration, CreatedByUserName, CreatedDateTimeUtc, UpdatedByUserName, UpdatedDateTimeUtc)")
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
                            .Append("FROM DeployProject")
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
                            .Append("FROM DeployProject")
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
                var sql = PetaPoco.Sql.Builder.Append("UPDATE DeployProject")
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
                                .Append("DELETE FROM DeployBranch WHERE DeployProjectID=@0;", projectId)
                                    .Append("DELETE FROM DeployComponentStep WHERE DeployProjectID=@0;", projectId)
                                .Append("DELETE FROM DeployComponent WHERE DeployProjectID=@0;", projectId)
                                    .Append("DELETE FROM DeployConfigurationStep WHERE DeployProjectID=@0;", projectId)
                                .Append("DELETE FROM DeployConfiguration WHERE DeployProjectID=@0;", projectId)
                                .Append("DELETE FROM DeployProject WHERE ID=@0", projectId);
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
                            .Append("WHERE DeployProjectID=@0", projectId);
                var list = db.Fetch<DeployConfiguration>(sql).ToList();
                foreach(var item in list)
                {
                    LoadConfigurationChildren(item);
                }
                return list;
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
                            .Append("INSERT INTO DeployConfiguration (ID, DeployProjectID, ConfigurationName, EnumDeploymentIsolationTypeID, CreatedByUserName, CreatedDateTimeUtc, UpdatedByUserName, UpdatedDateTimeUtc)")
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
                    sql = sql.Append("AND DeployProjectID=@0", projectId);
                }
                var item = db.SingleOrDefault<DeployConfiguration>(sql);
                if(item != null)
                {
                    this.LoadConfigurationChildren(item);
                }
                return item;
            }
        }

        private void LoadConfigurationChildren(DeployConfiguration item)
        {
            item.DeploymentStepList = this.GetConfigurationDeploymentStepList(item.Id);
        }

        private PetaPoco.Sql GetConfigurationBaseQuery()
        {
            return PetaPoco.Sql.Builder
                            .Append("SELECT ID, DeployProjectID AS ProjectID, ConfigurationName, EnumDeploymentIsolationTypeID AS IsolationType, CreatedByUserName, CreatedDateTimeUtc, UpdatedByUserName, UpdatedDateTimeUtc")
                            .Append("FROM DeployConfiguration");
        }

        public DeployConfiguration UpdateConfiguration(string configurationId, string projectId, string configurationName, EnumDeploymentIsolationType isolationType)
        {
            VerifyProjectExists(projectId);
            VerifyConfigurationExists(configurationId, projectId);
            using(var db = _sqlConnectionInfo.GetDB())
            {
                var sql = PetaPoco.Sql.Builder
                                .Append("UPDATE DeployConfiguration")
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
                                .Append("DELETE FROM DeployConfigurationStep WHERE DeployConfigurationID=@0;", configurationId)
                                .Append("DELETE FROM DeployConfiguration WHERE ID=@0", configurationId);
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
                var sql = GetComponentBaseQuery().Where("DeployProjectID=@0", projectId);
                var list = db.Fetch<DeployComponent>(sql).ToList();
                foreach(var item in list)
                {
                    LoadComponentChildren(item);
                }
                return list;
            }
        }

        private void LoadComponentChildren(DeployComponent item)
        {
            item.DeploymentStepList = this.GetComponentDeploymentStepList(item.Id);
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
                            .Append("INSERT INTO DeployComponent (ID, DeployProjectID, ComponentName, UseConfigurationGroup, DeployConfigurationID, EnumDeploymentIsolationTypeID, CreatedByUserName, CreatedDateTimeUtc, UpdatedByUserName, UpdatedDateTimeUtc)")
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
                    sql = sql.Append("AND DeployProjectID=@0", projectId);
                }
                var item = db.SingleOrDefault<DeployComponent>(sql);
                if(item != null)
                {
                    LoadComponentChildren(item);
                }
                return item;
            }
        }

        private PetaPoco.Sql GetComponentBaseQuery()
        {
            return PetaPoco.Sql.Builder
                    .Append("SELECT ID, DeployProjectID AS ProjectID, ComponentName, UseConfigurationGroup, DeployConfigurationID AS ConfigurationID, EnumDeploymentIsolationTypeID AS IsolationType, CreatedByUserName, CreatedDateTimeUtc, UpdatedByUserName, UpdatedDateTimeUtc")
                    .Append("FROM DeployComponent");
        }

        public DeployComponent GetOrCreateComponent(string projectId, string componentIdOrName)
        {
            if(string.IsNullOrEmpty(projectId))
            {
                throw new ArgumentNullException("Missing project ID");
            }
            if(string.IsNullOrEmpty(componentIdOrName))
            {
                throw new ArgumentNullException("Missing component ID or name");
            }
            VerifyProjectExists(projectId);
            var item = TryGetComponent(componentIdOrName);
            if(item == null)
            {
                item = TryGetComponentByName(componentIdOrName);
            }
            if(item == null)
            {
                item = CreateComponent(projectId, componentIdOrName, false, null, EnumDeploymentIsolationType.IsolatedPerMachine);
            }
            return item;
        }

        private DeployComponent TryGetComponentByName(string componentName)
        {
            using(var db = _sqlConnectionInfo.GetDB())
            {
                var sql = GetComponentBaseQuery().Append("WHERE ComponentName=@0", componentName);
                var item = db.SingleOrDefault<DeployComponent>(sql);
                if(item != null)
                {
                    LoadComponentChildren(item);
                }
                return item;
            }
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
                                .Append("UPDATE DeployComponent")
                                .Append("SET ComponentName=@0, UseConfigurationGroup=@1, DeployConfigurationID=@2, EnumDeploymentIsolationTypeID=@3, UpdatedByUserName=@4, UpdatedDateTimeUtc=@5", 
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
                                .Append("DELETE FROM DeployComponentStep WHERE DeployComponentID=@0;", componentId)
                                .Append("DELETE FROM DeployComponent WHERE ID=@0;", componentId);
                db.Execute(sql);
            }
        }

        public List<DeployStep> GetComponentDeploymentStepList(string componentId)
        {
            VerifyComponentExists(componentId, null);
            using(var db = _sqlConnectionInfo.GetDB())
            {
                var sql = GetComponentStepBaseQuery().Append("WHERE DeployComponentID=@0", componentId);
                return db.Fetch<DeployStep>(sql).ToList();
            }
        }

        public List<DeployStep> GetConfigurationDeploymentStepList(string configurationId)
        {
            if(string.IsNullOrEmpty(configurationId))
            {
                throw new ArgumentNullException("Missing configuration ID");
            }
            VerifyConfigurationExists(configurationId, null);
            using(var db = _sqlConnectionInfo.GetDB())
            {
                var sql = GetConfigurationStepBaseQuery().Append("WHERE DeployConfigurationID=@0", configurationId);
                return db.Fetch<DeployStep>(sql).ToList();
            }
        }

        private PetaPoco.Sql GetConfigurationStepBaseQuery()
        {
            return PetaPoco.Sql.Builder
                        .Append("SELECT ID, DeployProjectID AS ProjectID, DeployConfigurationID AS ParentId, 'Configuration' AS ParentType, StepName, TaskTypeName, TaskOptionsJson, CreatedDateTimeUtc, CreatedByUserName, UpdatedDateTimeUtc, UpdatedByUserName")
                        .Append("FROM DeployConfigurationStep");
        }

        public DeployStep CreateComponentDeploymentStep(string projectId, string componentId, string stepName, string taskTypeName, string taskOptionsJson, string sharedDeploymentStepId)
        {
            if(string.IsNullOrEmpty(projectId))
            {
                throw new ArgumentNullException("Missing project ID");
            }
            if(string.IsNullOrEmpty(componentId))
            {
                throw new ArgumentNullException("Missing component ID");
            }
            if(string.IsNullOrEmpty(stepName))
            {
                throw new ArgumentNullException("Missing step name");
            }
            if(string.IsNullOrEmpty(taskTypeName))
            {
                throw new ArgumentNullException("Missing task tpe name");
            }
            sharedDeploymentStepId = StringHelper.IsNullOrEmpty(sharedDeploymentStepId, Guid.NewGuid().ToString());
            VerifyProjectExists(projectId);
            VerifyComponentExists(componentId, projectId);
            var step = new DeployStep
            {
                Id = Guid.NewGuid().ToString(),
                ProjectId = projectId,
                ParentId = componentId,
                ParentType = EnumDeployStepParentType.Component,
                StepName = stepName,
                TaskTypeName = taskTypeName,
                TaskOptionsJson = taskOptionsJson,
                SharedDeploymentStepId = sharedDeploymentStepId,
                CreatedByUserName = _userIdentity.UserName,
                CreatedDateTimeUtc = DateTime.UtcNow,
                UpdatedByUserName = _userIdentity.UserName,
                UpdatedDateTimeUtc = DateTime.UtcNow
            };
            using(var db = _sqlConnectionInfo.GetDB())
            {
                var sql = PetaPoco.Sql.Builder
                            .Append("INSERT INTO DeployComponentStep (ID, DeployProjectID, DeployComponentID, StepName, TaskTypeName, TaskOptionsJson, SharedDeploymentStepID, CreatedDateTimeUtc, CreatedByUserName, UpdatedDateTimeUtc, UpdatedByUserName)")
                            .Append("VALUES (@Id, @ProjectId, @ParentId, @StepName, @TaskTypeName, @TaskOptionsJson, @SharedDeploymentStepId, @CreatedDateTimeUtc, @CreatedByUserName, @UpdatedDateTimeUtc, @UpdatedByUserName)", step);
                db.Execute(sql);
            }
            return step;
        }

        public DeployStep CreateConfigurationDeploymentStep(string projectId, string configurationId, string stepName, string taskTypeName, string taskOptionsJson)
        {
            if (string.IsNullOrEmpty(projectId))
            {
                throw new ArgumentNullException("Missing project ID");
            }
            if (string.IsNullOrEmpty(configurationId))
            {
                throw new ArgumentNullException("Missing configuration ID");
            }
            if (string.IsNullOrEmpty(stepName))
            {
                throw new ArgumentNullException("Missing step name");
            }
            if (string.IsNullOrEmpty(taskTypeName))
            {
                throw new ArgumentNullException("Missing task tpe name");
            }
            VerifyProjectExists(projectId);
            VerifyConfigurationExists(configurationId, projectId);
            var step = new DeployStep
            {
                Id = Guid.NewGuid().ToString(),
                ProjectId = projectId,
                ParentId = configurationId,
                ParentType = EnumDeployStepParentType.Configuration,
                StepName = stepName,
                TaskTypeName = taskTypeName,
                TaskOptionsJson = taskOptionsJson,
                SharedDeploymentStepId = null,
                CreatedByUserName = _userIdentity.UserName,
                CreatedDateTimeUtc = DateTime.UtcNow,
                UpdatedByUserName = _userIdentity.UserName,
                UpdatedDateTimeUtc = DateTime.UtcNow
            };
            using (var db = _sqlConnectionInfo.GetDB())
            {
                var sql = PetaPoco.Sql.Builder
                            .Append("INSERT INTO DeployConfigurationStep (ID, DeployProjectID, DeployConfigurationID, StepName, TaskTypeName, TaskOptionsJson, CreatedDateTimeUtc, CreatedByUserName, UpdatedDateTimeUtc, UpdatedByUserName)")
                            .Append("VALUES (@Id, @ProjectId, @ParentId, @StepName, @TaskTypeName, @TaskOptionsJson, @CreatedDateTimeUtc, @CreatedByUserName, @UpdatedDateTimeUtc, @UpdatedByUserName)", step);
                db.Execute(sql);
            }
            return step;
        }

        public DeployStep GetComponentDeploymentStep(string deploymentStepId)
        {
            if(string.IsNullOrEmpty(deploymentStepId))
            {
                throw new ArgumentNullException("Missing deployment step ID");
            }
            using(var db = _sqlConnectionInfo.GetDB())
            {
                var sql = GetComponentStepBaseQuery().Where("ID=@0", deploymentStepId);
                var item = db.SingleOrDefault<DeployStep>(sql);
                if(item == null)
                {   
                    throw new RecordNotFoundException(typeof(DeployStep), "Id", deploymentStepId);
                }
                return item;
            }
        }

        private PetaPoco.Sql GetComponentStepBaseQuery()
        {
            return PetaPoco.Sql.Builder
                .Append("SELECT ID, DeployProjectID AS ProjectID, DeployComponentID AS ParentID, 'Component' AS ParentType, StepName, TaskTypeName, TaskOptionsJson, SharedDeploymentStepID, CreatedDateTimeUtc, CreatedByUserName, UpdatedDateTimeUtc, UpdatedByUserName")
                .From("DeployComponentStep");
        }

        public DeployStep GetConfigurationDeploymentStep(string deploymentStepId)
        {
            if(string.IsNullOrEmpty(deploymentStepId))
            {
                throw new ArgumentNullException("Missing deployment step ID");
            }
            using(var db = _sqlConnectionInfo.GetDB())
            {
                var sql = GetConfigurationStepBaseQuery().Append("WHERE ID=@0", deploymentStepId);
                var item = db.SingleOrDefault<DeployStep>(sql);
                if(item == null)
                {
                    throw new RecordNotFoundException(typeof(DeployStep), "Id", deploymentStepId);
                }
                return item;
            }
        }

        public DeployStep UpdateComponentDeploymentStep(string deploymentStepId, string projectId, string componentId, string stepName, string taskTypeName, string taskOptionsJson, string sharedDeploymentStepId)
        {
            if(string.IsNullOrEmpty(projectId))
            {
                throw new ArgumentNullException("Missing project ID");
            }
            if(string.IsNullOrEmpty(componentId))
            {
                throw new ArgumentNullException("Missing component ID");
            }
            if(string.IsNullOrEmpty(stepName))
            {
                throw new ArgumentNullException("Missing step name");
            }
            if(string.IsNullOrEmpty(taskTypeName))
            {
                throw new ArgumentNullException("Missing task type name");
            }
            VerifyProjectExists(projectId);
            VerifyComponentExists(componentId, projectId);
            if (!string.IsNullOrEmpty(deploymentStepId))
            {
                VerifyComponentStepExists(deploymentStepId, componentId, projectId);
            }
            else if (!string.IsNullOrEmpty(sharedDeploymentStepId))
            {
                deploymentStepId = VerifySharedComponentStepExists(sharedDeploymentStepId, componentId, projectId);
            }
            else 
            {
                throw new ArgumentNullException("Either deploymentStepId or sharedDeploymentStepId must be specified");
            }
            using (var db = _sqlConnectionInfo.GetDB())
            {
                var sql = PetaPoco.Sql.Builder
                            .Append("UPDATE DeployComponentStep")
                            .Append("SET StepName=@0, TaskTypeName=@1, TaskOptionsJson=@2, SharedDeploymentStepID=@3, UpdatedByUserName=@4, UpdatedDateTimeUtc=@5", 
                                        stepName, taskTypeName, taskOptionsJson, sharedDeploymentStepId, _userIdentity.UserName, DateTime.UtcNow)
                            .Append("WHERE ID=@0", deploymentStepId);
                if(!string.IsNullOrEmpty(projectId))
                {
                    sql = sql.Append("AND DeployProjectID=@0", projectId);
                }
                if(!string.IsNullOrEmpty(componentId))
                {
                    sql = sql.Append("AND DeployComponentID=@0", componentId);
                }
                db.Execute(sql);
            }
            return this.GetComponentDeploymentStep(deploymentStepId);
        }

        public DeployStep UpdateConfigurationDeploymentStep(string deploymentStepId, string projectId, string configurationId, string stepName, string taskTypeName, string taskOptionsJson)
        {
            if (string.IsNullOrEmpty(deploymentStepId))
            {
                throw new ArgumentNullException("Missign dpeloyment step ID");
            }
            if (string.IsNullOrEmpty(projectId))
            {
                throw new ArgumentNullException("Missing project ID");
            }
            if (string.IsNullOrEmpty(configurationId))
            {
                throw new ArgumentNullException("Missing configuration ID");
            }
            if (string.IsNullOrEmpty(stepName))
            {
                throw new ArgumentNullException("Missing step name");
            }
            if (string.IsNullOrEmpty(taskTypeName))
            {
                throw new ArgumentNullException("Missing task type name");
            }
            VerifyProjectExists(projectId);
            VerifyConfigurationExists(configurationId, projectId);
            VerifyConfigurationStepExists(deploymentStepId, configurationId, projectId);
            using (var db = _sqlConnectionInfo.GetDB())
            {
                var sql = PetaPoco.Sql.Builder
                            .Append("UPDATE DeployConfigurationStep")
                            .Append("SET StepName=@0, TaskTypeName=@1, TaskOptionsJson=@2, UpdatedByUserName=@3, UpdatedDateTimeUtc=@4",
                                        stepName, taskTypeName, taskOptionsJson, _userIdentity.UserName, DateTime.UtcNow)
                            .Append("WHERE ID=@0", deploymentStepId);
                if (!string.IsNullOrEmpty(projectId))
                {
                    sql = sql.Append("AND DeployProjectID=@0", projectId);
                }
                if (!string.IsNullOrEmpty(configurationId))
                {
                    sql = sql.Append("AND DeployConfigurationID=@0", configurationId);
                }
                db.Execute(sql);
            }
            return this.GetConfigurationDeploymentStep(deploymentStepId);
        }

        public void DeleteComponentDeploymentStep(string deploymentStepId)
        {
            VerifyComponentStepExists(deploymentStepId, null, null);
            using(var db = _sqlConnectionInfo.GetDB())
            {
                var sql = PetaPoco.Sql.Builder
                            .Append("DELETE FROM DeployComponentStep")
                            .Append("WHERE ID=@0", deploymentStepId);
                db.Execute(sql);
            }
        }

        public void DeleteConfigurationDeploymentStep(string deploymentStepId)
        {
            VerifyConfigurationStepExists(deploymentStepId,null, null);
            using(var db = _sqlConnectionInfo.GetDB())
            {
                var sql = PetaPoco.Sql.Builder
                                .Append("DELETE FROM DeployConfigurationStep")
                                .Append("WHERE ID=@0", deploymentStepId);
                db.Execute(sql);
            }
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
                var sql = GetBranchBaseQuery().Append("WHERE DeployProjectID=@0", projectId);
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
                            .Append("INSERT INTO DeployBranch (ID, DeployProjectID, BranchName, CreatedByUserName, CreatedDateTimeUtc, UpdatedByUserName, UpdatedDateTimeUtc)")
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
                    sql = sql.Append("AND DeployProjectID=@0", projectId);
                }
                return db.SingleOrDefault<DeployProjectBranch>(sql);
            }
        }

        private PetaPoco.Sql GetBranchBaseQuery()
        {
            return PetaPoco.Sql.Builder
                .Append("SELECT ID, DeployProjectID AS ProjectID, BranchName, CreatedByUserName, CreatedDateTimeUtc, UpdatedByUserName, UpdatedDateTimeUtc")
                .Append("FROM DeployBranch");
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
                var sql = GetBranchBaseQuery().Where("DeployProjectID=@0 AND BranchName=@1", projectId, branchName);
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
                            .Append("UPDATE DeployBranch")
                            .Append("SET BranchName=@0, UpdatedByUserName=@1, UpdatedDateTimeUtc=@2", branchName, _userIdentity.UserName, DateTime.UtcNow)
                            .Append("WHERE ID=@0", branchId);
                if(!string.IsNullOrEmpty(projectId))
                {
                    sql = sql.Append("AND DeployProjectID=@0", projectId);
                }
                db.Execute(sql);
            }
            return GetBranch(branchId, projectId);
        }

        public void DeleteBranch(string branchId, string projectId)
        {
            if(string.IsNullOrEmpty(branchId))
            {
                throw new ArgumentNullException("Missing branch ID");
            }
            if(string.IsNullOrEmpty(projectId))
            {
                throw new ArgumentNullException("Missing project ID");
            }
            VerifyProjectExists(projectId);
            VerifyBranchExists(branchId, projectId);
            using(var db = _sqlConnectionInfo.GetDB())
            {
                var sql = PetaPoco.Sql.Builder
                                .Append("DELETE FROM DeployBranch WHERE ID=@0", branchId);
                if(!string.IsNullOrEmpty(projectId))
                {
                    sql = sql.Append("AND DeployProjectID=@0", projectId);
                }
                db.Execute(sql);
            }
        }

        public List<DeployEnvironment> GetEnvironmentList(string projectId)
        {
            VerifyProjectExists(projectId);
            using(var db = _sqlConnectionInfo.GetDB())
            {
                var sql = GetEnvironmentBaseQuery().Append("WHERE DeployProjectID=@0", projectId);
                var list = db.Fetch<DeployEnvironment>(sql);
                foreach(var item in list)
                {
                    LoadEnvironmentChildren(item);
                }
                return list;
            }
        }

        public DeployEnvironment CreateEnvironment(string projectId, string environmentName, IEnumerable<DeployEnvironmentConfiguration> componentList, IEnumerable<DeployEnvironmentConfiguration> configurationList)
        {
            if(string.IsNullOrEmpty(projectId))
            {
                throw new ArgumentNullException("Missing project ID");
            }
            if(string.IsNullOrEmpty(environmentName))
            {
                throw new ArgumentNullException("Missing environment name");
            }
            VerifyProjectExists(projectId);
            var item = new DeployEnvironment
            {
                Id = Guid.NewGuid().ToString(),
                ProjectId = projectId,
                EnvironmentName = environmentName,
                CreatedByUserName = _userIdentity.UserName,
                CreatedDateTimeUtc = DateTime.UtcNow,
                UpdatedByUserName = _userIdentity.UserName,
                UpdatedDateTimeUtc = DateTime.UtcNow,
                ComponentList = (componentList ?? new List<DeployEnvironmentConfiguration>()).ToList(),
                ConfigurationList = (configurationList ?? new List<DeployEnvironmentConfiguration>()).ToList()
            };
            UpdateEnvironmentComponentList(item.ComponentList, item.Id, projectId, environmentName, EnumDeployStepParentType.Component);
            UpdateEnvironmentComponentList(item.ConfigurationList, item.Id, projectId, environmentName, EnumDeployStepParentType.Configuration);
            using (var db = _sqlConnectionInfo.GetDB())
            {
                var sql = PetaPoco.Sql.Builder
                            .Append("INSERT INTO DeployEnvironment (ID, DeployProjectID, EnvironmentName, CreatedDateTimeUtc, CreatedByUserName, UpdatedDateTimeUtc, UpdatedByUserName)")
                            .Append("VALUES (@Id, @ProjectId, @EnvironmentName, @CreatedDateTimeUtc, @CreatedByUserName, @UpdatedDateTimeUtc, @UpdatedByUserName)", item);
                db.Execute(sql);
            }
            SaveEnvironmentConfigurationList(item.Id, item.ComponentList, EnumDeployStepParentType.Component);
            SaveEnvironmentConfigurationList(item.Id, item.ConfigurationList, EnumDeployStepParentType.Configuration);
            return item;
        }

        private void SaveEnvironmentConfigurationList(string environmentId, IEnumerable<DeployEnvironmentConfiguration> componentList, EnumDeployStepParentType parentType)
        {
            var itemsToInsert = new List<DeployEnvironmentConfiguration>();
            var itemsToUpdate = new List<DeployEnvironmentConfiguration>();
            var itemsToDelete = new List<DeployEnvironmentConfiguration>();

            var existingItemList = this.GetEnvironmentConfigurationList(environmentId, parentType);

            foreach(var newItem in componentList)
            {
                var existingItem = existingItemList.Any(i=>i.Id == newItem.Id);
                if(existingItem)
                {
                    itemsToUpdate.Add(newItem);
                }
                else 
                {
                    itemsToInsert.Add(newItem);
                }
            }
            foreach(var existingItem in existingItemList)
            {
                var newItem = componentList.Any(i=>i.Id == existingItem.Id);
                if(!newItem)
                {
                    itemsToDelete.Add(existingItem);
                }
            }
            using(var db = _sqlConnectionInfo.GetDB())
            {
                foreach (var item in itemsToInsert)
                {
                    if(string.IsNullOrEmpty(item.Id))
                    {
                        item.Id = Guid.NewGuid().ToString();
                    }
                    item.CreatedByUserName = _userIdentity.UserName;
                    item.CreatedDateTimeUtc = DateTime.UtcNow;
                    item.UpdatedByUserName = _userIdentity.UserName;
                    item.UpdatedDateTimeUtc = DateTime.UtcNow;

                    var sql = PetaPoco.Sql.Builder
                                .Append("INSERT INTO DeployEnvironmentConfiguration (ID, DeployProjectID, DeployEnvironmentID, ParentID, EnumDeployStepParentTypeID, DeployCredentialsId, CreatedDateTimeUtc, CreatedByUserName, UpdatedDateTimeUtc, UpdatedByUserName)")
                                .Append("VALUES (@Id, @ProjectId, @EnvironmentId, @ParentId, @ParentType, @DeployCredentialsId, @CreatedDateTimeUtc, @CreatedByUserName, @UpdatedDateTimeUtc, @UpdatedByUserName)", item);
                    db.Execute(sql);

                    SaveMachineList(item);
                    SaveEnvironmentComponentValueList(item);
                }
                foreach (var item in itemsToUpdate)
                {
                    item.UpdatedByUserName = _userIdentity.UserName;
                    item.UpdatedDateTimeUtc = DateTime.UtcNow;

                    var sql = PetaPoco.Sql.Builder
                                .Append("UPDATE DeployEnvironmentConfiguration")
                                .Append("SET ParentID=@ParentId, EnumDeployStepParentTypeID=@ParentType, DeployCredentialsId=@DeployCredentialsId, UpdatedDateTimeUtc=@UpdatedDateTimeUtc, UpdatedByUserName=@UpdatedByUserName", item)
                                .Append("WHERE ID=@0", item.Id);
                    db.Execute(sql);
                    SaveMachineList(item);
                    SaveEnvironmentComponentValueList(item);
                }
                foreach (var item in itemsToDelete)
                {
                    item.UpdatedByUserName = _userIdentity.UserName;
                    item.UpdatedDateTimeUtc = DateTime.UtcNow;

                    var sql = PetaPoco.Sql.Builder
                                .Append("DELETE FROM DeployMachineConfigurationValue WHERE DeployMachineID IN (SELECT ID FROM DeployMachine WHERE DeployEnvironmentConfigurationID=@0);", item.Id)
                                .Append("DELETE FROM DeployMachine WHERE DeployEnvironmentConfigurationID=@0;", item.Id)
                                .Append("DELETE FROM DeployEnvironmentConfigurationValue WHERE DeployEnvironmentConfigurationID=@0;", item.Id)
                                .Append("DELETE FROM DeployEnvironmentConfiguration WHERE ID=@0;", item.Id);
                    db.Execute(sql); 
                }
            }
        }

        private void SaveMachineList(DeployEnvironmentConfiguration configuration)
        {
            var itemsToInsert = new List<DeployMachine>();
            var itemsToUpdate = new List<DeployMachine>();
            var itemsToDelete = new List<DeployMachine>();

            var existingList = GetMachineList(configuration.Id);

            foreach(var newItem in configuration.MachineList)
            {
                bool existingItem = existingList.Any(i=>i.Id == newItem.Id);
                if(existingItem)
                {
                    itemsToUpdate.Add(newItem);
                }
                else 
                {
                    itemsToInsert.Add(newItem);
                }
            }
            foreach(var existingItem in existingList)
            {
                var newItem = configuration.MachineList.Any(i=>i.Id == existingItem.Id);
                if(!newItem)
                {
                    itemsToDelete.Add(existingItem);
                }
            }

            using(var db = _sqlConnectionInfo.GetDB())
            {
                foreach(var item in itemsToInsert)
                {
                    var sql = PetaPoco.Sql.Builder
                                .Append("INSERT INTO DeployMachine (ID, DeployProjectID, DeployEnvironmentID, EnvironmentName, DeployEnvironmentConfigurationID, MachineName, CreatedDateTimeUtc, CreatedByUserName, UpdatedDateTimeUtc, UpdatedByUserName)")
                                .Append("VALUES (@Id, @ProjectId, @EnvironmentId, @EnvironmentName, @ParentId, @MachineName, @CreatedDateTimeUtc, @CreatedByUserName, @UpdatedDateTimeUtc, @UpdatedByUserName)", item);
                    db.Execute(sql);
                    this.SaveMachineConfigurationValueList(item);
                }
                foreach(var item in itemsToUpdate)
                {
                    var sql = PetaPoco.Sql.Builder
                            .Append("UPDATE DeployMachine")
                            .Append("SET EnvironmentName=@EnvironmentName, MachineName=@MachineName, UpdatedDateTimeUtc=@UpdatedDateTimeUtc, UpdatedByUserName=@UpdatedByUserName", item)
                            .Append("WHERE ID=@0", item.Id);
                    db.Execute(sql);
                    this.SaveMachineConfigurationValueList(item);
                }
                foreach(var item in itemsToDelete)
                {
                    var sql = PetaPoco.Sql.Builder
                                .Append("DELETE FROM DeployMachineConfigurationValue WHERE DeployMachineID=@0;", item.Id)
                                .Append("DELETE FROM DeployMachine WHERE ID=@0;", item.Id);
                    db.Execute(sql);
                }
            }
        }

        private void SaveMachineConfigurationValueList(DeployMachine machine)
        {
            var itemsToInsert = new List<ConfigurationPair>();
            var itemsToUpdate = new List<ConfigurationPair>();
            var itemsToDelete = new List<ConfigurationPair>();

            var existingList = GetMachineConfigurationValueList(machine.Id);

            foreach (var newKey in machine.ConfigurationValueList.Keys)
            {
                var existingItem = existingList.SingleOrDefault(i => i.ConfigurationName == newKey);
                if (existingItem == null)
                {
                    itemsToInsert.Add(new ConfigurationPair { ConfigurationName = newKey, ConfigurationValue = machine.ConfigurationValueList[newKey] });
                }
                else if (existingItem.ConfigurationValue != machine.ConfigurationValueList[newKey])
                {
                    existingItem.ConfigurationValue = machine.ConfigurationValueList[newKey];
                    itemsToUpdate.Add(existingItem);
                }
            }
            foreach (var existingItem in existingList)
            {
                if (!machine.ConfigurationValueList.ContainsKey(existingItem.ConfigurationName))
                {
                    itemsToDelete.Add(existingItem);
                }
            }

            using (var db = _sqlConnectionInfo.GetDB())
            {
                foreach (var item in itemsToInsert)
                {
                    item.Id = Guid.NewGuid().ToString();
                    var sql = PetaPoco.Sql.Builder
                            .Append("INSERT INTO DeployMachineConfigurationValue (ID, DeployMachineID, ConfigurationName, ConfigurationValue, CreatedByUserName, CreatedDateTimeUtc, UpdatedByUserName, UpdatedDateTimeUtc)")
                            .Append("VALUES (@0, @1, @2, @3, @4, @5, @6, @7)", item.Id, machine.Id, item.ConfigurationName, item.ConfigurationValue, _userIdentity.UserName, DateTime.UtcNow, _userIdentity.UserName, DateTime.UtcNow);
                    db.Execute(sql);
                }
                foreach (var item in itemsToUpdate)
                {
                    var sql = PetaPoco.Sql.Builder
                            .Append("UPDATE DeployMachineConfigurationValue")
                            .Append("SET ConfigurationValue=@0", item.ConfigurationValue)
                            .Append("WHERE ID=@0", item.Id);
                    db.Execute(sql);
                }
                foreach (var item in itemsToDelete)
                {
                    var sql = PetaPoco.Sql.Builder
                            .Append("DELETE FROM DeployMachineConfigurationValue")
                            .Append("WHERE ID=@0", item.Id);
                    db.Execute(sql);
                }
            }
        }

        private List<ConfigurationPair> GetMachineConfigurationValueList(string machineId)
        {
            using (var db = _sqlConnectionInfo.GetDB())
            {
                var sql = PetaPoco.Sql.Builder
                            .Append("SELECT ID, ConfigurationName, ConfigurationValue")
                            .Append("FROM DeployMachineConfigurationValue")
                            .Append("WHERE DeployMachineID=@0", machineId);
                return db.Fetch<ConfigurationPair>(sql).ToList();
            }
        }

        private List<DeployMachine> GetMachineList(string environmentConfigurationID)
        {
            using(var db = _sqlConnectionInfo.GetDB())
            {
                var sql = PetaPoco.Sql.Builder
                                .Append("SELECT ID, DeployProjectID AS ProjectID, DeployEnvironmentID AS EnvironmentID, EnvironmentName, DeployEnvironmentConfigurationID AS ParentID, MachineName, CreatedDateTimeUtc, CreatedByUserName, UpdatedDateTimeUtc, UpdatedByUserName")
                                .Append("FROM DeployMachine")
                                .Append("WHERE DeployEnvironmentConfigurationID=@0", environmentConfigurationID);
                var list = db.Fetch<DeployMachine>(sql).ToList();
                foreach(var item in list)
                {
                    LoadMachineChildren(item);
                }
                return list;
            }
        }

        private void LoadMachineChildren(DeployMachine item)
        {
            item.ConfigurationValueList = this.GetMachineConfigurationValueList(item.Id).ToDictionary(i=>i.ConfigurationName, i=>i.ConfigurationValue);
        }

        private void SaveEnvironmentComponentValueList(DeployEnvironmentConfiguration configuration)
        {
            var itemsToInsert = new List<ConfigurationPair>();
            var itemsToUpdate = new List<ConfigurationPair>();
            var itemsToDelete = new List<ConfigurationPair>();

            var existingList = GetEnvironmentConfigurationValueList(configuration.Id);

            foreach(var newKey in configuration.ConfigurationValueList.Keys)
            {
                var existingItem = existingList.SingleOrDefault(i=>i.ConfigurationName == newKey);
                if(existingItem == null)
                {
                    itemsToInsert.Add(new ConfigurationPair { ConfigurationName=newKey, ConfigurationValue=configuration.ConfigurationValueList[newKey] });
                }
                else if(existingItem.ConfigurationValue != configuration.ConfigurationValueList[newKey])
                {
                    existingItem.ConfigurationValue = configuration.ConfigurationValueList[newKey];
                    itemsToUpdate.Add(existingItem);
                }
            }
            foreach(var existingItem in existingList)
            {
                if(!configuration.ConfigurationValueList.ContainsKey(existingItem.ConfigurationName))
                {
                    itemsToDelete.Add(existingItem);
                }
            }

            using(var db = _sqlConnectionInfo.GetDB())
            {
                foreach(var item in itemsToInsert)
                {
                    item.Id = Guid.NewGuid().ToString();
                    var sql = PetaPoco.Sql.Builder
                            .Append("INSERT INTO DeployEnvironmentConfigurationValue (ID, DeployEnvironmentConfigurationID, ConfigurationName, ConfigurationValue, CreatedByUserName, CreatedDateTimeUtc, UpdatedByUserName, UpdatedDateTimeUtc)")
                            .Append("VALUES (@0, @1, @2, @3, @4, @5, @6, @7)", item.Id, configuration.Id, item.ConfigurationName, item.ConfigurationValue, _userIdentity.UserName, DateTime.UtcNow, _userIdentity.UserName, DateTime.UtcNow);
                    db.Execute(sql);
                }
                foreach(var item in itemsToUpdate)
                {
                    var sql = PetaPoco.Sql.Builder
                            .Append("UPDATE DeployEnvironmentConfigurationValue")
                            .Append("SET ConfigurationValue=@0", item.ConfigurationValue)
                            .Append("WHERE ID=@0", item.Id);
                    db.Execute(sql);
                }
                foreach(var item in itemsToDelete)
                {
                    var sql = PetaPoco.Sql.Builder
                            .Append("DELETE FROM DeployEnvironmentConfigurationValue")
                            .Append("WHERE ID=@0", item.Id);
                    db.Execute(sql);
                }
            }
        }

        private List<DeployEnvironmentConfiguration> GetEnvironmentConfigurationList(string environmentID, EnumDeployStepParentType parentType)
        {
            using(var db = _sqlConnectionInfo.GetDB())
            {
                var sql = GetEnvironmentConfigurationBaseQuery()
                                .Append("WHERE DeployEnvironmentID=@0 AND EnumDeployStepParentTypeID=@1", environmentID, parentType);
                var list = db.Fetch<DeployEnvironmentConfiguration>(sql).ToList();
                foreach(var item in list)
                {
                    LoadEnvironmentConfigurationChildren(item);
                }
                return list;
            }
        }

        private void LoadEnvironmentConfigurationChildren(DeployEnvironmentConfiguration item)
        {
            item.ConfigurationValueList = GetEnvironmentConfigurationValueList(item.Id).ToDictionary(i=>i.ConfigurationName, i=>i.ConfigurationValue);;
            item.MachineList = GetMachineList(item.Id);
        }

        private List<ConfigurationPair> GetEnvironmentConfigurationValueList(string environmentConfigurationID)
        {
            using(var db = _sqlConnectionInfo.GetDB())
            {
                var sql = PetaPoco.Sql.Builder
                            .Append("SELECT ID, ConfigurationName, ConfigurationValue")
                            .Append("FROM DeployEnvironmentConfigurationValue")
                            .Append("WHERE DeployEnvironmentConfigurationID=@0", environmentConfigurationID);
                return db.Fetch<ConfigurationPair>(sql).ToList();
            }
        }

        private PetaPoco.Sql GetEnvironmentConfigurationBaseQuery()
        {
            return PetaPoco.Sql.Builder
                    .Append("SELECT ID, DeployProjectID AS ProjectID, DeployEnvironmentID AS EnvironmentID, ParentID, EnumDeployStepParentTypeID AS ParentType, DeployCredentialsId, CreatedDateTimeUtc, CreatedByUserName, UpdatedDateTimeUtc, UpdatedByUserName")
                    .Append("FROM DeployEnvironmentConfiguration");
        }

        private List<DeployEnvironmentConfiguration> UpdateEnvironmentComponentList(IEnumerable<DeployEnvironmentConfiguration> componentList, string environmentId, string projectId, string environmentName, EnumDeployStepParentType parentType)
        {
            foreach (var component in componentList)
            {
                switch(parentType)
                {
                    case EnumDeployStepParentType.Component:
                        VerifyComponentExists(component.ParentId, projectId);
                        break;
                    case EnumDeployStepParentType.Configuration:
                        VerifyConfigurationExists(component.ParentId, projectId);
                        break;
                    default:
                        throw new UnknownEnumValueException(parentType);
                }
                if (string.IsNullOrEmpty(component.Id))
                {
                    component.Id = Guid.NewGuid().ToString();
                    component.CreatedDateTimeUtc = DateTime.UtcNow;
                    component.CreatedByUserName = _userIdentity.UserName;
                }
                component.EnvironmentId = environmentId;
                component.ProjectId = projectId;
                component.ParentType = parentType;
                component.UpdatedDateTimeUtc = DateTime.UtcNow;
                component.UpdatedByUserName = _userIdentity.UserName;

                if (component.MachineList != null)
                {
                    foreach (var machine in component.MachineList)
                    {
                        if (string.IsNullOrEmpty(machine.Id))
                        {
                            machine.Id = Guid.NewGuid().ToString();
                            machine.CreatedDateTimeUtc = DateTime.UtcNow;
                            machine.CreatedByUserName = _userIdentity.UserName;
                        }
                        machine.ProjectId = projectId;
                        machine.ParentId = component.Id;
                        machine.EnvironmentId = environmentId;
                        machine.EnvironmentName = environmentName;
                        machine.UpdatedDateTimeUtc = DateTime.UtcNow;
                        machine.UpdatedByUserName = _userIdentity.UserName;
                    }
                }
            }
            return componentList.ToList();
        }

        public DeployEnvironment GetEnvironment(string environmentId)
        {
            VerifyEnvironmentExists(environmentId, null);
            using(var db = _sqlConnectionInfo.GetDB())
            {
                var sql = GetEnvironmentBaseQuery().Append("WHERE ID=@0", environmentId);
                var item = db.SingleOrDefault<DeployEnvironment>(sql);
                if(item == null)
                {
                    throw new RecordNotFoundException(typeof(DeployEnvironment), "Id", environmentId);
                }
                LoadEnvironmentChildren(item);
                return item;
            }
        }

        private void LoadEnvironmentChildren(DeployEnvironment item)
        {
            item.ComponentList = GetEnvironmentConfigurationList(item.Id, EnumDeployStepParentType.Component);
            item.ConfigurationList = GetEnvironmentConfigurationList(item.Id, EnumDeployStepParentType.Configuration);
        }

        private PetaPoco.Sql GetEnvironmentBaseQuery()
        {
            return PetaPoco.Sql.Builder
                    .Append("SELECT ID, DeployProjectID AS ProjectID, EnvironmentName, CreatedDateTimeUtc, CreatedByUserName, UpdatedDateTimeUtc, UpdatedByUserName")
                    .Append("FROM DeployEnvironment");
        }

        public DeployEnvironment UpdateEnvironment(string environmentId, string projectId, string environmentName, IEnumerable<DeployEnvironmentConfiguration> componentList, IEnumerable<DeployEnvironmentConfiguration> configurationList)
        {
            if (string.IsNullOrEmpty(environmentName))
            {
                throw new ArgumentNullException("Missing environment name");
            }
            VerifyProjectExists(projectId);

            var item = GetEnvironment(environmentId);
            UpdateEnvironmentComponentList(componentList, environmentId, projectId, environmentName, EnumDeployStepParentType.Component);
            UpdateEnvironmentComponentList(configurationList, environmentId, projectId, environmentName, EnumDeployStepParentType.Configuration);

            using(var db = _sqlConnectionInfo.GetDB())
            {
                var sql = PetaPoco.Sql.Builder
                            .Append("UPDATE DeployEnvironment")
                            .Append("SET EnvironmentName=@0, UpdatedByUserName=@1, UpdatedDateTimeUtc=@2", environmentName, _userIdentity.UserName, DateTime.UtcNow)
                            .Append("WHERE ID=@0", environmentId);
                db.Execute(sql);
            }

            SaveEnvironmentConfigurationList(environmentId, componentList, EnumDeployStepParentType.Component);
            SaveEnvironmentConfigurationList(environmentId, configurationList, EnumDeployStepParentType.Configuration);

            return GetEnvironment(environmentId);
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
