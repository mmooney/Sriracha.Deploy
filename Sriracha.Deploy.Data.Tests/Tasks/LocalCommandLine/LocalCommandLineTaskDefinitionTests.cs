using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Sriracha.Deploy.Data.Tasks;

namespace Sriracha.Deploy.Data.Tests.Tasks.LocalCommandLine
{
	public class LocalCommandLineTaskDefinitionTests
	{
		[Test]
		public void CanGetEnvironmentConfig()
		{
			var testData = LocalCommandLineTestData.Create();

			var environmentParameters = testData.TaskDefinition.GetEnvironmentTaskParameterList();

			Assert.AreEqual(testData.EnvironmentParameters.Count, environmentParameters.Count);
			for(int i = 0; i < testData.EnvironmentParameters.Count; i++)
			{
				Assert.AreEqual(testData.EnvironmentParameters[i], environmentParameters[i].FieldName);
				Assert.AreEqual(EnumTaskParameterType.String, environmentParameters[i].FieldType);
				Assert.AreEqual(false, environmentParameters[i].Sensitive);
			}
		}

		[Test]
		public void CanRecognizeSensitiveEnvironmentConfig()
		{
			var testData = LocalCommandLineTestData.Create();
			testData.EnvironmentParameters[0] = "SENSITIVE:" + testData.EnvironmentParameters[0];
			var environmentParameters = testData.TaskDefinition.GetEnvironmentTaskParameterList();

			Assert.AreEqual(testData.EnvironmentParameters.Count, environmentParameters.Count);
			for (int i = 0; i < testData.EnvironmentParameters.Count; i++)
			{
				if(testData.EnvironmentParameters[i].StartsWith("SENSITIVE:"))
				{
					Assert.AreEqual(testData.EnvironmentParameters[i].Substring("SENSITIVE:".Length), environmentParameters[i].FieldName);
					Assert.AreEqual(EnumTaskParameterType.String, environmentParameters[i].FieldType);
					Assert.AreEqual(true, environmentParameters[i].Sensitive);
				}
				else 
				{
					Assert.AreEqual(testData.EnvironmentParameters[i], environmentParameters[i].FieldName);
					Assert.AreEqual(EnumTaskParameterType.String, environmentParameters[i].FieldType);
					Assert.AreEqual(false, environmentParameters[i].Sensitive);
				}
			}
		}

		[Test]
		public void CanGetMachineConfig()
		{
			var testData = LocalCommandLineTestData.Create();

			var machineParameters = testData.TaskDefinition.GetMachineTaskParameterList();

			Assert.AreEqual(testData.MachineParameters.Count, machineParameters.Count);
			for (int i = 0; i < testData.MachineParameters.Count; i++)
			{
				Assert.AreEqual(testData.MachineParameters[i], machineParameters[i].FieldName);
				Assert.AreEqual(EnumTaskParameterType.String, machineParameters[i].FieldType);
				Assert.AreEqual(false, machineParameters[i].Sensitive);
			}
		}

		[Test]
		public void CanRecognizeSensitiveMachineConfig()
		{
			var testData = LocalCommandLineTestData.Create();
			testData.MachineParameters[0] = "SENSITIVE:" + testData.MachineParameters[0];
			var machineParameters = testData.TaskDefinition.GetMachineTaskParameterList();

			Assert.AreEqual(testData.MachineParameters.Count, machineParameters.Count);
			for (int i = 0; i < testData.MachineParameters.Count; i++)
			{
				if (testData.MachineParameters[i].StartsWith("SENSITIVE:"))
				{
					Assert.AreEqual(testData.MachineParameters[i].Substring("SENSITIVE:".Length), machineParameters[i].FieldName);
					Assert.AreEqual(EnumTaskParameterType.String, machineParameters[i].FieldType);
					Assert.AreEqual(true, machineParameters[i].Sensitive);
				}
				else
				{
					Assert.AreEqual(testData.MachineParameters[i], machineParameters[i].FieldName);
					Assert.AreEqual(EnumTaskParameterType.String, machineParameters[i].FieldType);
					Assert.AreEqual(false, machineParameters[i].Sensitive);
				}
			}
		}
	}
}
