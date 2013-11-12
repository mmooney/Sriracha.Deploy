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
        public IEnumerable<DeployProject> GetProjectList()
        {
            throw new NotImplementedException();
        }

        public DeployProject CreateProject(string projectName, bool usesSharedComponentConfiguration)
        {
            throw new NotImplementedException();
        }

        public DeployProject TryGetProject(string projectId)
        {
            throw new NotImplementedException();
        }

        public DeployProject GetProject(string projectId)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public void DeleteProject(string projectId)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public DeployProjectBranch CreateBranch(string projectId, string branchName)
        {
            throw new NotImplementedException();
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
