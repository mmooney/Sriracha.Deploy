using dropkick.Configuration;
using dropkick.Wmi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Tasks.Common.DropkickImpl
{
    internal class CommonDeploymentSettings : DropkickConfiguration
    {
        public string ServiceName { get; set; }
        public string ServiceExeName { get; set; }
        public ServiceStartMode ServiceStartMode { get; set; }
        public List<string> DependencyList { get; set; }
        public bool StartImmediately { get; set; }

        public string ServiceSourcePath { get; set; }
        public string ServiceSourceExeConfigPath { get; set; }

        public string ServiceTargetPath { get; set; }
        public string ServiceUserName { get; set; }
        public string ServiceUserPassword { get; set; }
        public string TargetMachineUserName { get; set; }
        public string TargetMachinePassword { get; set; }

        public string ApplicationPoolName { get; set; }

        public string SiteName { get; set; }

        public string VirtualDirectoryName { get; set; }
        public string TargetWebsitePath { get; set; }
    }
}
