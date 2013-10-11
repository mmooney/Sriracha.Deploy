using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Tasks.RoundhousE
{
	public class RoundhousETask : BaseDeployTaskDefinition<RoundhousETaskOptions, RoundhousETaskExecutor>
	{
		public override IList<TaskParameter> GetStaticTaskParameterList()
		{
			throw new NotImplementedException();
		}

		public override IList<TaskParameter> GetEnvironmentTaskParameterList()
		{
			throw new NotImplementedException();
		}

		public override IList<TaskParameter> GetMachineTaskParameterList()
		{
			throw new NotImplementedException();
		}

		public override IList<TaskParameter> GetBuildTaskParameterList()
		{
			throw new NotImplementedException();
		}

		public override string TaskDefintionName
		{
			get { return "RoundhousE"; }
		}
	}
}
