using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Tasks.TaskImpl
{
	public class DeployTaskFactory : IDeployTaskFactory
	{
		public IDeployTaskExecutor CreateTaskExecutor(Type type)
		{
			throw new NotImplementedException();
		}

		public IDeployTaskDefinition CreateTaskDefinition(string taskTypeName, string taskOptionJson)
		{
			throw new NotImplementedException();
		}
	}
}
