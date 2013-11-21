using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Dto.Project
{
	public class DeployMachine
	{
		public string Id { get; set; }
		public string ProjectId { get; set; }
		public string ProjectName { get; set; }
		public string EnvironmentId { get; set; }
		public string EnvironmentName { get; set; }
		public string ParentId { get; set; }
		public string MachineName { get; set; }
        public DateTime CreatedDateTimeUtc { get; set; }
        public string CreatedByUserName { get; set; }
        public DateTime UpdatedDateTimeUtc { get; set; }
        public string UpdatedByUserName { get; set; }
        public Dictionary<string, string> ConfigurationValueList { get; set; }

		public DeployMachine()
		{
			this.ConfigurationValueList = new Dictionary<string,string>();
		}
	}
}
