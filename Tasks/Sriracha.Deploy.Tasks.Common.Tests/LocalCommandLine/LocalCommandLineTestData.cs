﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using Ploeh.AutoFixture;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Tasks;
using Sriracha.Deploy.Data.Build;
using Sriracha.Deploy.Data.Deployment;
using Sriracha.Deploy.Data.Utility;
using Sriracha.Deploy.Data.Credentials;
using Sriracha.Deploy.Data.Dto.Project;
using Sriracha.Deploy.Data.Dto.Build;
using Sriracha.Deploy.Tasks.Common.LocalCommandLine;

namespace Sriracha.Deploy.Tasks.Common.Tests.LocalCommandLine
{
	public class LocalCommandLineTestData
	{
		public LocalCommandLineTaskDefinition TaskDefinition { get; set; }
		public LocalCommandLineTaskExecutor TaskExecutor { get; set; }
		public Mock<IParameterParser> ParameterParser { get; set; }
		public Mock<IProcessRunner> ProcessRunner { get; set; }
		public Mock<IDeployTaskStatusManager> StatusManager { get; set; }
		public Mock<IDeploymentValidator> Validator { get; set; }
		public Mock<ICredentialsManager> CredentialsManager { get; set; }
		public Mock<IImpersonator> Impersonator { get; set; }
		public Mock<IParameterEvaluator> BuildParameterEvaluator { get; set; }
		public RuntimeSystemSettings RuntimeSystemSettings { get; set; }
		public List<string> MachineParameters { get; set; }
		public List<string> EnvironmentParameters { get; set; }
		public string DeployStateId { get; set; }
		public DeployComponent Component { get; set; }
		public DeployEnvironmentConfiguration EnvironmentComponent { get; set; }
		public DeployBuild Build { get; set; }

		public static LocalCommandLineTestData Create()
		{
			var fixture = new Fixture();
			var returnValue = new LocalCommandLineTestData
			{
				DeployStateId = fixture.Create<string>(),
				MachineParameters = new List<string>() {"MachineParameter1", "MachineParameter2"},
				EnvironmentParameters = new List<string>(){"EnvironmentParameter1", "EnvironmentParameter2"},
				ParameterParser = new Mock<IParameterParser>(),
				StatusManager = new Mock<IDeployTaskStatusManager>(),
				RuntimeSystemSettings = new RuntimeSystemSettings(),
				ProcessRunner = new Mock<IProcessRunner>(),
				CredentialsManager = new Mock<ICredentialsManager>(),
				Impersonator = new Mock<IImpersonator>(),
				Validator = new Mock<IDeploymentValidator>(),
				BuildParameterEvaluator = new Mock<IParameterEvaluator>(),
				Component = new DeployComponent
				{
					Id = Guid.NewGuid().ToString()
				},
				EnvironmentComponent = new DeployEnvironmentConfiguration
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
				},
				Build = new DeployBuild()
			};
			returnValue.TaskExecutor = new LocalCommandLineTaskExecutor(returnValue.ProcessRunner.Object, returnValue.Validator.Object, returnValue.BuildParameterEvaluator.Object, returnValue.CredentialsManager.Object, returnValue.Impersonator.Object);
			returnValue.TaskDefinition = new LocalCommandLineTaskDefinition(returnValue.ParameterParser.Object)
			{
				Options = new LocalCommandLineTaskOptions
				{
					ExecutablePath = fixture.Create<string>(),
					ExecutableArguments = "this is the arguments ${machine:MachineParameter1} ${machine:MachineParameter2} ${env:EnvironmentParameter1} ${env:EnvironmentParameter2}"
				}
			};
            returnValue.ParameterParser.Setup(i => i.FindEnvironmentParameters(returnValue.TaskDefinition.Options.ExecutableArguments)).Returns(returnValue.EnvironmentParameters);
            returnValue.ParameterParser.Setup(i => i.FindNestedEnvironmentParameters(returnValue.TaskDefinition.Options)).Returns(returnValue.EnvironmentParameters.Select(i=>new TaskParameter { FieldName=i, FieldType = EnumTaskParameterType.String}).ToList());
            returnValue.ParameterParser.Setup(i => i.FindMachineParameters(returnValue.TaskDefinition.Options.ExecutableArguments)).Returns(returnValue.MachineParameters);
            returnValue.ParameterParser.Setup(i => i.FindNestedMachineParameters(returnValue.TaskDefinition.Options)).Returns(returnValue.MachineParameters.Select(i => new TaskParameter { FieldName = i, FieldType = EnumTaskParameterType.String }).ToList());
            return returnValue;
		}
	}
}
