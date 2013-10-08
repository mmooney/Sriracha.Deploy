using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Dto
{
	public class DeployConfiguration
	{
		public string Id { get; set; }
		public string ProjectId { get; set; }
		public string ConfigurationName { get; set; }
		public List<DeployStep> DeploymentStepList { get; set; }
		public DateTime CreatedDateTimeUtc { get; set; }
		public string CreatedByUserName { get; set; }
		public DateTime UpdatedDateTimeUtc { get; set; }
		public string UpdatedByUserName { get; set; }

		public DeployConfiguration()
		{
			this.DeploymentStepList = new List<DeployStep>();
		}
	}
}
