using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Dto;

namespace Sriracha.Deploy.Data.Repository
{
	public interface IEmailQueueRepository
	{
		SrirachaEmailMessage CreateMessage(string subject, List<string> emailAddresseList, object dataObject, string razorView);
		SrirachaEmailMessage PopNextMessage();
		void UpdateMessageStatus(string emailMessageId, EnumEmailMessageStatus enumEmailMessageStatus);
		void AddReceipientResult(string emailMessageId, EnumEmailMessageStatus enumEmailMessageStatus, string emailAddress, Exception err = null);
	}
}
