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
			var existingItem = TryGetUserByUserName(user.UserName);
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
            var sourceUser = _documentSession.LoadEnsureNoCache<SrirachaUser>(FormatId(user.UserName));
			var targetUser = AutoMapper.Mapper.Map(user, sourceUser);
            targetUser.UpdatedByUserName = _userIdentity.UserName;
            targetUser.UpdatedDateTimeUtc = DateTime.UtcNow;
            _documentSession.Store(targetUser); //replace existing record
			_documentSession.SaveChanges();
            return targetUser;
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
            var dbUser = this.GetUserByUserName(user.UserName);
            _documentSession.Delete(dbUser);
            _documentSession.SaveChanges();
            return dbUser;
        }

        public SrirachaUser DeleteUser(string userId)
        {
            var dbUser = _documentSession.LoadEnsure<SrirachaUser>(userId);
            return _documentSession.DeleteSaveEvict(dbUser);
        }

        public SrirachaUser GetUser(string userId)
        {
            return _documentSession.LoadEnsureNoCache<SrirachaUser>(userId);
        }

        public SrirachaUser GetUserByUserName(string userName)
		{
			var user = this.TryGetUserByUserName(userName);
			if(user == null)
			{
				throw new RecordNotFoundException(typeof(SrirachaUser), "UserName", userName);
			}
			return user;
		}

		public SrirachaUser TryGetUserByUserName(string userName)
		{
			return _documentSession.Load<SrirachaUser>(FormatId(userName));
		}

		public SrirachaUser GetUserByUserGuid(Guid userGuid)
		{
			var user = this.TryGetUserByUserGuid(userGuid);
			if(user == null)
			{
				throw new RecordNotFoundException(typeof(SrirachaUser), "UserGuid", userGuid);
			}
			return user;
		}

		public SrirachaUser TryGetUserByUserGuid(Guid userGuid)
		{
			return _documentSession.Query<SrirachaUser>().FirstOrDefault(i=>i.UserGuid == userGuid);
		}

		public SrirachaUser GetUserByEmailAddress(string emailAddress)
		{
			var user = this.TryGetUserByEmailAddress(emailAddress);
			if(user == null)
			{
				throw new RecordNotFoundException(typeof(SrirachaUser), "EmailAddress", emailAddress);
			}
			return user;
		}

		public SrirachaUser TryGetUserByEmailAddress(string emailAddress)
		{
			return _documentSession.Query<SrirachaUser>().FirstOrDefault(i=>i.EmailAddress == emailAddress);
		}

		public bool UserNameExists(string userName)
		{
			var item = TryGetUserByUserName(userName);
			return (item != null);
		}

		public bool EmailAddressExists(string email)
		{
			return _documentSession.Query<SrirachaUser>().Any(i=>i.EmailAddress == email);
		}

        public PagedSortedList<SrirachaUser> GetUserList(ListOptions listOptions, List<string> userNameList=null)
        {
            var query = _documentSession.QueryNoCache<SrirachaUser>();
            if(userNameList != null && userNameList.Count > 0)
            {
                query = query.Where(i=>i.UserName.In(userNameList));
            }
            PagedList.IPagedList<SrirachaUser> pagedList;
            listOptions = ListOptions.SetDefaults(listOptions, 20, 1, "UserName", true);
            switch (listOptions.SortField)
            {
                case "UserName":
                    var temp = query.OrderBy(i => i.UserName);
                    pagedList = query.PageAndSort(listOptions, i => i.UserName);
                    break;
                case "EmailAddress":
                    pagedList = query.PageAndSort(listOptions, i => i.EmailAddress);
                    break;
                default:
                    throw new Exception("Unsupported sort field " + listOptions.SortField);
            }
            return new PagedSortedList<SrirachaUser>(pagedList, listOptions.SortField, listOptions.SortAscending.Value);
        }

        public PagedSortedList<SrirachaUser> GetUserList_old(ListOptions listOptions, Expression<Func<SrirachaUser, bool>> filter)
		{
			var query = GetQuery(filter);
			PagedList.IPagedList<SrirachaUser> pagedList;
            listOptions = ListOptions.SetDefaults(listOptions, 20, 1, "UserName", true);
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
