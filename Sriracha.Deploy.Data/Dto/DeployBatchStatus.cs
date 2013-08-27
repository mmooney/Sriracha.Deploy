using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Dto
{
	public class DeployBatchStatus
	{
		public string DeployBatchRequestId { get; set; }
		public DeployBatchRequest Request { get; set; }

		public List<DeployStateSummary> DeployStateList { get; set; }
	}
}
