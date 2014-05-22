using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Dto.Project
{
	public class DeployMachine : BaseDto
	{
		public string ProjectId { get; set; }
		public string EnvironmentId { get; set; }
		public string EnvironmentName { get; set; }
		public string ParentId { get; set; }
		public string MachineName { get; set; }
        public Dictionary<string, string> ConfigurationValueList { get; set; }

		public DeployMachine()
		{
			this.ConfigurationValueList = new Dictionary<string,string>();
		}
	}
}
