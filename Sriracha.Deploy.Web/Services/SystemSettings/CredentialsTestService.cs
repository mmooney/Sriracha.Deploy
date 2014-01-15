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
        private readonly IPermissionValidator _permissionValidator;

		public CredentialsTestService(IImpersonator impersonator, IPermissionValidator permissionValidator)
		{
			_impersonator = DIHelper.VerifyParameter(impersonator);
            _permissionValidator = DIHelper.VerifyParameter(permissionValidator);
		}

		public object Get(CredentialsTestRequest request)
		{
            _permissionValidator.VerifyCurrentUserSystemPermission(EnumSystemPermission.EditDeploymentCredentials);
            if (request == null)
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