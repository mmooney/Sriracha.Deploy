using ServiceStack.ServiceHost;
using Sriracha.Deploy.Data.Dto.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sriracha.Deploy.Web.Services.SystemSettings
{
    [Route("/systemSettings/systemRole")]
    [Route("/systemSettings/systemRole/{id}")]
    public class SystemRoleRequest : RequestBase<SystemRole>
    {
        public string RoleName { get; set; }
        public bool EveryoneRoleIndicator { get; set; }
        public SystemRoleAssignments Assignments { get; set; }
        public SystemRolePermissions Permissions { get; set; }
    }
}