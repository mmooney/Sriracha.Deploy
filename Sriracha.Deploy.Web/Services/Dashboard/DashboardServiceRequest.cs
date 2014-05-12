using ServiceStack.ServiceHost;
using Sriracha.Deploy.Data.Dto.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sriracha.Deploy.Web.Services.Dashboard
{
    [Route("/dashboard")]
    public class DashboardServiceRequest : DashboardRequest
    {
    }
}