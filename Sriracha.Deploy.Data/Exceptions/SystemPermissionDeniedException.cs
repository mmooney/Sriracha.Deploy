using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Exceptions
{
    public class SystemPermissionDeniedException : Exception
    {
        public EnumSystemPermission Permission { get; private set; }
        public string PermissionDescription { get; private set; }
        public string UserName { get; private set; }
        public string PageUrl { get; private set; }

        public SystemPermissionDeniedException(string userName, EnumSystemPermission permission) : base(FormatMessage(userName, permission))
        {
            this.UserName = userName;
            this.Permission = permission;
        }

        private static string FormatMessage(string userName, EnumSystemPermission permission)
        {
            return string.Format("User {0} does not have permission {1}", userName, permission);
        }
    }
}
