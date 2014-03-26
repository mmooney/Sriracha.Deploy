using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Sriracha.Deploy.Tasks.Azure.AzureDto.AzureCloudService
{
    [XmlType("DnsServer")]
    public class DnsServer
    {
        public string Name { get; set; }
        public string Address { get; set; }
    }
}
