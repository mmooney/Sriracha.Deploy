using Sriracha.Deploy.Data.Account;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Dto.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.SystemSettings
{
    public class SystemSetterUpper : ISystemSetterUpper
    {
        private readonly IUserManager _userManager;
        private readonly ISystemRoleManager _systemRoleManager;
        private readonly ISystemSettings _systemSettings;

        public SystemSetterUpper(IUserManager userManager, ISystemRoleManager systemRoleManager, ISystemSettings systemSettings)
        {
            _userManager = DIHelper.VerifyParameter(userManager);
            _systemRoleManager = DIHelper.VerifyParameter(systemRoleManager);
            _systemSettings = DIHelper.VerifyParameter(systemSettings);
        }

        public void SetupAdministratorUser(string adminstratorUserName, string adminstratorPassword, string adminstratorEmailAddress)
        {
            if(string.IsNullOrEmpty(adminstratorUserName)) 
            {
                throw new ArgumentNullException("Missing administrator user name");
            }
            if(string.IsNullOrEmpty(adminstratorPassword)) 
            {
                throw new ArgumentNullException("Missing administrator password");
            }
            if(string.IsNullOrEmpty(adminstratorEmailAddress))
            {
                throw new ArgumentNullException("Missing administrator email address");
            }
            var user = _userManager.TryGetUserByUserName(adminstratorUserName);
            if(user == null)
            {
                user = _userManager.CreateUser(adminstratorUserName, adminstratorEmailAddress, adminstratorPassword);
            }
            else 
            {
                user = _userManager.UpdateUser(user.Id, adminstratorUserName, adminstratorEmailAddress, adminstratorPassword);
            }

            var adminRole = _systemRoleManager.GetBuiltInRole(EnumSystemRoleType.Administrator);
            if(adminRole == null)
            {
                throw new Exception("Failed to retrieve administrator role");
            }
            if(!adminRole.Assignments.UserNameList.Contains(adminstratorUserName))
            {
                adminRole.Assignments.UserNameList.Add(adminstratorUserName);
                _systemRoleManager.UpdateSystemRole(adminRole.Id, adminRole.RoleName, adminRole.Permissions, adminRole.Assignments);
            }
        }


        public void SetupAdministratorUser()
        {
            throw new NotImplementedException();
        }


        public SrirachaUser GetAdministratorUser()
        {
            var role = _systemRoleManager.GetBuiltInRole(EnumSystemRoleType.Administrator);
            if(role != null && role.Assignments != null && role.Assignments.UserNameList != null && role.Assignments.UserNameList.Count != 0)
            {
                return _userManager.GetUserByUserName(role.Assignments.UserNameList.First());
            }
            else 
            {
                return null;
            }
        }


        public void SetupAdministratorUser(bool allowSelfRegistation, EnumPermissionAccess defaultAccess)
        {
            _systemSettings.AllowSelfRegistration = allowSelfRegistation;
            _systemSettings.DefaultAccess = defaultAccess;
        }
    }
}
