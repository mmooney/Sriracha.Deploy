using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MMDB.Azure.Management.AzureDto.AzureStorage
{
    [XmlType("CreateStorageServiceInput")]
    public class CreateStorageServiceRequest
    {
        public string ServiceName { get; set; }
        public string Description { get; set; }
        public string Label { get; set; }
        public string AffinityGroup { get; set; }
        public string Location { get; set; }
        public bool? GeoReplicationEnabled { get; set; }

        [XmlArray("ExtendedProperties")]
        public List<ExtendedProperty> ExtendedPropertyList { get; set; }

        public bool? SecondaryReadEnabled { get; set; }
    }
}
