using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Dto.Account
{
	public class AccountSettings
	{
		public string Id { get; set; }
		public string UserName { get; set; }
		public string EmailAddress { get; set; }

		public List<ProjectNotificationItem> ProjectNotificationItemList { get; set; }

		public AccountSettings()
		{
			this.ProjectNotificationItemList = new List<ProjectNotificationItem>();
		}
	}
}
