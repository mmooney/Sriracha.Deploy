using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using Ploeh.AutoFixture;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Tasks;
using Sriracha.Deploy.Data.Tasks.XmlConfigFile;
using Sriracha.Deploy.Data.Build;
using Sriracha.Deploy.Data.Deployment;
using Sriracha.Deploy.Data.Utility;
using Sriracha.Deploy.Data.Dto.Project;

namespace Sriracha.Deploy.Data.Tests.Tasks.XmlConfigFile
{
	public class XmlConfigFileTaskTestData
	{
		public XmlConfigFileTaskDefinition TaskDefinition { get; set; }
		public XmlConfigFileTaskExecutor TaskExecutor { get; set; }
		public DeployComponent Component { get; set; }
		public DeployEnvironmentConfiguration EnvironmentComponent { get; set; }
		public DeployBuild Build { get; set; }
		public RuntimeSystemSettings RuntimeSystemSettings { get; set; }
		public Mock<IFileWriter> FileWriter { get; set; }
		public Mock<IDeploymentValidator> Validator { get; set; }
		public Mock<IDeployTaskStatusManager> StatusManager { get; set; }
		public Mock<IBuildParameterEvaluator> BuildParameterEvaluator { get; set; } 
		public Dictionary<string, string> ExpectedResult { get; set; }
		public string DeployStateId { get; set; }

		public static XmlConfigFileTaskTestData Create()
		{
			Fixture fixture = new Fixture();
			XmlConfigFileTaskTestData returnValue = new XmlConfigFileTaskTestData()
			{
				FileWriter = new Mock<IFileWriter>(),
				DeployStateId = fixture.Create<string>(),
				StatusManager = new Mock<IDeployTaskStatusManager>(),
				BuildParameterEvaluator = new Mock<IBuildParameterEvaluator>(),
				RuntimeSystemSettings = fixture.Create<RuntimeSystemSettings>(),
				Validator = new Mock<IDeploymentValidator>(),
				Component = fixture.Create<DeployComponent>(),
				EnvironmentComponent = new DeployEnvironmentConfiguration
				{
					MachineList = new List<DeployMachine>
						{
							new DeployMachine
							{
								MachineName = fixture.Create<string>()
							},
							new DeployMachine
							{
								MachineName = fixture.Create<string>()
							}
						}
				},
				Build = new DeployBuild(),
				ExpectedResult = new Dictionary<string, string>()
			};
			returnValue.TaskDefinition = new XmlConfigFileTaskDefinition()
			{
				Options = new XmlConfigFileTaskOptions
				{
					XmlTemplate = new StringBuilder()
									.Append("<container>")
										.Append("<environmentValue></environmentValue>")
										.Append("<serverValue theValue=\"\"/>")
										.Append("<staticValue theValue=\"\"/>")
									.Append("</container>")
								.ToString(),
					TargetFileName = fixture.Create<string>(),
					XPathValueList = new List<XmlConfigFileTaskOptions.XPathValueItem>
						{
							new XmlConfigFileTaskOptions.XPathValueItem 
							{
								XPath = "/container/environmentValue",
								ConfigLevel = EnumConfigLevel.Environment,
								ValueName = "environmentValue1"
							},
							new XmlConfigFileTaskOptions.XPathValueItem 
							{
								XPath = "/container/serverValue/@theValue",
								ConfigLevel = EnumConfigLevel.Machine,
								ValueName = "machineValue1"
							},
							//new XmlConfigFileTaskOptions.XPathValueItem 
							//{
							//	XPath = "/container/staticValue/@theValue",
							//	ConfigLevel = EnumConfigLevel.Static,
							//	ValueName = "staticValue1"
							//}
						}
				}
			};
			returnValue.TaskExecutor = new XmlConfigFileTaskExecutor(returnValue.FileWriter.Object, returnValue.Validator.Object, returnValue.BuildParameterEvaluator.Object);
			foreach (var configItem in returnValue.TaskDefinition.Options.XPathValueList)
			{
				if (configItem.ConfigLevel == EnumConfigLevel.Environment)
				{
					returnValue.EnvironmentComponent.ConfigurationValueList.Add(configItem.ValueName, fixture.Create<string>());
				}
				else if (configItem.ConfigLevel == EnumConfigLevel.Machine)
				{
					foreach (var machine in returnValue.EnvironmentComponent.MachineList)
					{
						machine.ConfigurationValueList.Add(configItem.ValueName, fixture.Create<string>());
					}
				}
			}
			foreach (var machine in returnValue.EnvironmentComponent.MachineList)
			{
				string environmentValue = returnValue.EnvironmentComponent.ConfigurationValueList["environmentValue1"];
				string machineValue = machine.ConfigurationValueList["machineValue1"];
				string expectedValue = new StringBuilder()
											.Append("<container>")
												.AppendFormat("<environmentValue>{0}</environmentValue>", environmentValue)
												.AppendFormat("<serverValue theValue=\"{0}\"/>", machineValue)
												.Append("<staticValue theValue=\"\"/>")
											.Append("</container>")
										.ToString();
				returnValue.ExpectedResult.Add(machine.MachineName, expectedValue);
			}
			return returnValue;
		}

	}
}
