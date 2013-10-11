using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Dto;

namespace Sriracha.Deploy.Data.Tasks
{
	public interface IDeployTaskDefinition
	{
		string TaskDefintionName { get; }
		IList<TaskParameter> GetStaticTaskParameterList();
		IList<TaskParameter> GetEnvironmentTaskParameterList();
		IList<TaskParameter> GetMachineTaskParameterList();
		IList<TaskParameter> GetBuildTaskParameterList();
		Type GetTaskExecutorType();

		Type GetTaskOptionType();
		object DeployTaskOptions { get; set; }
	}
}
