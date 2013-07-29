using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MMDB.Shared;
using Newtonsoft.Json;
using Sriracha.Deploy.Data.Dto;

namespace Sriracha.Deploy.Data.Tasks.LocalCommandLine
{
	public class LocalCommandLineTaskExecutor : BaseDeployTaskExecutor<LocalCommandLineTaskDefinition>
	{
		private const string ValueMask = "*****";
		private readonly IProcessRunner _processRunner;
		private readonly IDeploymentValidator _validator;

		public LocalCommandLineTaskExecutor(IProcessRunner processRunner, IDeploymentValidator validator)
		{
			this._processRunner = DIHelper.VerifyParameter(processRunner);
			this._validator = DIHelper.VerifyParameter(validator);
		}

		protected override DeployTaskExecutionResult InternalExecute(string deployStateId, IDeployTaskStatusManager statusManager, LocalCommandLineTaskDefinition definition, DeployEnvironmentComponent environmentComponent, RuntimeSystemSettings runtimeSystemSettings)
		{
			statusManager.Info(deployStateId, string.Format("Starting LocalCommndLine for {0} ", definition.Options.ExecutablePath));
			var result = new DeployTaskExecutionResult();
			var validationResult = _validator.ValidateTaskDefinition(definition, environmentComponent);
			if (validationResult.Status != EnumRuntimeValidationStatus.Success)
			{
				throw new InvalidOperationException("Validation not complete:" + Environment.NewLine + JsonConvert.SerializeObject(validationResult));
			}
			foreach (var machine in environmentComponent.MachineList)
			{
				this.ExecuteMachine(deployStateId, statusManager, definition, environmentComponent, machine, runtimeSystemSettings, validationResult);
			}
			statusManager.Info(deployStateId, string.Format("Done LocalCommndLine for {0} ", definition.Options.ExecutablePath));
			return statusManager.BuildResult();
		}

		private void ExecuteMachine(string deployStateId, IDeployTaskStatusManager statusManager, LocalCommandLineTaskDefinition definition, DeployEnvironmentComponent environmentComponent, DeployMachine machine, RuntimeSystemSettings runtimeSystemSettings, TaskDefinitionValidationResult validationResult)
		{
			statusManager.Info(deployStateId, string.Format("Configuring local command line for machine {0}: {1} {2}", machine.MachineName, definition.Options.ExecutablePath, definition.Options.ExecutableArguments));
			var machineResult = validationResult.MachineResultList[machine.MachineName];
			string formattedArgs = this.ReplaceParameters(definition.Options.ExecutableArguments, validationResult.EnvironmentResultList, machineResult, false);
			string maskedFormattedArgs = this.ReplaceParameters(definition.Options.ExecutableArguments, validationResult.EnvironmentResultList, machineResult, true);

			Environment.CurrentDirectory = runtimeSystemSettings.GetLocalMachineDirectory(machine.MachineName);

			statusManager.Info(deployStateId, string.Format("Executing local command line for machine {0}: {1} {2}", machine.MachineName, definition.Options.ExecutablePath, maskedFormattedArgs));
			using (var standardOutputWriter = new StringWriter())
			using(var errorOutputWriter = new StringWriter())
			{
				int result = _processRunner.Run(definition.Options.ExecutablePath, formattedArgs, standardOutputWriter, errorOutputWriter);
				string standardOutput = standardOutputWriter.GetStringBuilder().ToString();
				string errorOutput = errorOutputWriter.GetStringBuilder().ToString();
				if(result == 0)
				{
					if(!string.IsNullOrEmpty(standardOutput))
					{
						statusManager.Info(deployStateId, standardOutput);
					}
					if(!string.IsNullOrEmpty(errorOutput))
					{
						statusManager.Error(deployStateId, errorOutput);
						throw new Exception("LocalCommandLine Task Failed: " + errorOutput); 
					}
				}
				else 
				{
					string errorText = StringHelper.IsNullOrEmpty(errorOutput, standardOutput, "Error Code " + result.ToString());
					statusManager.Error(deployStateId, errorText);
					throw new Exception("LocalCommandLine Task Failed: " + errorText);
				}
			}
			statusManager.Info(deployStateId, string.Format("Done executing local command line for machine {0}: {1} {2}", machine.MachineName, definition.Options.ExecutablePath, maskedFormattedArgs));
		}

		private string ReplaceParameters(string format, List<TaskDefinitionValidationResult.TaskDefinitionValidationResultItem> environmentValues, List<TaskDefinitionValidationResult.TaskDefinitionValidationResultItem> machineValues, bool masked)
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
