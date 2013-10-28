using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack.ServiceHost;
using Sriracha.Deploy.Data.Dto.Account;
using Sriracha.Deploy.Data.Dto;

namespace Sriracha.Deploy.Web.Services.Account
{
	[Route("/account")]
	public class AccountSettingsServiceRequest
	{
		public AccountSettings AccountSettings { get; set; }
		public UserEffectivePermissions EffectivePermissions { get; set; }
	}
}