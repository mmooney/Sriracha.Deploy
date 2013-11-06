using ServiceStack.ServiceInterface;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Dto.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sriracha.Deploy.Web.Services.Security.EffectivePermissions
{
	public class EffectivePermissionsService : Service
	{
		private readonly IPermissionValidator _permissionValidator;

		public EffectivePermissionsService(IPermissionValidator permissionValidator)
		{
			_permissionValidator = DIHelper.VerifyParameter(permissionValidator);
		}

		public object Get(UserEffectivePermissions request)
		{
			if(request == null)
			{
				throw new ArgumentNullException("request is null");
			}
			if(string.IsNullOrEmpty(request.UserName))
			{
				throw new ArgumentNullException("request.UserName is null");
			}
			return _permissionValidator.GetUserEffectivePermissions(request.UserName);
		}
	}
}