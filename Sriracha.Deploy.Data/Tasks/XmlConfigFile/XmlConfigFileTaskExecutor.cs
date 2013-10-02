using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using MMDB.Shared;
using Newtonsoft.Json;
using Sriracha.Deploy.Data.Dto;

namespace Sriracha.Deploy.Data.Tasks.XmlConfigFile
{
	public class XmlConfigFileTaskExecutor : BaseDeployTaskExecutor<XmlConfigFileTaskDefinition>
	{
		private readonly IFileWriter _fileWriter;
		private readonly IDeploymentValidator _validator;

		public XmlConfigFileTaskExecutor(IFileWriter fileWriter, IDeploymentValidator validator)
		{
			_fileWriter = DIHelper.VerifyParameter(fileWriter);
			_validator = DIHelper.VerifyParameter(validator);
		}

		protected override DeployTaskExecutionResult InternalExecute(string deployStateId, IDeployTaskStatusManager statusManager, XmlConfigFileTaskDefinition definition, DeployEnvironmentConfiguration environmentComponent, DeployMachine machine, RuntimeSystemSettings runtimeSystemSettings)
		{
			statusManager.Info(deployStateId, string.Format("Starting XmlConfigTask for {0} ", definition.Options.TargetFileName));
			var result = new DeployTaskExecutionResult();
			var validationResult = _validator.ValidateMachineTaskDefinition(definition, environmentComponent, machine);
			if (validationResult.Status != EnumRuntimeValidationStatus.Success)
			{
				throw new InvalidOperationException("Validation not complete:" + Environment.NewLine + JsonConvert.SerializeObject(validationResult));
			}
			this.ExecuteMachine(deployStateId, statusManager, definition, environmentComponent, machine, runtimeSystemSettings, validationResult);

			statusManager.Info(deployStateId, string.Format("Done XmlConfigTask for {0} ", definition.Options.TargetFileName));
			return statusManager.BuildResult();
		}

		private void ExecuteMachine(string deployStateId, IDeployTaskStatusManager statusManager, XmlConfigFileTaskDefinition definition, DeployEnvironmentConfiguration environmentComponent, DeployMachine machine, RuntimeSystemSettings runtimeSystemSettings, TaskDefinitionValidationResult validationResult)
		{
			statusManager.Info(deployStateId, string.Format("Configuring {0} for machine {1}", definition.Options.TargetFileName, machine.MachineName));
			var machineResult = validationResult.MachineResultList[machine.Id];
			var xmlDoc = new XmlDocument();
			xmlDoc.LoadXml(definition.Options.XmlTemplate);
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
					default:
						throw new UnknownEnumValueException(xpathItem.ConfigLevel);
				}
				this.UpdateXmlNode(xmlDoc, xpathItem.XPath, value);
			}
			string outputDirectory = runtimeSystemSettings.GetLocalMachineComponentDirectory(machine.MachineName, environmentComponent.ParentId);
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
