using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;
using Quartz;

namespace Sriracha.Deploy.Data.ServiceJobs.ServiceJobImpl
{
	public class FolderJobCleanupJob : IFolderCleanupJob
	{
		private readonly ICleanupManager _cleanupManager;
		private readonly Logger _logger;

		public FolderJobCleanupJob(ICleanupManager cleanupManager, Logger logger)
		{
			_cleanupManager = DIHelper.VerifyParameter(cleanupManager);
			_logger = DIHelper.VerifyParameter(logger);
		}

		public void Execute(IJobExecutionContext context)
		{
			this.InternalRun();
		}

		public void ForceRun()
		{
			this.InternalRun();
		}

		private void InternalRun() 
		{
			_logger.Info("Starting FolderCleanup.Execute");
			try 
			{
				bool done = false;
				while(!done)
				{
					var item = _cleanupManager.PopNextFolderCleanupTask(Environment.MachineName);
					if(item == null)
					{
						item = null;
					}
					else 
					{
						_logger.Info("Cleaning up folder {0} on machine {1}", item.FolderPath, null);
						try 
						{
							_cleanupManager.CleanupFolder(item);
							_cleanupManager.MarkItemSuccessful(item);
						}
						catch(Exception err)
						{
							_cleanupManager.MarkItemFailed(item, err);
						}
					}
				}
			}
			catch(Exception err)
			{
				this._logger.ErrorException("FolderCleanup.Execute Failed: " + err.ToString(), err);
			}
			_logger.Info("Done FolderCleanup.Execute");
		}
	}
}
