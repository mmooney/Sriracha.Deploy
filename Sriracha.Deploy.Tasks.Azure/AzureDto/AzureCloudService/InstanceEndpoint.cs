using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Sriracha.Deploy.Tasks.Azure.AzureDto.AzureCloudService
{
    [XmlType("InstanceEndpoint")]
    public class InstanceEndpoint
    {
        public string Name { get; set; }
        public string Vip { get; set; }
        public string PublicPort { get; set; }
        public string LocalPort { get; set; }
        public string Protocol { get; set; }
    }
}
