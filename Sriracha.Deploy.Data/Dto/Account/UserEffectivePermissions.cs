﻿using Sriracha.Deploy.Data.Dto.Project.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Dto.Account
{
	public class UserEffectivePermissions
	{
		public string UserName { get; set;  }
        public List<DeployProjectEffectivePermissions> ProjectPermissionList { get; set; }
        public SystemRolePermissions SystemPermissions { get; set; }

		public UserEffectivePermissions()
		{
            this.ProjectPermissionList = new List<DeployProjectEffectivePermissions>();
		}
	}
}
