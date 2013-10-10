using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.Shared;
using Raven.Client;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Dto;

namespace Sriracha.Deploy.RavenDB
{
	public class RavenMembershipRepository : IMembershipRepository
	{
		private readonly IDocumentSession _documentSession;

		public RavenMembershipRepository(IDocumentSession documentSession)
		{
			_documentSession = DIHelper.VerifyParameter(documentSession);
		}

		public SrirachaUser CreateUser(SrirachaUser user)
		{
			var dbUser = AutoMapper.Mapper.Map(user, new SrirachaUser());
			var existingItem = TryLoadUserByUserName(user.UserName);
			if(existingItem != null)
			{
				throw new ArgumentException(string.Format("User with username {0} already exists", user.UserName));
			}
			dbUser.Id = FormatId(dbUser.UserName);
			_documentSession.Store(dbUser);
			_documentSession.SaveChanges();
			return dbUser;
		}

		private string FormatId(string userName)
		{
			return "SrirachaUser_" + userName;
		}

		public SrirachaUser UpdateUser(SrirachaUser user)
		{
			var dbUser = this.LoadUserByUserName(user.UserName);
			AutoMapper.Mapper.Map(dbUser, user.UserName);
			_documentSession.SaveChanges();
			return dbUser;
		}

		public SrirachaUser DeleteUser(SrirachaUser user)
		{
			var dbUser = this.LoadUserByUserName(user.UserName);
			_documentSession.Delete(dbUser);
			_documentSession.SaveChanges();
			return dbUser;
		}

		public SrirachaUser LoadUserByUserName(string userName)
		{
			var user = this.TryLoadUserByUserName(userName);
			if(user == null)
			{
				throw new RecordNotFoundException(typeof(SrirachaUser), "UserName", userName);
			}
			return user;
		}

		public SrirachaUser TryLoadUserByUserName(string userName)
		{
			return _documentSession.Load<SrirachaUser>(FormatId(userName));
		}

		public SrirachaUser LoadUserByUserGuid(Guid userGuid)
		{
			var user = this.TryLoadUserByUserGuid(userGuid);
			if(user == null)
			{
				throw new RecordNotFoundException(typeof(SrirachaUser), "UserGuid", userGuid);
			}
			return user;
		}

		public SrirachaUser TryLoadUserByUserGuid(Guid userGuid)
		{
			return _documentSession.Query<SrirachaUser>().FirstOrDefault(i=>i.UserGuid == userGuid);
		}

		public SrirachaUser LoadUserByEmailAddress(string emailAddress)
		{
			var user = this.TryLoadUserByEmailAddress(emailAddress);
			if(user == null)
			{
				throw new RecordNotFoundException(typeof(SrirachaUser), "EmailAddress", emailAddress);
			}
			return user;
		}

		public SrirachaUser TryLoadUserByEmailAddress(string emailAddress)
		{
			return _documentSession.Query<SrirachaUser>().FirstOrDefault(i=>i.EmailAddress == emailAddress);
		}

		public bool UserNameExists(string userName)
		{
			var item = TryLoadUserByUserName(userName);
			return (item != null);
		}

		public bool EmailAddressExists(string email)
		{
			return _documentSession.Query<SrirachaUser>().Any(i=>i.EmailAddress == email);
		}

		public PagedSortedList<SrirachaUser> GetUserList(ListOptions listOptions)
		{
			var pagedList = _documentSession.QueryPageAndSort<SrirachaUser>(listOptions, "UserName", false);
			return new PagedSortedList<SrirachaUser>(pagedList, listOptions.SortField, listOptions.SortAscending.Value );
		}

		public PagedSortedList<SrirachaUser> GetUserList(ListOptions listOptions, Func<SrirachaUser, bool> filter)
		{
			var pagedList = _documentSession.QueryPageAndSort<SrirachaUser>(listOptions, "UserName", false, filter);
			return new PagedSortedList<SrirachaUser>(pagedList, listOptions.SortField, listOptions.SortAscending.Value );
		}

		public int GetUserCount()
		{
			return _documentSession.Query<SrirachaUser>().Count();
		}

		public int GetUserCount(Func<SrirachaUser, bool> filter)
		{
			return _documentSession.Query<SrirachaUser>().Count(filter);
		}
	}
}
