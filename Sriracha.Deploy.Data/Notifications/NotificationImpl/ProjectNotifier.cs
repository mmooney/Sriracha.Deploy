using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.RazorEmail;
using MMDB.Shared;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Dto.Account;
using Sriracha.Deploy.Data.Repository;
using Sriracha.Deploy.Data.Dto.Project;
using Sriracha.Deploy.Data.Dto.Deployment;
using Sriracha.Deploy.Data.Dto.Build;

namespace Sriracha.Deploy.Data.Notifications.NotificationImpl
{
	public class ProjectNotifier : IProjectNotifier
	{
		private readonly IMembershipRepository _membershipRepository;
		private readonly IEmailQueue _emailQueue;
		private readonly IRazorTemplateRepository _razorTemplateRepository;
		private readonly IUrlGenerator _urlGenerator;
		private readonly ISystemSettings _systemSettings;
		private readonly INotificationResourceViews _notificationResourceViews;
		private readonly IDeployRepository _deployRepository;
        private readonly IDeployStateRepository _deployStateRepository;

        public ProjectNotifier(IMembershipRepository membershipRepository, IEmailQueue emailQueue, IRazorTemplateRepository razorTemplateRepository, IUrlGenerator urlGenerator, ISystemSettings systemSettings, INotificationResourceViews notificationResourceViews, IDeployRepository deployRepository, IDeployStateRepository deployStateRepository)
		{
			_membershipRepository = DIHelper.VerifyParameter(membershipRepository);
			_emailQueue = DIHelper.VerifyParameter(emailQueue);
			_razorTemplateRepository = DIHelper.VerifyParameter(razorTemplateRepository);
			_urlGenerator = DIHelper.VerifyParameter(urlGenerator);
			_systemSettings = DIHelper.VerifyParameter(systemSettings);
			_notificationResourceViews = DIHelper.VerifyParameter(notificationResourceViews);
			_deployRepository = DIHelper.VerifyParameter(deployRepository);
            _deployStateRepository =  DIHelper.VerifyParameter(deployStateRepository);
		}

		private List<string> GetNotificationEmailAddresses(string projectId, Func<ProjectNotificationFlags, bool> flagsFilter)
		{
			List<string> emailAddresseList = new List<string>();
			bool done = false;
			int pageCounter = 1;
			while (!done)
			{
				var listOptions = new ListOptions
				{
					PageNumber = pageCounter,
					PageSize = 10,
					SortAscending = true,
					SortField = "UserName"
				};

				var userList = _membershipRepository.GetUserList(listOptions,
									//i => i.ProjectNotificationItemList.Any(j => j.ProjectId == projectId && flagsFilter(j.Flags)));
									i=>i.ProjectNotificationItemList.Any(j=>j.ProjectId == projectId && j.Flags.DeployRequested));
				foreach (var user in userList.Items)
				{
					if (!string.IsNullOrEmpty(user.EmailAddress))
					{
						emailAddresseList.Add(user.EmailAddress);
					}
				}
				if (userList.IsLastPage)
				{
					done = true;
				}
				else
				{
					pageCounter++;
				}
			}
			return emailAddresseList;
		}

		private List<string> GetNotificationEmailAddresses(List<string> projectIdList, Func<ProjectNotificationFlags, bool> flagsFilter)
		{
			var emailAddressList = new List<string>();
			foreach(string projectId in projectIdList)
			{
				var tempList = GetNotificationEmailAddresses(projectId, flagsFilter);
				foreach(string tempAddress in tempList)
				{
					if(!emailAddressList.Contains(tempAddress))
					{
						emailAddressList.Add(tempAddress);
					}
				}
			}
			return emailAddressList;
		}

		public void SendBuildPublishedNotification(DeployProject project, DeployBuild build)
		{
			var emailAddresseList = GetNotificationEmailAddresses(project.Id, i=>i.BuildPublished);
			if(emailAddresseList != null && emailAddresseList.Count > 0)
			{
				var dataObject = new
				{
					Project = project,
					Build = build,
					ViewBuildUrl = _urlGenerator.ViewBuildUrl(build.Id)
				};
				var template = _razorTemplateRepository.GetTemplate("BuildPublishEmail", _notificationResourceViews.BuildPublishEmailView);
				_emailQueue.QueueMessage("New Build Published", emailAddresseList, dataObject, template.ViewData);
			}
		}

