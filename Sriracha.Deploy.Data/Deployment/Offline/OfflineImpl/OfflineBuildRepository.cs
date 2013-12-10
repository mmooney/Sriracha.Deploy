using MMDB.Shared;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Dto.Build;
using Sriracha.Deploy.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Deployment.Offline.OfflineImpl
{
    public class OfflineBuildRepository : IBuildRepository
    {
        private readonly IOfflineDataProvider _offlineDataProvider;

        public OfflineBuildRepository(IOfflineDataProvider offlineDataProvider)
        {
            _offlineDataProvider = DIHelper.VerifyParameter(offlineDataProvider);
        }

        public PagedSortedList<DeployBuild> GetBuildList(ListOptions listOptions, string projectId = null, string branchId = null, string componentId = null)
        {
            throw new NotSupportedException();
        }

        public DeployBuild CreateBuild(string projectId, string projectName, string projectComponentId, string projectComponentName, string projectBranchId, string projectBranchName, string fileId, string version)
        {
            throw new NotSupportedException();
        }

        public DeployBuild GetBuild(string buildId)
        {
            DeployBuild build = null;
            var batchRequest = _offlineDataProvider.GetBatchRequest();
            var item = batchRequest.ItemList.FirstOrDefault(i=>i.Build.Id == buildId);
            if(item != null) 
            {
                build = item.Build;
            }
            if(build == null)
            {
                throw new RecordNotFoundException(typeof(DeployBuild), "Id", buildId);
            }
            return build;
        }

        public DeployBuild UpdateBuild(string buildId, string projectId, string projectName, string projectComponentId, string projectComponentName, string projectBranchId, string projectBranchName, string fileId, string version)
        {
            throw new NotSupportedException();
        }

        public void DeleteBuild(string buildId)
        {
            throw new NotSupportedException();
        }
    }
}
