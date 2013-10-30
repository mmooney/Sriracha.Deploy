using ServiceStack.ServiceHost;
using Sriracha.Deploy.Data.Dto.Credentials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sriracha.Deploy.Web.Services.SystemSettings
{
	[Route("/systemSettings/credentials")]
	[Route("/systemSettings/credentials/{id}")]
	public class CredentialsRequest : RequestBase<DeployCredentials>
	{
		public string Domain { get; set; }
		public string UserName { get; set; }
		public string Password { get; set; }
	}
}