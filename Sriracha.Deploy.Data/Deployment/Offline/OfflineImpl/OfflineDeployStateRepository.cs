using Sriracha.Deploy.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Deployment.Offline.OfflineImpl
{
    public class OfflineDeployStateRepository : IDeployStateRepository
    {
        public Dto.Deployment.DeployState CreateDeployment(Dto.Build.DeployBuild build, Dto.Project.DeployProjectBranch branch, Dto.Project.DeployEnvironment environment, Dto.Project.DeployComponent component, IEnumerable<Dto.Project.DeployMachine> machineList, string deployBatchRequestItemId)
        {
            throw new NotImplementedException();
        }

        public Dto.Deployment.DeployState TryGetDeployState(string projectId, string buildId, string environmentId, string machineId, string deployBatchRequestItemId)
        {
            throw new NotImplementedException();
        }

        public Dto.Deployment.DeployState GetDeployState(string deployStateId)
        {
            throw new NotImplementedException();
        }

        public List<Dto.Deployment.DeployState> FindDeployStateListForEnvironment(string buildId, string environmentId)
        {
            throw new NotImplementedException();
        }

        public List<Dto.Deployment.DeployState> FindDeployStateListForMachine(string buildId, string environmentId, string machineId)
        {
            throw new NotImplementedException();
        }

        public Dto.Deployment.DeployStateMessage AddDeploymentMessage(string deployStateId, string message)
        {
            throw new NotImplementedException();
        }

        public Dto.Deployment.DeployState UpdateDeploymentStatus(string deployStateId, EnumDeployStatus enumDeployStatus, Exception err = null)
        {
            throw new NotImplementedException();
        }

        public Dto.PagedSortedList<Dto.Deployment.ComponentDeployHistory> GetComponentDeployHistory(Dto.ListOptions listOptions = null, List<string> projectIdList = null, List<string> branchIdList = null, List<string> componentIdList = null, List<string> buildIdList = null, List<string> environmentIdList = null, List<string> environmentNameList = null, List<string> machineIdList = null, List<string> machineNameList = null, List<string> statusList = null)
        {
            throw new NotImplementedException();
        }

        public List<Dto.Deployment.DeployStateSummary> GetDeployStateSummaryListByDeployBatchRequestItemId(string deployBatchRequestItemId)
        {
            throw new NotImplementedException();
        }
    }
}
