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
		SrirachaUser DeleteUser(SrirachaUser user);

		SrirachaUser LoadUserByUserName(string userName);
		SrirachaUser TryLoadUserByUserName(string userName);
		SrirachaUser LoadUserByUserGuid(Guid userGuid);
		SrirachaUser TryLoadUserByUserGuid(Guid userGuid);
		SrirachaUser LoadUserByEmailAddress(string emailAddress);
		SrirachaUser TryLoadUserByEmailAddress(string emailAddress);
		
		bool UserNameExists(string userName);

		bool EmailAddressExists(string email);

		PagedSortedList<SrirachaUser> GetUserList(ListOptions listOptions, Expression<Func<SrirachaUser, bool>> filter = null);
		int GetUserCount(Expression<Func<SrirachaUser, bool>> filter = null);
	}
}
