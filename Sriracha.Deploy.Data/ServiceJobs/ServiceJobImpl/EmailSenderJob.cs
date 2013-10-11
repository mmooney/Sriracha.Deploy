using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.ConnectionSettings;
using MMDB.RazorEmail;
using NLog;
using Sriracha.Deploy.Data.Notifications;

namespace Sriracha.Deploy.Data.ServiceJobs.ServiceJobImpl
{
	public class EmailSenderJob : IEmailSenderJob
	{
		private readonly Logger _logger;
		private readonly IEmailQueue _emailQueue;
		private readonly IRazorEmailEngine _razorEmailEngine;
		private readonly IConnectionSettingsManager _connectionSettingsManager;
		private readonly ISystemSettings _systemSettings;
		private static volatile bool _isRunning = false;

		public EmailSenderJob(Logger logger, IEmailQueue emailQueue, IRazorEmailEngine razorEmailEngine, ISystemSettings systemSettings, IConnectionSettingsManager connectionSettingsManager)
		{
			_logger = DIHelper.VerifyParameter(logger);
			_emailQueue = DIHelper.VerifyParameter(emailQueue);
			_razorEmailEngine = DIHelper.VerifyParameter(razorEmailEngine);
			_systemSettings = DIHelper.VerifyParameter(systemSettings);
			_connectionSettingsManager = DIHelper.VerifyParameter(connectionSettingsManager);
		}

		public void Execute(Quartz.IJobExecutionContext context)
		{
			this._logger.Trace("Starting EmailSenderJob.Execute");
			lock (typeof(EmailSenderJob))
			{
				if (_isRunning)
				{
					this._logger.Info("EmailSenderJob already running");
					return;
				}
				else
				{
					_isRunning = true;
				}
			}
			try
			{
				bool done = false;
				while(!done)
				{
					bool anyError = false;
					var emailMessage = _emailQueue.PopNextMessage();
					if(emailMessage == null)
					{
						done = true;
						break;
					}
					foreach(string emailAddress in emailMessage.EmailAddressList)
					{
						try 
						{
							var emailSettings = _connectionSettingsManager.Load<MMDB.ConnectionSettings.EmailConnectionSettings>(EnumSettingSource.ConnectionString,"Email");
							AutoMapper.Mapper.CreateMap<MMDB.ConnectionSettings.EmailConnectionSettings, MMDB.RazorEmail.EmailServerSettings>();
							var razorEmailSettings = AutoMapper.Mapper.Map(emailSettings, new MMDB.RazorEmail.EmailServerSettings());
							_razorEmailEngine.SendEmail(razorEmailSettings, emailMessage.Subject, emailMessage.DataObject, emailMessage.RazorView, new List<string>{emailAddress}, _systemSettings.FromEmailAddress);
						}
						catch (Exception err)
						{
							_emailQueue.MarkReceipientFailed(emailMessage, emailAddress, err);
							anyError = true;
						}
					}
					if(anyError)
					{
						_emailQueue.MarkFailed(emailMessage);
					}
					else 
					{
						_emailQueue.MarkSucceeded(emailMessage);
					}
				}
			}
			catch (Exception err)
			{
				this._logger.ErrorException("EmailSenderJob failed: " + err.ToString(), err);
			}
			finally
			{
				_isRunning = false;
			}
			this._logger.Trace("Done EmailSenderJob.Execute");
		}
	}
}
