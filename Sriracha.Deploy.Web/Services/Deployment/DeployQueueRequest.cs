using ServiceStack.ServiceHost;
using Sriracha.Deploy.Data.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sriracha.Deploy.Web.Services.Deployment
{
	[Route("/deploy/queue")]
	public class DeployQueueRequest : RequestBase<DeployQueueItem>
	{
		
	}
}