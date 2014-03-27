using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MMDB.Azure.Management.AzureDto.AzureCloudService
{
    [XmlType("HostedServices")]
    public class HostedServiceListResponse
    {
        [XmlElement("HostedService")]
        public List<HostedService> HostedServiceList { get; set; }
    }
}
