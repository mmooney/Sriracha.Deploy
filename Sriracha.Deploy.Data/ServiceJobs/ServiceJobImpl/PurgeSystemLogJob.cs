using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;
using Quartz;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Repository;

namespace Sriracha.Deploy.Data.ServiceJobs.ServiceJobImpl
{
	public class PurgeSystemLogJob : IPurgeSystemLogJob
	{
		private readonly ISystemLogRepository _systemLogRepository;
		private readonly ISystemSettings _systemSettings;
		private readonly Logger _logger;

		public PurgeSystemLogJob(ISystemLogRepository systemLogRepository, ISystemSettings systemSettings, Logger logger)
		{
			_systemLogRepository = DIHelper.VerifyParameter(systemLogRepository);
			_systemSettings = DIHelper.VerifyParameter(systemSettings);
			_logger = DIHelper.VerifyParameter(logger);
		}

		public void Execute(IJobExecutionContext context)
		{
			this._logger.Info("PurgeSystemLogJob started");
			try 
			{
				DateTime utcNow = DateTime.UtcNow;
				_systemLogRepository.PurgeLogMessages(utcNow, EnumSystemLogType.Trace, _systemSettings.LogPurgeTraceAgeMinutes);
				_systemLogRepository.PurgeLogMessages(utcNow, EnumSystemLogType.Debug, _systemSettings.LogPurgeDebugAgeMinutes);
				_systemLogRepository.PurgeLogMessages(utcNow, EnumSystemLogType.Info, _systemSettings.LogPurgeInfoAgeMinutes);
				_systemLogRepository.PurgeLogMessages(utcNow, EnumSystemLogType.Warn, _systemSettings.LogPurgeWarnAgeMinutes);
				_systemLogRepository.PurgeLogMessages(utcNow, EnumSystemLogType.Error, _systemSettings.LogPurgeErrorAgeMinutes);
				_systemLogRepository.PurgeLogMessages(utcNow, EnumSystemLogType.Fatal, _systemSettings.LogPurgeFatalAgeMinutes);
			}
			catch(Exception err)
			{
				this._logger.ErrorException("PurgeSystemLogJob Failed: " + err.ToString(), err);
			}
			this._logger.Info("PurgeSystemLogJob completed");
		}
	}
}
