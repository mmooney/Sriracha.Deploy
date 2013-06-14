using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Tasks
{
	public interface IDeployTask
	{
		IList<TaskParameter> GetStaticTaskParameterList();
		IList<TaskParameter> GetEnvironmentTaskParameterList();
		IList<TaskParameter> GetMachineTaskParameterList();
	}
}
