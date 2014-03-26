using Sriracha.Deploy.Data.Deployment;
using Sriracha.Deploy.Data.Dto.Build;
using Sriracha.Deploy.Data.Dto.Project;
using Sriracha.Deploy.Data.Dto.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Tasks
{
    public class FormattingStringConverter : AutoMapper.ITypeConverter<string, string>
    {
        private const string ValueMask = "*****";
        private readonly List<TaskDefinitionValidationResult.TaskDefinitionValidationResultItem> _environmentValues;
        private readonly List<TaskDefinitionValidationResult.TaskDefinitionValidationResultItem> _machineValues;
        private readonly List<TaskParameter> _buildParameters;
        private readonly List<TaskParameter> _deployParameters;
        private readonly DeployBuild _build;
        private readonly RuntimeSystemSettings _runtimeSystemSettings;
        private readonly DeployMachine _machine;
        private readonly DeployComponent _component;
        private readonly bool _masked;
        private readonly IParameterEvaluator _parameterEvaluator;

        public FormattingStringConverter(List<TaskDefinitionValidationResult.TaskDefinitionValidationResultItem> environmentValues, 
                                            List<TaskDefinitionValidationResult.TaskDefinitionValidationResultItem> machineValues, 
                                            List<TaskParameter> buildParameters, List<TaskParameter> deployParameters, DeployBuild build, 
                                            RuntimeSystemSettings runtimeSystemSettings, DeployMachine machine, DeployComponent component, bool masked,
                                            IParameterEvaluator parameterEvaluator)
        {
            _environmentValues = environmentValues;
            _machineValues = machineValues;
            _buildParameters = buildParameters;
            _deployParameters = deployParameters;
            _build = build;
            _runtimeSystemSettings = runtimeSystemSettings;
            _machine = machine;
            _component = component;
            _masked = masked;
        }

        public string Convert(AutoMapper.ResolutionContext context)
        {
            if(context.IsSourceValueNull)
            {
                return null;
            }
            return this.ReplaceParameters((string)context.SourceValue);
        }

        private string ReplaceParameters(string format)
        {
            string returnValue = format;
            foreach (var item in _environmentValues)
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
                if (_masked && item.Sensitive)
                {
                    value = FormattingStringConverter.ValueMask;
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
            foreach (var item in _machineValues)
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
                if (_masked && item.Sensitive)
                {
                    value = FormattingStringConverter.ValueMask;
                }
                else
                {
                    value = item.FieldValue;
                }
                returnValue = ReplaceString(returnValue, fieldName, value, StringComparison.CurrentCultureIgnoreCase);
            }
            foreach (var item in _buildParameters)
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
                if (_masked && item.Sensitive)
                {
                    value = FormattingStringConverter.ValueMask;
                }
                else
                {
                    value = this.GetBuildParameterValue(item.FieldName, _build);
                }
                returnValue = ReplaceString(returnValue, fieldName, value, StringComparison.CurrentCultureIgnoreCase);
            }
            foreach (var item in _deployParameters)
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
                if (_masked && item.Sensitive)
                {
                    value = FormattingStringConverter.ValueMask;
                }
                else
                {
                    value = this.GetDeployParameterValue(item.FieldName, _runtimeSystemSettings, _machine, _component);
                }
                returnValue = ReplaceString(returnValue, fieldName, value, StringComparison.CurrentCultureIgnoreCase);
            }
            return returnValue;
        }

        protected string GetDeployParameterValue(string parameterName, RuntimeSystemSettings runtimeSystemSettings, DeployMachine machine, DeployComponent component)
        {
            return _parameterEvaluator.EvaluateDeployParameter(parameterName, runtimeSystemSettings, machine, component);
        }

        protected string GetBuildParameterValue(string parameterName, DeployBuild build)
        {
            return _parameterEvaluator.EvaluateBuildParameter(parameterName, build);
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
    }
}
