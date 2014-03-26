using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Sriracha.Deploy.Tasks.Azure.AzureDto.AzureCloudService
{
    [XmlType("ExtensionConfiguration")]
    public class ExtensionConfiguration
    {   
        public AllRoles AllRoles { get; set; }

        [XmlArray("NamedRoles")]
        public List<RoleItem> RoleList { get; set; }
    }
}
