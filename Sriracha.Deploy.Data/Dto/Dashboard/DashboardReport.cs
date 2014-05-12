using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Dto.Dashboard
{
    public class DashboardReport
    {
        public List<DashboardReportProject> ProjectList { get; set; }

        public DashboardReport()
        {
            this.ProjectList = new List<DashboardReportProject>();
        }
    }
}
