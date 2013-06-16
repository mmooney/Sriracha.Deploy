using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;

namespace Sriracha.Deploy.Data.Tasks
{
	public interface IDeployTaskStatusManager
	{
		DeployTaskExecutionResult BuildResult();
		void Debug(string message);
		void Info(string message);
		void Error(string message);
		void Error(Exception err);
}
}
