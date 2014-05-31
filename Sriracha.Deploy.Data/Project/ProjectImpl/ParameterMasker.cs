using MMDB.Shared;
using Sriracha.Deploy.Data.Deployment;
using Sriracha.Deploy.Data.Dto.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Project.ProjectImpl
{
    public class ParameterMasker : IParameterMasker
    {
        private readonly IDeploymentValidator _deploymentValidator;
        private readonly string MaskValue = "[**SENSITIVE**]";

        public ParameterMasker(IDeploymentValidator deploymentValidator)
        {
            _deploymentValidator = DIHelper.VerifyParameter(deploymentValidator);
        }

        public DeployProject Mask(DeployProject project)
        {
            if(project == null)
            {
                return null;
            }
            var newProject = AutoMapper.Mapper.Map(project, new DeployProject());
            if(project.EnvironmentList != null)
            {
                newProject.EnvironmentList = this.Mask(project, project.EnvironmentList).ToList();
            }
            return newProject;
        }

        public IEnumerable<DeployProject> Mask(IEnumerable<DeployProject> projectList)
        {
            if(projectList == null)
            {
                return null;
            }
            var newProjectList = new List<DeployProject>();
            foreach(var project in projectList)
            {
                var newProject = this.Mask(project);
                newProjectList.Add(project);
            }
            return newProjectList;
        }

        public IEnumerable<DeployEnvironment> Mask(DeployProject project, IEnumerable<DeployEnvironment> environmentList)
        {
            if(environmentList == null)
            {
                return null;
            }
            if (project == null)
            {
                throw new ArgumentNullException("Missing project");
            }
            var newEnvironmentList = new List<DeployEnvironment>();
            foreach(var environment in environmentList)
            {
                var newEnvironment = this.Mask(project, environment);
                newEnvironmentList.Add(newEnvironment);
            }
            return newEnvironmentList;
        }

        public DeployEnvironment Mask(DeployProject project, DeployEnvironment environment)
        {
            if(environment == null)
            {
                return null;
            }
            if (project == null)
            {
                throw new ArgumentNullException("Missing project");
            }
            var newEnvironment = AutoMapper.Mapper.Map(environment, new DeployEnvironment());
            if (environment.ConfigurationList != null)
            {
                newEnvironment.ConfigurationList = this.Mask(project, environment.ConfigurationList).ToList();
            }
            if(environment.ComponentList != null)
            {
                newEnvironment.ComponentList = this.Mask(project, environment.ComponentList).ToList();
            }
            return newEnvironment;
        }

        public IEnumerable<DeployEnvironmentConfiguration> Mask(DeployProject project, IEnumerable<DeployEnvironmentConfiguration> environmentConfigurationList)
        {
            if (environmentConfigurationList == null)
            {
                return null;
            }
            if (project == null)
            {
                throw new ArgumentNullException("Missing project");
            }
            var newConfigurationList = new List<DeployEnvironmentConfiguration>();
            foreach(var configuration in environmentConfigurationList)
            {
                var newConfiguration = this.Mask(project, configuration);
                newConfigurationList.Add(newConfiguration);
            }
            return newConfigurationList;
        }

        public DeployEnvironmentConfiguration Mask(DeployProject project, DeployEnvironmentConfiguration environmentConfiguration)
        {
            if(environmentConfiguration == null)
            {
                return null;
            }
            if (project == null)
            {
                throw new ArgumentNullException("Missing project");
            }
            var newEnvironmentConfiguration = AutoMapper.Mapper.Map(environmentConfiguration, new DeployEnvironmentConfiguration());
            if (newEnvironmentConfiguration.ConfigurationValueList != null && newEnvironmentConfiguration.ConfigurationValueList.Any())
            {
                List<DeployStep> deploymentStepList = new List<DeployStep>();
                switch (newEnvironmentConfiguration.ParentType)
                {
                    case EnumDeployStepParentType.Component:
                        var component = project.ComponentList.FirstOrDefault(i => i.Id == newEnvironmentConfiguration.ParentId);
                        if(component != null)
                        {
                            deploymentStepList = component.DeploymentStepList;
                        }
                        break;
                    case EnumDeployStepParentType.Configuration:
                        var configuration = project.ConfigurationList.FirstOrDefault(i => i.Id == newEnvironmentConfiguration.ParentId);
                        if(configuration != null)
                        {
                            deploymentStepList = configuration.DeploymentStepList;
                        }
                        break;
                }
                var definition = _deploymentValidator.GetComponentConfigurationDefinition(deploymentStepList);
                if(definition.EnvironmentTaskParameterList != null)
                {
                    foreach(var parameterDefinition in definition.EnvironmentTaskParameterList.Where(i=>i.Sensitive))
                    {
                        if (newEnvironmentConfiguration.ConfigurationValueList.ContainsKey(parameterDefinition.FieldName))
                        {
                            newEnvironmentConfiguration.ConfigurationValueList[parameterDefinition.FieldName] = this.MaskValue;
                        }
                    }
                }
                if(definition.MachineTaskParameterList != null && newEnvironmentConfiguration.MachineList != null)
                {
                    foreach (var parameterDefinition in definition.MachineTaskParameterList.Where(i => i.Sensitive))
                    {
                        foreach(var newMachineConfiguration in newEnvironmentConfiguration.MachineList)
                        {
                            if (newMachineConfiguration.ConfigurationValueList.ContainsKey(parameterDefinition.FieldName))
                            {
                                newMachineConfiguration.ConfigurationValueList[parameterDefinition.FieldName] = this.MaskValue;
                            }
                        }
                    }
                }
            }
            return newEnvironmentConfiguration;
        }

        public DeployEnvironment Unmask(DeployProject project, DeployEnvironment environment, DeployEnvironment originalEnvironment)
        {
            if(environment == null)
            {
                return null;
            }
            if (project == null)
            {
                throw new ArgumentNullException("Missing project");
            }
            var newEnvironment = AutoMapper.Mapper.Map(environment, new DeployEnvironment());
            if(originalEnvironment != null)
            {
                if (environment.ConfigurationList != null)
                {
                    newEnvironment.ConfigurationList = this.Unmask(project, environment.ConfigurationList, originalEnvironment).ToList();
                }
                if (environment.ComponentList != null)
                {
                    newEnvironment.ComponentList = this.Unmask(project, environment.ComponentList, originalEnvironment).ToList();
                }
            }
            return newEnvironment;
        }

        public IEnumerable<DeployEnvironmentConfiguration> Unmask(DeployProject project, IEnumerable<DeployEnvironmentConfiguration> environmentConfigurationList, DeployEnvironment originalEnvironment)
        {
            if (environmentConfigurationList == null)
            {
                return null;
            }
            if (project == null)
            {
                throw new ArgumentNullException("Missing project");
            }
            var newConfigurationList = new List<DeployEnvironmentConfiguration>();
            foreach (var configuration in environmentConfigurationList)
            {
                var newConfiguration = this.Unmask(project, configuration, originalEnvironment);
                newConfigurationList.Add(newConfiguration);
            }
            return newConfigurationList;
        }

        public DeployEnvironmentConfiguration Unmask(DeployProject project, DeployEnvironmentConfiguration environmentConfiguration, DeployEnvironment originalEnvironment)
        {
            if(environmentConfiguration == null)
            {
                return null;
            }
            if(project == null)
            {
                throw new ArgumentNullException("Missing project");
            }
            var newEnvironmentConfiguration = AutoMapper.Mapper.Map(environmentConfiguration, new DeployEnvironmentConfiguration());
            if (string.IsNullOrEmpty(environmentConfiguration.Id))
            {
                //New item being created, don't need to unmask
                return newEnvironmentConfiguration;
            }
            DeployEnvironmentConfiguration originalEnviromentConfiguration;
            switch(environmentConfiguration.ParentType)
            {
                case EnumDeployStepParentType.Configuration:
                    originalEnviromentConfiguration = originalEnvironment.ConfigurationList.FirstOrDefault(i => i.ParentType == environmentConfiguration.ParentType && i.ParentId == environmentConfiguration.ParentId);
                    break;
                case EnumDeployStepParentType.Component:
                    originalEnviromentConfiguration = originalEnvironment.ComponentList.FirstOrDefault(i => i.ParentType == environmentConfiguration.ParentType && i.ParentId == environmentConfiguration.ParentId);
                    break;
                default:
                    throw new UnknownEnumValueException(environmentConfiguration.ParentType);
            }
            if(originalEnviromentConfiguration != null)
            {
                List<DeployStep> deploymentStepList = new List<DeployStep>();
                switch (newEnvironmentConfiguration.ParentType)
                {
                    case EnumDeployStepParentType.Component:
                        var component = project.ComponentList.FirstOrDefault(i => i.Id == newEnvironmentConfiguration.ParentId);
                        if (component != null)
                        {
                            deploymentStepList = component.DeploymentStepList;
                        }
                        break;
                    case EnumDeployStepParentType.Configuration:
                        var configuration = project.ConfigurationList.FirstOrDefault(i => i.Id == newEnvironmentConfiguration.ParentId);
                        if (configuration != null)
                        {
                            deploymentStepList = configuration.DeploymentStepList;
                        }
                        break;
                    default:
                        throw new UnknownEnumValueException(newEnvironmentConfiguration.ParentType);
                }
                var definition = _deploymentValidator.GetComponentConfigurationDefinition(deploymentStepList);
                if (definition.EnvironmentTaskParameterList != null)
                {
                    foreach (var parameterDefinition in definition.EnvironmentTaskParameterList.Where(i => i.Sensitive))
                    {
                        if (newEnvironmentConfiguration.ConfigurationValueList.ContainsKey(parameterDefinition.FieldName)
                                && originalEnviromentConfiguration.ConfigurationValueList.ContainsKey(parameterDefinition.FieldName))
                        {
                            if(newEnvironmentConfiguration.ConfigurationValueList[parameterDefinition.FieldName] == this.MaskValue)
                            {
                                newEnvironmentConfiguration.ConfigurationValueList[parameterDefinition.FieldName] = originalEnviromentConfiguration.ConfigurationValueList[parameterDefinition.FieldName];
                            }
                        }
                    }
                }
                if (definition.MachineTaskParameterList != null && newEnvironmentConfiguration.MachineList != null && originalEnviromentConfiguration.MachineList != null) 
                {
                    foreach (var parameterDefinition in definition.MachineTaskParameterList.Where(i => i.Sensitive))
                    {
                        foreach(var newMachineConfiguration in newEnvironmentConfiguration.MachineList)
                        {
                            var originalMachineConfiguration = originalEnviromentConfiguration.MachineList.FirstOrDefault(i=>i.Id == newMachineConfiguration.Id);
                            if(originalMachineConfiguration != null)
                            {
                                if(newMachineConfiguration.ConfigurationValueList.ContainsKey(parameterDefinition.FieldName)
                                        && originalMachineConfiguration.ConfigurationValueList.ContainsKey(parameterDefinition.FieldName))
                                {
                                    if (newMachineConfiguration.ConfigurationValueList[parameterDefinition.FieldName] == this.MaskValue)
                                    {
                                        newMachineConfiguration.ConfigurationValueList[parameterDefinition.FieldName] = originalMachineConfiguration.ConfigurationValueList[parameterDefinition.FieldName];
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return newEnvironmentConfiguration;
        }
    }
}
