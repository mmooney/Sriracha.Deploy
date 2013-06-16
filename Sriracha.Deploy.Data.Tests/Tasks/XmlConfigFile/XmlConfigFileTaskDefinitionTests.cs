using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Sriracha.Deploy.Data.Tasks;
using Sriracha.Deploy.Data.Tasks.XmlConfigFile;

namespace Sriracha.Deploy.Data.Tests.Tasks.XmlConfigFile
{
	public class XmlConfigFileTaskDefinitionTests
	{
		[Test]
		public void CanCreateXmlConfigTask()
		{
			var fixture = new Fixture();
			string fieldName = fixture.Create<string>();
			var task = new XmlConfigFileTaskDefinition()
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
							XPath = "/container/environmentValue",
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
			var testData = XmlConfigFileTaskTestData.Create();
			var environmentSettings = testData.TaskDefinition.GetEnvironmentTaskParameterList();
			Assert.IsNotNull(environmentSettings);
			Assert.AreEqual(1, environmentSettings.Count);
			Assert.AreEqual(testData.TaskDefinition.Options.XPathValueList.First(i=>i.ConfigLevel==EnumConfigLevel.Environment).ValueName, environmentSettings[0].FieldName);
			Assert.AreEqual(EnumTaskParameterType.String, environmentSettings[0].FieldType);
		}

		[Test]
		public void CanFindMachineValues()
		{
			var testData = XmlConfigFileTaskTestData.Create();
			var environmentSettings = testData.TaskDefinition.GetMachineTaskParameterList();
			Assert.IsNotNull(environmentSettings);
			Assert.AreEqual(1, environmentSettings.Count);
			Assert.AreEqual(testData.TaskDefinition.Options.XPathValueList.First(i => i.ConfigLevel == EnumConfigLevel.Machine).ValueName, environmentSettings[0].FieldName);
			Assert.AreEqual(EnumTaskParameterType.String, environmentSettings[0].FieldType);
		}

		//[Test]
		//public void CanFindStaticValues()
		//{
		//	var testData = TestData.Create();
		//	var environmentSettings = testData.Sut.GetStaticTaskParameterList();
		//	Assert.IsNotNull(environmentSettings);
		//	Assert.AreEqual(1, environmentSettings.Count);
		//	Assert.AreEqual(testData.Sut.Options.XPathValueList.First(i => i.ConfigLevel == EnumConfigLevel.Static).ValueName, environmentSettings[0].FieldName);
		//	Assert.AreEqual(EnumTaskParameterType.String, environmentSettings[0].FieldType);
		//}

		[Test]
		public void CompleteConfig_PassesValidation()
		{
			var testData = XmlConfigFileTaskTestData.Create();
			var result = testData.TaskDefinition.ValidateRuntimeValues(testData.EnvironmentComponent);
			Assert.AreEqual(EnumRuntimeValidationStatus.Success, result.Status);
			Assert.IsNotNull(result);
		}

		[Test]
		public void MissingEnvironmentConfig_FailsValidation()
		{
			var testData = XmlConfigFileTaskTestData.Create();
			testData.EnvironmentComponent.ConfigurationValueList.Remove(testData.EnvironmentComponent.ConfigurationValueList.Keys.First());
			var result = testData.TaskDefinition.ValidateRuntimeValues(testData.EnvironmentComponent);
			Assert.AreEqual(EnumRuntimeValidationStatus.Incomplete, result.Status);
			Assert.IsNotNull(result);
		}

		[Test]
		public void MissingMachinConfig_FailsValidation()
		{
			var testData = XmlConfigFileTaskTestData.Create();
			testData.EnvironmentComponent.MachineList[0].ConfigurationValueList.Remove(testData.EnvironmentComponent.MachineList[0].ConfigurationValueList.Keys.First());
			var result = testData.TaskDefinition.ValidateRuntimeValues(testData.EnvironmentComponent);
			Assert.AreEqual(EnumRuntimeValidationStatus.Incomplete, result.Status);
			Assert.IsNotNull(result);
		}
	}
}
