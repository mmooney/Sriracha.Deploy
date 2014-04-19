using Sriracha.Deploy.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Tasks.Common.XmlConfigFile
{
    public class XmlConfigFileTaskOptions
	{
        public enum EnumTemplateSource 
        {
            XmlTemplate,
            File
        }

		public class XPathValueItem
		{
			public string XPath { get; set; }
			public EnumConfigLevel ConfigLevel { get; set; }
			public string ValueName { get; set; }
            public bool Sensitive { get; set; }
		}

        public class XmlNamespaceDefinition
        {
            public string Uri { get; set; }
            public string Prefix { get; set; }
        }
        
        public List<XPathValueItem> XPathValueList { get; set; }
        public List<XmlNamespaceDefinition> NamespaceDefinitionList { get; set; }
        public EnumTemplateSource TemplateSource { get; set; }
		public string XmlTemplate { get; set; }
        public string SourceFileName { get; set; }
		public string TargetFileName { get; set; }

		public XmlConfigFileTaskOptions()
		{
			this.XPathValueList = new List<XPathValueItem>();
            this.NamespaceDefinitionList = new List<XmlNamespaceDefinition>();
		}
	}
}
