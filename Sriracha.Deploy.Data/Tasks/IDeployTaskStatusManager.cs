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
		void Debug(string deployStateId, string message);
		void Info(string deployStateId, string message);
        void Warn(string deployStateId, string message);
        void Error(string deployStateId, string message);
		void Error(string deployStateId, Exception err);
    }
}
