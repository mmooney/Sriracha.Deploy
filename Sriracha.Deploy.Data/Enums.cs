﻿using MMDB.Shared;
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
		Archive,
        MarkFailed
	}

    public enum EnumDeployStatus
    {
        Unknown = 0,
        [EnumDisplayValue("Requested")]
        Requested = 1,
        [EnumDisplayValue("Approved")]
        Approved = 2,
        [EnumDisplayValue("Rejected")]
        Rejected = 3,
        [EnumDisplayValue("Not Started")]
        NotStarted = 4,
        [EnumDisplayValue("In Process")]
        InProcess = 5,
        [EnumDisplayValue("Warning")]
        Warning = 6,
        [EnumDisplayValue("Success")]
        Success = 7,
        [EnumDisplayValue("Error")]
        Error = 8,
        [EnumDisplayValue("Cancelled")]
        Cancelled = 9,
        [EnumDisplayValue("Offline Requested")]
        OfflineRequested = 10,
        [EnumDisplayValue("Offline Deployment Complete")]
        OfflineComplete
    }

    public enum EnumDeploymentIsolationType
    {
        IsolatedPerMachine = 0,
        IsolatedPerDeployment = 1,
        NoIsolation = 2
    }

	public enum EnumQueueStatus
	{
		New = 0,
		InProcess = 1,
		Completed = 2,
		Error = 3
	}

    public enum EnumPermissionAccess
    {
        Grant = 0,
        None = 1,
        Deny = 2
    }

    public enum EnumSystemPermission
    {
        Unknown = 0,
        EditSystemPermissions,
        EditUsers,
        EditDeploymentCredentials,
        EditBuildPurgeRules
    }
}
