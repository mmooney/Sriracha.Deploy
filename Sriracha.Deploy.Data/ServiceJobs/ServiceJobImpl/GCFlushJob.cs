using NLog;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.ServiceJobs.ServiceJobImpl
{
	public class GCFlushJob : IGCFlushJob
	{
		private readonly Logger _logger;
		public GCFlushJob(Logger logger)
		{
			_logger = logger;
		}

		public void Execute(IJobExecutionContext context)
		{
			_logger.Info("Starting GCFlushJob.Run");
			var existingMemory = GC.GetTotalMemory(false);
			_logger.Info("GCFlushJob: existing memory: " + existingMemory.ToString());
			GC.Collect();
			GC.WaitForPendingFinalizers();
			var afterMemory = GC.GetTotalMemory(true);
			_logger.Info("GCFlushJob: memory after collection: " + afterMemory.ToString());
			_logger.Info("Done GCFlushJob.Run");
		}
	}
}
