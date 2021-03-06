﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;
using Sriracha.Deploy.Data.Impl;
using Sriracha.Deploy.Data.Tasks;
using Sriracha.Deploy.Data.Dto.Project;

namespace Sriracha.Deploy.Data.Tests
{
	public class TaskManagerTests
	{
		public class GetAvailableTaskList
		{
			private class TestBaseTask : IDeployTaskDefinition
			{
				public IList<TaskParameter> GetStaticTaskParameterList()
				{
					throw new NotImplementedException();
				}

				public IList<TaskParameter> GetEnvironmentTaskParameterList()
				{
					throw new NotImplementedException();
				}

				public IList<TaskParameter> GetMachineTaskParameterList()
				{
					throw new NotImplementedException();
				}

				public void Execute(DeployEnvironmentConfiguration deployEnvironmentComponent, RuntimeSystemSettings runtimeSystemSettings)
				{
					throw new NotImplementedException();
				}


				public Type GetTaskExecutorType()
				{
					throw new NotImplementedException();
				}

				public string TaskDefintionName
				{
					get { throw new NotImplementedException(); }
				}


				public Type GetTaskOptionType()
				{
					throw new NotImplementedException();
				}

				public object DeployTaskOptions
				{
					get { throw new NotImplementedException(); }
					set { throw new NotImplementedException(); }
				}


				public IList<TaskParameter> GetBuildTaskParameterList()
				{
					throw new NotImplementedException();
				}


				public IList<TaskParameter> GetDeployTaskParameterList()
				{
					throw new NotImplementedException();
				}
			}
			private class TestDeployTask1 : TestBaseTask { }
			private class TestDeployTask2 : TestBaseTask { }
			private class TestDeployTask3 : TestBaseTask { }
			private interface ITestInterface : IDeployTaskDefinition { }
			private abstract class TestAbstractClass : IDeployTaskDefinition
			{
				public abstract IList<TaskParameter> GetStaticTaskParameterList();
				public abstract IList<TaskParameter> GetEnvironmentTaskParameterList();
				public abstract IList<TaskParameter> GetMachineTaskParameterList();
				public abstract IList<TaskParameter> GetBuildTaskParameterList();
				public abstract IList<TaskParameter> GetDeployTaskParameterList();
				public abstract Type GetTaskExecutorType();
				public abstract string TaskDefintionName { get; }
				public abstract Type GetTaskOptionType();
				public abstract object DeployTaskOptions { get; set; }
			}

            [TaskDefinitionMetadata(TaskName="Test Task With Metadata")]
            private class TestTaskWithMetadata : IDeployTaskDefinition
            {
                public string TaskDefintionName
                {
                    get { throw new NotImplementedException(); }
                }

                public IList<TaskParameter> GetStaticTaskParameterList()
                {
                    throw new NotImplementedException();
                }

                public IList<TaskParameter> GetEnvironmentTaskParameterList()
                {
                    throw new NotImplementedException();
                }

                public IList<TaskParameter> GetMachineTaskParameterList()
                {
                    throw new NotImplementedException();
                }

                public IList<TaskParameter> GetBuildTaskParameterList()
                {
                    throw new NotImplementedException();
                }

                public IList<TaskParameter> GetDeployTaskParameterList()
                {
                    throw new NotImplementedException();
                }

                public Type GetTaskExecutorType()
                {
                    throw new NotImplementedException();
                }

                public Type GetTaskOptionType()
                {
                    throw new NotImplementedException();
                }

                public object DeployTaskOptions
                {
                    get
                    {
                        throw new NotImplementedException();
                    }
                    set
                    {
                        throw new NotImplementedException();
                    }
                }
            }

