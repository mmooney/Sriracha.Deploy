using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Sriracha.Deploy.Tasks.Azure.AzureDto.AzureCloudService
{
    [XmlType("DataVirtualHardDisk")]
    public class DataVirtualHardDisk
    {
        public enum EnumDataHostCaching
        {
            None,
            ReadOnly,
            ReadWrite
        }

        public EnumDataHostCaching? HostCaching { get; set; }
        public string DiskName { get; set; }
        public int? Lun { get; set; }
        public int? LogicalDiskSizeInGB { get; set; }
        public string MediaLink { get; set; }
    }
}
