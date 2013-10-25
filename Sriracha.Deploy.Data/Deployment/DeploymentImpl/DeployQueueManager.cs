using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Notifications;
using Sriracha.Deploy.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Deployment.DeploymentImpl
{
	public class DeployQueueManager : IDeployQueueManager
	{
		private readonly IDeployRepository _deployRepository;
		private readonly IProjectNotifier _projectNotifier;

		public DeployQueueManager(IDeployRepository deployRepository, IProjectNotifier projectNotifier)
		{
			_deployRepository = DIHelper.VerifyParameter(deployRepository);
			_projectNotifier = DIHelper.VerifyParameter(projectNotifier);
		}

		public PagedSortedList<DeployBatchRequest> GetQueue(ListOptions listOptions, List<EnumDeployStatus> statusList=null, List<string> environmentIds=null, bool includeResumeRequested=true)
		{
			return _deployRepository.GetDeployQueue(listOptions, statusList, environmentIds, includeResumeRequested);
		}

		public DeployBatchRequest PopNextBatchDeployment()
		{
			var deployRequest = _deployRepository.PopNextBatchDeployment();
			if (deployRequest != null)
			{
				_projectNotifier.SendDeployStartedNotification(deployRequest);
			}
			return deployRequest;
		}

		public DeployBatchRequest RequeueDeployment(string deployBatchRequestId, string message)
		{
			return _deployRepository.RequeueDeployment(deployBatchRequestId, EnumDeployStatus.NotStarted, statusMessage: message);
		}
	}
}
