using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Dto
{
	public class DeployRequest
	{
		public string ProjectId { get; set; }
		public string BuildId { get; set; }
		public string EnvironmentId { get; set; }
		public List<string> MachineIdList { get; set; }
		public string DeployStateId { get; set; }

		public DeployRequest()
		{
			this.MachineIdList = new List<string>();
		}

		public string BranchId { get; set; }
	}
}
