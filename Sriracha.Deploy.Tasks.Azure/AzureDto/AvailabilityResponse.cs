using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Sriracha.Deploy.Tasks.Azure.AzureDto
{
    [XmlType("AvailabilityResponse")]
    public class AvailabilityResponse
    {
        public bool Result { get; set; }
        public string Reason { get; set; }
    }
}
