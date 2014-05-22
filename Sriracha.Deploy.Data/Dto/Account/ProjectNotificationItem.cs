using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Dto.Account
{
	public class ProjectNotificationItem
	{
		//public string Id { get; set; }
		public string UserName { get; set; }
		public string ProjectId { get; set; }
		public string ProjectName { get; set; }
		public bool ProjectInactive { get; set; }

		public ProjectNotificationFlags Flags { get; set; }

		public ProjectNotificationItem()
		{
			this.Flags = new ProjectNotificationFlags();
		}
	}
}
