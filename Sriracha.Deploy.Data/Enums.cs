using MMDB.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data
{
	public enum EnumConfigLevel
	{
		Static,
		Environment,
		Machine,
		Build
	}

	public enum EnumDeployBatchAction
	{
		Cancel,
		Resume,
		Restart,
		Archive
	}

	public enum EnumDeployStatus
	{
		Unknown,
		[EnumDisplayValue("Requested")]
		Requested,
		[EnumDisplayValue("Approved")]
		Approved,
		[EnumDisplayValue("Rejected")]
		Rejected,
		[EnumDisplayValue("Not Started")]
		NotStarted,
		[EnumDisplayValue("In Process")]
		InProcess,
		[EnumDisplayValue("Warning")]
		Warning,
		[EnumDisplayValue("Success")]
		Success,
		[EnumDisplayValue("Error")]
		Error,
		[EnumDisplayValue("Cancelling")]
		Cancelling,
		[EnumDisplayValue("Cancelled")]
		Cancelled
	}
}
