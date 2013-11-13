using MMDB.Shared;
using Sriracha.Deploy.Data.Dto.Deployment;
using Sriracha.Deploy.Data.Dto.Deployment.Plan;
using Sriracha.Deploy.Data.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Deployment.DeploymentImpl
{
    public class DeploymentPlanBuilder : IDeploymentPlanBuilder
    {
        private readonly IProjectManager _projectManager;

        public DeploymentPlanBuilder(IProjectManager projectManager)
        {
            _projectManager = DIHelper.VerifyParameter(projectManager);
        }

        public DeploymentPlan Build(DeployBatchRequest deployBatchRequest)
        {
            var returnValue = new DeploymentPlan()
            {
                DeployBatchRequestId = deployBatchRequest.Id
            };
            DeploymentPlanParallelBatch currentParallelBatch = null;
            if(deployBatchRequest.ItemList != null)
            {
                foreach(var item in deployBatchRequest.ItemList)
                {
                    EnumDeploymentIsolationType isolationType = _projectManager.GetComponentIsolationType(item.Build.ProjectId, item.Build.ProjectComponentId);
                    if(isolationType == EnumDeploymentIsolationType.IsolatedPerDeployment)
                    {
                        currentParallelBatch = null;
                        foreach(var machine in item.MachineList)
                        {
                            var parallelBatchItem = new DeploymentPlanParallelBatch
                            {
                                IsolationType = EnumDeploymentIsolationType.IsolatedPerDeployment,
                                MachineQueueList = new List<DeploymentPlanMachineQueue>
                                {
                                    new DeploymentPlanMachineQueue()
                                    {
										Id = Guid.NewGuid().ToString(),
                                        MachineName = machine.MachineName,
                                        MachineQueueItemList = new List<DeploymentPlanMachineQueueItem>
                                        {
                                            new DeploymentPlanMachineQueueItem
                                            {
                                                MachineId = machine.Id,
                                                DeployBatchRequestItem = item
                                            }
                                        }
                                    }
                                }
                            };
                            returnValue.ParallelBatchList.Add(parallelBatchItem);
                        }
                    }
                    else if (isolationType == EnumDeploymentIsolationType.IsolatedPerMachine)
                    {
                        if (currentParallelBatch == null || currentParallelBatch.IsolationType != EnumDeploymentIsolationType.IsolatedPerMachine)
                        {
                            currentParallelBatch = new DeploymentPlanParallelBatch
                            {
                                IsolationType = EnumDeploymentIsolationType.IsolatedPerMachine
                            };
                            returnValue.ParallelBatchList.Add(currentParallelBatch);
                        }
                        foreach(var machine in item.MachineList)
                        {
                            var machineQueue = currentParallelBatch.MachineQueueList.FirstOrDefault(i=>i.MachineName.Equals(machine.MachineName, StringComparison.CurrentCultureIgnoreCase));
                            if(machineQueue == null)
                            {
                                machineQueue = new DeploymentPlanMachineQueue
                                {
									Id = Guid.NewGuid().ToString(),
                                    MachineName = machine.MachineName
                                };
                                currentParallelBatch.MachineQueueList.Add(machineQueue);
                            }
                            var machineQueueItem = new DeploymentPlanMachineQueueItem
                            {
                                MachineId = machine.Id,
                                DeployBatchRequestItem = item
                            };
                            machineQueue.MachineQueueItemList.Add(machineQueueItem);
                        }
                    }
                    else if(isolationType == EnumDeploymentIsolationType.NoIsolation)
                    {
                        if (currentParallelBatch == null || currentParallelBatch.IsolationType != EnumDeploymentIsolationType.NoIsolation)
                        {
                            currentParallelBatch = new DeploymentPlanParallelBatch
                            {
                                IsolationType = EnumDeploymentIsolationType.NoIsolation
                            };
                            returnValue.ParallelBatchList.Add(currentParallelBatch);
                        }
                        foreach (var machine in item.MachineList)
                        {
                            var machineQueue = new DeploymentPlanMachineQueue
                            {
								Id = Guid.NewGuid().ToString(),
								MachineName = machine.MachineName
                            };
                            currentParallelBatch.MachineQueueList.Add(machineQueue);
                            var machineQueueItem = new DeploymentPlanMachineQueueItem
                            {
                                MachineId = machine.Id,
                                DeployBatchRequestItem = item
                            };
                            machineQueue.MachineQueueItemList.Add(machineQueueItem);
                        }
                    }
                    else 
                    {
                        throw new UnknownEnumValueException(isolationType);
                    }
                }
            }
            return returnValue;
        }
    }
}
