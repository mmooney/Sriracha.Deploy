using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Dto.Account;

namespace Sriracha.Deploy.Data.Dto
{
	public class MembershipRepositoryFilters
	{
		public ProjectNotificationFlags NotificationFlags { get; set; }
		public string EmailAddress { get; set; }
		public string UserName { get; set; }
	}
}
