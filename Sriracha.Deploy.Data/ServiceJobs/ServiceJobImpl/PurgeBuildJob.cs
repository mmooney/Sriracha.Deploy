using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Build;

namespace Sriracha.Deploy.Data.ServiceJobs.ServiceJobImpl
{
	public class PurgeBuildJob : IPurgeBuildJob
	{
		private readonly Logger _logger;
		private IBuildPurger _buildPurger;
		private IBuildManager _buildManager;

		public PurgeBuildJob(Logger logger, IBuildManager buildManager, IBuildPurger buildPurger)
		{
			_logger = DIHelper.VerifyParameter(logger);
			_buildManager = DIHelper.VerifyParameter(buildManager);
			_buildPurger = DIHelper.VerifyParameter(buildPurger);
		}

		public void Execute(Quartz.IJobExecutionContext context)
		{
			_logger.Info("Starting PurgeBuildJob.Execute");
			try 
			{
				bool doneList = false;
				int pageNumber = 1;
				while(!doneList)
				{
					var listOptions = new ListOptions
					{
						PageNumber = pageNumber,
						PageSize = 100,
						SortAscending = true,
						SortField = "UpdatedDateTimeUtc"
					};
					var list = _buildManager.GetBuildList(listOptions);
					if(list.Items != null)
					{
						foreach(var build in list.Items)
						{
							try 
							{
								_buildPurger.PurgeBuildIfNecessary(build);
							}
							catch(Exception err)
							{
								_logger.ErrorException(string.Format("Error purging build \"{0}\": {1}", build.DisplayValue, err.ToString()), err);
							}
						}
					}
					if(list.IsLastPage)
					{
						doneList = true;
					}
					else 
					{
						pageNumber++;
					}
				}
			}
			catch(Exception err)
			{
				this._logger.ErrorException("PurgeBuildJob.Execute Failed: " + err.ToString(), err);
			}
			_logger.Info("Done PurgeBuildJob.Execute");
		}
	}
}
