using ServiceStack.ServiceInterface;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Account;
using Sriracha.Deploy.Data.Dto.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sriracha.Deploy.Web.Services.SystemSettings
{
    public class SystemRoleService : Service
    {
        private readonly ISystemRoleManager _systemRoleManager;
        private readonly IPermissionValidator _permissionValidator;

        public SystemRoleService(ISystemRoleManager systemRoleManager, IPermissionValidator permissionValidator)
        {
            _systemRoleManager = DIHelper.VerifyParameter(systemRoleManager);
            _permissionValidator = DIHelper.VerifyParameter(permissionValidator);
        }

        public object Get(SystemRoleRequest request)
        {
            //_permissionValidator.VerifyCurrentUserSystemPermission(EnumSystemPermission.EditSystemPermissions);
            if(request == null)
            {
                throw new ArgumentNullException("request is null");
            }
            if(!string.IsNullOrEmpty(request.Id))
            {
                return _systemRoleManager.GetSystemRole(request.Id);
            }
            else if(!string.IsNullOrEmpty(request.UserId))
            {
                return _systemRoleManager.GetSystemRoleListForUserId(request.UserId);
            }
            else 
            {
                return _systemRoleManager.GetSystemRoleList(request.BuildListOptions());
            }
        }

        public object Post(SystemRoleRequest request)
        {
            return this.Save(request);
        }

        public object Put(SystemRoleRequest request)
        {
            return this.Save(request);
        }

        private SystemRole Save(SystemRoleRequest request)
        {
            _permissionValidator.VerifyCurrentUserSystemPermission(EnumSystemPermission.EditSystemPermissions);
            if(request == null)
            {
                throw new ArgumentNullException("request is null");
            }
            if(string.IsNullOrEmpty(request.Id))
            {
                return _systemRoleManager.CreateSystemRole(request.RoleName, request.Permissions, request.Assignments);
            }
            else 
            {
                return _systemRoleManager.UpdateSystemRole(request.Id, request.RoleName, request.Permissions, request.Assignments);
            }
        }

        public object Delete(SystemRoleRequest request)
        {
            _permissionValidator.VerifyCurrentUserSystemPermission(EnumSystemPermission.EditSystemPermissions);
            if(request == null)
            {
                throw new ArgumentNullException("request is null");
            }
            if(string.IsNullOrEmpty(request.Id))
            {
                throw new ArgumentNullException("request.id is null");
            }
            return _systemRoleManager.DeleteSystemRole(request.Id);
        }

    }
}