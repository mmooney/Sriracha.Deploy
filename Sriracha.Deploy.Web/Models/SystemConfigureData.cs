using Sriracha.Deploy.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sriracha.Deploy.Web.Models
{
    public class SystemConfigureData
    {
        public bool AllowSelfRegistration { get; set; }

        [Required]
        public EnumPermissionAccess? DefaultAccess { get; set; }
    }
}