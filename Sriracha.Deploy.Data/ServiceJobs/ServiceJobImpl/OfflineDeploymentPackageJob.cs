using Common.Logging;
using MMDB.QuartzJobs;
using NLog;
using Quartz;
using Sriracha.Deploy.Data.Deployment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.ServiceJobs.ServiceJobImpl
{
    [DisallowConcurrentExecution]
    public class OfflineDeploymentPackageJob : BaseInterruptableJob, IOfflineDeploymentPackageJob
    {
        private readonly ILog _logger;
        private readonly IOfflineDeploymentManager _offlineDeploymentManager;

        public OfflineDeploymentPackageJob(IOfflineDeploymentManager offlineDeploymentManager, ILog logger) : base(logger)
        {
            _logger = DIHelper.VerifyParameter(logger);
            _offlineDeploymentManager = DIHelper.VerifyParameter(offlineDeploymentManager);
        }

        protected override bool RunNextItem()
        {
            var offlineDeployment = _offlineDeploymentManager.PopNextOfflineDeploymentToCreate();
            if (offlineDeployment == null)
            {
                this.LogDebug("No pending offline deployment packages found");
                return false;
            }
            else
            {
                this.LogDebug("Found pending offline deployment to package: {0}");
                _offlineDeploymentManager.CreateOfflineDeploymentPackage(offlineDeployment.Id);
                return true;
            }
        }
    }
}
