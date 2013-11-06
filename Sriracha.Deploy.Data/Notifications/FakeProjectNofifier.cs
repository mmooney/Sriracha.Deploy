using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Dto.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Notifications
{
	public class FakeProjectNofifier : IProjectNotifier
	{
		public void SendBuildPublishedNotification(DeployProject project, Dto.DeployBuild build)
		{
		}

		public void SendDeployRequestedNotification(DeployBatchRequest deployRequest)
		{
		}

		public void SendDeployApprovedNotification(DeployBatchRequest deployRequest)
		{
		}

		public void SendDeployRejectedNotification(DeployBatchRequest deployRequest)
		{
		}

		public void SendDeployStartedNotification(DeployBatchRequest deployRequest)
		{
		}

		public void SendDeploySuccessNotification(DeployBatchRequest deployRequest)
		{
		}

		public void SendDeployFailedNotification(DeployBatchRequest deployRequest)
		{
		}

		public void SendDeployCancelledNotification(DeployBatchRequest deployRequest)
		{
		}
	}
}
