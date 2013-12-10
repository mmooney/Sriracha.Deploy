using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Deployment.Offline.OfflineImpl
{
    public class OfflineDeployStateManager : IDeployStateManager
    {
        public Dto.Deployment.DeployState GetDeployState(string deployStateId)
        {
            throw new NotImplementedException();
        }

        public Dto.Deployment.DeployState CreateDeployState(string projectId, string buildId, string environmentId, string machineId, string deployBatchRequestItemId)
        {
            throw new NotImplementedException();
        }

        public Dto.Deployment.DeployState GetOrCreateDeployState(string projectId, string buildId, string environmentId, string machineId, string deployBatchRequestItemId)
        {
            throw new NotImplementedException();
        }

        public Dto.Deployment.DeployStateMessage AddDeploymentMessage(string deployStateId, string message)
        {
            throw new NotImplementedException();
        }

        public void MarkDeploymentInProcess(string deployStateId)
        {
            throw new NotImplementedException();
        }

        public void MarkDeploymentSuccess(string deployStateId)
        {
            throw new NotImplementedException();
        }

        public void MarkDeploymentFailed(string deployStateId, Exception err)
        {
            throw new NotImplementedException();
        }

        public void MarkBatchDeploymentSuccess(string deployBatchRequestId)
        {
            throw new NotImplementedException();
        }

        public void MarkBatchDeploymentFailed(string deployBatchRequestId, Exception err)
        {
            throw new NotImplementedException();
        }

        public void MarkBatchDeploymentCancelled(string deployBatchRequestId, string cancelMessage)
        {
            throw new NotImplementedException();
        }

        public void MarkBatchDeploymentResumed(string deployBatchRequestId, string resumeMessage)
        {
            throw new NotImplementedException();
        }

        public Dto.Deployment.Plan.DeploymentPlan SaveDeploymentPlan(Dto.Deployment.Plan.DeploymentPlan plan)
        {
            throw new NotImplementedException();
        }


        public void MarkBatchDeploymentInProcess(string deployBatchRequestId)
        {
            throw new NotImplementedException();
        }
    }
}
