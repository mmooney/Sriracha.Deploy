using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Tasks;
using Sriracha.Deploy.Data.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sriracha.Deploy.Tasks.Common.LocalCommandLine
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

		public override IList<TaskParameter> GetBuildTaskParameterList()
		{
			return (from i in _parameterParser.FindBuildParameters(this.Options.ExecutableArguments)
					select new TaskParameter
					{
						FieldName = i,
						FieldType = EnumTaskParameterType.String,
						Sensitive = false
					}).ToList();
		}

		public override IList<TaskParameter> GetDeployTaskParameterList()
		{
			return (from i in _parameterParser.FindDeployParameters(this.Options.ExecutableArguments)
					select new TaskParameter
					{
						FieldName = i,
						FieldType = EnumTaskParameterType.String,
						Sensitive = false
					}).ToList();
		}

		public override string TaskDefintionName
		{
			get { return "LocalCommandLine"; }
		}
	}
}
