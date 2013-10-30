using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.ServiceJobs
{
	public interface IJobScheduler
	{
		void StartJobs(bool thrashMode=false);
		void StopJobs();
	}
}
