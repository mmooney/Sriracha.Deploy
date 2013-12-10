using Sriracha.Deploy.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Deployment.Offline.OfflineImpl
{
    public class OfflineProjectRepository : IProjectRepository
    {
        public IEnumerable<Dto.Project.DeployProject> GetProjectList()
        {
            throw new NotImplementedException();
        }

        public Dto.Project.DeployProject CreateProject(string projectName, bool usesSharedComponentConfiguration)
        {
            throw new NotImplementedException();
        }

        public Dto.Project.DeployProject TryGetProject(string projectId)
        {
            throw new NotImplementedException();
        }

        public Dto.Project.DeployProject GetProject(string projectId)
        {
            throw new NotImplementedException();
        }

        public Dto.Project.DeployProject GetOrCreateProject(string projectIdOrName)
        {
            throw new NotImplementedException();
        }

        public Dto.Project.DeployProject TryGetProjectByName(string projectName)
        {
            throw new NotImplementedException();
        }

        public Dto.Project.DeployProject GetProjectByName(string projectName)
        {
            throw new NotImplementedException();
        }

        public Dto.Project.DeployProject UpdateProject(string projectId, string projectName, bool usesSharedComponentConfiguration)
        {
            throw new NotImplementedException();
        }

        public void DeleteProject(string projectId)
        {
            throw new NotImplementedException();
        }

        public List<Dto.Project.DeployConfiguration> GetConfigurationList(string projectId)
        {
            throw new NotImplementedException();
        }

        public Dto.Project.DeployConfiguration CreateConfiguration(string projectId, string configurationName, EnumDeploymentIsolationType isolationType)
        {
            throw new NotImplementedException();
        }

        public Dto.Project.DeployConfiguration GetConfiguration(string configurationId, string projectId)
        {
            throw new NotImplementedException();
        }

        public Dto.Project.DeployConfiguration TryGetConfiguration(string configurationId, string projectId)
        {
            throw new NotImplementedException();
        }

        public Dto.Project.DeployConfiguration UpdateConfiguration(string configurationId, string projectId, string configurationName, EnumDeploymentIsolationType isolationType)
        {
            throw new NotImplementedException();
        }

        public void DeleteConfiguration(string configurationId, string projectId)
        {
            throw new NotImplementedException();
        }

        public List<Dto.Project.DeployComponent> GetComponentList(string projectId)
        {
            throw new NotImplementedException();
        }

        public Dto.Project.DeployComponent CreateComponent(string projectId, string componentName, bool useConfigurationGroup, string configurationId, EnumDeploymentIsolationType isolationType)
        {
            throw new NotImplementedException();
        }

        public Dto.Project.DeployComponent GetComponent(string componentId, string projectId)
        {
            throw new NotImplementedException();
        }

        public Dto.Project.DeployComponent TryGetComponent(string componentId, string projectId)
        {
            throw new NotImplementedException();
        }

        public Dto.Project.DeployComponent GetOrCreateComponent(string projectId, string componentIdOrName)
        {
            throw new NotImplementedException();
        }

        public Dto.Project.DeployComponent UpdateComponent(string componentId, string projectId, string componentName, bool useConfigurationGroup, string configurationId, EnumDeploymentIsolationType isolationType)
        {
            throw new NotImplementedException();
        }

        public void DeleteComponent(string projectId, string componentId)
        {
            throw new NotImplementedException();
        }

        public List<Dto.Project.DeployStep> GetComponentDeploymentStepList(string componentId, string projectId)
        {
            throw new NotImplementedException();
        }

        public List<Dto.Project.DeployStep> GetConfigurationDeploymentStepList(string configurationId, string projectId)
        {
            throw new NotImplementedException();
        }

        public Dto.Project.DeployStep CreateComponentDeploymentStep(string projectId, string componentId, string stepName, string taskTypeName, string taskOptionsJson, string sharedDeploymentStepId)
        {
            throw new NotImplementedException();
        }

        public Dto.Project.DeployStep CreateConfigurationDeploymentStep(string projectId, string configurationId, string stepName, string taskTypeName, string taskOptionsJson)
        {
            throw new NotImplementedException();
        }

        public Dto.Project.DeployStep GetComponentDeploymentStep(string deploymentStepId, string projectId)
        {
            throw new NotImplementedException();
        }

        public Dto.Project.DeployStep GetConfigurationDeploymentStep(string deploymentStepId, string projectId)
        {
            throw new NotImplementedException();
        }

        public Dto.Project.DeployStep UpdateComponentDeploymentStep(string deploymentStepId, string projectId, string componentId, string stepName, string taskTypeName, string taskOptionsJson, string sharedDeploymentStepId)
        {
            throw new NotImplementedException();
        }

        public Dto.Project.DeployStep UpdateConfigurationDeploymentStep(string deploymentStepId, string projectId, string configurationId, string stepName, string taskTypeName, string taskOptionsJson)
        {
            throw new NotImplementedException();
        }

        public void DeleteComponentDeploymentStep(string deploymentStepId, string projectId)
        {
            throw new NotImplementedException();
        }

        public void DeleteConfigurationDeploymentStep(string deploymentStepId, string projectId)
        {
            throw new NotImplementedException();
        }

        public List<Dto.Project.DeployProjectBranch> GetBranchList(string projectId)
        {
            throw new NotImplementedException();
        }

        public Dto.Project.DeployProjectBranch CreateBranch(string projectId, string branchName)
        {
            throw new NotImplementedException();
        }

        public Dto.Project.DeployProjectBranch GetBranch(string branchId, string projectId)
        {
            throw new NotImplementedException();
        }

        public Dto.Project.DeployProjectBranch TryGetBranch(string branchId, string projectId)
        {
            throw new NotImplementedException();
        }

        public Dto.Project.DeployProjectBranch GetBranchByName(string projectId, string branchName)
        {
            throw new NotImplementedException();
        }

        public Dto.Project.DeployProjectBranch TryGetBranchByName(string projectId, string branchName)
        {
            throw new NotImplementedException();
        }

        public Dto.Project.DeployProjectBranch GetOrCreateBranch(string projectId, string branchIdOrName)
        {
            throw new NotImplementedException();
        }

        public Dto.Project.DeployProjectBranch UpdateBranch(string branchId, string projectId, string branchName)
        {
            throw new NotImplementedException();
        }

        public void DeleteBranch(string branchId, string projectId)
        {
            throw new NotImplementedException();
        }

        public List<Dto.Project.DeployEnvironment> GetEnvironmentList(string projectId)
        {
            throw new NotImplementedException();
        }

        public Dto.Project.DeployEnvironment CreateEnvironment(string projectId, string enviornmentName, IEnumerable<Dto.Project.DeployEnvironmentConfiguration> componentList, IEnumerable<Dto.Project.DeployEnvironmentConfiguration> configurationList)
        {
            throw new NotImplementedException();
        }

        public Dto.Project.DeployEnvironment GetEnvironment(string environmentId, string projectId)
        {
            throw new NotImplementedException();
        }

        public Dto.Project.DeployEnvironment UpdateEnvironment(string environmentId, string projectId, string environmentName, IEnumerable<Dto.Project.DeployEnvironmentConfiguration> componentList, IEnumerable<Dto.Project.DeployEnvironmentConfiguration> configurationList)
        {
            throw new NotImplementedException();
        }

        public void DeleteEnvironment(string environmentId, string projectId)
        {
            throw new NotImplementedException();
        }
    }
}
