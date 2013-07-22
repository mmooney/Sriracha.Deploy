using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;
using Quartz;
using Quartz.Spi;

namespace Sriracha.Deploy.Data.ServiceJobs.ServiceJobImpl
{
	public class JobFactory : IJobFactory
	{
		private readonly IDIFactory _diFactory;
		private readonly Logger _logger;

		public JobFactory(IDIFactory diFactory, Logger logger)
		{
			_diFactory = DIHelper.VerifyParameter(diFactory);
			_logger = DIHelper.VerifyParameter(logger);
		}

		public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
		{
			try
			{
				this._logger.Trace(string.Format("JobFactory.NewJob called for {0}", bundle.JobDetail.JobType.FullName));
				var returnValue = _diFactory.CreateInjectedObject(bundle.JobDetail.JobType);
				this._logger.Trace(string.Format("Done JobFactory.NewJob called for {0}, returning {1}", bundle.JobDetail.JobType.FullName, returnValue.GetType().FullName));
				return (IJob)returnValue;
			}
			catch (Exception err)
			{
				this._logger.ErrorException(string.Format("JobFactory failed to create instance of {0}", bundle.JobDetail.JobType.FullName), err);
				throw;
			}
		}

		public void ReturnJob(IJob job)
		{
			//https://groups.google.com/forum/?fromgroups=#!topic/quartznet/nu_OKpi3rLw
			/* 
				This method is for job factory to allow returning of the instance back 
				to IoC container for proper cleanup. By default you don't need to do 
				anything if you haven't managed destroying of object using the 
				container before. 
			 */
		}
	}
}
