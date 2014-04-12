using System;
using System.IO;
using System.Xml;
using System.Linq;
using MMDB.Shared;
using Newtonsoft.Json;
using Sriracha.Deploy.Data.Deployment;
using Sriracha.Deploy.Data.Utility;
using Sriracha.Deploy.Data.Dto.Project;
using Sriracha.Deploy.Data.Dto.Build;
using Sriracha.Deploy.Data.Dto.Validation;
using Sriracha.Deploy.Data.Tasks;
using Sriracha.Deploy.Data;

namespace Sriracha.Deploy.Tasks.Common.XmlConfigFile
{
	public class XmlConfigFileTaskExecutor : BaseDeployTaskExecutor<XmlConfigFileTaskDefinition, XmlConfigFileTaskOptions>
	{
		private readonly IFileWriter _fileWriter;
		private readonly IDeploymentValidator _validator;

		public XmlConfigFileTaskExecutor(IFileWriter fileWriter, IDeploymentValidator validator, IParameterEvaluator buildParameterEvaluator) : base(buildParameterEvaluator, validator)
		{
			_fileWriter = DIHelper.VerifyParameter(fileWriter);
			_validator = DIHelper.VerifyParameter(validator);
		}

		protected override DeployTaskExecutionResult InternalExecute(string deployStateId, IDeployTaskStatusManager statusManager, XmlConfigFileTaskDefinition definition, DeployComponent component, DeployEnvironmentConfiguration environmentComponent, DeployMachine machine, DeployBuild build, RuntimeSystemSettings runtimeSystemSettings)
		{
			statusManager.Info(deployStateId, string.Format("Starting XmlConfigTask for {0} ", definition.Options.TargetFileName));
            var context = this.GetTaskExecutionContext(deployStateId, statusManager, definition, component, environmentComponent, machine, build, runtimeSystemSettings);
			if (context.ValidationResult.Status != EnumRuntimeValidationStatus.Success)
			{
                throw new InvalidOperationException("Validation not complete:" + Environment.NewLine + JsonConvert.SerializeObject(context.ValidationResult));
			}
			this.ExecuteMachine(context);

			statusManager.Info(deployStateId, string.Format("Done XmlConfigTask for {0} ", definition.Options.TargetFileName));
			return statusManager.BuildResult();
		}

        private void ExecuteMachine(TaskExecutionContext<XmlConfigFileTaskDefinition, XmlConfigFileTaskOptions> context)
        {
            context.Info(string.Format("Configuring {0} for machine {1}", context.MaskedFormattedOptions.TargetFileName, context.Machine.MachineName));
            var machineResult = context.ValidationResult.MachineResultList[context.Machine.Id];
			var xmlDoc = new XmlDocument();
            switch(context.FormattedOptions.TemplateSource)
            {
                case XmlConfigFileTaskOptions.EnumTemplateSource.XmlTemplate:
                    if (string.IsNullOrEmpty(context.FormattedOptions.XmlTemplate)) 
                    {
                        throw new ArgumentNullException("TemplateSource=XmlTemplate but XmlTemplate is null");
                    }
                    xmlDoc.LoadXml(context.FormattedOptions.XmlTemplate);
                    break;
                case XmlConfigFileTaskOptions.EnumTemplateSource.File:
                    if (string.IsNullOrEmpty(context.FormattedOptions.SourceFileName))
                    {
                        throw new ArgumentNullException("TemplateSource=File but SourceFileName is null");
                    }
                    if (!File.Exists(context.FormattedOptions.SourceFileName))
                    {
                        throw new FileNotFoundException("SourceFileName does not exist:" + context.MaskedFormattedOptions.SourceFileName, context.MaskedFormattedOptions.SourceFileName);
                    }
                    xmlDoc.Load(context.FormattedOptions.SourceFileName);
                    break;
            }
            var namespaceManager = CreateXmlNamespaceManager(xmlDoc, context.FormattedOptions);
            foreach (var xpathItem in context.FormattedOptions.XPathValueList)
			{
				string value;
				switch (xpathItem.ConfigLevel)
				{
					case EnumConfigLevel.Environment:
                        value = context.ValidationResult.EnvironmentResultList.First(i => i.FieldName == xpathItem.ValueName).FieldValue;
						break;
					case EnumConfigLevel.Machine:
						value = machineResult.First(i => i.FieldName == xpathItem.ValueName).FieldValue;
						break;
					case EnumConfigLevel.Build:
						value = this.GetBuildParameterValue(xpathItem.ValueName, context.Build);
						break;
					default:
						throw new UnknownEnumValueException(xpathItem.ConfigLevel);
				}
				this.UpdateXmlNode(xmlDoc, namespaceManager, xpathItem.XPath, value);
			}
			string outputDirectory = context.SystemSettings.GetLocalMachineComponentDirectory(context.Machine.MachineName, context.Component.Id);
			if(!Directory.Exists(outputDirectory))
			{
				Directory.CreateDirectory(outputDirectory);
			}
            string outputFilePath = Path.Combine(outputDirectory, context.FormattedOptions.TargetFileName);
			_fileWriter.WriteText(outputFilePath, xmlDoc.OuterXml, false);
            context.Info(string.Format("Writing XML {0} for machine {1}: {2}", context.MaskedFormattedOptions.TargetFileName, context.Machine, xmlDoc.OuterXml));
            context.Info(string.Format("Done configuring {0} for machine {1}", context.MaskedFormattedOptions.TargetFileName, context.Machine.MachineName));
		}

        private XmlNamespaceManager CreateXmlNamespaceManager(XmlDocument xmlDoc, XmlConfigFileTaskOptions options)
        {
            XmlNamespaceManager namespaceManager = new XmlNamespaceManager(xmlDoc.NameTable);
            if (options.NamespaceDefinitionList != null)
            {
                foreach(var item in options.NamespaceDefinitionList)
                {
                    namespaceManager.AddNamespace(item.Prefix, item.Uri);
                }
            }

            return namespaceManager;
        }

		private void UpdateXmlNode(XmlDocument xmlDoc, XmlNamespaceManager namespaceManager, string xpath, string value)
		{
			var node = xmlDoc.SelectSingleNode(xpath, namespaceManager);
			if (node == null)
			{
				throw new Exception("Node not found: " + xpath);
			}
			node.InnerText = value;
		}

	}
}
