using Sriracha.Deploy.Data.Dto.Build;
using Sriracha.Deploy.Data.Dto.Project;
using Sriracha.Deploy.Data.Dto.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Tasks
{
    public class TaskExecutionContext<TaskDefinitionType, TaskOptionsType> where TaskDefinitionType:IDeployTaskDefinition
    {
        public string DeployStateId { get; private set; }
        public IDeployTaskStatusManager StatusManager { get; private set; }
        public TaskDefinitionType TaskDefinition { get; private set; }
        public DeployComponent Component { get; private set; }
        public DeployEnvironmentConfiguration EnvironmentComponent { get; private set; }
        public DeployMachine Machine { get; private set; }
        public DeployBuild Build { get; private set; }
        public RuntimeSystemSettings SystemSettings { get; private set; }
        public TaskDefinitionValidationResult ValidationResult { get; private set; }
        public TaskOptionsType FormattedOptions { get; private set; }
        public TaskOptionsType MaskedFormattedOptions { get; private set; }

        public TaskExecutionContext(string deployStateId, IDeployTaskStatusManager statusManager, TaskDefinitionType taskDefinition, 
                                    DeployComponent component, DeployEnvironmentConfiguration environmentComponent, DeployMachine machine, 
                                    DeployBuild build, RuntimeSystemSettings runtimeSystemSettings, TaskDefinitionValidationResult validationResult, 
                                    TaskOptionsType formattedTaskOptions, TaskOptionsType maskedFormattedTaskOptions)
        {
            this.DeployStateId = deployStateId;
            this.StatusManager = statusManager;
            this.TaskDefinition = taskDefinition;
            this.Component = component;
            this.EnvironmentComponent = environmentComponent;
            this.Machine = machine;
            this.Build = build;
            this.SystemSettings = runtimeSystemSettings;
            this.ValidationResult = validationResult;
            this.FormattedOptions = formattedTaskOptions;
            this.MaskedFormattedOptions = maskedFormattedTaskOptions;
        }

        public void Debug(string message, params object[] args)
        {
            if (args != null && args.Length > 0)
            {
                message = string.Format(message, args);
            }
            this.StatusManager.Debug(this.DeployStateId, message);
        }

        public void Info(string message, params object[] args)
        {
            if (args != null && args.Length > 0)
            {
                message = string.Format(message, args);
            }
            this.StatusManager.Info(this.DeployStateId, message);
        }

        public void Warn(string message, params object[] args)
        {
            if (args != null && args.Length > 0)
            {
                message = string.Format(message, args);
            }
            this.StatusManager.Warn(this.DeployStateId, message);
        }

        public void Error(string message, params object[] args)
        {
            if (args != null && args.Length > 0)
            {
                message = string.Format(message, args);
            }
            this.StatusManager.Error(this.DeployStateId, message);
        }

        public void Error(string message, Exception err)
        {
            this.StatusManager.Error(this.DeployStateId, err);
        }

        public DeployTaskExecutionResult BuildResult()
        {
            return this.StatusManager.BuildResult();
        }

    }
}
