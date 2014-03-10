using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.SystemSettings
{
    public interface ISystemSetterUpper
    {
        void SetupSystem(string adminstratorUserName, string adminstratorPassword, string adminstratorEmailAddress);
    }
}
