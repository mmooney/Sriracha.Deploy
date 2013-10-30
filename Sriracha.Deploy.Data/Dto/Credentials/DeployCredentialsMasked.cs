using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Dto.Credentials
{
	public class DeployCredentialsMasked
	{
		public string Id { get; set; }
		public string Domain { get; set; }
		public string UserName { get; set; }

		public string DisplayValue
		{
			get 
			{
				if(string.IsNullOrEmpty(this.Domain) && string.IsNullOrEmpty(this.UserName))
				{
					return null;
				}
				else if(string.IsNullOrEmpty(this.Domain) && !string.IsNullOrEmpty(this.UserName))
				{
					return this.UserName;
				}
				else if (!string.IsNullOrEmpty(this.Domain) && string.IsNullOrEmpty(this.UserName))
				{
					return this.Domain + "\\?";
				}
				else 
				{
					return this.Domain + "\\" + this.UserName;
				}
			}
		}
	}
}
