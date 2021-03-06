﻿using dropkick.Wmi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Tasks.Common.DeployWindowsService
{
    public class DeployWindowsServiceTaskOptions
    {
        public class ServiceDependency
        {
            public string ServiceName { get; set; }
        }

        public string ServiceName { get; set; }
        public string ServiceExeName { get; set; }
        public ServiceStartMode ServiceStartMode { get; set; }
        public List<ServiceDependency> DependencyList { get; set; }
        public bool StartImmediately { get; set; }

        public string ServiceSourcePath { get; set; }
        public string ServiceSourceExeConfigPath { get; set; }

        public string ServiceTargetPath { get; set; }
        public string ServiceUserName { get; set; }
        public string ServiceUserPassword { get; set; }
        public string TargetMachineName { get; set; }
        public string TargetMachineUserName { get; set; }
        public string TargetMachinePassword { get; set; }

    }
}
