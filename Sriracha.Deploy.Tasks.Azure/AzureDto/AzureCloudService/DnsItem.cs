using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Sriracha.Deploy.Tasks.Azure.AzureDto.AzureCloudService
{
    [XmlType("Dns")]
    public class DnsItem
    {
        [XmlArray("DnsServers")]
        public List<DnsServer> DnsServerList { get; set;  }
    }
}
