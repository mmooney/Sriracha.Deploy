using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Sriracha.Deploy.Tasks.Azure.AzureDto.AzureCloudService
{
    [XmlType("InputEndpoint")]
    public class InputEndpoint
    {
        public string LoadBalancedEndpointSetName { get; set; }
        public string LocalPort { get; set; }
        public string Name { get; set; }
        public string Port { get; set; }

        public LoadBalancerProbe LoadBalancerProbe { get; set; }

        public string Protocol { get; set; }
        public string Vip { get; set; }
    }
}
