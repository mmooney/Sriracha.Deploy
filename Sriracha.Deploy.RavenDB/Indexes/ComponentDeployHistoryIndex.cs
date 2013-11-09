using Raven.Client.Indexes;
using Sriracha.Deploy.Data.Dto.Deployment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.RavenDB.Indexes
{
    public class ComponentDeployHistoryIndex : AbstractIndexCreationTask<DeployState>
    {
        public ComponentDeployHistoryIndex()
        {
            Map = deployStates => from i in deployStates
                                select new ComponentDeployHistory
                                {
                                    DeployStateId = i.Id,
                                    DeployBatchRequestItemId = i.DeployBatchRequestItemId,
                                    Status = i.Status,
                                    StatusDisplayValue = i.StatusDisplayValue,
                                    ErrorDetails = i.ErrorDetails,
                                    DeploymentStartedDateTimeUtc = i.DeploymentStartedDateTimeUtc,
                                    DeploymentCompleteDateTimeUtc = i.DeploymentCompleteDateTimeUtc,
                                    
                                    ProjectId = i.Build.ProjectId,
                                    ProjectName = i.Build.ProjectName,
                                    ProjectComponentId = i.Component.Id,
                                    ProjectComponentName = i.Component.ComponentName,
                                    ProjectBranchId = i.Build.ProjectBranchId,
                                    ProjectBranchName = i.Build.ProjectBranchName,
                                    FileId = i.Build.FileId,
                                    Version = i.Build.Version,

                                    EnvironmentId = i.Environment.Id,
                                    EnvironmentName = i.Environment.EnvironmentName,
                                    MachineId = i.MachineList.Select(j=>j.Id).FirstOrDefault(),
                                    MachineName = i.MachineList.Select(j => j.MachineName).FirstOrDefault()
                                };
        }
    }

}
