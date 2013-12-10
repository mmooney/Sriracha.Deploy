using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.QuartzJobs
{
    public interface IBaseInterruptableJob : IBaseJob, IInterruptableJob
    {
    }
}
