using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Dto.Build;
using Sriracha.Deploy.Data.Dto.Deployment;
using Sriracha.Deploy.Data.Dto.Project;
using Sriracha.Deploy.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Notifications
{
	public class FakeProjectNofifier : IProjectNotifier
	{
		public void SendBuildPublishedNotification(DeployProject project, DeployBuild build)
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
