using Sriracha.Deploy.Data.Dto.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Dashboard
{
    public interface IDashboardReporter
    {
        DashboardReport GetReport(DashboardRequest request);
    }
}
