using Sriracha.Deploy.Data.Account;
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
            throw new NotImplementedException();
        }
    }
}
