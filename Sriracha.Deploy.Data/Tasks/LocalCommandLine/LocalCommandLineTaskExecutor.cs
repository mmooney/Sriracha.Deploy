using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MMDB.Shared;
using Newtonsoft.Json;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Build;
using Sriracha.Deploy.Data.Deployment;
using Sriracha.Deploy.Data.Utility;
using Sriracha.Deploy.Data.Credentials;
using System.Security.Principal;
using Sriracha.Deploy.Data.Dto.Project;
using Sriracha.Deploy.Data.Dto.Build;
using Sriracha.Deploy.Data.Dto.Validation;

namespace Sriracha.Deploy.Data.Tasks.LocalCommandLine
{
	public class LocalCommandLineTaskExecutor : BaseDeployTaskExecutor<LocalCommandLineTaskDefinition>
	{
		private const string ValueMask = "*****";
		private readonly IProcessRunner _processRunner;
		private readonly IDeploymentValidator _validator;
		private readonly ICredentialsManager _credentialsManager;
		private readonly IImpersonator _impersonator;

		public LocalCommandLineTaskExecutor(IProcessRunner processRunner, IDeploymentValidator validator, IParameterEvaluator buildParameterEvaluator, ICredentialsManager credentialsManager, IImpersonator impersonator) : base(buildParameterEvaluator)
		{
			_processRunner = DIHelper.VerifyParameter(processRunner);
			_validator = DIHelper.VerifyParameter(validator);
			_credentialsManager = DIHelper.VerifyParameter(credentialsManager);
			_impersonator = DIHelper.VerifyParameter(impersonator);
		}

		protected override DeployTaskExecutionResult InternalExecute(string deployStateId, IDeployTaskStatusManager statusManager, LocalCommandLineTaskDefinition definition, DeployComponent component, DeployEnvironmentConfiguration environmentComponent, DeployMachine machine, DeployBuild build, RuntimeSystemSettings runtimeSystemSettings)
		{
			statusManager.Info(deployStateId, string.Format("Starting LocalCommndLine for {0} ", definition.Options.ExecutablePath));
			var result = new DeployTaskExecutionResult();
			var validationResult = _validator.ValidateMachineTaskDefinition(definition, environmentComponent, machine);
			if (validationResult.Status != EnumRuntimeValidationStatus.Success)
			{
				throw new InvalidOperationException("Validation not complete:" + Environment.NewLine + JsonConvert.SerializeObject(validationResult));
			}
			this.ExecuteMachine(deployStateId, statusManager, definition, component, environmentComponent, machine, build, runtimeSystemSettings, validationResult);
			statusManager.Info(deployStateId, string.Format("Done LocalCommndLine for {0} ", definition.Options.ExecutablePath));
			return statusManager.BuildResult();
		}

