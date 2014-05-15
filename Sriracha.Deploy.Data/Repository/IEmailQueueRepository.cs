using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Dto;

namespace Sriracha.Deploy.Data.Repository
{
	public interface IEmailQueueRepository
	{
		SrirachaEmailMessage CreateMessage(string subject, List<string> emailAddressList, object dataObject, string razorView);
        SrirachaEmailMessage GetMessage(string id);
        SrirachaEmailMessage PopNextMessage();
		SrirachaEmailMessage UpdateMessageStatus(string emailMessageId, EnumQueueStatus status);
        SrirachaEmailMessage AddReceipientResult(string emailMessageId, EnumQueueStatus status, string emailAddress, Exception err = null);

    }
}
