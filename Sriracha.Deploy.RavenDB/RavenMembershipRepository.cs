using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using MMDB.Shared;
using Raven.Client;
using Raven.Client.Linq;
using Sriracha.Deploy.Data;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Repository;

namespace Sriracha.Deploy.RavenDB
{
	public class RavenMembershipRepository : IMembershipRepository
	{
		private readonly IDocumentSession _documentSession;
		private readonly IUserIdentity _userIdentity;

		public RavenMembershipRepository(IDocumentSession documentSession, IUserIdentity userIdentity)
		{
			_documentSession = DIHelper.VerifyParameter(documentSession);
			_userIdentity = DIHelper.VerifyParameter(userIdentity);
		}

		public SrirachaUser CreateUser(SrirachaUser user)
		{
			var dbUser = AutoMapper.Mapper.Map(user, new SrirachaUser());
			var existingItem = TryLoadUserByUserName(user.UserName);
			if(existingItem != null)
			{
				throw new ArgumentException(string.Format("User with username {0} already exists", user.UserName));
			}
			dbUser.CreatedByUserName = _userIdentity.UserName;
			dbUser.CreatedDateTimcUtc = DateTime.UtcNow;
			dbUser.UpdatedByUserName = _userIdentity.UserName;
			dbUser.UpdatedDateTimeUtc = DateTime.UtcNow;
			dbUser.Id = FormatId(dbUser.UserName);
			_documentSession.Store(dbUser);
			try 
			{
				_documentSession.SaveChanges();
			}
			catch(Exception err)
			{
				Debug.WriteLine(err.ToString());
				throw;
			}
			return dbUser;
		}

		private string FormatId(string userName)
		{
			return "SrirachaUser_" + userName.Replace('\\','_');
		}

		public SrirachaUser UpdateUser(SrirachaUser user)
		{
			var dbUser = this.LoadUserByUserName(user.UserName);
			AutoMapper.Mapper.Map(dbUser, user.UserName);
			user.UpdatedByUserName = _userIdentity.UserName;
			user.UpdatedDateTimeUtc = DateTime.UtcNow;
			_documentSession.SaveChanges();
			return dbUser;
		}

		public SrirachaUser TryUpdateUser(SrirachaUser user)
		{
			try 
			{
				return this.UpdateUser(user);
			}
			catch(Raven.Abstractions.Exceptions.ConcurrencyException)
			{
				return null;
			}
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

		public PagedSortedList<SrirachaUser> GetUserList(ListOptions listOptions, Expression<Func<SrirachaUser, bool>> filter = null)
		{
			var query = GetQuery(filter);
			PagedList.IPagedList<SrirachaUser> pagedList;
			listOptions.SortField = StringHelper.IsNullOrEmpty(listOptions.SortField, "UserName");
			switch(listOptions.SortField)
			{
				case "UserName":
					var temp = query.OrderBy(i=>i.UserName);
					pagedList = query.PageAndSort(listOptions, i=>i.UserName);
					break;
				case "EmailAddress":
					pagedList = query.PageAndSort(listOptions, i=>i.EmailAddress);
					break;
				default:
					throw new Exception("Unsupported sort field " + listOptions.SortField);
			}
			return new PagedSortedList<SrirachaUser>(pagedList, listOptions.SortField, listOptions.SortAscending.Value );
		}

		public int GetUserCount(Expression<Func<SrirachaUser, bool>> filter=null)
		{
			return GetQuery(filter).Count();
		}

		private IRavenQueryable<SrirachaUser> GetQuery(Expression<Func<SrirachaUser, bool>> filter)
		{
			var query = _documentSession.Query<SrirachaUser>();
			if (filter != null)
			{
				return query.Where(filter);
			}
			else
			{
				return query;
			}
		}

	}
}
