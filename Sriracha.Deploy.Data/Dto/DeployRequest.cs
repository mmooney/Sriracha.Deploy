using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Dto
{
	public class DeployRequest
	{
		public string Id { get; set; }
		public DeployEnvironment DeployEnvironment { get; set; }
		public DeployBuild Build { get; set; }
	}
}
