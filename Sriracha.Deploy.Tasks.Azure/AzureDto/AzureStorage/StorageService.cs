using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Sriracha.Deploy.Tasks.Azure.AzureDto.AzureStorage
{
    [XmlType("StorageService")]
    public class StorageService
    {
        public string Url { get; set; }
        public string ServiceName { get; set; }
        public StorageServiceProperties StorageServiceProperties { get; set; }

        [XmlArray("ExtendedProperties")]
        public List<ExtendedProperty> ExtendedPropertyList { get; set; }

        public StorageServiceKeys StorageServiceKeys { get; set; }
    }
}
