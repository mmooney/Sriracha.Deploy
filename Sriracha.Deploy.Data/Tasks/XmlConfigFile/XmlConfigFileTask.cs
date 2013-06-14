using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Tasks.XmlConfigFile
{
	public class XmlConfigFileTask : BaseDeployTask<XmlConfigFileTaskOptions>
	{
		public string XmlTemplate { get; set; }

		public override IList<TaskParameter> GetStaticTaskParameterList()
		{
			throw new NotImplementedException();
		}

		public override IList<TaskParameter> GetEnvironmentTaskParameterList()
		{
			return 
			(
				from i in this.Options.XPathValueList
				where i.ConfigLevel == EnumConfigLevel.Environment
				select new TaskParameter
				{
					FieldName = i.ValueName,
					FieldType = EnumTaskParameterType.String 
				}
			).ToList();
		}

		public override IList<TaskParameter> GetMachineTaskParameterList()
		{
			throw new NotImplementedException();
		}
	}
}
