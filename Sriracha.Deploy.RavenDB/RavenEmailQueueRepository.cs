using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Repository;

namespace Sriracha.Deploy.RavenDB
{
	public class RavenEmailQueueRepository : IEmailQueueRepository
	{
		private readonly IDocumentSession _documentSession;

		public RavenEmailQueueRepository(IDocumentSession documentSession)
		{
			_documentSession = DIHelper.VerifyParameter(documentSession);
		}

		public SrirachaEmailMessage CreateMessage(List<string> emailAddresseList, object dataObject, string razorView)
		{
			var email = new SrirachaEmailMessage
			{
				Id = Guid.NewGuid().ToString(),
				EmailAddresseList = emailAddresseList, 
				DataObject = dataObject,
				RazorView = razorView
			};
			_documentSession.Store(email);
			_documentSession.SaveChanges();
			return email;
		}
	}
}
