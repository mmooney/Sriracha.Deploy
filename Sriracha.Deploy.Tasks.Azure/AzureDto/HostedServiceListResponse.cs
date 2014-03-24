using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Sriracha.Deploy.Tasks.Azure.AzureDto
{
    [XmlType("HostedServices")]
    public class HostedServiceListResponse
    {
        [XmlElement("HostedService")]
        public List<HostedService> HostedServiceList { get; set; }
    }
}
