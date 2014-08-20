using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Build;
using Sriracha.Deploy.Data.Dto.Project;
using Sriracha.Deploy.Data.Dto.Build;
using Sriracha.Deploy.Data.Deployment;
using Sriracha.Deploy.Data.Dto.Validation;
using Newtonsoft.Json;
using System.Collections;

namespace Sriracha.Deploy.Data.Tasks
{
    public abstract class BaseDeployTaskExecutor<TaskDefinition, TaskDefinitionOptions> : IDeployTaskExecutor 
		where TaskDefinition: IDeployTaskDefinition
	{
        private const string ValueMask = "*****";
        private readonly IParameterEvaluator _parameterEvaluator;
        private readonly IDeploymentValidator _validator;

		public BaseDeployTaskExecutor(IParameterEvaluator paramterEvaluator, IDeploymentValidator validator)
		{
			_parameterEvaluator = DIHelper.VerifyParameter(paramterEvaluator);
            _validator = DIHelper.VerifyParameter(validator);
		}

		public DeployTaskExecutionResult Execute(string deployStateId, IDeployTaskStatusManager statusManager, IDeployTaskDefinition definition, DeployComponent component, DeployEnvironmentConfiguration environmentComponent, DeployMachine machine, DeployBuild build, RuntimeSystemSettings runtimeSystemSettings)
		{
			if(definition == null)
			{
				throw new ArgumentNullException("Missing definition parameter");
			}
			if(!(definition is TaskDefinition))
			{
				throw new ArgumentException(string.Format("Task definition must be {0}, found {1}", typeof(TaskDefinition).FullName, definition.GetType().FullName));
			}
			var typedDefinition = (TaskDefinition)definition;
            var context = this.GetTaskExecutionContext(deployStateId, statusManager, (TaskDefinition)definition, component, environmentComponent, machine, build, runtimeSystemSettings);
			return this.InternalExecute(context);
		}

		protected string GetBuildParameterValue(string parameterName, DeployBuild build)
		{
			return _parameterEvaluator.EvaluateBuildParameter(parameterName, build);
		}

		protected string GetDeployParameterValue(string parameterName, RuntimeSystemSettings runtimeSystemSettings, DeployMachine machine, DeployComponent component)
		{
			return _parameterEvaluator.EvaluateDeployParameter(parameterName, runtimeSystemSettings, machine, component);
		}

        protected TaskExecutionContext<TaskDefinition, TaskDefinitionOptions> GetTaskExecutionContext(string deployStateId, IDeployTaskStatusManager statusManager, TaskDefinition taskDefinition, DeployComponent component, DeployEnvironmentConfiguration environmentComponent, DeployMachine machine, DeployBuild build, RuntimeSystemSettings runtimeSystemSettings)
        {
            var validationResult = _validator.ValidateMachineTaskDefinition(taskDefinition, environmentComponent, machine);
            if (validationResult.Status != EnumRuntimeValidationStatus.Success)
            {
                throw new InvalidOperationException("Validation not complete:" + Environment.NewLine + JsonConvert.SerializeObject(validationResult));
            }
            var machineResult = validationResult.MachineResultList[machine.Id];

            var formattedOptions = AutoMapper.Mapper.Map<TaskDefinitionOptions>((TaskDefinitionOptions)taskDefinition.DeployTaskOptions);
            this.FormatOptions(formattedOptions, validationResult, build, component, machine, runtimeSystemSettings, false);

            var maskedFormattedOptions = AutoMapper.Mapper.Map<TaskDefinitionOptions>((TaskDefinitionOptions)taskDefinition.DeployTaskOptions);
            this.FormatOptions(maskedFormattedOptions, validationResult, build, component, machine, runtimeSystemSettings, true);

            var returnValue = new TaskExecutionContext<TaskDefinition, TaskDefinitionOptions>
            (
                deployStateId, statusManager, taskDefinition, component, environmentComponent, machine, build,
                runtimeSystemSettings, validationResult, formattedOptions, maskedFormattedOptions
            );
            return returnValue;
        }

        private void FormatOptions(object formattedOptions, TaskDefinitionValidationResult validationResult, DeployBuild build, DeployComponent component, DeployMachine machine, RuntimeSystemSettings runtimeSystemSettings, bool masked)
        {
            var machineResult = validationResult.MachineResultList[machine.Id];
            foreach (var propInfo in formattedOptions.GetType().GetProperties())
            {
                if (propInfo.PropertyType == typeof(string))
                {
                    string rawValue = (string)propInfo.GetValue(formattedOptions, null);
                    if (string.IsNullOrEmpty(rawValue))
                    {
                        propInfo.SetValue(formattedOptions, rawValue, null);
                    }
                    else
                    {
                        string formattedValue = this.ReplaceParameters(rawValue, validationResult.EnvironmentResultList, machineResult, validationResult.BuildParameterList, validationResult.DeployParameterList, build, runtimeSystemSettings, machine, component, masked);
                        propInfo.SetValue(formattedOptions, formattedValue, null);
                    }
                }
                else if (propInfo.PropertyType.IsClass)
                {
                    var childObject = propInfo.GetValue(formattedOptions, null);
                    if (childObject != null)
                    {
                        if(typeof(IEnumerable).IsAssignableFrom(childObject.GetType()))
                        {
                            foreach(var arrayItem in (IEnumerable)childObject)
                            {
                                this.FormatOptions(arrayItem, validationResult, build, component, machine, runtimeSystemSettings, masked);
                            }
                        }
                        else 
                        {
                            this.FormatOptions(childObject, validationResult, build, component, machine, runtimeSystemSettings, masked);
                        }
                    }
                }
            }
        }

        private string ReplaceParameters(string format, List<TaskDefinitionValidationResult.TaskDefinitionValidationResultItem> environmentValues, List<TaskDefinitionValidationResult.TaskDefinitionValidationResultItem> machineValues, List<TaskParameter> buildParameters, List<TaskParameter> deployParameters, DeployBuild build, RuntimeSystemSettings runtimeSystemSettings, DeployMachine machine, DeployComponent component, bool masked)
        {
            string returnValue = format;
            foreach (var item in environmentValues)
            {
                returnValue = ReplaceParameter(returnValue, "env", item.FieldName, item.Sensitive, item.FieldValue, masked); 
            }
            foreach (var item in machineValues)
            {
                returnValue = ReplaceParameter(returnValue, "machine", item.FieldName, item.Sensitive, item.FieldValue, masked);
            }
            foreach (var item in buildParameters)
            {
                string value = this.GetBuildParameterValue(item.FieldName, build);
                returnValue = ReplaceParameter(returnValue, "build", item.FieldName, item.Sensitive, value, masked);
            }
            foreach (var item in deployParameters)
            {
                string value = this.GetDeployParameterValue(item.FieldName, runtimeSystemSettings, machine, component);
                returnValue = ReplaceParameter(returnValue, "deploy", item.FieldName, item.Sensitive, value, masked);
            }
            return returnValue;
        }

        private string ReplaceParameter(string input, string prefix, string fieldName, bool sensitive, string fieldValue, bool masked)
        {
            string value;
            if (masked && sensitive)
            {
                value = ValueMask;
            }
            else
            {
                value = fieldValue;
            }
            return _parameterEvaluator.ReplaceParameter(input, prefix, fieldName, value);
        }

        //protected abstract DeployTaskExecutionResult InternalExecute(string deployStateId, IDeployTaskStatusManager statusManager, TaskDefinition definition, DeployComponent component, DeployEnvironmentConfiguration environmentComponent, DeployMachine machine, DeployBuild build, RuntimeSystemSettings runtimeSystemSettings);
        protected abstract DeployTaskExecutionResult InternalExecute(TaskExecutionContext<TaskDefinition, TaskDefinitionOptions> context);
    }
}
