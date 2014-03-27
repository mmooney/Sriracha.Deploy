using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MMDB.Azure.Management.AzureDto.AzureStorage
{
    [XmlType("StorageServices")]
    public class StorageServiceListResponse
    {
        [XmlElement("StorageService")]
        public List<StorageService> StorageServiceList { get; set; }
    }
}