            [Test]
            public void UsesMetadataAttributeIfAvaialble()
            {
                var moduleInspector = new Mock<IModuleInspector>();
                ITaskManager sut = new TaskManager(moduleInspector.Object);
                var data = new List<Type>()
				{
					typeof(TestTaskWithMetadata)
				};
                moduleInspector.Setup(i => i.FindTypesImplementingInterfaces(typeof(IDeployTaskDefinition))).Returns(data);

                var result = sut.GetAvailableTaskList();

                moduleInspector.Verify(i => i.FindTypesImplementingInterfaces(typeof(IDeployTaskDefinition)), Times.Once());
                Assert.AreEqual(data.Count, result.Count);
                foreach (var metadata in result)
                {
                    var type = data.FirstOrDefault(i=>i.FullName == metadata.TaskTypeName);
                    Assert.IsNotNull(type);
                    string displayName = ((TaskDefinitionMetadataAttribute)type.GetCustomAttributes(typeof(TaskDefinitionMetadataAttribute), true).First()).TaskName;
                    Assert.AreEqual(displayName, metadata.TaskDisplayName);
                }

            }

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
				moduleInspector.Setup(i=>i.FindTypesImplementingInterfaces(typeof(IDeployTaskDefinition))).Returns(data);

				var result = sut.GetAvailableTaskList();

				moduleInspector.Verify(i=>i.FindTypesImplementingInterfaces(typeof(IDeployTaskDefinition)), Times.Once());
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
				moduleInspector.Setup(i => i.FindTypesImplementingInterfaces(typeof(IDeployTaskDefinition))).Returns(data);

				var result = sut.GetAvailableTaskList();

				moduleInspector.Verify(i => i.FindTypesImplementingInterfaces(typeof(IDeployTaskDefinition)), Times.Once());
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
				moduleInspector.Setup(i => i.FindTypesImplementingInterfaces(typeof(IDeployTaskDefinition))).Returns(data);

				var result = sut.GetAvailableTaskList();

				moduleInspector.Verify(i => i.FindTypesImplementingInterfaces(typeof(IDeployTaskDefinition)), Times.Once());
				Assert.AreEqual(1, result.Count);
				Assert.AreEqual(typeof(TestDeployTask1).FullName, result[0].TaskTypeName);
			}
		}
	}

    public class GetTaskOptionsView
    {
        [TaskDefinitionMetadata(TaskName = "Test Task With Metadata", OptionsViewResourceId = "TestView")]
        private class TestTaskWithMetadata : IDeployTaskDefinition
        {
            public string TaskDefintionName
            {
                get { throw new NotImplementedException(); }
            }

            public IList<TaskParameter> GetStaticTaskParameterList()
            {
                throw new NotImplementedException();
            }

            public IList<TaskParameter> GetEnvironmentTaskParameterList()
            {
                throw new NotImplementedException();
            }

            public IList<TaskParameter> GetMachineTaskParameterList()
            {
                throw new NotImplementedException();
            }

            public IList<TaskParameter> GetBuildTaskParameterList()
            {
                throw new NotImplementedException();
            }

            public IList<TaskParameter> GetDeployTaskParameterList()
            {
                throw new NotImplementedException();
            }

            public Type GetTaskExecutorType()
            {
                throw new NotImplementedException();
            }

            public Type GetTaskOptionType()
            {
                throw new NotImplementedException();
            }

            public object DeployTaskOptions
            {
                get
                {
                    throw new NotImplementedException();
                }
                set
                {
                    throw new NotImplementedException();
                }
            }
        }
        
        [Test]
        public void ReturnsOptionsView()
        {
            var moduleInspector = new Mock<IModuleInspector>();
            ITaskManager sut = new TaskManager(moduleInspector.Object);
            var data = new List<Type>()
				{
					typeof(TestTaskWithMetadata)
				};
            moduleInspector.Setup(i => i.FindTypesImplementingInterfaces(typeof(IDeployTaskDefinition))).Returns(data);

            var result = sut.GetTaskOptionsView(data[0].FullName);

            Assert.IsNotNullOrEmpty(result);
            Assert.AreEqual(TestDataResources.TestView, result);
        }
    }
}
