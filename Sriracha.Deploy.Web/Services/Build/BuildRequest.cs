using ServiceStack.ServiceHost;
using Sriracha.Deploy.Data.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sriracha.Deploy.Web.Services.Build
{
	[Route("/build")]
	[Route("/build/{id*}")]
	public class BuildRequest : RequestBase<DeployBuild>
	{
		public string ProjectId { get; set; }
		public string ProjectBranchId { get; set; }
		public string ProjectComponentId { get; set; }
		public string FileId { get; set; }
		public string Version { get; set; }
	}
}