using Common.Logging;
using MMDB.Azure.Management;
using MMDB.Azure.Management.AzureDto.AzureCloudService;
using MMDB.Azure.Management.AzureDto.AzureStorage;
using Newtonsoft.Json;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Deployment;
using Sriracha.Deploy.Data.Dto.Build;
using Sriracha.Deploy.Data.Dto.Project;
using Sriracha.Deploy.Data.Dto.Validation;
using Sriracha.Deploy.Data.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Tasks.Azure.DeployCloudService
{
    public class DeployCloudServiceTaskExecutor : BaseDeployTaskExecutor<DeployCloudServiceTaskDefinition, DeployCloudServiceTaskOptions>
    {
        private readonly ILog _logger;
        private readonly IParameterEvaluator _parameterEvaluator;

        public DeployCloudServiceTaskExecutor(IParameterEvaluator parameterEvaluator, ILog logger, IDeploymentValidator validator) : base(parameterEvaluator, validator)
        {
            _logger = DIHelper.VerifyParameter(logger);
        }

        protected override DeployTaskExecutionResult InternalExecute(string deployStateId, IDeployTaskStatusManager statusManager, DeployCloudServiceTaskDefinition definition, DeployComponent component, DeployEnvironmentConfiguration environmentComponent, DeployMachine machine, DeployBuild build, RuntimeSystemSettings runtimeSystemSettings)
        {
            _logger.Info("Starting Dep loyCloudService.InternalExecute");
            var context = this.GetTaskExecutionContext(deployStateId, statusManager, definition, component, environmentComponent, machine, build, runtimeSystemSettings);

            //string formattedSubscriptionIdentifier = this.ReplaceParameters(definition.Options.AzureSubscriptionIdentifier, validationResult.EnvironmentResultList, machineResult, validationResult.BuildParameterList, validationResult.DeployParameterList, build, runtimeSystemSettings, machine, component, false);
            //string maskedFormattedSubscriptionIdentifier = this.ReplaceParameters(definition.Options.AzureSubscriptionIdentifier, validationResult.EnvironmentResultList, machineResult, validationResult.BuildParameterList, validationResult.DeployParameterList, build, runtimeSystemSettings, machine, component, true);

            //string formattedManagementCertificate = this.ReplaceParameters(definition.Options.AzureManagementCertificate, validationResult.EnvironmentResultList, machineResult, validationResult.BuildParameterList, validationResult.DeployParameterList, build, runtimeSystemSettings, machine, component, false);
            //string maskedFormattedManagementCertificate = this.ReplaceParameters(definition.Options.AzureManagementCertificate, validationResult.EnvironmentResultList, machineResult, validationResult.BuildParameterList, validationResult.DeployParameterList, build, runtimeSystemSettings, machine, component, true);

            //string formattedServiceName = this.ReplaceParameters(definition.Options.ServiceName, validationResult.EnvironmentResultList, machineResult, validationResult.BuildParameterList, validationResult.DeployParameterList, build, runtimeSystemSettings, machine, component, false);
            //string maskedFormattedServiceName = this.ReplaceParameters(definition.Options.ServiceName, validationResult.EnvironmentResultList, machineResult, validationResult.BuildParameterList, validationResult.DeployParameterList, build, runtimeSystemSettings, machine, component, true);

            //Environment.CurrentDirectory = runtimeSystemSettings.GetLocalMachineComponentDirectory(machine.MachineName, component.Id);

            var client = new AzureClient(context.FormattedOptions.AzureSubscriptionIdentifier, context.FormattedOptions.AzureManagementCertificate);
            var service = client.GetCloudService(context.FormattedOptions.ServiceName);
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
                service = client.GetCloudService(context.FormattedOptions.ServiceName);
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

                string message;
                if (!client.CheckStorageAccountNameAvailability(context.FormattedOptions.StorageAccountName, out message))
                {
                    throw new ArgumentException(string.Format("Storage Account name {0} not available: {1}", context.MaskedFormattedOptions.StorageAccountName, message));
                }
                client.CreateStorageAccount(context.FormattedOptions.StorageAccountName);
                storageAccount = client.GetStorageAccount(context.FormattedOptions.StorageAccountName);
            }
            else 
            {
                context.Info("Storage Account {0} exists", context.MaskedFormattedOptions.StorageAccountName);
            }

            storageAccount = client.WaitForStorageAccountStatus(context.FormattedOptions.StorageAccountName, StorageServiceProperties.EnumStorageServiceStatus.Created, TimeSpan.FromMinutes(context.FormattedOptions.AzureTimeoutMinutes.GetValueOrDefault(30)));

            var keys = client.GetStorageAccountKeys(context.FormattedOptions.StorageAccountName);

            context.Info("Uploading Azure package file to blog storage: {0}", context.MaskedFormattedOptions.AzurePackagePath);
            var blobUrl = client.UploadBlobFile(context.FormattedOptions.StorageAccountName, keys.Secondary, context.FormattedOptions.AzurePackagePath, "srirachadeploy");

            string configurationData = File.ReadAllText(context.FormattedOptions.AzureConfigPath);

            DeploymentItem deployment = null;
            if(service.DeploymentList != null)
            {
                deployment = service.DeploymentList.FirstOrDefault(i => context.FormattedOptions.DeploymentSlot.Equals(i.DeploymentSlot, StringComparison.CurrentCultureIgnoreCase));
            }
            if(deployment == null)
            {
                context.Info("Deployment does not yet exist, creating...");
                client.CreateCloudServiceDeployment(context.FormattedOptions.ServiceName, blobUrl, configurationData, context.FormattedOptions.DeploymentSlot);
            }
            else 
            {
                context.Info("Deployment already exists, upgrading...");
                client.UpgradeCloudServiceDeployment(context.FormattedOptions.ServiceName, blobUrl, configurationData, context.FormattedOptions.DeploymentSlot);
            }
            deployment = client.WaitForCloudServiceDeploymentStatus(context.FormattedOptions.ServiceName, context.FormattedOptions.DeploymentSlot, DeploymentItem.EnumDeploymentItemStatus.Running, TimeSpan.FromMinutes(context.FormattedOptions.AzureTimeoutMinutes.GetValueOrDefault(30)));
            deployment = client.WaitForAllCloudServiceInstanceStatus(context.FormattedOptions.ServiceName, context.FormattedOptions.DeploymentSlot, RoleInstance.EnumInstanceStatus.ReadyRole, TimeSpan.FromMinutes(context.FormattedOptions.AzureTimeoutMinutes.GetValueOrDefault(30)));
            _logger.Info("Done DeployCloudService.InternalExecute");
            return context.BuildResult();
        }

    }
}
