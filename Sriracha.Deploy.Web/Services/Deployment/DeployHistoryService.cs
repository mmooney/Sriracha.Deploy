using ServiceStack.ServiceInterface;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Deployment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sriracha.Deploy.Web.Services.Deployment
{
    public class DeployHistoryService : Service
    {
        private readonly IDeployHistoryReporter _deployHistoryReporter;

        public DeployHistoryService(IDeployHistoryReporter deployHistoryReporter)
        {
            _deployHistoryReporter = DIHelper.VerifyParameter(deployHistoryReporter);
        }

        public object Get(DeployHistoryRequest request)
        {
            if(request == null)
            {
                throw new ArgumentNullException("request is null");
            }
            return _deployHistoryReporter.GetComponentDeployHistory(request.BuildListOptions(), request.ProjectIdList, request.BranchIdList, request.ComponentIdList, request.BuildIdList, request.EnvironentIdList, request.EnvironentNameList, request.MachineIdList, request.MachineNameList, request.StatusList);
        }
    }
}