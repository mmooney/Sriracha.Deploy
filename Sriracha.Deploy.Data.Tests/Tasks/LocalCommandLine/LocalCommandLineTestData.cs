using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using Ploeh.AutoFixture;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Tasks;
using Sriracha.Deploy.Data.Tasks.LocalCommandLine;

namespace Sriracha.Deploy.Data.Tests.Tasks.LocalCommandLine
{
	public class LocalCommandLineTestData
	{
		public LocalCommandLineTaskDefinition TaskDefinition { get; set; }
		public LocalCommandLineTaskExecutor TaskExecutor { get; set; }
		public Mock<IParameterParser> ParameterParser { get; set; }
		public Mock<IProcessRunner> ProcessRunner { get; set; }
		public Mock<IDeployTaskStatusManager> StatusManager { get; set; }
		public Mock<IDeploymentValidator> Validator { get; set; }
		public RuntimeSystemSettings RuntimeSystemSettings { get; set; }
		public List<string> MachineParameters { get; set; }
		public List<string> EnvironmentParameters { get; set; }
		public DeployEnvironmentComponent EnvironmentComponent { get; set; }

		public static LocalCommandLineTestData Create()
		{
			var fixture = new Fixture();
			var returnValue = new LocalCommandLineTestData
			{
				MachineParameters = new List<string>() {"MachineParameter1", "MachineParameter2"},
				EnvironmentParameters = new List<string>(){"EnvironmentParameter1", "EnvironmentParameter2"},
				ParameterParser = new Mock<IParameterParser>(),
				StatusManager = new Mock<IDeployTaskStatusManager>(),
				RuntimeSystemSettings = new RuntimeSystemSettings(),
				ProcessRunner = new Mock<IProcessRunner>(),
				Validator = new Mock<IDeploymentValidator>(),
				EnvironmentComponent = new DeployEnvironmentComponent
				{
					ConfigurationValueList = new Dictionary<string,string>
					{
						{"EnvironmentParameter1", fixture.Create<string>()},
						{"EnvironmentParameter2", fixture.Create<string>()}
					},
					MachineList = new List<DeployMachine>()
					{
						new DeployMachine
						{
							MachineName = fixture.Create<string>(),
							ConfigurationValueList = new Dictionary<string,string>
							{
								{"MachineParameter1", fixture.Create<string>()},
								{"MachineParameter2", fixture.Create<string>()}
							}
						}
					}
				}
			};
			returnValue.TaskExecutor = new LocalCommandLineTaskExecutor(returnValue.ProcessRunner.Object, returnValue.Validator.Object);
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