		private void ExecuteMachine(string deployStateId, IDeployTaskStatusManager statusManager, LocalCommandLineTaskDefinition definition, DeployComponent component, DeployEnvironmentConfiguration environmentComponent, DeployMachine machine, DeployBuild build, RuntimeSystemSettings runtimeSystemSettings, TaskDefinitionValidationResult validationResult)
		{
			statusManager.Info(deployStateId, string.Format("Configuring local command line for machine {0}: {1} {2}", machine.MachineName, definition.Options.ExecutablePath, definition.Options.ExecutableArguments));
			var machineResult = validationResult.MachineResultList[machine.Id];

            string formattedExePath = this.ReplaceParameters(definition.Options.ExecutablePath, validationResult.EnvironmentResultList, machineResult, validationResult.BuildParameterList, validationResult.DeployParameterList, build, runtimeSystemSettings, machine, component, false);
            string maskedFormattedExePath = this.ReplaceParameters(definition.Options.ExecutablePath, validationResult.EnvironmentResultList, machineResult, validationResult.BuildParameterList, validationResult.DeployParameterList, build, runtimeSystemSettings, machine, component, true);

			string formattedArgs = this.ReplaceParameters(definition.Options.ExecutableArguments, validationResult.EnvironmentResultList, machineResult, validationResult.BuildParameterList, validationResult.DeployParameterList, build, runtimeSystemSettings, machine, component, false);
			string maskedFormattedArgs = this.ReplaceParameters(definition.Options.ExecutableArguments, validationResult.EnvironmentResultList, machineResult, validationResult.BuildParameterList, validationResult.DeployParameterList, build, runtimeSystemSettings, machine, component, true);

			Environment.CurrentDirectory = runtimeSystemSettings.GetLocalMachineComponentDirectory(machine.MachineName, component.Id);

            statusManager.Info(deployStateId, string.Format("Executing local command line for machine {0}: {1} {2}", machine.MachineName, formattedExePath, maskedFormattedArgs));
			using (var standardOutputWriter = new StringWriter())
			using(var errorOutputWriter = new StringWriter())
			{
				int result;
				if(string.IsNullOrEmpty(environmentComponent.DeployCredentialsId) || !AppSettingsHelper.GetBoolSetting("AllowImpersonation", true))
				{
                    result = _processRunner.Run(formattedExePath, formattedArgs, standardOutputWriter, errorOutputWriter);
				}
				else 
				{
					var credentials = _credentialsManager.GetCredentials(environmentComponent.DeployCredentialsId);
					using(var impersonation = _impersonator.BeginImpersonation(credentials.Id))
					{
						statusManager.Info(deployStateId, string.Format("Starting process as {0} impersonating {1}", WindowsIdentity.GetCurrent().Name, credentials.DisplayValue));
						using(var password = _credentialsManager.DecryptPasswordSecure(credentials))
						{
                            string exePath = Path.GetFullPath(formattedExePath);
                            statusManager.Info(deployStateId, string.Format("For Options.ExecutablePath {0}, using {1}", maskedFormattedExePath, exePath));
							//result = _processRunner.RunAsUser(exePath, formattedArgs, standardOutputWriter, errorOutputWriter, credentials.Domain, credentials.UserName, password);
							result = _processRunner.RunAsToken(exePath, formattedArgs, standardOutputWriter, errorOutputWriter, impersonation.TokenHandle);
						}
					}
				}
				string standardOutput = standardOutputWriter.GetStringBuilder().ToString();
				string errorOutput = errorOutputWriter.GetStringBuilder().ToString();
				if(result == 0)
				{
					if(!string.IsNullOrWhiteSpace(standardOutput))
					{
						statusManager.Info(deployStateId, standardOutput);
					}
					if(!string.IsNullOrWhiteSpace(errorOutput))
					{
						statusManager.Error(deployStateId, errorOutput);
						throw new Exception("LocalCommandLine Task Failed: " + errorOutput); 
					}
				}
				else 
				{
					string errorText;
					if(!string.IsNullOrWhiteSpace(errorOutput)) 
					{
						errorText = errorOutput;
					}
					else if (!string.IsNullOrWhiteSpace(standardOutput))
					{
						errorText = standardOutput;
					}
					else 
					{
						errorText = "Error Code " + result.ToString();
					}
					statusManager.Error(deployStateId, errorText);
					throw new Exception("LocalCommandLine Task Failed: " + errorText);
				}
			}
            statusManager.Info(deployStateId, string.Format("Done executing local command line for machine {0}: {1} {2}", machine.MachineName, maskedFormattedExePath, maskedFormattedArgs));
		}

		private string ReplaceParameters(string format, List<TaskDefinitionValidationResult.TaskDefinitionValidationResultItem> environmentValues, List<TaskDefinitionValidationResult.TaskDefinitionValidationResultItem> machineValues, List<TaskParameter> buildParameters, List<TaskParameter> deployParameters, DeployBuild build, RuntimeSystemSettings runtimeSystemSettings, DeployMachine machine, DeployComponent component, bool masked)
		{
			string returnValue = format;
			foreach(var item in environmentValues)
			{
				string value;
				string fieldName;
				if(item.Sensitive)
				{
					fieldName = string.Format("${{env:sensitive:{0}}}", item.FieldName);
				}
				else
				{
					fieldName = string.Format("${{env:{0}}}", item.FieldName);
				}
				if(masked && item.Sensitive)
				{
					value = LocalCommandLineTaskExecutor.ValueMask;
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
			foreach(var item in machineValues)
			{
				string value;
				string fieldName;
				if(item.Sensitive)
				{
					fieldName = string.Format("${{machine:sensitive:{0}}}", item.FieldName);
				}
				else 
				{
					fieldName = string.Format("${{machine:{0}}}", item.FieldName);
				}
				if (masked && item.Sensitive)
				{
					value = LocalCommandLineTaskExecutor.ValueMask;
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
					value = LocalCommandLineTaskExecutor.ValueMask;
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
					value = LocalCommandLineTaskExecutor.ValueMask;
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
	}
}
