using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Dto.Dashboard
{
    public class DashboardRequest
    {
        public class DashboardRequestProject
        {
            public string ProjectId { get; set; }
            public List<string> ComponentIdList { get; set; }
            public List<string> EnvironmentIdList { get; set; }
        }

        public List<DashboardRequestProject> ProjectList { get; set; }
    }
}
