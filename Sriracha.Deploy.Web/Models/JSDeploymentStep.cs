using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sriracha.Deploy.Web.Models
{
	public class JSDeploymentStep
	{
		public string TaskType { get; set; }
		public string StepName { get; set; }
		public dynamic TaskOptions { get; set; }
	}
}