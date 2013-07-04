using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Tasks
{
	public enum EnumDeployTaskExecutionResultStatus
	{
		Unknown,
		Success,
		Warning,
		Error
	}

	public class DeployTaskExecutionResult
	{
		public EnumDeployTaskExecutionResultStatus Status { get; set; }
	}
}
