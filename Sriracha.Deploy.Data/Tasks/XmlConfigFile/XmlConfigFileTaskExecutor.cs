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

		public XmlConfigFileTaskExecutor(IFileWriter fileWriter)
		{
			_fileWriter = DIHelper.VerifyParameter(fileWriter);
		}

		protected override void InternalExecute(XmlConfigFileTaskDefinition definition, DeployEnvironmentComponent environmentComponent, RuntimeSystemSettings runtimeSystemSettings)
		{
			var validationResult = definition.ValidateRuntimeValues(environmentComponent);
			if (validationResult.Status != EnumRuntimeValidationStatus.Success)
			{
				throw new InvalidOperationException("Validation not complete:" + Environment.NewLine + JsonConvert.SerializeObject(validationResult));
			}
			foreach (var machine in environmentComponent.MachineList)
			{
				this.ExecuteMachine(definition, environmentComponent, machine, runtimeSystemSettings, validationResult);
			}
		}

		private void ExecuteMachine(XmlConfigFileTaskDefinition definition, DeployEnvironmentComponent environmentComponent, DeployMachine machine, RuntimeSystemSettings runtimeSystemSettings, RuntimeValidationResult validationResult)
		{
			var machineResult = validationResult.MachineResultList[machine.MachineName];
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
			string outputDirectory = runtimeSystemSettings.GetLocalMachineDirectory(machine.MachineName);
			string outputFilePath = Path.Combine(outputDirectory, definition.Options.TargetFileName);
			_fileWriter.WriteText(outputFilePath, xmlDoc.OuterXml, false);
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
