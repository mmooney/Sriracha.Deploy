using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Dto.Account;
using Sriracha.Deploy.Data.Repository;

namespace Sriracha.Deploy.Data.Impl
{
	public class AccountSettingsManager : IAccountSettingsManager
	{
		private readonly IMembershipRepository _membershipRepository; 
		private readonly IProjectRepository _projectRepository;
		private readonly IUserIdentity _userIdentity;

		public AccountSettingsManager(IMembershipRepository membershipRepository, IProjectRepository projectRepository, IUserIdentity userIdentity)
		{
			_membershipRepository = DIHelper.VerifyParameter(membershipRepository);
			_projectRepository = DIHelper.VerifyParameter(projectRepository);
			_userIdentity = DIHelper.VerifyParameter(userIdentity);
		}

		public AccountSettings GetCurrentUserSettings()
		{
			return this.GetAccountSettings(_userIdentity.UserName);
		}

		public AccountSettings GetAccountSettings(string userName)
		{
			var settings = new AccountSettings
			{
				UserName = userName
			};
			var user = _membershipRepository.TryLoadUserByUserName(userName);
			if(user != null)
			{
				settings.EmailAddress = user.EmailAddress;
				settings.ProjectNotificationItemList = user.ProjectNotificationItemList;
			}
			if(settings.ProjectNotificationItemList == null)
			{
				settings.ProjectNotificationItemList = new List<ProjectNotificationItem>();
			}
			UpdateNotificationList(userName, settings.ProjectNotificationItemList);
			return settings;
		}

		private void UpdateNotificationList(string userName, List<ProjectNotificationItem> projectNotificationItemList)
		{
			var projectList = _projectRepository.GetProjectList();
			var projectsToAdd = (from p in projectList
								 where !projectNotificationItemList.Select(i => i.ProjectId).Contains(p.Id)
								 select p);
			var projectsToDeactivate = (from pni in projectNotificationItemList
										where projectList.Select(i => i.Id).Contains(pni.ProjectId)
										select pni);
			foreach (var project in projectsToAdd)
			{
				var notificationItem = new ProjectNotificationItem
				{
					ProjectId = project.Id,
					ProjectName = project.ProjectName,
					ProjectInactive = false,
					UserName = userName
				};
				projectNotificationItemList.Add(notificationItem);
			}
			foreach (var pni in projectsToDeactivate)
			{
				pni.ProjectInactive = true;
			}
		}


		public AccountSettings UpdateCurrentUserSettings(string emailAddress, List<ProjectNotificationItem> projectNotificationItemList)
		{
			UpdateNotificationList(_userIdentity.UserName, projectNotificationItemList);
			var user = _membershipRepository.TryLoadUserByUserName(_userIdentity.UserName);
			if(user == null)
			{
				user = new SrirachaUser
				{
					UserGuid = Guid.NewGuid(),
					UserName = _userIdentity.UserName,
					EmailAddress = emailAddress,
					ProjectNotificationItemList = projectNotificationItemList
				};
				user = _membershipRepository.CreateUser(user);
			}
			else 
			{
				user.EmailAddress = emailAddress;
				user.ProjectNotificationItemList = projectNotificationItemList;
				user = _membershipRepository.UpdateUser(user);
			}
			return new AccountSettings
			{
				UserName = user.UserName,
				EmailAddress = user.EmailAddress,
				ProjectNotificationItemList = projectNotificationItemList
			};
		}
	}
}
