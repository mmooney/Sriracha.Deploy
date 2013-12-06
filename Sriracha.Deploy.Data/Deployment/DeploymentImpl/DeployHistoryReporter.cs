using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Dto.Deployment;
using Sriracha.Deploy.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Deployment.DeploymentImpl
{
    public class DeployHistoryReporter : IDeployHistoryReporter
    {
        private readonly IDeployStateRepository _deployStateRepository;

        public DeployHistoryReporter(IDeployStateRepository deployStateRepository)
        {
            _deployStateRepository = DIHelper.VerifyParameter(deployStateRepository);
        }

        public PagedSortedList<ComponentDeployHistory> GetComponentDeployHistory(ListOptions listOptions, List<string> projectIdList, List<string> branchIdList, List<string> componentIdList, List<string> buildIdList, List<string> environmentIdList, List<string> environmentNameList, List<string> machineIdList, List<string> machineNameList, List<string> statusList)
        {
            listOptions = ListOptions.SetDefaults(listOptions, 10, 1, "DeploymentStartedDateTimeUtc", false);
            return _deployStateRepository.GetComponentDeployHistory(listOptions, projectIdList, branchIdList, componentIdList, buildIdList, environmentIdList, environmentNameList, machineIdList, machineNameList, statusList);
        }
    }
}
