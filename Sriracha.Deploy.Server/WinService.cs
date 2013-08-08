using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.ServiceJobs;

namespace Sriracha.Deploy.Server
{
	public partial class WinService : ServiceBase
	{
		private readonly IJobScheduler _jobScheduler;

		public WinService(IJobScheduler jobScheduler)
		{
			InitializeComponent();
			_jobScheduler = DIHelper.VerifyParameter(jobScheduler);
		}

		protected override void OnStart(string[] args)
		{
			this._jobScheduler.StartJobs();
		}

		protected override void OnStop()
		{
			this._jobScheduler.StopJobs();
		}

		public void DebugStart()
		{
			this._jobScheduler.StartJobs();
			while (true)
			{
				Thread.Sleep(1000);
			}
		}
	}
}
