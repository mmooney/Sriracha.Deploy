using ServiceStack.ServiceInterface;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Credentials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sriracha.Deploy.Web.Services.SystemSettings
{
	public class CredentialsService : Service
	{
		private readonly ICredentialsManager _credentialsManager;
        private readonly IPermissionValidator _permissionValidator;

		public CredentialsService(ICredentialsManager credentialsManager, IPermissionValidator permissionValidator)
		{
			_credentialsManager = DIHelper.VerifyParameter(credentialsManager);
            _permissionValidator = DIHelper.VerifyParameter(permissionValidator);
		}

		public object Get(CredentialsRequest request)
		{
            //_permissionValidator.VerifyCurrentUserSystemPermission(EnumSystemPermission.EditDeploymentCredentials);
                //yes, we can read the credentials (we need that to select them on the environment page), we just can't update them without permission
            if (request == null)
			{
				throw new ArgumentNullException("request is null");
			}
			else if (!string.IsNullOrEmpty(request.Id))
			{
				return _credentialsManager.GetMaskedCredentials(request.Id);
			}
			else 
			{
				return _credentialsManager.GetMaskedCredentialList(request.BuildListOptions());
			}
		}

		public object Post(CredentialsRequest request)
		{
            _permissionValidator.VerifyCurrentUserSystemPermission(EnumSystemPermission.EditDeploymentCredentials);
            if (request == null)
			{
				throw new ArgumentNullException("request is null");
			}
			if(string.IsNullOrEmpty(request.UserName)) 
			{
				throw new ArgumentNullException("request.UserName is null");
			}
			if(string.IsNullOrEmpty(request.Password))
			{
				throw new ArgumentNullException("request.Password is null");
			}
			if(string.IsNullOrEmpty(request.Id))
			{
				return _credentialsManager.CreateCredentials(request.Domain, request.UserName, request.Password);
			}
			else 
			{
				return _credentialsManager.UpdateCredentials(request.Id, request.Domain, request.UserName, request.Password);
			}
		}

        public object Delete(CredentialsRequest request)
        {
            _permissionValidator.VerifyCurrentUserSystemPermission(EnumSystemPermission.EditDeploymentCredentials);
            if(request == null)
            {
                throw new ArgumentNullException("request is null");
            }
            if(string.IsNullOrEmpty(request.Id))
            {
                throw new ArgumentNullException("request.id is null");
            }
            return _credentialsManager.DeleteCredentials(request.Id);
        }
	}
}