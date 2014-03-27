using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MMDB.Azure.Management.AzureDto.AzureCloudService
{
    [XmlType("LoadBalancerProbe")]
    public class LoadBalancerProbe
    {
        public string Path { get; set; }
        public string Port { get; set; }
        public string Protocol { get; set; }
    }
}
