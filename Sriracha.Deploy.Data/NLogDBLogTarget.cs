using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog.Targets;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Repository;

namespace Sriracha.Deploy.Data
{
	public class NLogDBLogTarget : TargetWithLayout
	{
		private readonly IUserIdentity _userIdentity;
		private readonly IDIFactory _diFactory;

		public NLogDBLogTarget(IDIFactory diFactory, IUserIdentity userIdentity)
		{
			_diFactory = DIHelper.VerifyParameter(diFactory);		
			_userIdentity = DIHelper.VerifyParameter(userIdentity);
		}

		protected override void Write(NLog.LogEventInfo logEvent)
		{
			if(!string.IsNullOrEmpty(logEvent.LoggerName) && logEvent.LoggerName.StartsWith("Raven.Client", StringComparison.CurrentCultureIgnoreCase))
			{
				//Don't cause SystemLogRepository messages to backfeed into themselves
				return;
			}
			string message = logEvent.FormattedMessage;
			if(logEvent.Exception != null)
			{
				message += ": " + logEvent.Exception.ToString();
			}
			EnumSystemLogType logType = EnumSystemLogType.Off;
			switch (logEvent.Level.Name)
			{
				case "Off":
					logType = EnumSystemLogType.Off;
					break;
				case "Fatal":
					logType = EnumSystemLogType.Fatal;
					break;
				case "Error":
					logType = EnumSystemLogType.Error;
					break;
				case "Warn":
					logType = EnumSystemLogType.Warn;
					break;
				case "Info":
					logType = EnumSystemLogType.Info;
					break;
				case "Debug":
					logType = EnumSystemLogType.Debug;
					break;
				case "Trace":
					logType = EnumSystemLogType.Trace;
					break;
			}
			//This object is created once and used over and over as a singleton, 
			//	so we don't want to hold an instance to the repository.  
			//	So use the IDIFactory interface to create the repository each time.
			var repository = _diFactory.CreateInjectedObject<ISystemLogRepository>();
			repository.LogMessage(logType, _userIdentity.UserName, logEvent.TimeStamp, message, logEvent.LoggerName);
			base.Write(logEvent);
		}

		protected override void InitializeTarget()
		{
			base.InitializeTarget();
		}

		protected override void Write(NLog.Common.AsyncLogEventInfo logEvent)
		{
			base.Write(logEvent);
		}

		protected override void Write(NLog.Common.AsyncLogEventInfo[] logEvents)
		{
			base.Write(logEvents);
		}
	}
}
