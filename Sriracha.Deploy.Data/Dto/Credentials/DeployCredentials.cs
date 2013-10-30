using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Dto.Credentials
{
	public class DeployCredentials
	{
		public string Id { get; set; }
		public string Domain { get; set; }
		public string UserName { get; set; }
		public string EncryptedPassword { get; set; }

		public DateTime CreatedDateTimeUtc { get; set; }
		public string CreatedByUserName { get; set; }
		public DateTime UpdatedDateTimeUtc { get; set; }
		public string UpdatedByUserName { get; set; }

	}
}
