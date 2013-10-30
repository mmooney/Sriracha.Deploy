using ServiceStack.ServiceInterface;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Credentials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sriracha.Deploy.Web.Services.SystemSettings
{
	public class CredentialsTestService : Service
	{
		private readonly IImpersonator _impersonator;

		public CredentialsTestService(IImpersonator impersonator)
		{
			_impersonator = DIHelper.VerifyParameter(impersonator);
		}

		public object Get(CredentialsTestRequest request)
		{
			if(request == null)
			{
				throw new ArgumentNullException("request is null");
			}
			if(string.IsNullOrEmpty(request.Id))
			{
				throw new ArgumentNullException("request.id is null");
			}
			using(var context = _impersonator.BeginImpersonation(request.Id))
			{
				return true;
			}
		}
	}
}