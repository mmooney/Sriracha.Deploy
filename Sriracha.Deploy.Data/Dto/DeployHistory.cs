using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Dto
{
	public enum EnumDeployHistoryStatus
	{
		Unkown,
		NotStarted,
		InProcess,
		Warning,
		Success,
		Error
	}

	public class DeployHistory
	{
		public string Id { get; set; }
		public string BuildId { get; set; }
		public string EnvironmentId { get; set; }
		public EnumDeployHistoryStatus Status { get; set; }
	}
}
