using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Dto.Account;
using Sriracha.Deploy.Data.Repository;

namespace Sriracha.Deploy.Data.Impl
{
	public class AccountSettingsManager : IAccountSettingsManager
	{
		private readonly IAccountSettingsRepository _accountSettingsRepository; 
		private readonly IProjectRepository _projectRepository;
		private readonly IUserIdentity _userIdentity;

		public AccountSettingsManager(IAccountSettingsRepository accountSettingsRepository, IProjectRepository projectRepository, IUserIdentity userIdentity)
		{
			_accountSettingsRepository = DIHelper.VerifyParameter(accountSettingsRepository);
			_projectRepository = DIHelper.VerifyParameter(projectRepository);
			_userIdentity = DIHelper.VerifyParameter(userIdentity);
		}

		public AccountSettings GetCurrentUserSettings()
		{
			return this.GetAccountSettings(_userIdentity.UserName);
		}

		public AccountSettings GetAccountSettings(string userName)
		{
			var settings = _accountSettingsRepository.GetAccountSettings(userName);
			if(settings == null)
			{
				settings = new AccountSettings
				{
					UserName = userName
				};
			}
			UpdateNotificationList(settings);
			return settings;
		}

		private void UpdateNotificationList(AccountSettings settings)
		{
			var projectList = _projectRepository.GetProjectList();
			var projectsToAdd = (from p in projectList
								 where !settings.ProjectNotificationItemList.Select(i => i.ProjectId).Contains(p.Id)
								 select p);
			var projectsToDeactivate = (from pni in settings.ProjectNotificationItemList
										where projectList.Select(i => i.Id).Contains(pni.ProjectId)
										select pni);
			foreach (var project in projectsToAdd)
			{
				var notificationItem = new ProjectNotificationItem
				{
					ProjectId = project.Id,
					ProjectName = project.ProjectName,
					ProjectInactive = false,
					UserName = settings.UserName
				};
				settings.ProjectNotificationItemList.Add(notificationItem);
			}
			foreach (var pni in projectsToDeactivate)
			{
				pni.ProjectInactive = true;
			}
		}


		public AccountSettings UpdateCurrentUserSettings(string emailAddress, List<ProjectNotificationItem> projectNotificationItemList)
		{
			var accountSettings = this.GetAccountSettings(_userIdentity.UserName);
			accountSettings.ProjectNotificationItemList = projectNotificationItemList;
			UpdateNotificationList(accountSettings);
			return _accountSettingsRepository.UpdateAccountSettings(_userIdentity.UserName, emailAddress, accountSettings.ProjectNotificationItemList);
		}
	}
}
