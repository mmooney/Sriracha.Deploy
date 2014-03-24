using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Sriracha.Deploy.Tasks.Azure.AzureDto
{
    [XmlType("HostedServiceProperties")]
    public class HostedServiceProperties
    {
        public enum EnumStatus
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
        public EnumStatus? Status { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateLastModified { get; set; }

        [XmlArray("ExtendedProperties")]
        public List<ExtendedProperty> ExtendedProperties { get; set; }
    }
}
