using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Dto.Account;
using Sriracha.Deploy.Data.Repository;

namespace Sriracha.Deploy.RavenDB
{
	public class RavenAccountSettingsRepository : IAccountSettingsRepository
	{
		private readonly IDocumentSession _documentSession;

		public RavenAccountSettingsRepository(IDocumentSession documentSession)
		{
			_documentSession = DIHelper.VerifyParameter(documentSession);
		}
		public AccountSettings GetAccountSettings(string userName)
		{
			return _documentSession.Load<AccountSettings>(userName);
		}
		
		public AccountSettings UpdateAccountSettings(string userName, string emailAddress, List<ProjectNotificationItem> notificationList)
		{
			var item = _documentSession.Load<AccountSettings>(userName);
			if(item == null)
			{
				item = new AccountSettings
				{
					Id = userName,
					UserName = userName,
				};
				_documentSession.Store(item);
			}
			item.EmailAddress = emailAddress;
			item.ProjectNotificationItemList = notificationList;
			_documentSession.SaveChanges();
			return item;
		}
	}
}
