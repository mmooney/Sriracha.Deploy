using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Dto.Dashboard
{
    public class DashboardReportProject
    {
        public string ProjectId { get; set; }
        public string ProjectName { get; set; }
        public List<DashboardReportEnvironment> EnvironmentList { get; set; }

        public DashboardReportProject()
        {
            this.EnvironmentList = new List<DashboardReportEnvironment>();
        }
    }
}
