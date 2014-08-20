using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Tasks;
using Sriracha.Deploy.Data.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sriracha.Deploy.Tasks.Common.LocalCommandLine
{
    [TaskDefinitionMetadata("Execute Command Line Locally", "LocalCommandLineTaskOptionsView")]
	public class LocalCommandLineTaskDefinition : BaseDeployTaskDefinition<LocalCommandLineTaskOptions, LocalCommandLineTaskExecutor>
	{
		public LocalCommandLineTaskDefinition(IParameterParser parameterParser) : base(parameterParser)
		{
		}
	}
}
