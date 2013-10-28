using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack.ServiceInterface;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Account;

namespace Sriracha.Deploy.Web.Services.Account
{
	public class AccountSettingsService : Service
	{
		private readonly IAccountSettingsManager _accountSettingsManager;
		private readonly IPermissionValidator _permissionValidator;

		public AccountSettingsService(IAccountSettingsManager accountSettingsManager, IPermissionValidator permissionValidator)
		{
			_accountSettingsManager = DIHelper.VerifyParameter(accountSettingsManager);
			_permissionValidator = DIHelper.VerifyParameter(permissionValidator);
		}

		public object Get(AccountSettingsServiceRequest request)
		{
			if(request == null)
			{
				throw new ArgumentNullException("request is null");
			}
			return new AccountSettingsServiceRequest 
			{
				AccountSettings = _accountSettingsManager.GetCurrentUserSettings(),
				EffectivePermissions = _permissionValidator.GetCurrentUserEffectivePermissions()
			};
		}

		public object Post(AccountSettingsServiceRequest request)
		{
			if(request == null)
			{
				throw new ArgumentNullException("request is null");
			}
			if(request.AccountSettings == null)
			{
				throw new ArgumentNullException("request.AccountSettings is null");
			}
			return _accountSettingsManager.UpdateCurrentUserSettings(request.AccountSettings.EmailAddress, request.AccountSettings.ProjectNotificationItemList);
		}
	}
}