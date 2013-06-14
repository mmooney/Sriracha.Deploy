using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Dto
{
	public class DeployEnvironment
	{
		public string Id { get; set; }
		public string ProjectId { get; set; }
		public string EnvironmentName { get; set; }
		public List<DeployEnvironmentComponent> ComponentList { get; set; }

		public DeployEnvironment()
		{
			this.ComponentList = new List<DeployEnvironmentComponent>();
		}
	}
}
