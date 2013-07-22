using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Impl
{
	public class DefaultSystemSettings : ISystemSettings
	{
		public int RunDeploymentPollingIntervalSeconds
		{
			get
			{
				return 60;
			}
		}
	}
}
