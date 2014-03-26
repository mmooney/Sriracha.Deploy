using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Sriracha.Deploy.Tasks.Azure.AzureDto.AzureStorage
{
    [XmlType("StorageServiceProperties")]
    public class StorageServiceProperties
    {
        public enum EnumStorageServiceStatus
        {
            Unknown,
            Creating,
            Created,
            Deleting,
            Deleted,
            Changing,
            ResolvingDns
        }

        public enum EnumStatusOfPrimary
        {
            Available,
            Unavailable
        }

        public enum EnumStatusOfSecondary
        {
            Available,
            Unavailable            
        }

        public string Description { get; set; }
        public string AffinityGroup { get; set; }
        public string Location { get; set; }
        public string Label { get; set; }
        public EnumStorageServiceStatus? Status { get; set; }

        [XmlArray("Endpoints")]
        [XmlArrayItem("Endpoint")]
        public List<string> EndpointList { get; set; }

        public bool? GeoReplicationEnabled { get; set; }
        public string GeoPrimaryRegion { get; set; }
        public EnumStatusOfPrimary? StatusOfPrimary { get; set; }
        public DateTime? LastGeoFailoverTime { get; set; }
        public string GeoSecondaryRegion { get; set; }
        public EnumStatusOfSecondary? StatusOfSecondary { get; set; }
        public DateTime CreationTime { get; set; }

        [XmlArray("CustomDomains")]
        public List<CustomDomain> CustomDomainList { get; set; }

        public bool? SecondaryReadEnabled { get; set; }

        [XmlArray("SecondaryEndpoints")]
        [XmlArrayItem("SecondaryEndpoint")]
        public List<string> SecondaryEndpointList { get; set; }
    }
}
