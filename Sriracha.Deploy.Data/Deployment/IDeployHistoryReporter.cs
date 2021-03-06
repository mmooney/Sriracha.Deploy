﻿using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Dto.Deployment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Deployment
{
    public interface IDeployHistoryReporter
    {
        PagedSortedList<ComponentDeployHistory> GetComponentDeployHistory(ListOptions listOptions, List<string> projectIdList, List<string> branchIdList, List<string> componentIdList, List<string> buildIdList, List<string> environmentIdList, List<string> environmentNameList, List<string> machineIdList, List<string> machineNameList, List<string> statusList);
    }
}
