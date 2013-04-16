using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Tasks;

namespace Sriracha.Deploy.Data.Tasks
{
	public abstract class BaseDeployTask<TaskOptions, EnvironmentConfiguration> : IDeployTask
		where TaskOptions : DeployTaskOptions, new()
		where EnvironmentConfiguration : DeployTaskEnvironmentConfiguration, new()
	{
		public TaskOptions Options { get; private set; }
		public EnvironmentConfiguration Configuration { get; private set; }
		
		public BaseDeployTask()
		{
			this.Options = new TaskOptions();
			this.Configuration = new EnvironmentConfiguration();
		}
	}
}
