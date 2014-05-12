using ServiceStack.ServiceInterface;
using Sriracha.Deploy.Data.Dashboard;
using Sriracha.Deploy.Data.Dto.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sriracha.Deploy.Web.Services.Dashboard
{
    public class DashboardService : Service
    {
        private readonly IDashboardReporter _dashboardReporter;

        public DashboardService(IDashboardReporter dashboardReporter)
        {
            _dashboardReporter = dashboardReporter;
        }

        public object Any(DashboardServiceRequest request)
        {
            return GetReport(request);
        }

        private DashboardReport GetReport(DashboardServiceRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request is null");
            }
            if (request.ProjectList == null || !request.ProjectList.Any())
            {
                throw new ArgumentNullException("request.projectList is null");
            }
            return _dashboardReporter.GetReport(request);
        }
    }
}