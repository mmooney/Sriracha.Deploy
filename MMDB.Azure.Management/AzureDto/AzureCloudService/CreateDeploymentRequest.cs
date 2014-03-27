using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MMDB.Azure.Management.AzureDto.AzureCloudService
{
    [XmlType("CreateDeployment")]
    public class CreateDeploymentRequest
    {
        public string Name { get; set; }
        public string PackageUrl { get; set; }
        public string Label { get; set; }
        public string Configuration { get; set; }
        public bool StartDeployment { get; set; }
        public bool TreatWarningsAsError { get; set; }
        
        [XmlArray("ExtendedProperties")]
        public List<ExtendedProperty> ExtendedPropertyList { get; set; }

        public ExtensionConfiguration ExtensionConfiguration { get; set; }
    }
}
