using ServiceStack.ServiceHost;
using Sriracha.Deploy.Data.Dto.BuildPurgeRules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sriracha.Deploy.Web.Services.SystemSettings
{
    [Route("/systemSettings/buildPurgeRule")]
    [Route("/systemSettings/buildPurgeRule/{id}")]
    public class BuildPurgeRuleRequest : RequestBase<BuildPurgeRule>
    {
        public string ProjectId { get; set; }
        public int? BuildRetentionMinutes { get; set; }
        public List<string> EnvironmentIdList { get; set; }
        public List<string> EnvironmentNameList { get; set; }
        public List<string> MachineIdList { get; set; }
        public List<string> MachineNameList { get; set; }
    }
}