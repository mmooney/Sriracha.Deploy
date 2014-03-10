using Sriracha.Deploy.Data.Account;
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

        public SystemSetterUpper(IUserManager userManager, ISystemRoleManager systemRoleManager)
        {
            _userManager = DIHelper.VerifyParameter(userManager);
            _systemRoleManager = DIHelper.VerifyParameter(systemRoleManager);
        }

        public void SetupSystem(string adminstratorUserName, string adminstratorPassword, string adminstratorEmailAddress)
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
    }
}
