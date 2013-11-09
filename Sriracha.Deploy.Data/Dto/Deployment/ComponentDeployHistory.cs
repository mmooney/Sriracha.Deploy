using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Dto.Deployment
{
    public class ComponentDeployHistory
    {
        public string DeployStateId { get; set; }
        public string DeployBatchRequestItemId { get; set; }

        public string ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string ProjectBranchId { get; set; }
        public string ProjectBranchName { get; set; }
        public string ProjectComponentId { get; set; }
        public string ProjectComponentName { get; set; }
        public string FileId { get; set; }
        public string Version { get; set; }
        public DateTime DeploymentStartedDateTimeUtc { get; set; }
        public DateTime? DeploymentCompleteDateTimeUtc { get; set; }
        public string ErrorDetails { get; set; }

        public string EnvironmentId { get; set; }
        public string EnvironmentName { get; set; }
        public string MachineId;
        public string MachineName;

        public EnumDeployStatus Status { get; set; }
        public string StatusDisplayValue { get; set; }
    }
}
