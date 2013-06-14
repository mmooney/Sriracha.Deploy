using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Tasks.XmlConfigFile
{
	public class XmlConfigFileTask : BaseDeployTask<XmlConfigFileTaskOptions>
	{
		public override IList<TaskParameter> GetStaticTaskParameterList()
		{
			return this.GetTaskParameterList(EnumConfigLevel.Static);
		}

		public override IList<TaskParameter> GetEnvironmentTaskParameterList()
		{
			return this.GetTaskParameterList(EnumConfigLevel.Environment);
		}

		public override IList<TaskParameter> GetMachineTaskParameterList()
		{
			return this.GetTaskParameterList(EnumConfigLevel.Machine);
		}


		private IList<TaskParameter> GetTaskParameterList(EnumConfigLevel enumConfigLevel)
		{
			return
			(
				from i in this.Options.XPathValueList
				where i.ConfigLevel == enumConfigLevel
				select new TaskParameter
				{
					FieldName = i.ValueName,
					FieldType = EnumTaskParameterType.String
				}
			).ToList();
		}
	}
}
