using ServiceStack.ServiceHost;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Dto.Deployment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sriracha.Deploy.Web.Services.Deployment
{
	[Route("/deploy/batch/request")]
	[Route("/deploy/batch/request/{id}")]
	public class BatchRequestRequest : RequestBase<DeployBatchRequest>
	{
		public List<DeployBatchRequestItem> ItemList { get; set; }
		public EnumDeployStatus Status { get; set; }
		public string DeploymentLabel { get; set; }
	}
}