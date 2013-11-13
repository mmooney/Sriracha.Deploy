using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack.ServiceHost;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Dto.Deployment;

namespace Sriracha.Deploy.Web.Services.Deployment
{
	[Route("/deploy/batch/status")]
	[Route("/deploy/batch/{id}/status")]
	public class DeployBatchStatusRequest : RequestBase<DeployBatchStatus>
	{
		public EnumDeployStatus? NewStatus { get; set; }
		public string StatusMessage { get; set; }
	}
}