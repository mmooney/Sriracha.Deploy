using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Dto.Dashboard
{
    public class DashboardReportComponent
    {
        public string ComponentId { get; set; }
        public string ComponentName { get; set; }
        public List<DashboardReportBuild> BuildList { get; set; }
    }
}
