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
        private readonly IParameterEvaluator _buildParameterEvaluator;
        private readonly IDeploymentValidator _validator;

		public BaseDeployTaskExecutor(IParameterEvaluator buildParamterEvaluator, IDeploymentValidator validator)
		{
			_buildParameterEvaluator = DIHelper.VerifyParameter(buildParamterEvaluator);
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
			return _buildParameterEvaluator.EvaluateBuildParameter(parameterName, build);
		}

		protected string GetDeployParameterValue(string parameterName, RuntimeSystemSettings runtimeSystemSettings, DeployMachine machine, DeployComponent component)
		{
			return _buildParameterEvaluator.EvaluateDeployParameter(parameterName, runtimeSystemSettings, machine, component);
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
                string value;
                string fieldName;
                if (item.Sensitive)
                {
                    fieldName = string.Format("${{env:sensitive:{0}}}", item.FieldName);
                }
                else
                {
                    fieldName = string.Format("${{env:{0}}}", item.FieldName);
                }
                if (masked && item.Sensitive)
                {
                    value = ValueMask;
                }
                else
                {
                    value = item.FieldValue;
                    //if(!string.IsNullOrEmpty(value))
                    //{
                    //	//Because this is going into a DOS command line, need to escape certain characters
                    //	//	http://www.robvanderwoude.com/escapechars.php
                    //	value = value.Replace("%","%%")
                    //				.Replace("^","^^")
                    //				.Replace("&","^&")
                    //				.Replace("<","^<")
                    //				.Replace(">","^>")
                    //				.Replace("|","|^")
                    //				;	
                    //}
                }
                returnValue = ReplaceString(returnValue, fieldName, value, StringComparison.CurrentCultureIgnoreCase);
            }
            foreach (var item in machineValues)
            {
                string value;
                string fieldName;
                if (item.Sensitive)
                {
                    fieldName = string.Format("${{machine:sensitive:{0}}}", item.FieldName);
                }
                else
                {
                    fieldName = string.Format("${{machine:{0}}}", item.FieldName);
                }
                if (masked && item.Sensitive)
                {
                    value = ValueMask;
                }
                else
                {
                    value = item.FieldValue;
                }
                returnValue = ReplaceString(returnValue, fieldName, value, StringComparison.CurrentCultureIgnoreCase);
            }
            foreach (var item in buildParameters)
            {
                string value;
                string fieldName;
                if (item.Sensitive)
                {
                    fieldName = string.Format("${{build:sensitive:{0}}}", item.FieldName);
                }
                else
                {
                    fieldName = string.Format("${{build:{0}}}", item.FieldName);
                }
                if (masked && item.Sensitive)
                {
                    value = ValueMask;
                }
                else
                {
                    value = this.GetBuildParameterValue(item.FieldName, build);
                }
                returnValue = ReplaceString(returnValue, fieldName, value, StringComparison.CurrentCultureIgnoreCase);
            }
            foreach (var item in deployParameters)
            {
                string value;
                string fieldName;
                if (item.Sensitive)
                {
                    fieldName = string.Format("${{deploy:sensitive:{0}}}", item.FieldName);
                }
                else
                {
                    fieldName = string.Format("${{deploy:{0}}}", item.FieldName);
                }
                if (masked && item.Sensitive)
                {
                    value = ValueMask;
                }
                else
                {
                    value = this.GetDeployParameterValue(item.FieldName, runtimeSystemSettings, machine, component);
                }
                returnValue = ReplaceString(returnValue, fieldName, value, StringComparison.CurrentCultureIgnoreCase);
            }
            return returnValue;
        }

        //http://stackoverflow.com/questions/244531/is-there-an-alternative-to-string-replace-that-is-case-insensitive
        //http://stackoverflow.com/a/244933/203479
        public static string ReplaceString(string str, string oldValue, string newValue, StringComparison comparison)
        {
            StringBuilder sb = new StringBuilder();

            int previousIndex = 0;
            int index = str.IndexOf(oldValue, comparison);
            while (index != -1)
            {
                sb.Append(str.Substring(previousIndex, index - previousIndex));
                sb.Append(newValue);
                index += oldValue.Length;

                previousIndex = index;
                index = str.IndexOf(oldValue, index, comparison);
            }
            sb.Append(str.Substring(previousIndex));

            return sb.ToString();
        }
        
        //protected abstract DeployTaskExecutionResult InternalExecute(string deployStateId, IDeployTaskStatusManager statusManager, TaskDefinition definition, DeployComponent component, DeployEnvironmentConfiguration environmentComponent, DeployMachine machine, DeployBuild build, RuntimeSystemSettings runtimeSystemSettings);
        protected abstract DeployTaskExecutionResult InternalExecute(TaskExecutionContext<TaskDefinition, TaskDefinitionOptions> context);
    }
}
