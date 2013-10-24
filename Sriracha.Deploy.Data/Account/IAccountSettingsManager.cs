using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Dto.Account;

namespace Sriracha.Deploy.Data.Account
{
	public interface IAccountSettingsManager
	{
		AccountSettings GetCurrentUserSettings();
		AccountSettings GetAccountSettings(string userName);

		AccountSettings UpdateCurrentUserSettings(string emailAddress, List<ProjectNotificationItem> projectNotificationItemList);
	}
}
