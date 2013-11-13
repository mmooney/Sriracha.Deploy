using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Dto.Project;
using Sriracha.Deploy.Data.Dto.Deployment;
using Sriracha.Deploy.Data.Dto.Build;

namespace Sriracha.Deploy.Data.Notifications
{
	public interface IProjectNotifier
	{
		void SendBuildPublishedNotification(DeployProject project, DeployBuild build);

		void SendDeployRequestedNotification(DeployBatchRequest deployRequest);
		void SendDeployApprovedNotification(DeployBatchRequest deployRequest);
		void SendDeployRejectedNotification(DeployBatchRequest deployRequest);

		void SendDeployStartedNotification(DeployBatchRequest deployRequest);
		void SendDeploySuccessNotification(DeployBatchRequest deployRequest);
		void SendDeployFailedNotification(DeployBatchRequest deployRequest);
		void SendDeployCancelledNotification(DeployBatchRequest deployRequest);
	}
}
