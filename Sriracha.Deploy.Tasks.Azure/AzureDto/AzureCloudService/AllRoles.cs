using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Sriracha.Deploy.Tasks.Azure.AzureDto.AzureCloudService
{
    [XmlType("AllRoles")]
    public class AllRoles
    {
        public Extension Extension { get; set; }
    }
}
