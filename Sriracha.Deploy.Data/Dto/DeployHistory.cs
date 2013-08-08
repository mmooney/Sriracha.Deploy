using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.Shared;

namespace Sriracha.Deploy.Data.Dto
{
	public enum EnumDeployStatus
	{
		Unkown,
		[EnumDisplayValue("Requested")]
		Requested,
		[EnumDisplayValue("Not Started")]
		NotStarted,
		[EnumDisplayValue("In Process")]
		InProcess,
		[EnumDisplayValue("Warning")]
		Warning,
		[EnumDisplayValue("Success")]
		Success,
		[EnumDisplayValue("Error")]
		Error
	}

	public class DeployHistory
	{
		public string Id { get; set; }
		public string BuildId { get; set; }
		public string EnvironmentId { get; set; }
		public EnumDeployStatus Status { get; set; }
	}
}
