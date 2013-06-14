using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Tasks;

namespace Sriracha.Deploy.Data.Tasks
{
	public abstract class BaseDeployTask<TaskOptions> : IDeployTask
		where TaskOptions : new()
	{
		public TaskOptions Options { get; set; }
		
		public BaseDeployTask()
		{
			this.Options = new TaskOptions();
		}

		public abstract IList<TaskParameter> GetStaticTaskParameterList();
		public abstract IList<TaskParameter> GetEnvironmentTaskParameterList();
		public abstract IList<TaskParameter> GetMachineTaskParameterList();
	}
}
