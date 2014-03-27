using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MMDB.Azure.Management.AzureDto.AzureCloudService
{
    [XmlType("Deployment")]
    public class DeploymentItem
    {
        public enum EnumDeploymentItemStatus
        {
            Unknown,
            Running,
            Suspended,
            RunningTransitioning,
            SuspendedTransitioning,
            Starting,
            Suspending,
            Deploying,
            Deleting
        }

        public string Name { get; set; }
        public string DeploymentSlot { get; set; }
        public string PrivateID { get; set; }
        public EnumDeploymentItemStatus? Status { get; set; }
        public string Label { get; set; }
        public string Url { get; set; }
        public string Configuration { get; set; }
        
        [XmlArray("RoleInstanceList")]
        public List<RoleInstance> RoleInstanceList { get; set; }

        public UpgradeStatus UpgradeStatus { get; set; }

        public int? UpgradeDomainCount { get; set; }

        [XmlArray("RoleList")]
        public List<RoleItem> RoleList { get; set; }

        public string SdkVersion { get; set; }
        public bool? Locked { get; set; }
        public bool? RollbackAllowed { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime LastModifiedTime { get; set; }
        public string VirtualNetworkName { get; set; }

        public DnsItem Dns { get; set; }

        [XmlArray("ExtendedProperties")]
        public List<ExtendedProperty> ExtendedPropertyList { get; set; }

        public PersistentVMDowntime PersistentVMDowntime { get; set; }

        [XmlArray("VirtualIPs")]
        public List<VirtualIP> VirtualIPList { get; set; }
    }
}
