using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Dto.Account;
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
