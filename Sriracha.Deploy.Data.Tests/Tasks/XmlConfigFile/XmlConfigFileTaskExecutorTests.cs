using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using XmlUnit;

namespace Sriracha.Deploy.Data.Tests.Tasks.XmlConfigFile
{
	[Ignore]
	public class XmlConfigFileTaskExecutorTests
	{
		[Test]
		public void ExecuteWithValidConfiguration_CreatesXmlFiles()
		{
			var testData = XmlConfigFileTaskTestData.Create();
			testData.TaskExecutor.Execute(testData.DeployStateId, testData.StatusManager.Object, testData.TaskDefinition, testData.EnvironmentComponent, testData.EnvironmentComponent.MachineList.First(), testData.RuntimeSystemSettings);
			foreach (var machine in testData.EnvironmentComponent.MachineList)
			{
				string outputPath = Path.Combine(testData.RuntimeSystemSettings.GetLocalMachineComponentDirectory(machine.MachineName, testData.EnvironmentComponent.ParentId), testData.TaskDefinition.Options.TargetFileName);
				string expectedResult = testData.ExpectedResult[machine.MachineName];
				//testData.FileWriter.Verify(i => i.WriteText(outputPath, expectedResult, false), Times.Once());
				testData.FileWriter.Verify(i => i.WriteText(outputPath, It.Is<string>(j => CompareXml(expectedResult, j)), false), Times.AtLeastOnce());
			}
		}

		[Test]
		public void ExecuteWithInvalidConfiguration_ThrowsException()
		{
			var testData = XmlConfigFileTaskTestData.Create();
			testData.EnvironmentComponent.ConfigurationValueList = new Dictionary<string, string>();
			Assert.Throws<InvalidOperationException>(() => testData.TaskExecutor.Execute(testData.DeployStateId, testData.StatusManager.Object, testData.TaskDefinition, testData.EnvironmentComponent, testData.EnvironmentComponent.MachineList.First(), testData.RuntimeSystemSettings));
		}

		private bool CompareXml(string expectedResult, string actualResult)
		{
			XmlDiff diff = new XmlDiff(expectedResult, actualResult);
			return diff.Compare().Equal;
		}
	}
}
