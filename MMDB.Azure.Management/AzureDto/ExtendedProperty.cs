using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MMDB.Azure.Management
{
    [XmlType("ExtendedProperty")]
    public class ExtendedProperty
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
