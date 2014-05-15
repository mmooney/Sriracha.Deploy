using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Repository;

namespace Sriracha.Deploy.Data.Notifications.NotificationImpl
{
	public class EmailQueue : IEmailQueue
	{
		private readonly IEmailQueueRepository _emailQueueRepository;

		public EmailQueue(IEmailQueueRepository emailQueueRepository)
		{
			_emailQueueRepository = DIHelper.VerifyParameter(emailQueueRepository);
		}

		public SrirachaEmailMessage QueueMessage(string subject, List<string> emailAddresseList, object dataObject, string razorView)
		{
			return _emailQueueRepository.CreateMessage(subject, emailAddresseList, dataObject, razorView);
		}

		public SrirachaEmailMessage PopNextMessage()
		{
			return _emailQueueRepository.PopNextMessage();
		}

		public void MarkSucceeded(SrirachaEmailMessage emailMessage)
		{
            _emailQueueRepository.UpdateMessageStatus(emailMessage.Id, EnumQueueStatus.Completed);
		}

		public void MarkFailed(SrirachaEmailMessage emailMessage)
		{
            _emailQueueRepository.UpdateMessageStatus(emailMessage.Id, EnumQueueStatus.Error);
		}

		public void MarkReceipientSucceeded(SrirachaEmailMessage emailMessage, string emailAddress)
		{
            _emailQueueRepository.AddReceipientResult(emailMessage.Id, EnumQueueStatus.Error, emailAddress);
		}

		public void MarkReceipientFailed(SrirachaEmailMessage emailMessage, string emailAddress, Exception err)
		{
            _emailQueueRepository.AddReceipientResult(emailMessage.Id, EnumQueueStatus.Error, emailAddress, err);
		}
	}
}
