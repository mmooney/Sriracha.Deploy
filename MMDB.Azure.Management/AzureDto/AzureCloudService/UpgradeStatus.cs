using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MMDB.Azure.Management.AzureDto.AzureCloudService
{
    [XmlType("UpgradeStatus")]
    public class UpgradeStatus
    {
        public enum EnumUpgradeType
        {
            Auto,
            Manual,
            Simultaneous
        }

        public enum EnumUpgradeDomainState
        {
            Before,
            During
        }

        public EnumUpgradeType UpgradeType { get; set; }
        public EnumUpgradeDomainState CurrentUpgradeDomainState { get; set; }
        public string CurrentUpgradeDomain { get; set; }
    }
}
