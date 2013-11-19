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
		Build,
		Deploy
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
		[EnumDisplayValue("Cancelled")]
		Cancelled
	}

    public enum EnumDeploymentIsolationType
    {
        IsolatedPerMachine = 0,
        IsolatedPerDeployment = 1,
        NoIsolation = 2
    }
}
