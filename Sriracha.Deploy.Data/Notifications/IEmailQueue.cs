using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Dto;

namespace Sriracha.Deploy.Data.Notifications
{
	public interface IEmailQueue
	{
		SrirachaEmailMessage QueueMessage(string subject, List<string> emailAddresseList, object dataObject, string razorView);

		SrirachaEmailMessage PopNextMessage();

		void MarkSucceeded(SrirachaEmailMessage emailMessage);
		void MarkFailed(SrirachaEmailMessage emailMessage);

		void MarkReceipientSucceeded(SrirachaEmailMessage emailMessage, string emailAddress);
		void MarkReceipientFailed(SrirachaEmailMessage emailMessage, string emailAddress, Exception err);

	}
}
