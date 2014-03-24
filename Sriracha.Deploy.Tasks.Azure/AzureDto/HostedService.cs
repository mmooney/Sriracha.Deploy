using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Sriracha.Deploy.Tasks.Azure.AzureDto
{
    [XmlType("HostedService")]
    public class HostedService
    {
        public string Url { get; set; }
        public string ServiceName { get; set; }

        [XmlElement("HostedServiceProperties")]
        public HostedServiceProperties Properties{ get; set; }
    }
}
