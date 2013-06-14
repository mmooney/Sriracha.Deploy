using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Dto
{
	public class DeployMachine
	{
		public string Id { get; set; }
		public string ProjectId { get; set; }
		public string EnvironmentId { get; set; }
		public string EnvironmentComponentId { get; set; }
		public string MachineName { get; set; }
		public string RoleName { get; set; }
		public Dictionary<string, string> ConfigurationValueList { get; set; }

		public DeployMachine()
		{
			this.ConfigurationValueList = new Dictionary<string,string>();
		}
	}
}
