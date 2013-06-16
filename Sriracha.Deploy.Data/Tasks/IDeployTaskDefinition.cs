using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Dto;

namespace Sriracha.Deploy.Data.Tasks
{
	public interface IDeployTaskDefinition
	{
		IList<TaskParameter> GetStaticTaskParameterList();
		IList<TaskParameter> GetEnvironmentTaskParameterList();
		IList<TaskParameter> GetMachineTaskParameterList();
		Type GetTaskExecutorType();
	}
}
