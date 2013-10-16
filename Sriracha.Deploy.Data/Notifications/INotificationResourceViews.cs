using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Notifications
{
	public interface INotificationResourceViews
	{
		string BuildPublishEmailView { get; }
		string DeployRequestedEmailView { get; }
		string DeployApprovedEmailView { get; }
		string DeployRejectedEmailView { get; }
		string DeployStartedEmailView { get; }
		string DeploySuccessEmailView { get; }
		string DeployFailedEmailView { get; }
	}
}
