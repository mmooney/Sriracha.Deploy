using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack.ServiceHost;
using Sriracha.Deploy.Data.Dto;

namespace Sriracha.Deploy.Web.Services.Deployment
{
	[Route("/deployBatchStatus")]
	[Route("/deployBatchStatus/{id}")]
	public class DeployBatchStatusRequest : RequestBase<DeployBatchStatus>
	{
	}
}