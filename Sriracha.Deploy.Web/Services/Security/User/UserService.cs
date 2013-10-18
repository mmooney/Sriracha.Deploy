using ServiceStack.ServiceInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace Sriracha.Deploy.Web.Services.Security.User
{
	public class UserService : Service
	{
		public UserService()
		{

		}

		public object Get(UserRequest request)
		{
			List<string> userNameList = new List<string>();
			var userCollection = Membership.GetAllUsers();
			foreach(MembershipUser user in userCollection)
			{
				userNameList.Add(user.UserName);
			}
			return new 
			{
				UserNameList = userNameList
			};
		}
	}
}