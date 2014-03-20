using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using MMDB.Shared;
using Newtonsoft.Json;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Tasks;
using Sriracha.Deploy.Data;

namespace Sriracha.Deploy.Tasks.Common.XmlConfigFile
{
    [TaskDefinitionMetadata("Configure XML File", "XmlConfigFileTaskOptionsView")]
	public class XmlConfigFileTaskDefinition : BaseDeployTaskDefinition<XmlConfigFileTaskOptions, XmlConfigFileTaskExecutor>
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

		public override IList<TaskParameter> GetBuildTaskParameterList()
		{
			return this.GetTaskParameterList(EnumConfigLevel.Build);
		}

		public override IList<TaskParameter> GetDeployTaskParameterList()
		{
			return this.GetTaskParameterList(EnumConfigLevel.Deploy);
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

		public override string TaskDefintionName
		{
			get { return "XmlConfigFile"; }
		}
	}
}
