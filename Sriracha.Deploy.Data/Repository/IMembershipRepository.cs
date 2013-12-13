using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Sriracha.Deploy.Data.Dto;
using Sriracha.Deploy.Data.Dto.Account;

namespace Sriracha.Deploy.Data.Repository
{
	public interface IMembershipRepository
	{
		SrirachaUser CreateUser(SrirachaUser user);
		SrirachaUser UpdateUser(SrirachaUser user);
		SrirachaUser TryUpdateUser(SrirachaUser user);
        SrirachaUser DeleteUser(SrirachaUser user);
        SrirachaUser DeleteUser(string userId);

        SrirachaUser GetUser(string userId);
        SrirachaUser GetUserByUserName(string userName);
		SrirachaUser TryGetUserByUserName(string userName);
		SrirachaUser GetUserByUserGuid(Guid userGuid);
		SrirachaUser TryGetUserByUserGuid(Guid userGuid);
		SrirachaUser GetUserByEmailAddress(string emailAddress);
		SrirachaUser TryGetUserByEmailAddress(string emailAddress);
		
		bool UserNameExists(string userName);

		bool EmailAddressExists(string email);

        PagedSortedList<SrirachaUser> GetUserList(ListOptions listOptions, List<string> userNameList = null);
        [Obsolete("User other one instead")]
        PagedSortedList<SrirachaUser> GetUserList_old(ListOptions listOptions, Expression<Func<SrirachaUser, bool>> filter);

        int GetUserCount(Expression<Func<SrirachaUser, bool>> filter = null);

    }
}
