using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Dto
{
	public class DeployRequest
	{
		public string Id { get; set; }
		public string ProjectId { get; set; }
		public string EnvironmentId { get; set; }
		public string BuildId { get; set; }
	}
}
