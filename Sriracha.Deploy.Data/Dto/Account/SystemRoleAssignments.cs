using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Dto.Account
{
    public class SystemRoleAssignments
    {
        public string Id { get; set; }
        public string SystemRoleId { get; set; }

        public List<string> UserNameList { get; set; }

        public string CreatedByUserName { get; set; }
        public DateTime CreatedDateTimeUtc { get; set; }
        public string UpdatedByUserName { get; set; }
        public DateTime UpdateDateTimeUtc { get; set; }

        public SystemRoleAssignments()
        {
            this.UserNameList = new List<string>();
        }
    }
}
