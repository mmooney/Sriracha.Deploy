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
using Sriracha.Deploy.Data.Tasks;
using Sriracha.Deploy.Data;

namespace Sriracha.Deploy.Tasks.Common.LocalCommandLine
{
	public class LocalCommandLineTaskExecutor : BaseDeployTaskExecutor<LocalCommandLineTaskDefinition, LocalCommandLineTaskOptions>
	{
		private const string ValueMask = "*****";
		private readonly IProcessRunner _processRunner;
		private readonly IDeploymentValidator _validator;
		private readonly ICredentialsManager _credentialsManager;
		private readonly IImpersonator _impersonator;

		public LocalCommandLineTaskExecutor(IProcessRunner processRunner, IDeploymentValidator validator, IParameterEvaluator buildParameterEvaluator, ICredentialsManager credentialsManager, IImpersonator impersonator) : base(buildParameterEvaluator, validator)
		{
			_processRunner = DIHelper.VerifyParameter(processRunner);
			_validator = DIHelper.VerifyParameter(validator);
			_credentialsManager = DIHelper.VerifyParameter(credentialsManager);
			_impersonator = DIHelper.VerifyParameter(impersonator);
		}

        protected override DeployTaskExecutionResult InternalExecute(TaskExecutionContext<LocalCommandLineTaskDefinition, LocalCommandLineTaskOptions> context)
        {
			context.Info("Starting LocalCommndLine for {0} ", context.MaskedFormattedOptions.ExecutablePath);
			var result = new DeployTaskExecutionResult();

			context.Info("Configuring local command line for machine {0}: {1} {2}", context.Machine.MachineName, context.MaskedFormattedOptions.ExecutablePath, context.MaskedFormattedOptions.ExecutableArguments);
			Environment.CurrentDirectory = context.SystemSettings.GetLocalMachineComponentDirectory(context.Machine.MachineName, context.Component.Id);

            context.Info("Executing local command line for machine {0}: {1} {2}", context.Machine.MachineName, context.MaskedFormattedOptions.ExecutablePath, context.MaskedFormattedOptions.ExecutableArguments);

			using (var standardOutputWriter = new StringWriter())
			using(var errorOutputWriter = new StringWriter())
			{
				int exeResult;
				if(string.IsNullOrEmpty(context.EnvironmentComponent.DeployCredentialsId) || !AppSettingsHelper.GetBoolSetting("AllowImpersonation", true))
				{
                    exeResult = _processRunner.Run(context.FormattedOptions.ExecutablePath, context.FormattedOptions.ExecutableArguments, standardOutputWriter, errorOutputWriter);
				}
				else 
				{
					var credentials = _credentialsManager.GetCredentials(context.EnvironmentComponent.DeployCredentialsId);
					using(var impersonation = _impersonator.BeginImpersonation(credentials.Id))
					{
						context.Info("Starting process as {0} impersonating {1}", WindowsIdentity.GetCurrent().Name, credentials.DisplayValue);
						using(var password = _credentialsManager.DecryptPasswordSecure(credentials))
						{
                            string exePath = Path.GetFullPath(context.FormattedOptions.ExecutablePath);
                            context.Info("For Options.ExecutablePath {0}, using {1}", context.MaskedFormattedOptions.ExecutablePath, exePath);
							//result = _processRunner.RunAsUser(exePath, formattedArgs, standardOutputWriter, errorOutputWriter, credentials.Domain, credentials.UserName, password);
							exeResult = _processRunner.RunAsToken(exePath, context.FormattedOptions.ExecutableArguments, standardOutputWriter, errorOutputWriter, impersonation.TokenHandle);
						}
					}
				}
				string standardOutput = standardOutputWriter.GetStringBuilder().ToString();
				string errorOutput = errorOutputWriter.GetStringBuilder().ToString();
				if(exeResult == 0)
				{
					if(!string.IsNullOrWhiteSpace(standardOutput))
					{
						context.Info(standardOutput);
					}
					if(!string.IsNullOrWhiteSpace(errorOutput))
					{
						context.Error(errorOutput);
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
						errorText = "Error Code " + exeResult.ToString();
					}
					context.Error(errorText);
					throw new Exception("LocalCommandLine Task Failed: " + errorText);
				}
			}
            context.Info("Done executing local command line for machine {0}: {1} {2}", context.Machine.MachineName, context.MaskedFormattedOptions.ExecutablePath, context.MaskedFormattedOptions.ExecutableArguments);

			context.Info(string.Format("Done LocalCommndLine for {0} ",  context.MaskedFormattedOptions.ExecutablePath));
			return context.BuildResult();
		}

    }
}
