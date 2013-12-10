using Sriracha.Deploy.Data.Deployment.DeploymentImpl;
using Sriracha.Deploy.Data.Dto.Deployment;
using Sriracha.Deploy.Data.Dto.Project;
using Sriracha.Deploy.Data.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Deployment.Offline.OfflineImpl
{
    public class OfflineDeploymentPlanBuilder : DeploymentPlanBuilder, IDeploymentPlanBuilder
    {
        private readonly IOfflineDataProvider _offlineDataProvider;

        public OfflineDeploymentPlanBuilder(IProjectManager projectManager, IOfflineDataProvider offlineDataProvider) : base(projectManager)
        {
            _offlineDataProvider = DIHelper.VerifyParameter(offlineDataProvider);
        }

        public override List<DeployBatchRequestItem> FilterItemList(List<DeployBatchRequestItem> itemList)
        {
            itemList = base.FilterItemList(itemList);
            var selectionList = _offlineDataProvider.GetSelectionList();
            var newList = new List<DeployBatchRequestItem>();
            foreach(var item in itemList)
            {
                var selectionItem = selectionList.FirstOrDefault(i=>i.BatchRequestItem.Id == item.Id);
                if(selectionItem != null && selectionItem.SelectedMachineList.Any())
                {
                    var newItem = AutoMapper.Mapper.Map(item, new DeployBatchRequestItem());
                    var machinesToDelete = new List<DeployMachine>();
                    foreach(var machine in newItem.MachineList)
                    {
                        if(!selectionItem.SelectedMachineList.Any(i=>i.Id == machine.Id))
                        {
                            machinesToDelete.Add(machine);
                        }
                    }
                    foreach(var machine in machinesToDelete)
                    {
                        newItem.MachineList.Remove(machine);
                    }
                    if(newItem.MachineList.Any())
                    {
                        newList.Add(newItem);
                    }
                }
            }
            return newList;
        }
    }
}
