using ServiceStack.ServiceHost;
using Sriracha.Deploy.Data.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sriracha.Deploy.Web.Services.SystemSettings
{
    [Route("/systemSettings/users")]
    [Route("/systemSettings/users/{id}")]
    public class UserRequest : RequestBase<SrirachaUser>
    {
        public string UserName { get; set; }
        public string  EmailAddress { get; set; }
        public string Password { get; set; }
        public List<string> UserNameList { get; set; }
    }
}