﻿using System;
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
	public class XmlConfigFileTaskExecutor : BaseDeployTaskExecutor<XmlConfigFileTaskDefinition>
	{
		private readonly IFileWriter _fileWriter;
		private readonly IDeploymentValidator _validator;

		public XmlConfigFileTaskExecutor(IFileWriter fileWriter, IDeploymentValidator validator, IParameterEvaluator buildParameterEvaluator) : base(buildParameterEvaluator)
		{
			_fileWriter = DIHelper.VerifyParameter(fileWriter);
			_validator = DIHelper.VerifyParameter(validator);
		}

		protected override DeployTaskExecutionResult InternalExecute(string deployStateId, IDeployTaskStatusManager statusManager, XmlConfigFileTaskDefinition definition, DeployComponent component, DeployEnvironmentConfiguration environmentComponent, DeployMachine machine, DeployBuild build, RuntimeSystemSettings runtimeSystemSettings)
		{
			statusManager.Info(deployStateId, string.Format("Starting XmlConfigTask for {0} ", definition.Options.TargetFileName));
			var result = new DeployTaskExecutionResult();
			var validationResult = _validator.ValidateMachineTaskDefinition(definition, environmentComponent, machine);
			if (validationResult.Status != EnumRuntimeValidationStatus.Success)
			{
				throw new InvalidOperationException("Validation not complete:" + Environment.NewLine + JsonConvert.SerializeObject(validationResult));
			}
			this.ExecuteMachine(deployStateId, statusManager, definition, component, environmentComponent, machine, build, runtimeSystemSettings, validationResult);

			statusManager.Info(deployStateId, string.Format("Done XmlConfigTask for {0} ", definition.Options.TargetFileName));
			return statusManager.BuildResult();
		}

		private void ExecuteMachine(string deployStateId, IDeployTaskStatusManager statusManager, XmlConfigFileTaskDefinition definition, DeployComponent component, DeployEnvironmentConfiguration environmentComponent, DeployMachine machine, DeployBuild build, RuntimeSystemSettings runtimeSystemSettings, TaskDefinitionValidationResult validationResult)
		{
			statusManager.Info(deployStateId, string.Format("Configuring {0} for machine {1}", definition.Options.TargetFileName, machine.MachineName));
			var machineResult = validationResult.MachineResultList[machine.Id];
			var xmlDoc = new XmlDocument();
            switch(definition.Options.TemplateSource)
            {
                case XmlConfigFileTaskOptions.EnumTemplateSource.XmlTemplate:
                    if(string.IsNullOrEmpty(definition.Options.XmlTemplate)) 
                    {
                        throw new ArgumentNullException("TemplateSource=XmlTemplate but XmlTemplate is null");
                    }
                    xmlDoc.LoadXml(definition.Options.XmlTemplate);
                    break;
                case XmlConfigFileTaskOptions.EnumTemplateSource.File:
                    if(string.IsNullOrEmpty(definition.Options.SourceFileName))
                    {
                        throw new ArgumentNullException("TemplateSource=File but SourceFileName is null");
                    }
                    if(!File.Exists(definition.Options.SourceFileName))
                    {
                        throw new FileNotFoundException("SourceFileName does not exist:" + definition.Options.SourceFileName, definition.Options.SourceFileName);
                    }
                    xmlDoc.Load(definition.Options.SourceFileName);
                    break;
            }
			foreach (var xpathItem in definition.Options.XPathValueList)
			{
				string value;
				switch (xpathItem.ConfigLevel)
				{
					case EnumConfigLevel.Environment:
						value = validationResult.EnvironmentResultList.First(i => i.FieldName == xpathItem.ValueName).FieldValue;
						break;
					case EnumConfigLevel.Machine:
						value = machineResult.First(i => i.FieldName == xpathItem.ValueName).FieldValue;
						break;
					case EnumConfigLevel.Build:
						value = this.GetBuildParameterValue(xpathItem.ValueName, build);
						break;
					default:
						throw new UnknownEnumValueException(xpathItem.ConfigLevel);
				}
				this.UpdateXmlNode(xmlDoc, xpathItem.XPath, value);
			}
			string outputDirectory = runtimeSystemSettings.GetLocalMachineComponentDirectory(machine.MachineName, component.Id);
			if(!Directory.Exists(outputDirectory))
			{
				Directory.CreateDirectory(outputDirectory);
			}
			string outputFilePath = Path.Combine(outputDirectory, definition.Options.TargetFileName);
			_fileWriter.WriteText(outputFilePath, xmlDoc.OuterXml, false);
			statusManager.Info(deployStateId, string.Format("Writing XML {0} for machine {1}: {2}", definition.Options.TargetFileName, machine.MachineName, xmlDoc.OuterXml));
			statusManager.Info(deployStateId, string.Format("Done configuring {0} for machine {1}", definition.Options.TargetFileName, machine.MachineName));
		}

		private void UpdateXmlNode(XmlDocument xmlDoc, string xpath, string value)
		{
			var node = xmlDoc.SelectSingleNode(xpath);
			if (node == null)
			{
				throw new Exception("Node not found: " + xpath);
			}
			node.InnerText = value;
		}

	}
}