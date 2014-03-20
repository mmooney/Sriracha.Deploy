﻿using ServiceStack.ServiceHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sriracha.Deploy.Web.Services
{
    [Route("/taskOptionsView/{taskTypeName*}")]
    public class TaskOptionsViewRequest 
    {
        public string TaskTypeName { get; set; }
    }
}