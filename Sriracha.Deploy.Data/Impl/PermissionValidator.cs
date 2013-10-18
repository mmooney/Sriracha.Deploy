using Sriracha.Deploy.Data.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Impl
{
	public class PermissionValidator : IPermissionValidator
	{
		public UserEffectivePermissions GetUserEffectivePermissions(string userName)
		{
			throw new NotImplementedException();
			//var returnValue = new UserEffectivePermissions
			//{
			//	UserName = userName
			//};

			//return returnValue;
		}
	}
}
