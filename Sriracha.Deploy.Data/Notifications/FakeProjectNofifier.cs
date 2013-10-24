using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Notifications
{
	public class FakeProjectNofifier : IProjectNotifier
	{
		public void SendBuildPublishedNotification(Dto.DeployProject project, Dto.DeployBuild build)
		{
		}

		public void SendDeployRequestedNotification(Dto.DeployBatchRequest deployRequest)
		{
		}

		public void SendDeployApprovedNotification(Dto.DeployBatchRequest deployRequest)
		{
		}

		public void SendDeployRejectedNotification(Dto.DeployBatchRequest deployRequest)
		{
		}

		public void SendDeployStartedNotification(Dto.DeployBatchRequest deployRequest)
		{
		}

		public void SendDeploySuccessNotification(Dto.DeployBatchRequest deployRequest)
		{
		}

		public void SendDeployFailedNotification(Dto.DeployBatchRequest deployRequest)
		{
		}

		public void SendDeployCancelledNotification(Dto.DeployBatchRequest deployRequest)
		{
		}
	}
}
