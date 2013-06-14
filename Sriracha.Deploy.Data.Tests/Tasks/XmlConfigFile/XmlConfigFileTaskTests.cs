using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Tasks;
using Sriracha.Deploy.Data.Tasks.XmlConfigFile;

namespace Sriracha.Deploy.Data.Tests.Tasks.XmlConfigFile
{
	public class XmlConfigFileTaskTests
	{
		private class TestData
		{
			public XmlConfigFileTask Sut { get; set; }
			public DeployEnvironmentComponent EnvironmentComponent { get; set; }
			public static TestData Create()
			{
				Fixture fixture = new Fixture();
				TestData returnValue = new TestData()
				{
					EnvironmentComponent = new DeployEnvironmentComponent
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
					Sut = new XmlConfigFileTask()
					{
						Options = new XmlConfigFileTaskOptions
						{
							XmlTemplate = new StringBuilder()
											.Append("<container>")
												.Append("<enviornmentValue></enviornmentValue>")
												.Append("<serverValue/>")
												.Append("<staticValue theValue=\"\"/>")
											.Append("</container>")
										.ToString(),
							XPathValueList = new List<XmlConfigFileTaskOptions.XPathValueItem>
							{
								new XmlConfigFileTaskOptions.XPathValueItem 
								{
									XPath = "/container/environmentValue",
									ConfigLevel = EnumConfigLevel.Environment,
									ValueName = fixture.Create<string>()
								},
								new XmlConfigFileTaskOptions.XPathValueItem 
								{
									XPath = "/container/serverValue",
									ConfigLevel = EnumConfigLevel.Machine,
									ValueName = fixture.Create<string>()
								},
								new XmlConfigFileTaskOptions.XPathValueItem 
								{
									XPath = "/container/staticValue/@theValue",
									ConfigLevel = EnumConfigLevel.Static,
									ValueName = fixture.Create<string>()
								}
							}
						}
					}
				};
				foreach(var configItem in returnValue.Sut.Options.XPathValueList)
				{
					if(configItem.ConfigLevel == EnumConfigLevel.Environment)
					{
						returnValue.EnvironmentComponent.ConfigurationValueList.Add(configItem.ValueName, fixture.Create<string>());
					}
					else if (configItem.ConfigLevel == EnumConfigLevel.Machine)
					{
						foreach(var machine in returnValue.EnvironmentComponent.MachineList)
						{
							machine.ConfigurationValueList.Add(configItem.ValueName, fixture.Create<string>());
						}
					}
				}
				return returnValue;
			}

		}

		[Test]
		public void CanCreateXmlConfigTask()
		{
			var fixture = new Fixture();
			string fieldName = fixture.Create<string>();
			var task = new XmlConfigFileTask()
			{
				Options = new XmlConfigFileTaskOptions
				{
					XmlTemplate = new StringBuilder()
									.Append("<container>")
										.Append("<environmentValue></environmentValue>")
									.Append("</container>")
								.ToString(),
					TargetFileName = fixture.Create<string>(),
					XPathValueList = new List<XmlConfigFileTaskOptions.XPathValueItem>
					{
						new XmlConfigFileTaskOptions.XPathValueItem 
						{
							XPath = "/container/enviornmentValue",
							ConfigLevel = EnumConfigLevel.Environment,
							ValueName = fieldName
						}
					}
				}
			};
		}

		[Test]
		public void CanFindEnvironmentValues()
		{
			var testData = TestData.Create();
			var environmentSettings = testData.Sut.GetEnvironmentTaskParameterList();
			Assert.IsNotNull(environmentSettings);
			Assert.AreEqual(1, environmentSettings.Count);
			Assert.AreEqual(testData.Sut.Options.XPathValueList.First(i=>i.ConfigLevel==EnumConfigLevel.Environment).ValueName, environmentSettings[0].FieldName);
			Assert.AreEqual(EnumTaskParameterType.String, environmentSettings[0].FieldType);
		}

		[Test]
		public void CanFindMachineValues()
		{
			var testData = TestData.Create();
			var environmentSettings = testData.Sut.GetMachineTaskParameterList();
			Assert.IsNotNull(environmentSettings);
			Assert.AreEqual(1, environmentSettings.Count);
			Assert.AreEqual(testData.Sut.Options.XPathValueList.First(i => i.ConfigLevel == EnumConfigLevel.Machine).ValueName, environmentSettings[0].FieldName);
			Assert.AreEqual(EnumTaskParameterType.String, environmentSettings[0].FieldType);
		}

		[Test]
		public void CanFindStaticValues()
		{
			var testData = TestData.Create();
			var environmentSettings = testData.Sut.GetStaticTaskParameterList();
			Assert.IsNotNull(environmentSettings);
			Assert.AreEqual(1, environmentSettings.Count);
			Assert.AreEqual(testData.Sut.Options.XPathValueList.First(i => i.ConfigLevel == EnumConfigLevel.Static).ValueName, environmentSettings[0].FieldName);
			Assert.AreEqual(EnumTaskParameterType.String, environmentSettings[0].FieldType);
		}

		[Test]
		public void CompleteConfig_PassesValidation()
		{
			var testData = TestData.Create();
			var result = testData.Sut.ValidateRuntimeValues(testData.EnvironmentComponent);
			Assert.AreEqual(EnumRuntimeValidationStatus.Success, result.Status);
			Assert.IsNotNull(result);
		}

		[Test]
		public void MissingEnvironmentConfig_FailsValidation()
		{
			var testData = TestData.Create();
			testData.EnvironmentComponent.ConfigurationValueList.Remove(testData.EnvironmentComponent.ConfigurationValueList.Keys.First());
			var result = testData.Sut.ValidateRuntimeValues(testData.EnvironmentComponent);
			Assert.AreEqual(EnumRuntimeValidationStatus.Incomplete, result.Status);
			Assert.IsNotNull(result);
		}

		[Test]
		public void MissingMachinConfig_FailsValidation()
		{
			var testData = TestData.Create();
			testData.EnvironmentComponent.MachineList[0].ConfigurationValueList.Remove(testData.EnvironmentComponent.MachineList[0].ConfigurationValueList.Keys.First());
			var result = testData.Sut.ValidateRuntimeValues(testData.EnvironmentComponent);
			Assert.AreEqual(EnumRuntimeValidationStatus.Incomplete, result.Status);
			Assert.IsNotNull(result);
		}
	}
}
