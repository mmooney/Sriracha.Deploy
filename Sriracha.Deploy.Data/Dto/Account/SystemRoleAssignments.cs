using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Dto.Account
{
    public class SystemRoleAssignments
    {
        public List<string> UserNameList { get; set; }

        public SystemRoleAssignments()
        {
            this.UserNameList = new List<string>();
        }
    }
}