		public void SendDeployRequestedNotification(DeployBatchRequest deployRequest)
		{
			var projectIdList = deployRequest.ItemList.Select(i=>i.Build.ProjectId).Distinct().ToList();
			var emailAddresseList = GetNotificationEmailAddresses(projectIdList, i=>i.DeployRequested);
			if(emailAddresseList != null && emailAddresseList.Count > 0)
			{
				var dataObject = new 
				{
					DeployRequest = deployRequest,
					DisplayTimeZoneIdentifier = _systemSettings.DisplayTimeZoneIdentifier,
					DeployStatusUrl = _urlGenerator.DeployStatusUrl(deployRequest.Id)
				};
				var template = _razorTemplateRepository.GetTemplate("DeployRequestedEmail", _notificationResourceViews.DeployRequestedEmailView);
				var machineNames = string.Join(",", deployRequest.ItemList.SelectMany(i=>i.MachineList.Select(j=>j.MachineName)).Distinct().ToArray());
				string deployLabel = deployRequest.DeploymentLabel;
				string subject = string.Format("New Deployment Requested: {0} ({1})", StringHelper.IsNullOrEmpty(deployLabel,string.Empty), machineNames);
				_emailQueue.QueueMessage(subject, emailAddresseList, dataObject, template.ViewData);
			}
		}


		public void SendDeployApprovedNotification(DeployBatchRequest deployRequest)
		{
			var projectIdList = deployRequest.ItemList.Select(i => i.Build.ProjectId).Distinct().ToList();
			var emailAddresseList = GetNotificationEmailAddresses(projectIdList, i => i.DeployApproved);
			if (emailAddresseList != null && emailAddresseList.Count > 0)
			{
				var dataObject = new
				{
					DeployRequest = deployRequest,
					DisplayTimeZoneIdentifier = _systemSettings.DisplayTimeZoneIdentifier,
					DeployStatusUrl = _urlGenerator.DeployStatusUrl(deployRequest.Id)
				};
				var template = _razorTemplateRepository.GetTemplate("DeployApprovedEmail", _notificationResourceViews.DeployApprovedEmailView);
				var machineNames = string.Join(",", deployRequest.ItemList.SelectMany(i => i.MachineList.Select(j => j.MachineName)).Distinct().ToArray());
				string deployLabel = deployRequest.DeploymentLabel;
				string subject = string.Format("Deployment Approved: {0} ({1})", StringHelper.IsNullOrEmpty(deployLabel, string.Empty), machineNames);
				_emailQueue.QueueMessage(subject, emailAddresseList, dataObject, template.ViewData);
			}
		}


		public void SendDeployRejectedNotification(DeployBatchRequest deployRequest)
		{
			var projectIdList = deployRequest.ItemList.Select(i => i.Build.ProjectId).Distinct().ToList();
			var emailAddresseList = GetNotificationEmailAddresses(projectIdList, i => i.DeployRejected);
			if (emailAddresseList != null && emailAddresseList.Count > 0)
			{
				var dataObject = new
				{
					DeployRequest = deployRequest,
					DisplayTimeZoneIdentifier = _systemSettings.DisplayTimeZoneIdentifier,
					DeployStatusUrl = _urlGenerator.DeployStatusUrl(deployRequest.Id)
				};
				var template = _razorTemplateRepository.GetTemplate("DeployRejectedEmail", _notificationResourceViews.DeployRejectedEmailView);
				var machineNames = string.Join(",", deployRequest.ItemList.SelectMany(i => i.MachineList.Select(j => j.MachineName)).Distinct().ToArray());
				string deployLabel = deployRequest.DeploymentLabel;
				string subject = string.Format("Deployment Rejected: {0} ({1})", StringHelper.IsNullOrEmpty(deployLabel, string.Empty), machineNames);
				_emailQueue.QueueMessage(subject, emailAddresseList, dataObject, template.ViewData);
			}
		}


		public void SendDeployStartedNotification(DeployBatchRequest deployRequest)
		{
			var projectIdList = deployRequest.ItemList.Select(i => i.Build.ProjectId).Distinct().ToList();
			var emailAddresseList = GetNotificationEmailAddresses(projectIdList, i => i.DeployStarted);
			if (emailAddresseList != null && emailAddresseList.Count > 0)
			{
				var dataObject = new
				{
					DeployRequest = deployRequest,
					DisplayTimeZoneIdentifier = _systemSettings.DisplayTimeZoneIdentifier,
					DeployStatusUrl = _urlGenerator.DeployStatusUrl(deployRequest.Id)
				};
				var template = _razorTemplateRepository.GetTemplate("DeployStartedEmail", _notificationResourceViews.DeployStartedEmailView);
				var machineNames = string.Join(",", deployRequest.ItemList.SelectMany(i => i.MachineList.Select(j => j.MachineName)).Distinct().ToArray());
				string deployLabel = deployRequest.DeploymentLabel;
				string subject = string.Format("Deployment Started: {0} ({1})", StringHelper.IsNullOrEmpty(deployLabel, string.Empty), machineNames);
				_emailQueue.QueueMessage(subject, emailAddresseList, dataObject, template.ViewData);
			}
		}

