using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MMDB.Azure.Management.AzureDto.AzureCloudService
{
    [XmlType("UpgradeDeployment")]
    public class UpgradeDeploymentRequest
    {
        public enum EnumUpgradeDeploymentMode
        {
            Auto,
            Manual,
            Simultaneous 
        }

        public EnumUpgradeDeploymentMode? Mode { get; set; }
        public string PackageUrl { get; set; }
        public string Configuration { get; set; }
        public string Label { get; set; }
        public string RoleToUpgrade { get; set; }
        public bool Force { get; set; }
        
        [XmlArray("ExtendedProperties")]
        public List<ExtendedProperty> ExtendedPropertyList { get; set; }

        public ExtensionConfiguration ExtensionConfiguration { get; set; }
    }
}
