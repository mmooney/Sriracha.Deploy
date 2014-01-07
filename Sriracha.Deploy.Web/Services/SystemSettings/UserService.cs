using ServiceStack.ServiceInterface;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sriracha.Deploy.Web.Services.SystemSettings
{
    public class UserService : Service
    {
        private readonly IUserManager _userManager;
        private readonly IPermissionValidator _permissionValidator;

        public UserService(IUserManager userManager, IPermissionValidator permissionValidator)
        {
            _userManager = DIHelper.VerifyParameter(userManager);
            _permissionValidator = DIHelper.VerifyParameter(permissionValidator);
        }
        
        public object Get(UserRequest request)
        {
            _permissionValidator.VerifyCurrentUserSystemPermission(EnumSystemPermission.EditUsers);
            if (request == null)
            {
                throw new ArgumentNullException("request is null");
            }
            else if(!string.IsNullOrEmpty(request.Id))
            {
                return _userManager.GetUser(request.Id);
            }
            else 
            {
                return _userManager.GetUserList(request.BuildListOptions(), request.UserNameList);
            }
        }
        
        public object Delete(UserRequest request)
        {
            _permissionValidator.VerifyCurrentUserSystemPermission(EnumSystemPermission.EditUsers);
            if (request == null)
            {
                throw new ArgumentNullException("request is null");
            }
            if(string.IsNullOrEmpty(request.Id))
            {
                throw new ArgumentNullException("request.id is null");
            }
            return _userManager.DeleteUser(request.Id);
        }

        public object Post(UserRequest request)
        {
            return this.Save(request);
        }

        public object Put(UserRequest request)
        {
            return this.Save(request);
        }

        private SrirachaUser Save(UserRequest request)
        {
            _permissionValidator.VerifyCurrentUserSystemPermission(EnumSystemPermission.EditUsers);
            if (string.IsNullOrEmpty(request.Id))
            {
                return _userManager.CreateUser(request.UserName, request.EmailAddress, request.Password);
            }
            else 
            {
                return _userManager.UpdateUser(request.Id, request.UserName, request.EmailAddress, request.Password);
            }
        }
    }
}