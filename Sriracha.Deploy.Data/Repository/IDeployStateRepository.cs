using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Dto.Build;
using Sriracha.Deploy.Data.Dto.Deployment;
using Sriracha.Deploy.Data.Dto.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Repository
{
    public interface IDeployStateRepository
    {
        DeployState CreateDeployment(DeployBuild build, DeployProjectBranch branch, DeployEnvironment environment, DeployComponent component, IEnumerable<DeployMachine> machineList, string deployBatchRequestItemId);
        DeployState TryGetDeployState(string projectId, string buildId, string environmentId, string machineId, string deployBatchRequestItemId);
        DeployState GetDeployState(string deployStateId);
        List<DeployState> FindDeployStateListForEnvironment(string buildId, string environmentId);
        List<DeployState> FindDeployStateListForMachine(string buildId, string environmentId, string machineId);
        DeployStateMessage AddDeploymentMessage(string deployStateId, string message);
        DeployState UpdateDeploymentStatus(string deployStateId, EnumDeployStatus enumDeployStatus, Exception err = null);

        PagedSortedList<ComponentDeployHistory> GetComponentDeployHistory(ListOptions listOptions = null, List<string> projectIdList = null, List<string> branchIdList = null, List<string> componentIdList = null, List<string> buildIdList = null, List<string> environmentIdList = null, List<string> environmentNameList = null, List<string> machineIdList = null, List<string> machineNameList = null, List<string> statusList = null);
    }
}
