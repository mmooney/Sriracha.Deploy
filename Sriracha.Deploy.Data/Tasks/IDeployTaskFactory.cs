using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Tasks
{
	public interface IDeployTaskFactory
	{
		IDeployTaskExecutor CreateTaskExecutor(Type type);

		IDeployTaskDefinition CreateTaskDefinition(string taskTypeName, string taskOptionJson);
	}
}
