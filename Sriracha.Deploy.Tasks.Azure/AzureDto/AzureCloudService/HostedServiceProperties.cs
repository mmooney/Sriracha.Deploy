using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Sriracha.Deploy.Tasks.Azure.AzureDto.AzureCloudService
{
    [XmlType("HostedServiceProperties")]
    public class HostedServiceProperties
    {
        public enum EnumHostedServiceStatus
        {
            Creating,
            Created,
            Deleting,
            Deleted,
            Changing,
            ResolvingDns
        }

        public string Description { get; set; }
        public string AffinityGroup { get; set; }
        public string Location { get; set; }
        public string Label { get; set; }
        public EnumHostedServiceStatus? Status { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateLastModified { get; set; }

        [XmlArray("ExtendedProperties")]
        public List<ExtendedProperty> ExtendedProperties { get; set; }
    }
}
