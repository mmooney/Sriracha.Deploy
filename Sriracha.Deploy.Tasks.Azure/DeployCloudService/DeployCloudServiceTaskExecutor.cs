using Common.Logging;
using Newtonsoft.Json;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Deployment;
using Sriracha.Deploy.Data.Dto.Build;
using Sriracha.Deploy.Data.Dto.Project;
using Sriracha.Deploy.Data.Dto.Validation;
using Sriracha.Deploy.Data.Tasks;
using Sriracha.Deploy.Tasks.Azure.AzureDto;
using Sriracha.Deploy.Tasks.Azure.AzureDto.AzureCloudService;
using Sriracha.Deploy.Tasks.Azure.AzureDto.AzureStorage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Tasks.Azure.DeployCloudService
{
    public class DeployCloudServiceTaskExecutor : BaseDeployTaskExecutor<DeployCloudServiceTaskDefinition>
    {
        private const string ValueMask = "*****";
        private readonly ILog _logger;
        private readonly IDeploymentValidator _validator;
        private readonly IParameterEvaluator _parameterEvaluator;

        public DeployCloudServiceTaskExecutor(IParameterEvaluator parameterEvaluator, ILog logger, IDeploymentValidator validator) : base(parameterEvaluator)
        {
            _logger = DIHelper.VerifyParameter(logger);
            _validator = DIHelper.VerifyParameter(validator);
        }

        protected override DeployTaskExecutionResult InternalExecute(string deployStateId, IDeployTaskStatusManager statusManager, DeployCloudServiceTaskDefinition definition, DeployComponent component, DeployEnvironmentConfiguration environmentComponent, DeployMachine machine, DeployBuild build, RuntimeSystemSettings runtimeSystemSettings)
        {
            _logger.Info("Starting DeployCloudService.InternalExecute");
            var context = GetTaskExecutionContext(deployStateId, statusManager, definition, component, environmentComponent, machine, build, runtimeSystemSettings);

            //string formattedSubscriptionIdentifier = this.ReplaceParameters(definition.Options.AzureSubscriptionIdentifier, validationResult.EnvironmentResultList, machineResult, validationResult.BuildParameterList, validationResult.DeployParameterList, build, runtimeSystemSettings, machine, component, false);
            //string maskedFormattedSubscriptionIdentifier = this.ReplaceParameters(definition.Options.AzureSubscriptionIdentifier, validationResult.EnvironmentResultList, machineResult, validationResult.BuildParameterList, validationResult.DeployParameterList, build, runtimeSystemSettings, machine, component, true);

            //string formattedManagementCertificate = this.ReplaceParameters(definition.Options.AzureManagementCertificate, validationResult.EnvironmentResultList, machineResult, validationResult.BuildParameterList, validationResult.DeployParameterList, build, runtimeSystemSettings, machine, component, false);
            //string maskedFormattedManagementCertificate = this.ReplaceParameters(definition.Options.AzureManagementCertificate, validationResult.EnvironmentResultList, machineResult, validationResult.BuildParameterList, validationResult.DeployParameterList, build, runtimeSystemSettings, machine, component, true);

            //string formattedServiceName = this.ReplaceParameters(definition.Options.ServiceName, validationResult.EnvironmentResultList, machineResult, validationResult.BuildParameterList, validationResult.DeployParameterList, build, runtimeSystemSettings, machine, component, false);
            //string maskedFormattedServiceName = this.ReplaceParameters(definition.Options.ServiceName, validationResult.EnvironmentResultList, machineResult, validationResult.BuildParameterList, validationResult.DeployParameterList, build, runtimeSystemSettings, machine, component, true);

            //Environment.CurrentDirectory = runtimeSystemSettings.GetLocalMachineComponentDirectory(machine.MachineName, component.Id);

            var client = new AzureClient(context.FormattedOptions.AzureSubscriptionIdentifier, context.FormattedOptions.AzureManagementCertificate);
            var service = client.GetHostedService(context.FormattedOptions.ServiceName);
            //var list = client.GetCloudServiceList();

            ////var existingService = computeManagementClient.HostedServices.Get(formattedServiceName);
            //var service = list.FirstOrDefault(i=>i.ServiceName == formattedServiceName);
            if (service == null)
            {
                context.Info("Service {0} does not yet exist, creating...", context.MaskedFormattedOptions.ServiceName);

                string message;
                if (!client.CheckCloudServiceNameAvailability(context.FormattedOptions.ServiceName, out message))
                {
                    throw new ArgumentException(string.Format("Service name {0} not available: {1}", context.MaskedFormattedOptions.ServiceName, message));
                }
                client.CreateCloudService(context.FormattedOptions.ServiceName);
                service = client.GetHostedService(context.FormattedOptions.ServiceName);
                context.Info("Service {0} created successfully", context.MaskedFormattedOptions.ServiceName);
            }
            else
            {
                context.Info("Service {0} already exists", context.MaskedFormattedOptions.ServiceName);
            }

            var storageAccount = client.GetStorageAccount(context.FormattedOptions.StorageAccountName);
            if(storageAccount == null)
            {
                context.Info("Storage Account {0} does not exist, creating...", context.MaskedFormattedOptions.StorageAccountName);

                if (!client.CheckStorageAccountNameAvailability(context.FormattedOptions.StorageAccountName))
                {
                    throw new ArgumentException(string.Format("Storage Account name {0} not available", context.MaskedFormattedOptions.StorageAccountName));
                }
                client.CreateStorageAccount(context.FormattedOptions.StorageAccountName);
                storageAccount = client.GetStorageAccount(context.FormattedOptions.StorageAccountName);
            }
            else 
            {
                context.Info("Storage Account {0} exists", context.MaskedFormattedOptions.StorageAccountName);
            }

            storageAccount = client.WaitForStorageAccountStatus(context.FormattedOptions.StorageAccountName, StorageServiceProperties.EnumStorageServiceStatus.Created, TimeSpan.FromSeconds(120));

            var keys = client.GetStorageAccountKeys(context.FormattedOptions.StorageAccountName);

            context.Info("Uploading Azure package file to blog storage: {0}", context.MaskedFormattedOptions.AzurePackagePath);
            var blobUrl = client.UploadBlobFile(context.FormattedOptions.StorageAccountName, keys.Secondary, context.FormattedOptions.AzurePackagePath, "srirachadeploy");

            string configurationData = File.ReadAllText(context.FormattedOptions.AzureConfigPath);

            DeploymentItem deployment = null;
            if(service.DeploymentList != null)
            {
                deployment = service.DeploymentList.FirstOrDefault(i => i.DeploymentSlot == context.FormattedOptions.DeploymentSlot);
            }
            if(deployment == null)
            {
                context.Info("Deployment does not yet exist, creating...");
                client.CreateCloudServiceDeployment(context.FormattedOptions.ServiceName, blobUrl, configurationData, context.FormattedOptions.DeploymentSlot);
            }
            else 
            {
                context.Info("Deployment already exists, upgrading...");
                throw new NotImplementedException();
            }
            deployment = client.WaitForCloudServiceDeploymentStatus(context.FormattedOptions.ServiceName, context.FormattedOptions.DeploymentSlot, DeploymentItem.EnumDeploymentItemStatus.Running, TimeSpan.FromMinutes(10));
            deployment = client.WaitForAllCloudServiceInstanceStatus(context.FormattedOptions.ServiceName, context.FormattedOptions.DeploymentSlot, RoleInstance.EnumInstanceStatus.ReadyRole, TimeSpan.FromMinutes(10));
            //DeploymentItem deployment = null;
            //if(service.DeploymentList != null && service.DeploymentList.Any())
            //{
            //    if (deployment == null && !string.IsNullOrEmpty(definition.Options.DeploymentName))
            //    {
            //        deployment = service.DeploymentList.FirstOrDefault(i=>i.Name == definition.Options.DeploymentName);
            //    }
            //    if(deployment == null && !string.IsNullOrEmpty(definition.Options.DeploymentSlot))
            //    {
            //        deployment = service.DeploymentList.FirstOrDefault(i=>i.DeploymntSlot == definition.Options.DeploymentSlot);
            //    }
            //}
            //if(deployment == null)
            //{
            //    statusManager.Info(deployStateId, "Azure Deployment not found, creating...");
            //}
            //else 
            //{
            //    statusManager.Info(deployStateId, "Azure Deployment not found, updating...");
            //}
            //var x = client.GetDeploymentList(formattedServiceName);

            //statusManager.Info(deployStateId, string.Format("Executing local command line for machine {0}: {1} {2}", machine.MachineName, formattedExePath, maskedFormattedArgs));
            //using (var standardOutputWriter = new StringWriter())
            //using (var errorOutputWriter = new StringWriter())
            //{
            //    int result;
            //    if (string.IsNullOrEmpty(environmentComponent.DeployCredentialsId) || !AppSettingsHelper.GetBoolSetting("AllowImpersonation", true))
            //    {
            //        result = _processRunner.Run(formattedExePath, formattedArgs, standardOutputWriter, errorOutputWriter);
            //    }
            //    else
            //    {
            //        var credentials = _credentialsManager.GetCredentials(environmentComponent.DeployCredentialsId);
            //        using (var impersonation = _impersonator.BeginImpersonation(credentials.Id))
            //        {
            //            statusManager.Info(deployStateId, string.Format("Starting process as {0} impersonating {1}", WindowsIdentity.GetCurrent().Name, credentials.DisplayValue));
            //            using (var password = _credentialsManager.DecryptPasswordSecure(credentials))
            //            {
            //                string exePath = Path.GetFullPath(formattedExePath);
            //                statusManager.Info(deployStateId, string.Format("For Options.ExecutablePath {0}, using {1}", maskedFormattedExePath, exePath));
            //                //result = _processRunner.RunAsUser(exePath, formattedArgs, standardOutputWriter, errorOutputWriter, credentials.Domain, credentials.UserName, password);
            //                result = _processRunner.RunAsToken(exePath, formattedArgs, standardOutputWriter, errorOutputWriter, impersonation.TokenHandle);
            //            }
            //        }
            //    }
            //    string standardOutput = standardOutputWriter.GetStringBuilder().ToString();
            //    string errorOutput = errorOutputWriter.GetStringBuilder().ToString();
            //    if (result == 0)
            //    {
            //        if (!string.IsNullOrWhiteSpace(standardOutput))
            //        {
            //            statusManager.Info(deployStateId, standardOutput);
            //        }
            //        if (!string.IsNullOrWhiteSpace(errorOutput))
            //        {
            //            statusManager.Error(deployStateId, errorOutput);
            //            throw new Exception("LocalCommandLine Task Failed: " + errorOutput);
            //        }
            //    }
            //    else
            //    {
            //        string errorText;
            //        if (!string.IsNullOrWhiteSpace(errorOutput))
            //        {
            //            errorText = errorOutput;
            //        }
            //        else if (!string.IsNullOrWhiteSpace(standardOutput))
            //        {
            //            errorText = standardOutput;
            //        }
            //        else
            //        {
            //            errorText = "Error Code " + result.ToString();
            //        }
            //        statusManager.Error(deployStateId, errorText);
            //        throw new Exception("LocalCommandLine Task Failed: " + errorText);
            //    }
            //}
            //statusManager.Info(deployStateId, string.Format("Done executing local command line for machine {0}: {1} {2}", machine.MachineName, maskedFormattedExePath, maskedFormattedArgs));
            _logger.Info("Done DeployCloudService.InternalExecute");
            return context.BuildResult();
        }

        private TaskExecutionContext<DeployCloudServiceTaskDefinition, DeployCloudServiceTaskOptions> GetTaskExecutionContext(string deployStateId, IDeployTaskStatusManager statusManager, DeployCloudServiceTaskDefinition taskDefinition, DeployComponent component, DeployEnvironmentConfiguration environmentComponent, DeployMachine machine, DeployBuild build, RuntimeSystemSettings runtimeSystemSettings)
        {
            var validationResult = _validator.ValidateMachineTaskDefinition(taskDefinition, environmentComponent, machine);
            if (validationResult.Status != EnumRuntimeValidationStatus.Success)
            {
                throw new InvalidOperationException("Validation not complete:" + Environment.NewLine + JsonConvert.SerializeObject(validationResult));
            }
            var machineResult = validationResult.MachineResultList[machine.Id];

            var formattedOptions = AutoMapper.Mapper.Map<DeployCloudServiceTaskOptions>(taskDefinition.Options);
            this.FormatOptions(formattedOptions, validationResult, build, component, machine, runtimeSystemSettings, false);

            var maskedFormattedOptions = AutoMapper.Mapper.Map<DeployCloudServiceTaskOptions>(taskDefinition.Options);
            this.FormatOptions(maskedFormattedOptions, validationResult, build, component, machine, runtimeSystemSettings, true);

            var returnValue = new TaskExecutionContext<DeployCloudServiceTaskDefinition, DeployCloudServiceTaskOptions>
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
                if(propInfo.PropertyType == typeof(string))
                {
                    string rawValue = (string)propInfo.GetValue(formattedOptions, null);
                    if(string.IsNullOrEmpty(rawValue))
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
                    if(childObject != null)
                    {
                        this.FormatOptions(childObject, validationResult, build, component, machine, runtimeSystemSettings, masked);
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
                    value = DeployCloudServiceTaskExecutor.ValueMask;
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
                    value = DeployCloudServiceTaskExecutor.ValueMask;
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
                    value = DeployCloudServiceTaskExecutor.ValueMask;
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
                    value = DeployCloudServiceTaskExecutor.ValueMask;
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
