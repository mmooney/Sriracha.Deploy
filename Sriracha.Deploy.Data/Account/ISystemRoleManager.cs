using Sriracha.Deploy.Data.Dto.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Account
{
    public interface ISystemRoleManager
    {
        List<SystemRole> GetSystemRoleListForUser(string userName);
    }
}
