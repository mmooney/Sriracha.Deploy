using Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Sriracha.Deploy.Data.Deployment;
using Sriracha.Deploy.Data.Deployment.DeploymentImpl;
using Sriracha.Deploy.Data.Dto.Deployment;
using Sriracha.Deploy.Data.Dto.Project;
using Sriracha.Deploy.Data.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Tests
{
    public class DeploymentPlanBuilderTests
    {
        private class TestData
        {
            public Fixture Fixture { get; set; }
            public DeployBatchRequest DeployBatchRequest { get; set; }
            public Mock<IProjectManager> ProjectManager { get; set; }
            public IDeploymentPlanBuilder Sut { get; set; }

            public static TestData Create(int stepCount, int machinesPerStep)
            {
                var fixture = new Fixture();
                var returnValue = new TestData
                {
                    DeployBatchRequest = fixture.Build<DeployBatchRequest>()
                                    .With(i=>i.ItemList,
                                            fixture.Build<DeployBatchRequestItem>()
                                                .With(j=>j.MachineList, fixture.CreateMany<DeployMachine>(machinesPerStep).ToList())
                                                .CreateMany(stepCount).ToList())
                                    .Create(),
                    ProjectManager = new Mock<IProjectManager>()
                };
                var deployMachineNameList = fixture.CreateMany("DeployMachineName", machinesPerStep).ToList();
                foreach(var item in returnValue.DeployBatchRequest.ItemList)
                {
                    for(int i = 0; i < machinesPerStep; i++)
                    {
                        item.MachineList[i].MachineName = deployMachineNameList[i];
                    }
                }
                returnValue.Sut = new DeploymentPlanBuilder(returnValue.ProjectManager.Object);

                return returnValue;
            }

            public void SetIsolationType(DeployBatchRequestItem item, EnumDeploymentIsolationType isolationType)
            {
                this.ProjectManager.Setup(i=>i.GetComponentIsolationType(item.Build.ProjectId, item.Build.ProjectComponentId)).Returns(isolationType);
            }

        }

        [Test]
        public void SingleMachinesFullyIsolated()
        {
            var testData = TestData.Create(10, 1);
            foreach (var item in testData.DeployBatchRequest.ItemList)
            {
                testData.SetIsolationType(item, EnumDeploymentIsolationType.IsolatedPerDeployment);
            }

            var result = testData.Sut.Build(testData.DeployBatchRequest);

            Assert.IsNotNull(result);
            Assert.AreEqual(testData.DeployBatchRequest.Id, result.DeployBatchRequestId);
            Assert.AreEqual(testData.DeployBatchRequest.ItemList.Count, result.ParallelBatchList.Count());
            foreach(var parallelBatchItem in result.ParallelBatchList)
            {
                Assert.AreEqual(EnumDeploymentIsolationType.IsolatedPerDeployment, parallelBatchItem.IsolationType);
                Assert.AreEqual(1, parallelBatchItem.MachineQueueList.Count);
                Assert.AreEqual(1, parallelBatchItem.MachineQueueList[0].MachineQueueItemList.Count);
				Assert.IsNotNullOrEmpty(parallelBatchItem.MachineQueueList[0].Id);
            }
            for(int i = 0; i < testData.DeployBatchRequest.ItemList.Count; i++)
            {
                Assert.AreEqual(testData.DeployBatchRequest.ItemList[i], result.ParallelBatchList[i].MachineQueueList[0].MachineQueueItemList[0].DeployBatchRequestItem);
            }
        }

        [Test]
        public void MultipleMachinesFullyIsolated()
        {
            var testData = TestData.Create(10, 5);
            foreach (var item in testData.DeployBatchRequest.ItemList)
            {
                testData.SetIsolationType(item, EnumDeploymentIsolationType.IsolatedPerDeployment);
            }

            var result = testData.Sut.Build(testData.DeployBatchRequest);

            Assert.IsNotNull(result);
			Assert.AreEqual(testData.DeployBatchRequest.Id, result.DeployBatchRequestId);
			Assert.AreEqual(50, result.ParallelBatchList.Count());
            foreach (var parallelBatchItem in result.ParallelBatchList)
            {
                Assert.AreEqual(EnumDeploymentIsolationType.IsolatedPerDeployment, parallelBatchItem.IsolationType);
                Assert.AreEqual(1, parallelBatchItem.MachineQueueList.Count);
                Assert.AreEqual(1, parallelBatchItem.MachineQueueList[0].MachineQueueItemList.Count);
				Assert.IsNotNullOrEmpty(parallelBatchItem.MachineQueueList[0].Id);
            }
            int counter = 0;
            foreach(var item in testData.DeployBatchRequest.ItemList)
            {
                foreach(var machine in item.MachineList)
                {
                    var machineQueue = result.ParallelBatchList[counter].MachineQueueList[0];
					Assert.IsNotNullOrEmpty(machineQueue.Id);
                    Assert.AreEqual(item, machineQueue.MachineQueueItemList[0].DeployBatchRequestItem);
                    Assert.AreEqual(machine.Id, machineQueue.MachineQueueItemList[0].MachineId);
                    counter++;
                }
            }
        }

        [Test]
        public void SingleMachinesFullyParallel()
        {
            var testData = TestData.Create(10, 1);
            foreach (var item in testData.DeployBatchRequest.ItemList)
            {
                testData.SetIsolationType(item, EnumDeploymentIsolationType.NoIsolation);
            }

            var result = testData.Sut.Build(testData.DeployBatchRequest);

            Assert.IsNotNull(result);
			Assert.AreEqual(testData.DeployBatchRequest.Id, result.DeployBatchRequestId);
			Assert.AreEqual(1, result.ParallelBatchList.Count());
            Assert.AreEqual(testData.DeployBatchRequest.ItemList.Count, result.ParallelBatchList[0].MachineQueueList.Count);
            Assert.AreEqual(EnumDeploymentIsolationType.NoIsolation, result.ParallelBatchList[0].IsolationType);
            foreach (var machineQueue in result.ParallelBatchList[0].MachineQueueList)
            {
				Assert.IsNotNullOrEmpty(machineQueue.Id);
                Assert.AreEqual(1, machineQueue.MachineQueueItemList.Count);
            }
            int counter = 0;
            foreach (var item in testData.DeployBatchRequest.ItemList)
            {
                foreach (var machine in item.MachineList)
                {
                    var machineQueue = result.ParallelBatchList[0].MachineQueueList[counter];
					Assert.IsNotNullOrEmpty(machineQueue.Id);
                    Assert.AreEqual(item, machineQueue.MachineQueueItemList[0].DeployBatchRequestItem);
                    Assert.AreEqual(machine.Id, machineQueue.MachineQueueItemList[0].MachineId);
                    counter++;
                }
            }
        }

        [Test]
        public void MultipleMachinesFullyParallel()
        {
            var testData = TestData.Create(10, 5);
            foreach (var item in testData.DeployBatchRequest.ItemList)
            {
                testData.SetIsolationType(item, EnumDeploymentIsolationType.NoIsolation);
            }

            var result = testData.Sut.Build(testData.DeployBatchRequest);

            Assert.IsNotNull(result);
			Assert.AreEqual(testData.DeployBatchRequest.Id, result.DeployBatchRequestId);
			Assert.AreEqual(1, result.ParallelBatchList.Count());
            Assert.AreEqual(50, result.ParallelBatchList[0].MachineQueueList.Count);
            Assert.AreEqual(EnumDeploymentIsolationType.NoIsolation, result.ParallelBatchList[0].IsolationType);
            foreach (var machineQueue in result.ParallelBatchList[0].MachineQueueList)
            {
				Assert.IsNotNullOrEmpty(machineQueue.Id);
                Assert.AreEqual(1, machineQueue.MachineQueueItemList.Count);
            }
            int counter = 0;
            foreach (var item in testData.DeployBatchRequest.ItemList)
            {
                foreach (var machine in item.MachineList)
                {
                    var machineQueue = result.ParallelBatchList[0].MachineQueueList[counter];
					Assert.IsNotNullOrEmpty(machineQueue.Id);
                    Assert.AreEqual(item, machineQueue.MachineQueueItemList[0].DeployBatchRequestItem);
                    Assert.AreEqual(machine.Id, machineQueue.MachineQueueItemList[0].MachineId);
                    counter++;
                }
            }
        }

        [Test]
        public void SingleMachinesServerIsolation()
        {
            var testData = TestData.Create(10, 1);
            foreach (var item in testData.DeployBatchRequest.ItemList)
            {
                testData.SetIsolationType(item, EnumDeploymentIsolationType.IsolatedPerMachine);
            }

            var result = testData.Sut.Build(testData.DeployBatchRequest);

            Assert.IsNotNull(result);
			Assert.AreEqual(testData.DeployBatchRequest.Id, result.DeployBatchRequestId);
			Assert.AreEqual(1, result.ParallelBatchList.Count());
            Assert.AreEqual(1, result.ParallelBatchList[0].MachineQueueList.Count);
            Assert.AreEqual(EnumDeploymentIsolationType.IsolatedPerMachine, result.ParallelBatchList[0].IsolationType);
            foreach (var machineQueue in result.ParallelBatchList[0].MachineQueueList)
            {
				Assert.IsNotNullOrEmpty(machineQueue.Id);
                Assert.AreEqual(testData.DeployBatchRequest.ItemList.Count, machineQueue.MachineQueueItemList.Count);
            }
            int counter = 0;
            foreach (var item in testData.DeployBatchRequest.ItemList)
            {
                foreach (var machine in item.MachineList)
                {
                    var machineQueue = result.ParallelBatchList[0].MachineQueueList[0];
					Assert.IsNotNullOrEmpty(machineQueue.Id);
                    Assert.AreEqual(item, machineQueue.MachineQueueItemList[counter].DeployBatchRequestItem);
                    Assert.AreEqual(machine.Id, machineQueue.MachineQueueItemList[counter].MachineId);
                    counter++;
                }
            }
        }

        [Test]
        public void MultipleMachinesServerIsolation()
        {
            var testData = TestData.Create(10, 5);
            foreach (var item in testData.DeployBatchRequest.ItemList)
            {
                testData.SetIsolationType(item, EnumDeploymentIsolationType.IsolatedPerMachine);
            }

            var result = testData.Sut.Build(testData.DeployBatchRequest);

            Assert.IsNotNull(result);
			Assert.AreEqual(testData.DeployBatchRequest.Id, result.DeployBatchRequestId);
			Assert.AreEqual(1, result.ParallelBatchList.Count());
            Assert.AreEqual(5, result.ParallelBatchList[0].MachineQueueList.Count);
            Assert.AreEqual(EnumDeploymentIsolationType.IsolatedPerMachine, result.ParallelBatchList[0].IsolationType); 
            foreach (var machineQueue in result.ParallelBatchList[0].MachineQueueList)
            {
				Assert.IsNotNullOrEmpty(machineQueue.Id);
                Assert.AreEqual(testData.DeployBatchRequest.ItemList.Count, machineQueue.MachineQueueItemList.Count);
            }
            int itemCounter = 0;
            foreach (var item in testData.DeployBatchRequest.ItemList)
            {
                int machineCounter = 0;
                foreach (var machine in item.MachineList)
                {
                    var machineQueue = result.ParallelBatchList[0].MachineQueueList[machineCounter];
					Assert.IsNotNullOrEmpty(machineQueue.Id);
                    Assert.AreEqual(item, machineQueue.MachineQueueItemList[itemCounter].DeployBatchRequestItem);
                    Assert.AreEqual(machine.Id, machineQueue.MachineQueueItemList[itemCounter].MachineId);
                    machineCounter++;
                }
                itemCounter++;
            }
        }

        [Test]
        public void SingleMachinesServerIsolationWithFullyIsolatedFirstStep()
        {
            var testData = TestData.Create(10, 1);
            foreach (var item in testData.DeployBatchRequest.ItemList)
            {
                testData.SetIsolationType(item, EnumDeploymentIsolationType.IsolatedPerMachine);
            }
            testData.SetIsolationType(testData.DeployBatchRequest.ItemList[0], EnumDeploymentIsolationType.IsolatedPerDeployment);

            var result = testData.Sut.Build(testData.DeployBatchRequest);

            Assert.IsNotNull(result);
			Assert.AreEqual(testData.DeployBatchRequest.Id, result.DeployBatchRequestId);
			Assert.AreEqual(2, result.ParallelBatchList.Count());
            //first, fully isolated queue
            Assert.AreEqual(1, result.ParallelBatchList[0].MachineQueueList.Count);
            Assert.AreEqual(EnumDeploymentIsolationType.IsolatedPerDeployment, result.ParallelBatchList[0].IsolationType);
            Assert.AreEqual(testData.DeployBatchRequest.ItemList[0], result.ParallelBatchList[0].MachineQueueList[0].MachineQueueItemList[0].DeployBatchRequestItem);
			Assert.IsNotNullOrEmpty(result.ParallelBatchList[0].MachineQueueList[0].Id);

            //next, machine isolated queues
            Assert.AreEqual(1, result.ParallelBatchList[1].MachineQueueList.Count);
            Assert.AreEqual(EnumDeploymentIsolationType.IsolatedPerMachine, result.ParallelBatchList[1].IsolationType);
            foreach (var machineQueue in result.ParallelBatchList[1].MachineQueueList)
            {
				Assert.IsNotNullOrEmpty(machineQueue.Id);
                Assert.AreEqual(testData.DeployBatchRequest.ItemList.Count-1, machineQueue.MachineQueueItemList.Count);
            }
            int counter = 0;
            foreach (var item in testData.DeployBatchRequest.ItemList.Skip(1))
            {
                foreach (var machine in item.MachineList)
                {
                    var machineQueue = result.ParallelBatchList[1].MachineQueueList[0];
					Assert.IsNotNullOrEmpty(machineQueue.Id);
                    Assert.AreEqual(item, machineQueue.MachineQueueItemList[counter].DeployBatchRequestItem);
                    Assert.AreEqual(machine.Id, machineQueue.MachineQueueItemList[counter].MachineId);
                    counter++;
                }
            }
        }

        [Test]
        public void MultipleMachinesServerIsolationWithFullyIsolatedFirstStep()
        {
            var testData = TestData.Create(10, 3);
            foreach (var item in testData.DeployBatchRequest.ItemList)
            {
                testData.SetIsolationType(item, EnumDeploymentIsolationType.IsolatedPerMachine);
            }
            testData.SetIsolationType(testData.DeployBatchRequest.ItemList[0], EnumDeploymentIsolationType.IsolatedPerDeployment);

            var result = testData.Sut.Build(testData.DeployBatchRequest);

            Assert.IsNotNull(result);
			Assert.AreEqual(testData.DeployBatchRequest.Id, result.DeployBatchRequestId);
			Assert.AreEqual(4, result.ParallelBatchList.Count());
            //first, 3 fully isolated queues
            Assert.AreEqual(1, result.ParallelBatchList[0].MachineQueueList.Count);
            Assert.AreEqual(testData.DeployBatchRequest.ItemList[0], result.ParallelBatchList[0].MachineQueueList[0].MachineQueueItemList[0].DeployBatchRequestItem);
            Assert.AreEqual(EnumDeploymentIsolationType.IsolatedPerDeployment, result.ParallelBatchList[0].IsolationType);
			Assert.IsNotNullOrEmpty(result.ParallelBatchList[0].MachineQueueList[0].Id);

            Assert.AreEqual(1, result.ParallelBatchList[1].MachineQueueList.Count);
            Assert.AreEqual(testData.DeployBatchRequest.ItemList[0], result.ParallelBatchList[1].MachineQueueList[0].MachineQueueItemList[0].DeployBatchRequestItem);
            Assert.AreEqual(EnumDeploymentIsolationType.IsolatedPerDeployment, result.ParallelBatchList[1].IsolationType);
			Assert.IsNotNullOrEmpty(result.ParallelBatchList[1].MachineQueueList[0].Id);

            Assert.AreEqual(1, result.ParallelBatchList[2].MachineQueueList.Count);
            Assert.AreEqual(testData.DeployBatchRequest.ItemList[0], result.ParallelBatchList[2].MachineQueueList[0].MachineQueueItemList[0].DeployBatchRequestItem);
            Assert.AreEqual(EnumDeploymentIsolationType.IsolatedPerDeployment, result.ParallelBatchList[2].IsolationType);
			Assert.IsNotNullOrEmpty(result.ParallelBatchList[2].MachineQueueList[0].Id);

            //next, machine isolated queue
            Assert.AreEqual(3, result.ParallelBatchList[3].MachineQueueList.Count);
            Assert.AreEqual(EnumDeploymentIsolationType.IsolatedPerMachine, result.ParallelBatchList[3].IsolationType);
            foreach (var machineQueue in result.ParallelBatchList[3].MachineQueueList)
            {
				Assert.IsNotNullOrEmpty(machineQueue.Id);
                Assert.AreEqual(testData.DeployBatchRequest.ItemList.Count - 1, machineQueue.MachineQueueItemList.Count);
            }
            int itemCounter = 1;
            foreach (var item in testData.DeployBatchRequest.ItemList.Skip(1))
            {
                int machineCounter = 0;
                foreach (var machine in item.MachineList)
                {
                    var machineQueue = result.ParallelBatchList[3].MachineQueueList[machineCounter];
					Assert.IsNotNullOrEmpty(machineQueue.Id);
                    Assert.AreEqual(item, machineQueue.MachineQueueItemList[itemCounter-1].DeployBatchRequestItem);
                    Assert.AreEqual(machine.Id, machineQueue.MachineQueueItemList[itemCounter-1].MachineId);
                    machineCounter++;
                }
                itemCounter++;
            }
        }

        [Test]
        public void MultipleMachinesServerIsolationWithFullyIsolatedMiddleStep()
        {
            var testData = TestData.Create(10, 3);
            foreach (var item in testData.DeployBatchRequest.ItemList)
            {
                testData.SetIsolationType(item, EnumDeploymentIsolationType.IsolatedPerMachine);
            }
            testData.SetIsolationType(testData.DeployBatchRequest.ItemList[5], EnumDeploymentIsolationType.IsolatedPerDeployment);

            var result = testData.Sut.Build(testData.DeployBatchRequest);

            Assert.IsNotNull(result);
			Assert.AreEqual(testData.DeployBatchRequest.Id, result.DeployBatchRequestId);
			Assert.AreEqual(5, result.ParallelBatchList.Count());
            //first 5 steps with server isolation
            {
                Assert.AreEqual(3, result.ParallelBatchList[0].MachineQueueList.Count);
                Assert.AreEqual(EnumDeploymentIsolationType.IsolatedPerMachine, result.ParallelBatchList[0].IsolationType);
                foreach (var machineQueue in result.ParallelBatchList[0].MachineQueueList)
                {
					Assert.IsNotNullOrEmpty(machineQueue.Id);
                    Assert.AreEqual(5, machineQueue.MachineQueueItemList.Count);
                } 
                int itemCounter = 0;
                foreach (var item in testData.DeployBatchRequest.ItemList.Take(5))
                {
                    int machineCounter = 0;
                    foreach (var machine in item.MachineList)
                    {
                        var machineQueue = result.ParallelBatchList[0].MachineQueueList[machineCounter];
						Assert.IsNotNullOrEmpty(machineQueue.Id);
                        Assert.AreEqual(item, machineQueue.MachineQueueItemList[itemCounter].DeployBatchRequestItem);
                        Assert.AreEqual(machine.Id, machineQueue.MachineQueueItemList[itemCounter].MachineId);
                        machineCounter++;
                    }
                    itemCounter++;
                }
            }


            //Next, 3 fully isolated queues for step 6
            Assert.AreEqual(1, result.ParallelBatchList[1].MachineQueueList.Count);
            Assert.AreEqual(testData.DeployBatchRequest.ItemList[5], result.ParallelBatchList[1].MachineQueueList[0].MachineQueueItemList[0].DeployBatchRequestItem);
            Assert.AreEqual(EnumDeploymentIsolationType.IsolatedPerDeployment, result.ParallelBatchList[1].IsolationType);
			Assert.IsNotNullOrEmpty(result.ParallelBatchList[1].MachineQueueList[0].Id);

            Assert.AreEqual(1, result.ParallelBatchList[2].MachineQueueList.Count);
            Assert.AreEqual(testData.DeployBatchRequest.ItemList[5], result.ParallelBatchList[2].MachineQueueList[0].MachineQueueItemList[0].DeployBatchRequestItem);
            Assert.AreEqual(EnumDeploymentIsolationType.IsolatedPerDeployment, result.ParallelBatchList[2].IsolationType);
			Assert.IsNotNullOrEmpty(result.ParallelBatchList[2].MachineQueueList[0].Id);

            Assert.AreEqual(1, result.ParallelBatchList[3].MachineQueueList.Count);
            Assert.AreEqual(testData.DeployBatchRequest.ItemList[5], result.ParallelBatchList[3].MachineQueueList[0].MachineQueueItemList[0].DeployBatchRequestItem);
            Assert.AreEqual(EnumDeploymentIsolationType.IsolatedPerDeployment, result.ParallelBatchList[3].IsolationType);
			Assert.IsNotNullOrEmpty(result.ParallelBatchList[3].MachineQueueList[0].Id);

            //Then 1 more machine isolated queue
            {
                Assert.AreEqual(3, result.ParallelBatchList[4].MachineQueueList.Count);
                Assert.AreEqual(EnumDeploymentIsolationType.IsolatedPerMachine, result.ParallelBatchList[4].IsolationType);
                foreach (var machineQueue in result.ParallelBatchList[4].MachineQueueList)
                {
					Assert.IsNotNullOrEmpty(machineQueue.Id);
                    Assert.AreEqual(4, machineQueue.MachineQueueItemList.Count);
                }
                int itemCounter = 6;
                foreach (var item in testData.DeployBatchRequest.ItemList.Skip(6))
                {
                    int machineCounter = 0;
                    foreach (var machine in item.MachineList)
                    {
                        var machineQueueItem = result.ParallelBatchList[4].MachineQueueList[machineCounter];
                        Assert.AreEqual(item, machineQueueItem.MachineQueueItemList[itemCounter - 6].DeployBatchRequestItem);
                        Assert.AreEqual(machine.Id, machineQueueItem.MachineQueueItemList[itemCounter - 6].MachineId);
                        machineCounter++;
                    }
                    itemCounter++;
                }
            }
        }

        [Test]
        public void SwitchFromMachineIsolationToNoIsolationCreatesNewQueueBatch()
        {
            var testData = TestData.Create(10, 3);
            foreach (var item in testData.DeployBatchRequest.ItemList.Take(5))
            {
                testData.SetIsolationType(item, EnumDeploymentIsolationType.IsolatedPerMachine);
            }
            foreach (var item in testData.DeployBatchRequest.ItemList.Skip(5))
            {
                testData.SetIsolationType(item, EnumDeploymentIsolationType.NoIsolation);
            }

            var result = testData.Sut.Build(testData.DeployBatchRequest);

            Assert.AreEqual(2, result.ParallelBatchList.Count());
            Assert.AreEqual(EnumDeploymentIsolationType.IsolatedPerMachine, result.ParallelBatchList[0].IsolationType);

            Assert.AreEqual(EnumDeploymentIsolationType.NoIsolation, result.ParallelBatchList[1].IsolationType);
        }
    }
}
