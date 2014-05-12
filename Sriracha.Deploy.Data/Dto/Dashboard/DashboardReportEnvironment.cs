using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Dto.Dashboard
{
    public class DashboardReportEnvironment
    {
        public string EnvironmentId { get; set; }
        public string EnvironmentName { get; set; }
        public List<DashboardReportComponent> ComponentList { get; set; }

        public DashboardReportEnvironment()
        {
            this.ComponentList = new List<DashboardReportComponent>();
        }
    }
}
