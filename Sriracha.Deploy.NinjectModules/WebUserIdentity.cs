using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Sriracha.Deploy.Data;

namespace Sriracha.Deploy.NinjectModules
{
	public class WebUserIdentity : IUserIdentity
	{
		public string UserName
		{
			get 
			{ 
				if(HttpContext.Current != null && HttpContext.Current.User != null
						&& HttpContext.Current.User.Identity != null 
						&& !string.IsNullOrEmpty(HttpContext.Current.User.Identity.Name))
				{
					return HttpContext.Current.User.Identity.Name;
				}
				else 
				{
					return "(None)";
				}
			}
		}
	}
}
