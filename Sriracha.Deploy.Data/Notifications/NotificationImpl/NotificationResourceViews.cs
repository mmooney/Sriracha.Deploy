using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Notifications.NotificationImpl
{
	public class NotificationResourceViews : INotificationResourceViews
	{
		public string BuildPublishEmailView
		{
			get { return SrirachaResources.BuildPublishEmailView; }
		}

		public string DeployRequestedEmailView
		{
			get { return SrirachaResources.DeployRequestedEmailView; }
		}

		public string DeployApprovedEmailView
		{
			get { return SrirachaResources.DeployApprovedEmailView; }
		}

		public string DeployRejectedEmailView
		{
			get { return SrirachaResources.DeployRejectedEmailView; }
		}


		public string DeployStartedEmailView
		{
			get { return SrirachaResources.DeployStartedEmailView; }
		}


		public string DeploySuccessEmailView
		{
			get { return SrirachaResources.DeploySuccessEmailView; }
		}


		public string DeployFailedEmailView
		{
			get { return SrirachaResources.DeployFailedEmailView; }
		}
	}
}
