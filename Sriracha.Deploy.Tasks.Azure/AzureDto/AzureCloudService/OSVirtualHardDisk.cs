using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Sriracha.Deploy.Tasks.Azure.AzureDto.AzureCloudService
{
    [XmlType("OSVirtualHardDisk")]
    public class OSVirtualHardDisk
    {
        public enum EnumOSHostCaching
        {
            ReadOnly,
            ReadWrite
        }

        public EnumOSHostCaching? HostCaching { get; set; }
        public string DiskName { get; set; }
        public string MediaLink { get; set; }
        public string SourceImageName { get; set; }
        public string OS { get; set; }
    }
}
