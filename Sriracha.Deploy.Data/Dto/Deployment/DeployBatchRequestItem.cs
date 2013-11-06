using Sriracha.Deploy.Data.Dto.Build;
using Sriracha.Deploy.Data.Dto.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Dto.Deployment
{
	public class DeployBatchRequestItem
	{
		public string Id { get; set; }
		public DeployBuild Build { get; set; }
		public List<DeployMachine> MachineList { get; set; }
	}
}
