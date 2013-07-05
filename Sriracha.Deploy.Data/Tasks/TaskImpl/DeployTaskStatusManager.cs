using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Tasks.TaskImpl
{
	public class DeployTaskStatusManager : IDeployTaskStatusManager
	{
		private List<string> DebugList { get; set; }
		private List<string> InfoList { get; set; }
		private List<string> ErrorList { get; set; }

		public DeployTaskStatusManager()
		{
			this.InfoList = new List<string>();
			this.DebugList = new List<string>();
			this.ErrorList = new List<string>();
		}
		public DeployTaskExecutionResult BuildResult()
		{
			var returnValue = new DeployTaskExecutionResult();
			if(this.ErrorList.Any())
			{
				returnValue.Status = EnumDeployTaskExecutionResultStatus.Error;
			}
			else 
			{
				returnValue.Status = EnumDeployTaskExecutionResultStatus.Success;
			}
			return returnValue;
		}

		public void Debug(string message)
		{
			this.DebugList.Add(message);
		}

		public void Info(string message)
		{
			this.InfoList.Add(message);
		}

		public void Error(string message)
		{
			this.ErrorList.Add(message);
		}

		public void Error(Exception err)
		{
			this.ErrorList.Add(err.ToString());
		}
	}
}
