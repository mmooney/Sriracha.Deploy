using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Dto
{
	public class SrirachaUser
	{
		public string Id { get; set; }
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

		public DateTime CreatedDateTimcUtc { get; set; }
		public string CreatedByUserName { get; set; }
		public DateTime ModifiedDateTimeUtc { get; set; }
		public string ModifiedByUserName { get; set; }
	}
}
