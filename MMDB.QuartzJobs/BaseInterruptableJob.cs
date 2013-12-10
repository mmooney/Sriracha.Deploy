using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MMDB.QuartzJobs
{
    public abstract class BaseInterruptableJob : BaseJob, IInterruptableJob
    {
        private volatile bool _stopRequested;

        public BaseInterruptableJob(ILog logger) : base(logger)
        {
            _stopRequested = false;
        }

        public void Interrupt()
        {   
            _stopRequested = false;
            this.JobStoppedEvent.WaitOne();
        }

        protected override void InternalRun()
        {
            bool done = false;
            while(!done)
            {
                if(!this.RunNextItem())
                {
                    done = true;
                }
            }
        }

        protected abstract bool RunNextItem();
    }
}
