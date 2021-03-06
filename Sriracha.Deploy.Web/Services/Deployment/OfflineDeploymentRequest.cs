﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack.ServiceHost;
using Sriracha.Deploy.Data.Dto.Deployment;
using Sriracha.Deploy.Data.Dto.Deployment.Offline;

namespace Sriracha.Deploy.Web.Services.Deployment
{
	[Route("/deploy/offline")]
	[Route("/deploy/offline/{id}")]
	public class OfflineDeploymentRequest : RequestBase<OfflineDeployment>
	{
		public string DeployBatchRequestId { get; set; }
        public string ResultFileId { get; set; }
    }
}