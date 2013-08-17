using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack.ServiceHost;

namespace Sriracha.Deploy.Web.Services.ValidateEnvironment
{
	[Route("/validateEnvironment")]
	public class ValidateEnvironmentRequest
	{
		public string EnvironmentId { get; set; }
		public string BuildId { get; set; }
	}
}