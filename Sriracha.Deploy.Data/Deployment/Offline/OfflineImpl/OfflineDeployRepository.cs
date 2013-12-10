using MMDB.Shared;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Dto.Deployment;
using Sriracha.Deploy.Data.Dto.Deployment.Offline;
using Sriracha.Deploy.Data.Dto.Deployment.Plan;
using Sriracha.Deploy.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Deployment.Offline.OfflineImpl
{
    public class OfflineDeployRepository : IDeployRepository
    {
        private readonly IOfflineDataProvider _offlineDataProvider;

        public OfflineDeployRepository(IOfflineDataProvider offlineDataProvider)
        {
            _offlineDataProvider = DIHelper.VerifyParameter(offlineDataProvider);
        }

        public PagedSortedList<DeployBatchRequest> GetBatchRequestList(ListOptions listOptions)
        {
            throw new NotImplementedException();
        }

        public DeployBatchRequest CreateBatchRequest(List<DeployBatchRequestItem> itemList, DateTime submittedDateTimeUtc, EnumDeployStatus status, string deploymentLabel)
        {
            throw new NotImplementedException();
        }

        public DeployBatchRequest PopNextBatchDeployment()
        {
            throw new NotImplementedException();
        }

        public DeployBatchRequest GetBatchRequest(string id)
        {
            var item = _offlineDataProvider.GetBatchRequest();
            if(item == null || item.Id != id)
            {
                throw new RecordNotFoundException(typeof(DeployBatchRequest), "Id", id);
            }
            return item;
        }

        public DeployBatchRequest UpdateBatchDeploymentStatus(string deployBatchRequestId, EnumDeployStatus status, Exception err = null, string statusMessage = null, bool addToMessageHistory = true)
        {
            return _offlineDataProvider.UpdateBatchDeploymentStatus(deployBatchRequestId, status, err, statusMessage, addToMessageHistory);
        }

        public PagedSortedList<DeployBatchRequest> GetDeployQueue(ListOptions listOptions, List<EnumDeployStatus> statusList = null, List<string> environmentIds = null, bool includeResumeRequested = true)
        {
            throw new NotImplementedException();
        }

        public DeployBatchRequest RequeueDeployment(string deployBatchRequestId, EnumDeployStatus enumDeployStatus, string statusMessage)
        {
            throw new NotImplementedException();
        }

        public DeployBatchRequest SetCancelRequested(string deployBatchRequestId, string userMessage)
        {
            throw new NotImplementedException();
        }

        public bool HasCancelRequested(string deployBatchRequestId)
        {
            throw new NotImplementedException();
        }

        public DeployBatchRequest SetResumeRequested(string deployBatchRequestId, string userMessage)
        {
            throw new NotImplementedException();
        }

        public bool IsStopped(string deployBatchRequestId)
        {
            throw new NotImplementedException();
        }

        public bool IsCancelled(string deployBatchRequestId)
        {
            throw new NotImplementedException();
        }

        public DeploymentPlan SaveDeploymentPlan(DeploymentPlan plan)
        {
            throw new NotImplementedException();
        }
    }
}