		public void SendDeploySuccessNotification(DeployBatchRequest deployRequest)
		{
			var projectIdList = deployRequest.ItemList.Select(i => i.Build.ProjectId).Distinct().ToList();
			var emailAddresseList = GetNotificationEmailAddresses(projectIdList, i => i.DeployStarted);
			if (emailAddresseList != null && emailAddresseList.Count > 0)
			{
				var dataObject = new
				{
					DeployBatchStatus = new DeployBatchStatus
					{
						DeployBatchRequestId = deployRequest.Id,
						Request = _deployRepository.GetBatchRequest(deployRequest.Id),
                        DeployStateList = _deployStateRepository.GetDeployStateSummaryListByDeployBatchRequestItemId(deployRequest.Id)
					},
					DisplayTimeZoneIdentifier = _systemSettings.DisplayTimeZoneIdentifier,
					DeployStatusUrl = _urlGenerator.DeployStatusUrl(deployRequest.Id)
				};
				var template = _razorTemplateRepository.GetTemplate("DeploySuccessEmail", _notificationResourceViews.DeploySuccessEmailView);
				var machineNames = string.Join(",", deployRequest.ItemList.SelectMany(i => i.MachineList.Select(j => j.MachineName)).Distinct().ToArray());
				string deployLabel = deployRequest.DeploymentLabel;
				string subject = string.Format("Deployment Succeeded: {0} ({1})", StringHelper.IsNullOrEmpty(deployLabel, string.Empty), machineNames);
				_emailQueue.QueueMessage(subject, emailAddresseList, dataObject, template.ViewData);
			}
		}

		public void SendDeployFailedNotification(DeployBatchRequest deployRequest)
		{
			var projectIdList = deployRequest.ItemList.Select(i => i.Build.ProjectId).Distinct().ToList();
			var emailAddresseList = GetNotificationEmailAddresses(projectIdList, i => i.DeployStarted);
			if (emailAddresseList != null && emailAddresseList.Count > 0)
			{
				var dataObject = new
				{
					DeployBatchStatus = new DeployBatchStatus
					{
						DeployBatchRequestId = deployRequest.Id,
						Request = _deployRepository.GetBatchRequest(deployRequest.Id),
                        DeployStateList = _deployStateRepository.GetDeployStateSummaryListByDeployBatchRequestItemId(deployRequest.Id)
					},
					DisplayTimeZoneIdentifier = _systemSettings.DisplayTimeZoneIdentifier,
					DeployStatusUrl = _urlGenerator.DeployStatusUrl(deployRequest.Id)
				};
				var template = _razorTemplateRepository.GetTemplate("DeployFailedEmail", _notificationResourceViews.DeployFailedEmailView);
				var machineNames = string.Join(",", deployRequest.ItemList.SelectMany(i => i.MachineList.Select(j => j.MachineName)).Distinct().ToArray());
				string deployLabel = deployRequest.DeploymentLabel;
				string subject = string.Format("Deployment Failed: {0} ({1})", StringHelper.IsNullOrEmpty(deployLabel, string.Empty), machineNames);
				_emailQueue.QueueMessage(subject, emailAddresseList, dataObject, template.ViewData);
			}
		}

		public void SendDeployCancelledNotification(DeployBatchRequest deployRequest)
		{
			var projectIdList = deployRequest.ItemList.Select(i => i.Build.ProjectId).Distinct().ToList();
			var emailAddresseList = GetNotificationEmailAddresses(projectIdList, i => i.DeployStarted);
			if (emailAddresseList != null && emailAddresseList.Count > 0)
			{
				var dataObject = new
				{
					DeployBatchStatus = new DeployBatchStatus
					{
						DeployBatchRequestId = deployRequest.Id,
						Request = _deployRepository.GetBatchRequest(deployRequest.Id),
                        DeployStateList = _deployStateRepository.GetDeployStateSummaryListByDeployBatchRequestItemId(deployRequest.Id)
					},
					DisplayTimeZoneIdentifier = _systemSettings.DisplayTimeZoneIdentifier,
					DeployStatusUrl = _urlGenerator.DeployStatusUrl(deployRequest.Id)
				};
				var template = _razorTemplateRepository.GetTemplate("DeployCancelledEmail", _notificationResourceViews.DeployCancelledEmailView);
				var machineNames = string.Join(",", deployRequest.ItemList.SelectMany(i => i.MachineList.Select(j => j.MachineName)).Distinct().ToArray());
				string deployLabel = deployRequest.DeploymentLabel;
				string subject = string.Format("Deployment Cancelled: {0} ({1})", StringHelper.IsNullOrEmpty(deployLabel, string.Empty), machineNames);
				_emailQueue.QueueMessage(subject, emailAddresseList, dataObject, template.ViewData);
			}
		}
	}
}
