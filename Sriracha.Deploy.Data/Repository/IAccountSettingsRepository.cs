using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Dto.Account;

namespace Sriracha.Deploy.Data.Repository
{
	public interface IAccountSettingsRepository
	{
		AccountSettings GetAccountSettings(string userName);
		AccountSettings UpdateAccountSettings(string userName, string emailAddress, List<ProjectNotificationItem> notificationList);
	}
}
