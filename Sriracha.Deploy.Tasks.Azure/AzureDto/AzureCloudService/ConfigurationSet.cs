using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Sriracha.Deploy.Tasks.Azure.AzureDto.AzureCloudService
{
    [XmlType("ConfigurationSet")]
    public class ConfigurationSet
    {
        public string ConfigurationSetType { get; set; }

        [XmlArray("InputEndpoints")]
        public List<InputEndpoint> InputEndpointList { get; set; }

        [XmlArray("SubnetNames")]
        [XmlArrayItem("SubnetName")]
        public List<string> SubnetNameList { get; set; }
    }
}
