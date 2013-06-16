using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Tasks.LocalCommandLine
{
	public class LocalCommandLineTaskDefinition : BaseDeployTaskDefinition<LocalCommandLineTaskOptions, LocalCommandLineTaskExecutor>
	{
		private readonly IParameterParser _parameterParser;

		public LocalCommandLineTaskDefinition(IParameterParser parameterParser)
		{
			_parameterParser = DIHelper.VerifyParameter(parameterParser);
		}

		public override IList<TaskParameter> GetStaticTaskParameterList()
		{
			return new List<TaskParameter>();
		}

		public override IList<TaskParameter> GetEnvironmentTaskParameterList()
		{
			return (from i in _parameterParser.FindEnvironmentParameters(this.Options.ExecutableArguments)
						select new TaskParameter
						{
							FieldName = (i.StartsWith("SENSITIVE:", StringComparison.CurrentCultureIgnoreCase))
											? i.Substring("SENSITIVE:".Length)
											: i,
							FieldType = EnumTaskParameterType.String,
							Sensitive = i.StartsWith("SENSITIVE:", StringComparison.CurrentCultureIgnoreCase)
						}).ToList();
		}

		public override IList<TaskParameter> GetMachineTaskParameterList()
		{
			return (from i in _parameterParser.FindMachineParameters(this.Options.ExecutableArguments)
					select new TaskParameter
					{
						FieldName = (i.StartsWith("SENSITIVE:", StringComparison.CurrentCultureIgnoreCase))
										? i.Substring("SENSITIVE:".Length)
										: i,
						FieldType = EnumTaskParameterType.String,
						Sensitive = i.StartsWith("SENSITIVE:", StringComparison.CurrentCultureIgnoreCase)
					}).ToList();
		}
	}
}
