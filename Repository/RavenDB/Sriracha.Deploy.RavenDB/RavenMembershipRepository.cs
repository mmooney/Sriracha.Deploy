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
using Sriracha.Deploy.Data.Exceptions;

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
            if(user == null)
            {
                throw new ArgumentNullException("user");
            }
            if(string.IsNullOrEmpty(user.UserName))
            {
                throw new ArgumentNullException("user.UserName");
            }
			var dbUser = AutoMapper.Mapper.Map(user, new SrirachaUser());
            if(dbUser.UserGuid == Guid.Empty)
            {
                dbUser.UserGuid = Guid.NewGuid();
            }
			var existingItem = TryGetUserByUserName(user.UserName);
			if(existingItem != null)
			{
				throw new ArgumentException(string.Format("User with username {0} already exists", user.UserName));
			}
            dbUser.SetCreatedFields(_userIdentity.UserName);
			dbUser.Id = FormatId(dbUser.UserName);
            return _documentSession.StoreSaveEvict(dbUser);
		}

		private string FormatId(string userName)
		{
            if(string.IsNullOrEmpty(userName))
            {
                throw new ArgumentNullException("userName");
            }
			return "SrirachaUser_" + userName.Replace('\\','_');
		}

		public SrirachaUser UpdateUser(SrirachaUser user)
		{
            if(user == null)
            {
                throw new ArgumentNullException("user");
            }
            if(string.IsNullOrEmpty(user.UserName))
            {
                throw new ArgumentNullException("user.UserName");
            }
            var sourceUser = _documentSession.LoadEnsureNoCache<SrirachaUser>(FormatId(user.UserName));
			var targetUser = AutoMapper.Mapper.Map(user, sourceUser);
            targetUser.CreatedDateTimeUtc = sourceUser.CreatedDateTimeUtc;
            targetUser.CreatedByUserName = sourceUser.CreatedByUserName;
            targetUser.UpdatedByUserName = _userIdentity.UserName;
            targetUser.UpdatedDateTimeUtc = DateTime.UtcNow;
            return _documentSession.StoreSaveEvict(targetUser);
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
            return _documentSession.LoadEnsureNoCache<SrirachaUser>(FormatId(userName));
        }

		public SrirachaUser TryGetUserByUserName(string userName)
		{
			return _documentSession.LoadNoCache<SrirachaUser>(FormatId(userName));
		}

		public SrirachaUser TryGetUserByUserGuid(Guid userGuid)
		{
            if(userGuid == Guid.Empty)
            {
                throw new ArgumentNullException("userGuid");
            }
			return _documentSession.QueryNoCache<SrirachaUser>().FirstOrDefault(i=>i.UserGuid == userGuid);
		}

		public SrirachaUser TryGetUserByEmailAddress(string emailAddress)
		{
            if(string.IsNullOrEmpty(emailAddress))
            {
                throw new ArgumentNullException("emailAddress");
            }
			return _documentSession.QueryNoCache<SrirachaUser>().FirstOrDefault(i=>i.EmailAddress == emailAddress);
		}

		public bool UserNameExists(string userName)
		{
			var item = TryGetUserByUserName(userName);
			return (item != null);
		}

		public bool EmailAddressExists(string email)
		{
            if(string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException("email");
            }
			return _documentSession.QueryNoCache<SrirachaUser>().Any(i=>i.EmailAddress == email);
		}

        public PagedSortedList<SrirachaUser> GetUserList(ListOptions listOptions, List<string> userNameList=null, List<string> emailAddressList=null)
        {
            var query = _documentSession.QueryNoCache<SrirachaUser>();
            if(userNameList != null && userNameList.Any())
            {
                query = query.Where(i=>i.UserName.In(userNameList));
            }
            if(emailAddressList != null && emailAddressList.Any())
            {
                query = query.Where(i=>i.EmailAddress.In(emailAddressList));
            }
            listOptions = ListOptions.SetDefaults(listOptions, 20, 1, "UserName", true);
            switch (listOptions.SortField)
            {
                case "UserName":
                    return query.PageAndSort(listOptions, i => i.UserName);
                case "EmailAddress":
                    return query.PageAndSort(listOptions, i => i.EmailAddress);
                default:
                    throw new UnrecognizedSortFieldException<SrirachaUser>(listOptions);
            }
        }

        //public PagedSortedList<SrirachaUser> GetUserList_old(ListOptions listOptions, Expression<Func<SrirachaUser, bool>> filter)
        //{
        //    var query = GetQuery(filter);
        //    PagedList.IPagedList<SrirachaUser> pagedList;
        //    listOptions = ListOptions.SetDefaults(listOptions, 20, 1, "UserName", true);
        //    switch(listOptions.SortField)
        //    {
        //        case "UserName":
        //            return query.PageAndSort(listOptions, i=>i.UserName);
        //        case "EmailAddress":
        //            return query.PageAndSort(listOptions, i=>i.EmailAddress);
        //        default:
        //            throw new UnrecognizedSortFieldException<SrirachaUser>(listOptions);
        //    }
        //}

		public int GetUserCount(DateTime? lastActivityDateTimeUtc=null)
		{
            var query = _documentSession.QueryNoCache<SrirachaUser>();
            if(lastActivityDateTimeUtc.HasValue)
            {
                query = query.Where(i=>i.LastActivityDateTimeUtc >= lastActivityDateTimeUtc.Value);
            }
			return query.Count();
		}

    }
}
