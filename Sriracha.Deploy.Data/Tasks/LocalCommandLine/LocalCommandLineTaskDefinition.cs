using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Tasks.LocalCommandLine
{
	public class LocalCommandLineTaskDefinition : BaseDeployTaskDefinition<LocalCommandLineTaskOptions, LocalCommandLineTaskExecutor>
	{
		public override IList<TaskParameter> GetStaticTaskParameterList()
		{
			return new List<TaskParameter>();
		}

		public override IList<TaskParameter> GetEnvironmentTaskParameterList()
		{
			throw new NotImplementedException();
		}

		public override IList<TaskParameter> GetMachineTaskParameterList()
		{
			throw new NotImplementedException();
		}
	}
}
