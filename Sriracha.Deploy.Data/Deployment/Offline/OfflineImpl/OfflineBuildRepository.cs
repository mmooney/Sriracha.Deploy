using Sriracha.Deploy.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Deployment.Offline.OfflineImpl
{
    public class OfflineBuildRepository : IBuildRepository
    {
        public Dto.PagedSortedList<Dto.Build.DeployBuild> GetBuildList(Dto.ListOptions listOptions, string projectId = null, string branchId = null, string componentId = null)
        {
            throw new NotImplementedException();
        }

        public Dto.Build.DeployBuild CreateBuild(string projectId, string projectName, string projectComponentId, string projectComponentName, string projectBranchId, string projectBranchName, string fileId, string version)
        {
            throw new NotImplementedException();
        }

        public Dto.Build.DeployBuild GetBuild(string buildId)
        {
            throw new NotImplementedException();
        }

        public Dto.Build.DeployBuild UpdateBuild(string buildId, string projectId, string projectName, string projectComponentId, string projectComponentName, string projectBranchId, string projectBranchName, string fileId, string version)
        {
            throw new NotImplementedException();
        }

        public void DeleteBuild(string buildId)
        {
            throw new NotImplementedException();
        }
    }
}
