using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Dto
{
	public class DeployBatchRequestItem
	{
		public string Id { get; set; }
		public string ProjectId { get; set; }
		public string ProjectName { get; set; }
		public string ComponentId { get; set; }
		public string ComponentName { get; set; }
		public string BranchId { get; set; }
		public string BranchName { get; set; }
	}
}
