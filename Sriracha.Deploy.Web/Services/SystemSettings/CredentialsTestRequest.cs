using ServiceStack.ServiceHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sriracha.Deploy.Web.Services.SystemSettings
{
	[Route("/systemSettings/credentials/test/{id}")]
	public class CredentialsTestRequest
	{
		public string Id { get; set; }
	}
}