using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Impl;
using Sriracha.Deploy.Data.Tasks;
using Sriracha.Deploy.Data.Deployment.DeploymentImpl;
using Sriracha.Deploy.Data.Credentials;
using Sriracha.Deploy.Data.Dto.Project;

namespace Sriracha.Deploy.Data.Tests
{
	public class DeployComponentRunnerTests
	{
		private class TestData
		{
			public Mock<IDeployTaskFactory> TaskFactory { get; set; }
			public Mock<IDeployTaskStatusManager> StatusManager { get; set; }
			public TupleList<Mock<IDeployTaskDefinition>, Mock<IDeployTaskExecutor>, Type> TaskDefinitionExecutorList { get; set; }
			public Mock<IImpersonator> Impersonator { get; set; }
			public DeployComponent Component { get; set; }
			public DeployEnvironmentConfiguration EnvironmentComponent { get; set; }
			public DeployBuild Build { get; set; }
			public RuntimeSystemSettings RuntimeSystemSettings { get; set; }
			public DeployComponentRunner Sut { get; set; }
			public string DeployStateId { get; set; }

			public List<IDeployTaskDefinition> GetTaskDefinitionList()
			{
				return this.TaskDefinitionExecutorList.Select(i=>i.Item1.Object).ToList();
			}

			private static Type CreateType(Fixture fixture)
			{
				var myDomain = AppDomain.CurrentDomain;
				var myAsmName = new AssemblyName(fixture.Create<string>());
				var assembyBuilder = myDomain.DefineDynamicAssembly(myAsmName, System.Reflection.Emit.AssemblyBuilderAccess.RunAndSave);
				var moduleBuilder = assembyBuilder.DefineDynamicModule(myAsmName.Name, myAsmName.Name + ".dll");
				var typeBuilder = moduleBuilder.DefineType(fixture.Create<string>(), TypeAttributes.Public);
				return typeBuilder.CreateType();
			}

			public static TestData Create(int taskCount)
			{
				var fixture = new Fixture();
				var returnValue = new TestData
				{
					TaskFactory = new Mock<IDeployTaskFactory>(),
					StatusManager = new Mock<IDeployTaskStatusManager>(),
					Component = fixture.Create<DeployComponent>(),
					EnvironmentComponent = fixture.Create<DeployEnvironmentConfiguration>(),
					Impersonator = new Mock<IImpersonator>(),
					Build = fixture.Create<DeployBuild>(),
					RuntimeSystemSettings = fixture.Create<RuntimeSystemSettings>(),
					TaskDefinitionExecutorList = new TupleList<Mock<IDeployTaskDefinition>,Mock<IDeployTaskExecutor>,Type>(),
					DeployStateId = fixture.Create<string>()
				};
				returnValue.EnvironmentComponent.DeployCredentialsId = null;
				returnValue.Sut = new DeployComponentRunner(returnValue.TaskFactory.Object, returnValue.Impersonator.Object);

				for (int index = 0; index < taskCount; index++)
				{
					var executorType = CreateType(fixture);
					var taskDefinition = new Mock<IDeployTaskDefinition>();
					var taskExecutor = new Mock<IDeployTaskExecutor>();
					taskDefinition.Setup(i=>i.GetTaskExecutorType()).Returns(executorType);
					returnValue.TaskFactory.Setup(i=>i.CreateTaskExecutor(executorType)).Returns(taskExecutor.Object);
					returnValue.TaskDefinitionExecutorList.Add(taskDefinition, taskExecutor, executorType);
					var successResult = new DeployTaskExecutionResult
					{
						Status = EnumDeployTaskExecutionResultStatus.Success
					};
					taskExecutor.Setup(i => i.Execute(returnValue.DeployStateId, returnValue.StatusManager.Object, taskDefinition.Object, returnValue.Component, returnValue.EnvironmentComponent, It.IsAny<DeployMachine>(), It.IsAny<DeployBuild>(), returnValue.RuntimeSystemSettings)).Returns(successResult);
				}
				return returnValue;
			}
		}

		[Test] 
		public void RunsExecutorForEachTask()
		{
			var testData = TestData.Create(2);
			testData.Sut.Run(testData.DeployStateId, testData.StatusManager.Object, testData.GetTaskDefinitionList(), testData.Component, testData.EnvironmentComponent, testData.EnvironmentComponent.MachineList.First(), testData.Build, testData.RuntimeSystemSettings);
			
			foreach(var pair in testData.TaskDefinitionExecutorList)
			{
				var taskDefinition = pair.Item1;
				var taskExecutor = pair.Item2;
				var exectutorType = pair.Item3;
				taskDefinition.Verify(i=>i.GetTaskExecutorType(), Times.Once());
				testData.TaskFactory.Verify(i => i.CreateTaskExecutor(exectutorType), Times.Once());
				taskExecutor.Verify(i => i.Execute(testData.DeployStateId, testData.StatusManager.Object, taskDefinition.Object, testData.Component, testData.EnvironmentComponent, testData.EnvironmentComponent.MachineList.First(), testData.Build, testData.RuntimeSystemSettings), Times.Once());
			}
		}

		//[Test]
		//public void StopsRunningOnFirstFailure()
		//{
		//	var testData = TestData.Create(2);
		//	var failureResult = new DeployTaskExecutionResult
		//	{
		//		Status = EnumDeployTaskExecutionResultStatus.Error
		//	};
		//	testData.TaskDefinitionExecutorList[0].Item2.Setup(i => i.Execute(testData.DeployStateId, testData.StatusManager.Object, testData.TaskDefinitionExecutorList[0].Item1.Object, testData.EnvironmentComponent, It.IsAny<DeployMachine>(), testData.RuntimeSystemSettings)).Returns(failureResult);

		//	testData.Sut.Run(testData.DeployStateId, testData.StatusManager.Object, testData.GetTaskDefinitionList(), testData.EnvironmentComponent, testData.EnvironmentComponent.MachineList.First(), testData.RuntimeSystemSettings);

		//	{
		//		var pair = testData.TaskDefinitionExecutorList[0];
		//		var taskDefinition = pair.Item1;
		//		var taskExecutor = pair.Item2;
		//		var exectutorType = pair.Item3;
		//		taskDefinition.Verify(i => i.GetTaskExecutorType(), Times.Once());
		//		testData.TaskFactory.Verify(i => i.CreateTaskExecutor(exectutorType), Times.Once());
		//		taskExecutor.Verify(i => i.Execute(testData.DeployStateId, testData.StatusManager.Object, taskDefinition.Object, testData.EnvironmentComponent, testData.EnvironmentComponent.MachineList.First(), testData.RuntimeSystemSettings), Times.Once());
		//	}
		//	{
		//		var pair = testData.TaskDefinitionExecutorList[1];
		//		var taskDefinition = pair.Item1;
		//		var taskExecutor = pair.Item2;
		//		var exectutorType = pair.Item3;
		//		taskDefinition.Verify(i => i.GetTaskExecutorType(), Times.Never());
		//		testData.TaskFactory.Verify(i => i.CreateTaskExecutor(exectutorType), Times.Never());
		//		taskExecutor.Verify(i => i.Execute(testData.DeployStateId, testData.StatusManager.Object, taskDefinition.Object, testData.EnvironmentComponent, testData.RuntimeSystemSettings), Times.Never());
		//	}
		//}
	}
}
