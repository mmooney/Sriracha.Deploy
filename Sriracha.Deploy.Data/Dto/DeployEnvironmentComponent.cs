using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Dto
{
	public class DeployEnvironmentComponent
	{
		public string Id { get; set; }
		public string ProjectId { get; set; }
		public string EnvironmentId { get; set; }
		public string ComponentId { get; set; }
		public List<DeployServer> ServerList { get; set; }

		public DeployEnvironmentComponent()
		{
			this.ServerList = new List<DeployServer>();
		}
	}
}
