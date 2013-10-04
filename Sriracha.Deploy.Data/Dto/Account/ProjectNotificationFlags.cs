using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Dto.Account
{
	public class ProjectNotificationFlags
	{
		public bool BuildPublished { get; set; }
		public bool DeployRequested {  get; set; }
		public bool DeployApproved { get; set; }
		public bool DeployRejected {  get; set; }
		public bool DeployStarted { get; set; }
		public bool DeploySuccess { get; set; }
		public bool DeployFailure { get; set; }
	}
}
