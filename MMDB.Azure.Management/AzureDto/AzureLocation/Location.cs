using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MMDB.Azure.Management.AzureDto.AzureLocation
{
    [XmlType("Location")]
    public class Location
    {
        public enum EnumAvailableService
        {
            Compute,
            Storage,
            PersistentVMRole,
            HighMemory
        }

        public string Name { get; set; }
        public string DisplayName { get; set; }

        [XmlArray("AvailableServices")]
        [XmlArrayItem("AvailableService")]
        public List<EnumAvailableService> AvailableServiceList { get; set; }
    }
}
