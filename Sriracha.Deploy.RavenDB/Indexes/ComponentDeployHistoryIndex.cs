using Raven.Client;
using Raven.Client.Linq;
using Raven.Client.Indexes;
using Sriracha.Deploy.Data.Dto.Deployment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.RavenDB.Indexes
{
    //public class ComponentDeployHistoryIndex : AbstractIndexCreationTask<DeployState, ComponentDeployHistory>
    //{
    //    public ComponentDeployHistoryIndex()
    //    {
    //        Map = deployStates => from i in deployStates
    //                            select new 
    //                            {
    //                                DeployStateId = i.Id,
    //                                DeployBatchRequestItemId = i.DeployBatchRequestItemId,
    //                                Status = i.Status,
    //                                StatusDisplayValue = i.StatusDisplayValue,
    //                                ErrorDetails = i.ErrorDetails,
    //                                DeploymentStartedDateTimeUtc = i.DeploymentStartedDateTimeUtc,
    //                                DeploymentCompleteDateTimeUtc = i.DeploymentCompleteDateTimeUtc,
                                    
    //                                ProjectId = i.Build.ProjectId,
    //                                ProjectName = i.Build.ProjectName,
    //                                ProjectComponentId = i.Component.Id,
    //                                ProjectComponentName = i.Component.ComponentName,
    //                                ProjectBranchId = i.Build.ProjectBranchId,
    //                                ProjectBranchName = i.Build.ProjectBranchName,

    //                                BuildId = i.Build.Id,
    //                                FileId = i.Build.FileId,
    //                                Version = i.Build.Version,

    //                                EnvironmentId = i.Environment.Id,
    //                                EnvironmentName = i.Environment.EnvironmentName,
    //                                MachineId = i.MachineList.Select(j=>j.Id).FirstOrDefault(),
    //                                MachineName = i.MachineList.Select(j => j.MachineName).FirstOrDefault()
    //                            };
    //        TransformResults = (database, results) => from i in results
    //                                                    select new ComponentDeployHistory
    //                                                    {
    //                                                        DeployStateId = i.DeployStateId,
    //                                                        DeployBatchRequestItemId = i.DeployBatchRequestItemId,
    //                                                        Status = i.Status,
    //                                                        StatusDisplayValue = i.StatusDisplayValue,
    //                                                        ErrorDetails = i.ErrorDetails,
    //                                                        DeploymentStartedDateTimeUtc = i.DeploymentStartedDateTimeUtc,
    //                                                        DeploymentCompleteDateTimeUtc = i.DeploymentCompleteDateTimeUtc,

    //                                                        ProjectId = i.ProjectId,
    //                                                        ProjectName = i.ProjectName,
    //                                                        ProjectComponentId = i.ProjectComponentId,
    //                                                        ProjectComponentName = i.ProjectComponentName,
    //                                                        ProjectBranchId = i.ProjectBranchId,
    //                                                        ProjectBranchName = i.ProjectBranchName,

    //                                                        BuildId = i.BuildId,
    //                                                        FileId = i.FileId,
    //                                                        Version = i.Version,

    //                                                        EnvironmentId = i.EnvironmentId,
    //                                                        EnvironmentName = i.EnvironmentName,
    //                                                        MachineId = i.MachineId,
    //                                                        MachineName = i.MachineName
    //                                                    };
    //        Index(x => x.ProjectId, Raven.Abstractions.Indexing.FieldIndexing.Analyzed);
    //        Index(x => x.ProjectComponentId, Raven.Abstractions.Indexing.FieldIndexing.Analyzed);
    //        Index(x => x.ProjectBranchId, Raven.Abstractions.Indexing.FieldIndexing.Analyzed);
    //        Index(x => x.EnvironmentId, Raven.Abstractions.Indexing.FieldIndexing.Analyzed);
    //        Index(x => x.EnvironmentName, Raven.Abstractions.Indexing.FieldIndexing.Analyzed);
    //        Index(x => x.BuildId, Raven.Abstractions.Indexing.FieldIndexing.Analyzed);
    //        Index(x => x.MachineId, Raven.Abstractions.Indexing.FieldIndexing.Analyzed);
    //        Index(x => x.MachineName, Raven.Abstractions.Indexing.FieldIndexing.Analyzed);
    //    }
    //}

}
