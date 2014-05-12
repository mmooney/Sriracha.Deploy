using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Dto.Dashboard
{
    public class DashboardReportBuild
    {
        public string BuildId { get; set; }
        public string BuildDisplayValue { get; set; }
        public string Version { get; set; }
        public string DeployStateId { get; set; }
        public DateTime? DeploymentStartedDateTimeUtc { get; set; }
        public DateTime? DeploymentCompletedDateTimeUtc { get; set; }
        public EnumDeployStatus Status { get; set; }
        public string StatusDisplayValue { get; set; }
    }
}
