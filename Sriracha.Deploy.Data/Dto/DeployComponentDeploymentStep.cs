using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Tasks;

namespace Sriracha.Deploy.Data.Dto
{
	public class DeployComponentDeploymentStep
	{
		public string Id { get; set; }
		public string StepName { get; set; }
		public string TaskTypeName { get; set; }
		public string TaskOptionJSON { get; set; }
	}
}
