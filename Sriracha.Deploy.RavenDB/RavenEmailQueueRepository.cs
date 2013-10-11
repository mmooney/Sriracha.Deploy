using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using MMDB.Shared;
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

		public SrirachaEmailMessage CreateMessage(string subject, List<string> emailAddresseList, object dataObject, string razorView)
		{
			var email = new SrirachaEmailMessage
			{
				Id = Guid.NewGuid().ToString(),
				EmailAddressList = emailAddresseList, 
				Subject = subject,
				DataObject = dataObject,
				RazorView = razorView,
				QueueDateTimeUtc = DateTime.UtcNow,
				Status = EnumEmailMessageStatus.New
			};
			_documentSession.Store(email);
			_documentSession.SaveChanges();
			return email;
		}


		public SrirachaEmailMessage PopNextMessage()
		{
			string idToReturn = null;
			using(var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel= IsolationLevel.Serializable}))
			{
				var nextSet = _documentSession.Query<SrirachaEmailMessage>()
												.Where(i=>i.Status == EnumEmailMessageStatus.New)
												.OrderBy(i=>i.QueueDateTimeUtc)
												.Take(5);
				foreach(var nextItem in nextSet)
				{
					//The index may be stale, try loading it directly to ensure that the status is accurate
					var item = _documentSession.Load<SrirachaEmailMessage>(nextItem.Id);
					if(item.Status == EnumEmailMessageStatus.New)
					{
						item.Status = EnumEmailMessageStatus.InProcess;
						_documentSession.SaveChanges();
						idToReturn = item.Id;
						transaction.Complete();
						break;
					}
				}
			}
			if(!string.IsNullOrEmpty(idToReturn))
			{
				return _documentSession.Load<SrirachaEmailMessage>(idToReturn);
			}
			else 
			{
				return null;
			}
		}

		public void UpdateMessageStatus(string emailMessageId, EnumEmailMessageStatus status)
		{
			var item = _documentSession.Load<SrirachaEmailMessage>(emailMessageId);
			if(item == null)
			{
				throw new RecordNotFoundException(typeof(SrirachaEmailMessage), "Id", emailMessageId);
			}
			item.Status = status;
			_documentSession.SaveChanges();
		}

		public void AddReceipientResult(string emailMessageId, EnumEmailMessageStatus status, string emailAddress, Exception err = null)
		{
			var item = _documentSession.Load<SrirachaEmailMessage>(emailMessageId);
			if (item == null)
			{
				throw new RecordNotFoundException(typeof(SrirachaEmailMessage), "Id", emailMessageId);
			}
			var data = new SrirachaEmailMessage.SrirachaEmailMessageRecipientResult
			{
				Id = Guid.NewGuid().ToString(),
				SrirachaEmailMessageId = emailMessageId,
				EmailAddress = emailMessageId,
				Status = status,
				StatusDateTimeUtc = DateTime.UtcNow
			};
			if(err != null)
			{
				data.Details = err.ToString();
			}
			item.RecipientResultList.Add(data);
			_documentSession.SaveChanges();
		}
	}
}
