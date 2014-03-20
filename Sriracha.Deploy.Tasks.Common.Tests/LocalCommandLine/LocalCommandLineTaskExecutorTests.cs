using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;

namespace Sriracha.Deploy.Tasks.Common.Tests.LocalCommandLine
{
	[Ignore]
	public class LocalCommandLineTaskExecutorTests
	{
		[Test]
		public void ExecuteWithValidConfiguration_CallsProcessRunner()
		{
			var testData = LocalCommandLineTestData.Create();
			var result = testData.TaskExecutor.Execute(testData.DeployStateId, testData.StatusManager.Object, testData.TaskDefinition, testData.Component, testData.EnvironmentComponent, testData.EnvironmentComponent.MachineList.First(), testData.Build, testData.RuntimeSystemSettings);
			foreach (var machine in testData.EnvironmentComponent.MachineList)
			{
				string expectedParameters = testData.TaskDefinition.Options.ExecutableArguments
												.Replace("${machine:MachineParameter1}", machine.ConfigurationValueList["MachineParameter1"])
												.Replace("${machine:MachineParameter2}", machine.ConfigurationValueList["MachineParameter2"])
												.Replace("${env:EnvironmentParameter1}", testData.EnvironmentComponent.ConfigurationValueList["EnvironmentParameter1"])
												.Replace("${env:EnvironmentParameter2}", testData.EnvironmentComponent.ConfigurationValueList["EnvironmentParameter2"]);
				testData.ProcessRunner.Verify(i => i.Run(testData.TaskDefinition.Options.ExecutablePath, expectedParameters, It.IsAny<TextWriter>(), It.IsAny<TextWriter>()), Times.Once());
			}
		}

		[Test]
		public void ExecuteWithInvalidConfiguration_ThrowsException()
		{
			var testData = LocalCommandLineTestData.Create();
			testData.EnvironmentComponent.ConfigurationValueList = new Dictionary<string, string>();
			Assert.Throws<InvalidOperationException>(() => testData.TaskExecutor.Execute(testData.DeployStateId, testData.StatusManager.Object, testData.TaskDefinition, testData.Component, testData.EnvironmentComponent, testData.EnvironmentComponent.MachineList.First(), testData.Build, testData.RuntimeSystemSettings));
		}

		[Test]
		public void MasksSensitiveEnvironmentValues()
		{
			var testData = LocalCommandLineTestData.Create();
			testData.EnvironmentParameters = new List<string>{"sensitive:EnvironmentParameter1", "EnvironmentParameter2"};
			testData.TaskDefinition.Options.ExecutableArguments = testData.TaskDefinition.Options.ExecutableArguments.Replace("${env:EnvironmentParameter1}", "${env:sensitive:EnvironmentParameter1}");
			testData.ParameterParser.Setup(i => i.FindEnvironmentParameters(testData.TaskDefinition.Options.ExecutableArguments)).Returns(testData.EnvironmentParameters);
			testData.ParameterParser.Setup(i => i.FindMachineParameters(testData.TaskDefinition.Options.ExecutableArguments)).Returns(testData.MachineParameters);
			var result = testData.TaskExecutor.Execute(testData.DeployStateId, testData.StatusManager.Object, testData.TaskDefinition, testData.Component, testData.EnvironmentComponent, testData.EnvironmentComponent.MachineList.First(), testData.Build, testData.RuntimeSystemSettings);
			foreach (var machine in testData.EnvironmentComponent.MachineList)
			{
				string expectedParameters = testData.TaskDefinition.Options.ExecutableArguments
												.Replace("${machine:MachineParameter1}", machine.ConfigurationValueList["MachineParameter1"])
												.Replace("${machine:MachineParameter2}", machine.ConfigurationValueList["MachineParameter2"])
												.Replace("${env:sensitive:EnvironmentParameter1}", testData.EnvironmentComponent.ConfigurationValueList["EnvironmentParameter1"])
												.Replace("${env:EnvironmentParameter2}", testData.EnvironmentComponent.ConfigurationValueList["EnvironmentParameter2"]);
				testData.ProcessRunner.Verify(i => i.Run(testData.TaskDefinition.Options.ExecutablePath, expectedParameters, It.IsAny<TextWriter>(), It.IsAny<TextWriter>()), Times.Once());
			}
		}

		[Test]
		public void MasksSensitiveMachineValues()
		{
			var testData = LocalCommandLineTestData.Create();
			testData.MachineParameters = new List<string> { "sensitive:MachineParameter1", "MachineParameter2" };
			testData.TaskDefinition.Options.ExecutableArguments = testData.TaskDefinition.Options.ExecutableArguments.Replace("${machine:MachineParameter1}", "${machine:sensitive:MachineParameter1}");
			testData.ParameterParser.Setup(i => i.FindEnvironmentParameters(testData.TaskDefinition.Options.ExecutableArguments)).Returns(testData.EnvironmentParameters);
			testData.ParameterParser.Setup(i => i.FindMachineParameters(testData.TaskDefinition.Options.ExecutableArguments)).Returns(testData.MachineParameters);
			var result = testData.TaskExecutor.Execute(testData.DeployStateId, testData.StatusManager.Object, testData.TaskDefinition, testData.Component, testData.EnvironmentComponent, testData.EnvironmentComponent.MachineList.First(), testData.Build, testData.RuntimeSystemSettings);
			foreach (var machine in testData.EnvironmentComponent.MachineList)
			{
				string expectedParameters = testData.TaskDefinition.Options.ExecutableArguments
												.Replace("${machine:sensitive:MachineParameter1}", machine.ConfigurationValueList["MachineParameter1"])
												.Replace("${machine:MachineParameter2}", machine.ConfigurationValueList["MachineParameter2"])
												.Replace("${env:EnvironmentParameter1}", testData.EnvironmentComponent.ConfigurationValueList["EnvironmentParameter1"])
												.Replace("${env:EnvironmentParameter2}", testData.EnvironmentComponent.ConfigurationValueList["EnvironmentParameter2"]);
				testData.ProcessRunner.Verify(i => i.Run(testData.TaskDefinition.Options.ExecutablePath, expectedParameters, It.IsAny<TextWriter>(), It.IsAny<TextWriter>()), Times.Once());
			}
		}
	}
}
