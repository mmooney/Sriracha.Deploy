using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Sriracha.Deploy.Tasks.Azure.AzureDto.AzureCloudService
{
    [XmlType("CreateHostedService")]
    public class CreateHostedServiceRequest
    {
        public string ServiceName { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string AffinityGroup { get; set; }

        [XmlArray("ExtendedProperties")]
        public List<ExtendedProperty> ExtendedProperties { get; set; }
    }
}
