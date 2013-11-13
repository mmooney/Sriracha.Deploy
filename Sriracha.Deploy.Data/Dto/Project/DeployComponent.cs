using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Dto.Project
{
	public class DeployComponent
	{
		public string Id { get; set; }
		public string ProjectId { get; set; }
		public string ComponentName { get; set; }
		public bool UseConfigurationGroup { get; set; }
		public string ConfigurationId { get; set; }
		public List<DeployStep> DeploymentStepList { get; set; }
        public EnumDeploymentIsolationType IsolationType { get; set; }

        public DateTime CreatedDateTimeUtc { get; set; }
        public string CreatedByUserName { get; set; }
        public DateTime UpdatedDateTimeUtc { get; set; }
        public string UpdatedByUserName { get; set; }

		public DeployComponent()
		{
			this.DeploymentStepList = new List<DeployStep>();
		}
	}
}
