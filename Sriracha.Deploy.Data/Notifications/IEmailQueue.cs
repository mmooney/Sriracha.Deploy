using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sriracha.Deploy.Data.Dto;

namespace Sriracha.Deploy.Data.Notifications
{
	public interface IEmailQueue
	{
		SrirachaEmailMessage QueueMessage(List<string> emailAddresseList, object dataObject, string razorView);
	}
}
