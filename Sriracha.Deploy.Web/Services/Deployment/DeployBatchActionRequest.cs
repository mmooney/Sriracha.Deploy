using ServiceStack.ServiceHost;
using Sriracha.Deploy.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sriracha.Deploy.Web.Services.Deployment
{
	[Route("/deploy/batch/{id}/action")]
	public class DeployBatchActionRequest
	{
		public string Id { get; set; }
		public EnumDeployBatchAction Action { get; set; }
		public string UserMessage { get; set; }
	}
}