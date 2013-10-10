using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data.Repository
{
	public interface IEmailQueueRepository
	{
		Dto.SrirachaEmailMessage CreateMessage(List<string> emailAddresseList, object dataObject, string razorView);
	}
}
