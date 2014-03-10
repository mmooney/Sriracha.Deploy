using Sriracha.Deploy.Data.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.SystemSettings
{
    public interface ISystemSetterUpper
    {
        void SetupAdministratorUser(string adminstratorUserName, string adminstratorPassword, string adminstratorEmailAddress);
        SrirachaUser GetAdministratorUser();
        void SetupAdministratorUser(bool allowSelfRegistation, EnumPermissionAccess enumPermissionAccess);
    }
}
