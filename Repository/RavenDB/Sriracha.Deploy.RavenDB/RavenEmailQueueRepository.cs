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
        private readonly IUserIdentity _userIdentity;

		public RavenEmailQueueRepository(IDocumentSession documentSession, IUserIdentity userIdentity)
		{
			_documentSession = DIHelper.VerifyParameter(documentSession);
            _userIdentity = DIHelper.VerifyParameter(userIdentity);
		}

		public SrirachaEmailMessage CreateMessage(string subject, List<string> emailAddressList, object dataObject, string razorView)
		{
            if(string.IsNullOrEmpty(subject))
            {
                throw new ArgumentNullException("subject");
            }
            if(emailAddressList == null || emailAddressList.Count == 0)
            {
                throw new ArgumentNullException("emailAddressList");
            }
            if(dataObject == null)
            {
                throw new ArgumentNullException("dataObject");
            }
            if(string.IsNullOrEmpty(razorView))
            {
                throw new ArgumentNullException("razorView");
            }
			var email = new SrirachaEmailMessage
			{
				Id = Guid.NewGuid().ToString(),
				EmailAddressList = emailAddressList, 
				Subject = subject,
				DataObject = dataObject,
				RazorView = razorView,
				QueueDateTimeUtc = DateTime.UtcNow,
                Status = EnumQueueStatus.New,
                CreatedByUserName = _userIdentity.UserName,
                CreatedDateTimeUtc = DateTime.UtcNow,
                UpdatedByUserName = _userIdentity.UserName,
                UpdatedDateTimeUtc = DateTime.UtcNow
			};
            return _documentSession.StoreSaveEvict(email);
		}

        public SrirachaEmailMessage GetMessage(string id)
        {
            return _documentSession.LoadEnsureNoCache<SrirachaEmailMessage>(id);
        }

		public SrirachaEmailMessage PopNextMessage()
		{
			string idToReturn = null;
			using(var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel= IsolationLevel.Serializable}))
			{
				var nextSet = _documentSession.Query<SrirachaEmailMessage>()
                                                .Where(i => i.Status == EnumQueueStatus.New)
												.OrderBy(i=>i.QueueDateTimeUtc)
												.Take(5);
				foreach(var nextItem in nextSet)
				{
					//The index may be stale, try loading it directly to ensure that the status is accurate
					var item = _documentSession.Load<SrirachaEmailMessage>(nextItem.Id);
                    if (item.Status == EnumQueueStatus.New)
					{
                        item.Status = EnumQueueStatus.InProcess;
                        item.StartedDateTimeUtc = DateTime.UtcNow;
                        item.UpdatedByUserName = _userIdentity.UserName;
                        item.UpdatedDateTimeUtc = DateTime.UtcNow;
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

		public SrirachaEmailMessage UpdateMessageStatus(string emailMessageId, EnumQueueStatus status)
		{
			var item = _documentSession.Load<SrirachaEmailMessage>(emailMessageId);
			if(item == null)
			{
				throw new RecordNotFoundException(typeof(SrirachaEmailMessage), "Id", emailMessageId);
			}
			item.Status = status;
            item.UpdatedByUserName = _userIdentity.UserName;
            item.UpdatedDateTimeUtc = DateTime.UtcNow;
			return _documentSession.SaveEvict(item);
		}

        public SrirachaEmailMessage AddReceipientResult(string emailMessageId, EnumQueueStatus status, string emailAddress, Exception err = null)
		{
            if (string.IsNullOrEmpty(emailAddress))
            {
                throw new ArgumentNullException("emailAddress");
            }
            var message = _documentSession.LoadEnsure<SrirachaEmailMessage>(emailMessageId);
            if(message.EmailAddressList == null || !message.EmailAddressList.Contains(emailAddress))
            {
                throw new RecordNotFoundException(typeof(SrirachaEmailMessage), "EmailAddress", emailAddress);
            }
			var data = new SrirachaEmailMessage.SrirachaEmailMessageRecipientResult
			{
				Id = Guid.NewGuid().ToString(),
				SrirachaEmailMessageId = emailMessageId,
				EmailAddress = emailAddress,
				Status = status,
				StatusDateTimeUtc = DateTime.UtcNow,
                CreatedByUserName = _userIdentity.UserName,
                CreatedDateTimeUtc = DateTime.UtcNow,
                UpdatedByUserName = _userIdentity.UserName,
                UpdatedDateTimeUtc = DateTime.UtcNow
			};
			if(err != null)
			{
				data.Details = err.ToString();
			}
			message.RecipientResultList.Add(data);
            message.UpdatedByUserName = _userIdentity.UserName;
            message.UpdatedDateTimeUtc = DateTime.UtcNow;
			return _documentSession.SaveEvict(message);
		}

    }
}
