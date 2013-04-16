using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using Sriracha.Deploy.Data.Impl;
using Sriracha.Deploy.Data.Tasks;

namespace Sriracha.Deploy.Data.Tests
{
	public class TaskManagerTests
	{
		public class GetAvailableTaskList
		{
			private class TestDeployTask1 : IDeployTask { }
			private class TestDeployTask2 : IDeployTask { }
			private class TestDeployTask3 : IDeployTask { }
			private interface ITestInterface : IDeployTask { }
			private abstract class TestAbstractClass : IDeployTask { }

			[Test]
			public void CallsIModuleInspector()
			{
				var moduleInspector = new Mock<IModuleInspector>();
				ITaskManager sut = new TaskManager(moduleInspector.Object);
				var data = new List<Type>()
				{
					typeof(TestDeployTask1),
					typeof(TestDeployTask2),
					typeof(TestDeployTask3)
				};
				moduleInspector.Setup(i=>i.FindTypesImplementingInterfaces(typeof(IDeployTask))).Returns(data);

				var result = sut.GetAvailableTaskList();

				moduleInspector.Verify(i=>i.FindTypesImplementingInterfaces(typeof(IDeployTask)), Times.Once());
				Assert.AreEqual(data.Count, result.Count);
				foreach(var metadata in result)
				{
					Assert.Contains(Type.GetType(metadata.TaskTypeName), data);
				}
			}

			[Test]
			public void ExcludesAbstractClasses()
			{
				var moduleInspector = new Mock<IModuleInspector>();
				ITaskManager sut = new TaskManager(moduleInspector.Object);
				var data = new List<Type>()
				{
					typeof(TestDeployTask1),
					typeof(TestAbstractClass)
				};
				moduleInspector.Setup(i => i.FindTypesImplementingInterfaces(typeof(IDeployTask))).Returns(data);

				var result = sut.GetAvailableTaskList();

				moduleInspector.Verify(i => i.FindTypesImplementingInterfaces(typeof(IDeployTask)), Times.Once());
				Assert.AreEqual(1, result.Count);
				Assert.AreEqual(typeof(TestDeployTask1).FullName, result[0].TaskTypeName);
			}

			[Test]
			public void ExcludesInterfacesClasses()
			{
				var moduleInspector = new Mock<IModuleInspector>();
				ITaskManager sut = new TaskManager(moduleInspector.Object);
				var data = new List<Type>()
				{
					typeof(TestDeployTask1),
					typeof(ITestInterface)
				};
				moduleInspector.Setup(i => i.FindTypesImplementingInterfaces(typeof(IDeployTask))).Returns(data);

				var result = sut.GetAvailableTaskList();

				moduleInspector.Verify(i => i.FindTypesImplementingInterfaces(typeof(IDeployTask)), Times.Once());
				Assert.AreEqual(1, result.Count);
				Assert.AreEqual(typeof(TestDeployTask1).FullName, result[0].TaskTypeName);
			}
		}
	}
}
