using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.RazorEmail;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Dto.Account;
using Sriracha.Deploy.Data.Repository;

namespace Sriracha.Deploy.Data.Notifications.NotificationImpl
{
	public class ProjectNotifier : IProjectNotifier
	{
		private readonly IMembershipRepository _membershipRepository;
		private readonly IEmailQueue _emailQueue;
		private readonly IRazorTemplateRepository _razorTemplateRepository;
		private readonly IUrlGenerator _urlGenerator;
		private readonly ISystemSettings _systemSettings;

		public ProjectNotifier(IMembershipRepository membershipRepository, IEmailQueue emailQueue, IRazorTemplateRepository razorTemplateRepository, IUrlGenerator urlGenerator, ISystemSettings systemSettings)
		{
			_membershipRepository = DIHelper.VerifyParameter(membershipRepository);
			_emailQueue = DIHelper.VerifyParameter(emailQueue);
			_razorTemplateRepository = DIHelper.VerifyParameter(razorTemplateRepository);
			_urlGenerator = DIHelper.VerifyParameter(urlGenerator);
			_systemSettings = DIHelper.VerifyParameter(systemSettings);
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
					Build = build
				};
				var template = _razorTemplateRepository.GetTemplate("BuildPublishEmail", SrirachaResources.BuildPublishEmailView);
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
				var template = _razorTemplateRepository.GetTemplate("DeployRequestedEmail", SrirachaResources.DeployRequestedEmailView);
				_emailQueue.QueueMessage("New Deployment Requested", emailAddresseList, dataObject, template.ViewData);
			}
		}
	}
}
