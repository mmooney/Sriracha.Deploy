using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.Shared;

namespace Sriracha.Deploy.Data.Dto.Deployment
{
	public class DeployState : DeployStateSummary
	{
		public List<DeployStateMessage> MessageList { get; set; }

		public DeployState()
		{
			this.MessageList = new List<DeployStateMessage>();
		}
	}
}
