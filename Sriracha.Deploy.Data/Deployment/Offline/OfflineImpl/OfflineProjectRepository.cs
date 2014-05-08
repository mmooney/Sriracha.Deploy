using MMDB.Shared;
using Sriracha.Deploy.Data.Dto.Project;
using Sriracha.Deploy.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Deployment.Offline.OfflineImpl
{
    public class OfflineProjectRepository : IProjectRepository
    {
        private IOfflineDataProvider _offlineDataProvider;

        public OfflineProjectRepository(IOfflineDataProvider offlineDataProvider)
        {
            _offlineDataProvider = DIHelper.VerifyParameter(offlineDataProvider);
        }

        public IEnumerable<Dto.Project.DeployProject> GetProjectList()
        {
            throw new NotSupportedException();
        }

        public Dto.Project.DeployProject CreateProject(string projectName, bool usesSharedComponentConfiguration)
        {
            throw new NotSupportedException();
        }

        public DeployProject TryGetProject(string projectId)
        {
            return _offlineDataProvider.TryGetProject(projectId);
        }

        public DeployProject GetProject(string projectId)
        {
            var project = _offlineDataProvider.TryGetProject(projectId);
            if(project == null)
            {
                throw new RecordNotFoundException(typeof(DeployProject), "Id", projectId);
            }
            return project;
        }

        public DeployProject GetOrCreateProject(string projectIdOrName)
        {
            throw new NotSupportedException();
        }

        public DeployProject TryGetProjectByName(string projectName)
        {
            throw new NotSupportedException();
        }

        public DeployProject GetProjectByName(string projectName)
        {
            throw new NotSupportedException();
        }

        public DeployProject UpdateProject(string projectId, string projectName, bool usesSharedComponentConfiguration)
        {
            throw new NotSupportedException();
        }

        public void DeleteProject(string projectId)
        {
            throw new NotSupportedException();
        }

        public List<DeployConfiguration> GetConfigurationList(string projectId)
        {
            var project = GetProject(projectId);
            return project.ConfigurationList;
        }

        public DeployConfiguration CreateConfiguration(string projectId, string configurationName, EnumDeploymentIsolationType isolationType)
        {
            throw new NotSupportedException();
        }

        public DeployConfiguration GetConfiguration(string configurationId, string projectId)
        {
            var configuration = TryGetConfiguration(configurationId, projectId);
            if(configuration == null)
            {
                throw new RecordNotFoundException(typeof(DeployConfiguration), "Id", configurationId);
            }
            return configuration;
        }

        public DeployConfiguration TryGetConfiguration(string configurationId, string projectId)
        {
            var project = GetProject(projectId);
            return project.ConfigurationList.SingleOrDefault(i=>i.Id == configurationId);
        }

        public DeployConfiguration UpdateConfiguration(string configurationId, string projectId, string configurationName, EnumDeploymentIsolationType isolationType)
        {
            throw new NotSupportedException();
        }

        public void DeleteConfiguration(string configurationId, string projectId)
        {
            throw new NotSupportedException();
        }

        public List<DeployComponent> GetComponentList(string projectId)
        {
            var project = GetProject(projectId);
            return project.ComponentList;
        }

        public DeployComponent CreateComponent(string projectId, string componentName, bool useConfigurationGroup, string configurationId, EnumDeploymentIsolationType isolationType)
        {
            throw new NotSupportedException();
        }

        public DeployComponent GetComponent(string componentId, string projectId)
        {
            var component = TryGetComponent(componentId, projectId);
            if(component == null)
            {
                throw new RecordNotFoundException(typeof(DeployComponent), "Id", componentId);
            }
            return component;
        }

        public DeployComponent TryGetComponent(string componentId, string projectId)
        {
            var project = GetProject(projectId);
            return project.ComponentList.Single(i=>i.Id == componentId);
        }

        public DeployComponent GetOrCreateComponent(string projectId, string componentIdOrName)
        {
            throw new NotSupportedException();
        }

        public DeployComponent UpdateComponent(string componentId, string projectId, string componentName, bool useConfigurationGroup, string configurationId, EnumDeploymentIsolationType isolationType)
        {
            throw new NotSupportedException();
        }

        public void DeleteComponent(string projectId, string componentId)
        {
            throw new NotSupportedException();
        }

        public List<DeployStep> GetComponentDeploymentStepList(string componentId, string projectId)
        {
            var component = GetComponent(componentId, projectId);
            return component.DeploymentStepList;
        }

        public List<DeployStep> GetConfigurationDeploymentStepList(string configurationId, string projectId)
        {
            var configuration = GetConfiguration(configurationId, projectId);
            return configuration.DeploymentStepList;
        }

        public DeployStep CreateComponentDeploymentStep(string projectId, string componentId, string stepName, string taskTypeName, string taskOptionsJson, string sharedDeploymentStepId)
        {
            throw new NotSupportedException();
        }

        public DeployStep CreateConfigurationDeploymentStep(string projectId, string configurationId, string stepName, string taskTypeName, string taskOptionsJson)
        {
            throw new NotSupportedException();
        }

        public DeployStep GetComponentDeploymentStep(string deploymentStepId, string projectId)
        {
            var project = GetProject(projectId);
            var component = project.ComponentList.SingleOrDefault(i => i.DeploymentStepList.Any(j => j.Id == deploymentStepId));
            if (component == null)
            {
                throw new RecordNotFoundException(typeof(DeployStep), "Id", deploymentStepId);
            }
            return component.DeploymentStepList.FirstOrDefault(i => i.Id == deploymentStepId);
        }

        public DeployStep GetConfigurationDeploymentStep(string deploymentStepId, string projectId)
        {
            var project = GetProject(projectId);
            var configuration = project.ConfigurationList.SingleOrDefault(i=>i.DeploymentStepList.Any(j=>j.Id == deploymentStepId));
            if(configuration == null)
            {
                throw new RecordNotFoundException(typeof(DeployStep), "Id", deploymentStepId);
            }
            return configuration.DeploymentStepList.FirstOrDefault(i=>i.Id == deploymentStepId);
        }

        public DeployStep UpdateComponentDeploymentStep(string deploymentStepId, string projectId, string componentId, string stepName, string taskTypeName, string taskOptionsJson, string sharedDeploymentStepId, int? orderNumber=null)
        {
            throw new NotSupportedException();
        }

        public DeployStep UpdateConfigurationDeploymentStep(string deploymentStepId, string projectId, string configurationId, string stepName, string taskTypeName, string taskOptionsJson, int? orderNumber=null)
        {
            throw new NotSupportedException();
        }

        public void DeleteComponentDeploymentStep(string deploymentStepId, string projectId)
        {
            throw new NotSupportedException();
        }

        public void DeleteConfigurationDeploymentStep(string deploymentStepId, string projectId)
        {
            throw new NotSupportedException();
        }

        public List<DeployProjectBranch> GetBranchList(string projectId)
        {
            var project = GetProject(projectId);
            return project.BranchList;
        }

        public DeployProjectBranch CreateBranch(string projectId, string branchName)
        {
            throw new NotSupportedException();
        }

        public DeployProjectBranch GetBranch(string branchId, string projectId)
        {
            var branch = TryGetBranch(branchId, projectId);
            if(branch == null)
            {
                throw new RecordNotFoundException(typeof(DeployProjectBranch), "Id", branchId);
            }
            return branch;
        }

        public DeployProjectBranch TryGetBranch(string branchId, string projectId)
        {
            var project = GetProject(projectId);
            return project.BranchList.SingleOrDefault(i=>i.Id == branchId);
        }

        public DeployProjectBranch GetBranchByName(string projectId, string branchName)
        {
            throw new NotSupportedException();
        }

        public DeployProjectBranch TryGetBranchByName(string projectId, string branchName)
        {
            throw new NotSupportedException();
        }

        public DeployProjectBranch GetOrCreateBranch(string projectId, string branchIdOrName)
        {
            throw new NotSupportedException();
        }

        public DeployProjectBranch UpdateBranch(string branchId, string projectId, string branchName)
        {
            throw new NotSupportedException();
        }

        public void DeleteBranch(string branchId, string projectId)
        {
            throw new NotSupportedException();
        }

        public List<DeployEnvironment> GetEnvironmentList(string projectId)
        {
            var project = GetProject(projectId);
            return project.EnvironmentList;
        }

        public DeployEnvironment CreateEnvironment(string projectId, string enviornmentName, IEnumerable<DeployEnvironmentConfiguration> componentList, IEnumerable<DeployEnvironmentConfiguration> configurationList)
        {
            throw new NotSupportedException();
        }

        public DeployEnvironment GetEnvironment(string environmentId, string projectId)
        {
            var project = GetProject(projectId);
            var environment = project.EnvironmentList.SingleOrDefault(i=>i.Id == environmentId);
            if(environment == null)
            {
                throw new RecordNotFoundException(typeof(DeployEnvironment), "Id", environmentId);
            }
            return environment;
        }

        public DeployEnvironment UpdateEnvironment(string environmentId, string projectId, string environmentName, IEnumerable<DeployEnvironmentConfiguration> componentList, IEnumerable<DeployEnvironmentConfiguration> configurationList)
        {
            throw new NotSupportedException();
        }

        public void DeleteEnvironment(string environmentId, string projectId)
        {
            throw new NotSupportedException();
        }
    }
}
