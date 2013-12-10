using Common.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MMDB.QuartzJobs
{
    public abstract class BaseJob : IBaseJob
    {
        private readonly ILog _logger;

        protected ManualResetEvent JobStoppedEvent { get; private set; }

        public BaseJob(ILog logger)
        {
            _logger = logger;
            this.JobStoppedEvent = new ManualResetEvent(true);
        }

        public void RunNow()
        {
            this.JobStoppedEvent.Set();
            try 
            {
                this.InternalRun();
            }
            finally
            {
                this.JobStoppedEvent.Reset();
            }
        }

        public void Execute(IJobExecutionContext context)
        {
            this.LogDebug("Job Started");
            this.JobStoppedEvent.Set();
            try 
            {
                this.InternalRun();
                this.LogDebug("Job Complete");
            }
            catch(Exception err)
            {
                this.LogException("Job Failed", err);
            }
            finally
            {
                this.JobStoppedEvent.Reset();
            }
        }

        protected string FormatLogMessage(string message, object[] values)
        {
            if(!string.IsNullOrEmpty(message) && values != null && values.Length > 0)
            {
                return string.Format(message, values);
            }
            else 
            {
                return message;
            }
        }

        protected void LogException(string message, Exception err, params object[] values)
        {
            if(this._logger != null)
            {
                this._logger.Error(FormatLogMessage(message, values), err);
            }
        }

        protected void LogDebug(string message, params object[] values)
        {
            if(this._logger != null)
            {
                this._logger.Debug(FormatLogMessage(message, values));
            }
        }

        protected abstract void InternalRun();
    }
}
