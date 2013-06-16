using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using Ploeh.AutoFixture;
using Sriracha.Deploy.Data.Tasks.LocalCommandLine;

namespace Sriracha.Deploy.Data.Tests.Tasks.LocalCommandLine
{
	public class LocalCommandLineTestData
	{
		public LocalCommandLineTaskDefinition TaskDefinition { get; set; }
		public LocalCommandLineTaskExecutor TaskExecutor { get; set; }
		public Mock<IParameterParser> ParameterParser { get; set; }
		public List<string> MachineParameters { get; set; }
		public List<string> EnvironmentParameters { get; set; }

		public static LocalCommandLineTestData Create()
		{
			var fixture = new Fixture();
			var returnValue = new LocalCommandLineTestData
			{
				MachineParameters = new List<string>() {"MachineParameter1", "MachineParameter2"},
				EnvironmentParameters = new List<string>(){"EnvironmentParameter1", "EnvironmentParamater2"},
				ParameterParser = new Mock<IParameterParser>(),
				TaskExecutor = new LocalCommandLineTaskExecutor()
			};
			returnValue.TaskDefinition = new LocalCommandLineTaskDefinition(returnValue.ParameterParser.Object)
			{
				Options = new LocalCommandLineTaskOptions
				{
					ExecutablePath = fixture.Create<string>(),
					ExecutableArguments = "this is the arguments ${machine:MachineParameter1} ${machine:MachineParameter2} ${env:EnvironmentParameter1} ${env:EnvironmentParameter2}"
				}
			};
			returnValue.ParameterParser.Setup(i=>i.FindEnvironmentParameters(returnValue.TaskDefinition.Options.ExecutableArguments)).Returns(returnValue.EnvironmentParameters);
			returnValue.ParameterParser.Setup(i => i.FindMachineParameters(returnValue.TaskDefinition.Options.ExecutableArguments)).Returns(returnValue.MachineParameters);
			return returnValue;
		}
	}
}
