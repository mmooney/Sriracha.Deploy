using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Dto.Dashboard;
using Sriracha.Deploy.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Dashboard.DashboardImpl
{
    public class DashboardReporter : IDashboardReporter
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IDeployStateRepository _deployStateRepository;

        public DashboardReporter(IProjectRepository projectRepository, IDeployStateRepository deployStateRepository)
        {
            _projectRepository = projectRepository;
            _deployStateRepository = deployStateRepository;
        }

        public DashboardReport GetReport(DashboardRequest request)
        {
            var returnValue = new DashboardReport();
            foreach(var projectRequest in request.ProjectList)
            {
                var project = _projectRepository.GetProject(projectRequest.ProjectId);
                var reportProject = new DashboardReportProject
                {
                    ProjectId = project.Id,
                    ProjectName = project.ProjectName
                };
                returnValue.ProjectList.Add(reportProject);

                var environmentIdList = projectRequest.EnvironmentIdList;
                if(!environmentIdList.Any())
                {
                    environmentIdList = project.EnvironmentList.Select(i=>i.Id).ToList();
                }
                var componentIdList = projectRequest.ComponentIdList;
                if(!componentIdList.Any())
                {
                    componentIdList = project.ComponentList.Select(i=>i.Id).ToList();;
                }
                foreach(var environmentId in environmentIdList)
                {
                    var environment = project.GetEnvironment(environmentId);
                    var reportEnvironment = new DashboardReportEnvironment
                    {
                        EnvironmentId = environmentId,
                        EnvironmentName = environment.EnvironmentName
                    };
                    reportProject.EnvironmentList.Add(reportEnvironment);
                    foreach(var componentId in componentIdList)
                    {
                        var component = project.GetComponent(componentId);
                        var reportComponent = new DashboardReportComponent
                        {
                            ComponentId = component.Id,
                            ComponentName = component.ComponentName,
                        };
                        reportEnvironment.ComponentList.Add(reportComponent);

                        var deployList = _deployStateRepository.GetComponentDeployHistory(new ListOptions { PageNumber=1, PageSize=5, SortField="DeploymentStartedDateTimeUtc", SortAscending=false },
                                                                            projectIdList: project.Id.ListMe(), componentIdList: componentId.ListMe(), environmentIdList: environmentId.ListMe());
                        reportComponent.BuildList = (from i in deployList.Items
                                                    select new DashboardReportBuild
                                                    {
                                                        BuildId = i.BuildId,
                                                        BuildDisplayValue = i.BuildDisplayValue,
                                                        DeploymentCompletedDateTimeUtc = i.DeploymentCompleteDateTimeUtc,
                                                        DeploymentStartedDateTimeUtc = i.DeploymentStartedDateTimeUtc,
                                                        Status = i.Status,
                                                        StatusDisplayValue = i.StatusDisplayValue,
                                                        DeployStateId = i.DeployStateId,
                                                        Version = i.Version,
                                                    }).ToList();
                    }
                }
            }

            return returnValue;
        }
    }
}
