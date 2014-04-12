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
using Sriracha.Deploy.Data.Utility;

namespace Sriracha.Deploy.Tasks.Common.XmlConfigFile
{
    [TaskDefinitionMetadata("Configure XML File", "XmlConfigFileTaskOptionsView")]
	public class XmlConfigFileTaskDefinition : BaseDeployTaskDefinition<XmlConfigFileTaskOptions, XmlConfigFileTaskExecutor>
	{
        public XmlConfigFileTaskDefinition(IParameterParser parameterParser) : base(parameterParser)
        {
        }

		public override IList<TaskParameter> GetStaticTaskParameterList()
		{
			return this.GetTaskParameterList(EnumConfigLevel.Static);
		}

		public override IList<TaskParameter> GetEnvironmentTaskParameterList()
		{
            var parameters = base.GetEnvironmentTaskParameterList();
            return parameters.Union(this.GetTaskParameterList(EnumConfigLevel.Environment)).ToList();
		}

		public override IList<TaskParameter> GetMachineTaskParameterList()
		{
            var parameters = base.GetMachineTaskParameterList();
			return parameters.Union(this.GetTaskParameterList(EnumConfigLevel.Machine)).ToList();
		}

		public override IList<TaskParameter> GetBuildTaskParameterList()
		{
            var parameters = base.GetBuildTaskParameterList();
			return parameters.Union(this.GetTaskParameterList(EnumConfigLevel.Build)).ToList();
		}

		public override IList<TaskParameter> GetDeployTaskParameterList()
		{
            var parameters = base.GetDeployTaskParameterList();
			return parameters.Union(this.GetTaskParameterList(EnumConfigLevel.Deploy)).ToList();
		}


		private List<TaskParameter> GetTaskParameterList(EnumConfigLevel enumConfigLevel)
		{
            var xpathParameters = 
			(
				from i in this.Options.XPathValueList
				where i.ConfigLevel == enumConfigLevel
				select new TaskParameter
				{
					FieldName = i.ValueName,
					FieldType = EnumTaskParameterType.String,
                    Sensitive = i.Sensitive
				}
			).ToList();

            return xpathParameters;
		}

		public override string TaskDefintionName
		{
			get { return "XmlConfigFile"; }
		}
	}
}
