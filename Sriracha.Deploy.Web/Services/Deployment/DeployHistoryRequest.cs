using ServiceStack.ServiceHost;
using Sriracha.Deploy.Data.Dto.Deployment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sriracha.Deploy.Web.Services.Deployment
{
    [Route("/deploy/history")]
    public class DeployHistoryRequest : RequestBase<ComponentDeployHistory>
    {
        public List<string> ProjectIdList { get; set; }
        public List<string> BranchIdList { get; set; }
        public List<string> ComponentIdList { get; set; }
        public List<string> BuildIdList { get; set; }
        public List<string> EnvironentIdList { get; set; }
        public List<string> EnvironentNameList { get; set; }
        public List<string> MachineIdList { get; set; }
        public List<string> MachineNameList { get; set; }
    }
}