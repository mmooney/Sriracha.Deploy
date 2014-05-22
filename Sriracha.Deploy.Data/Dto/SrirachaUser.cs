using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Dto.Account;

namespace Sriracha.Deploy.Data.Dto
{
	public class SrirachaUser : BaseDto
	{
		public string UserName { get; set; }
		public Guid UserGuid { get; set; }
		public string EmailAddress { get; set; }
		public string EncryptedPassword { get; set; }
		public DateTime? LastPasswordChangedDateTimeUtc { get; set; }

		public string PasswordQuestion { get; set; }
		public string PasswordAnswer { get; set; }

		public DateTime? LastLockoutDateTimeUtc { get; set; }
		public DateTime? LastLoginDateDateTimeUtc { get; set; }


		public bool LockedIndicator { get; set; }

		public bool MustChangePasswordIndicator { get; set; }
		public DateTime LastActivityDateTimeUtc { get; set; }

		public List<ProjectNotificationItem> ProjectNotificationItemList { get; set; }

		public SrirachaUser()
		{
			this.ProjectNotificationItemList = new List<ProjectNotificationItem>();
		}
	}
}
