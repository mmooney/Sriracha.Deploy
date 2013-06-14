using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Tasks.XmlConfigFile
{
	public class XmlConfigFileTaskOptions
	{
		public class XPathValueItem
		{
			public string XPath { get; set; }
			public EnumConfigLevel ConfigLevel { get; set; }
			public string ValueName { get; set; }	
		}
		public List<XPathValueItem> XPathValueList { get; set; }
		public string XmlTemplate { get; set; }

		public XmlConfigFileTaskOptions()
		{
			this.XPathValueList = new List<XPathValueItem>();
		}

	}
}
