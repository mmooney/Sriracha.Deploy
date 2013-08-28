using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Dto
{
	public class DeployBatchRequest
	{
		public string Id { get; set; }
		public DateTime SubmittedDateTimeUtc { get; set; }
		public List<DeployBatchRequestItem> ItemList { get; set; }
		public EnumDeployStatus Status { get; set; }

		public DateTime? DeploymentStartedDateTimeUtc { get; set; }
		public DateTime? DeploymentCompleteDateTimeUtc { get; set; }
		public string ErrorDetails { get; set; }

	}
}
