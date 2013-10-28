using Sriracha.Deploy.Data.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data
{
	public interface IPermissionValidator
	{
		UserEffectivePermissions GetUserEffectivePermissions(string userName);
		UserEffectivePermissions GetCurrentUserEffectivePermissions();
	}
}
