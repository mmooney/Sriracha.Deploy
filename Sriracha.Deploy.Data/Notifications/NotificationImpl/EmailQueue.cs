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

		public SrirachaEmailMessage QueueMessage(List<string> emailAddresseList, object dataObject, string razorView)
		{
			return _emailQueueRepository.CreateMessage(emailAddresseList, dataObject, razorView);
		}
	}
}
