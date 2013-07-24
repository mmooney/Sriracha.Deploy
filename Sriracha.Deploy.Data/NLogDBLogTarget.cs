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
		private readonly ISystemLogRepository _systemLogRepository;
		private readonly IUserIdentity _userIdentity;

		public NLogDBLogTarget(ISystemLogRepository systemLogRepository, IUserIdentity userIdentity)
		{
			_systemLogRepository = DIHelper.VerifyParameter(systemLogRepository);		
			_userIdentity = DIHelper.VerifyParameter(userIdentity);
		}

		protected override void Write(NLog.LogEventInfo logEvent)
		{
			if(logEvent.LoggerName == "Raven.Client.Document.InMemoryDocumentSessionOperations")
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
			_systemLogRepository.LogMessage(logType, _userIdentity.UserName, logEvent.TimeStamp, message);
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
