using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MMDB.Azure.Management.AzureDto.AzureCloudService
{
    [XmlType("Role")]
    public class RoleItem
    {
        public string RoleName { get; set; }
        public string OSVersion { get; set; }

        [XmlArray("ConfigurationSets")]
        public List<NetworkConfigurationSet> ConfigurationSetList;

        public string AvailabilitySetName { get; set; }

        [XmlArray("DataVirtualHardDisks")]
        public List<DataVirtualHardDisk> DataVirtualHardDiskList { get; set; }

        public OSVirtualHardDisk OSVirtualHardDisk { get; set; }

        public string RoleSize { get; set; }

        [XmlArray("Extensions")]
        public List<Extension> ExtensionList { get; set; }
    }
}
