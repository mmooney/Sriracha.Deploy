using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Credentials
{
	public interface IImpersonator
	{
		ImpersonationContext BeginImpersonation(string credentialsId);
	}
}
