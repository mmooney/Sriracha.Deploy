using Sriracha.Deploy.Data.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sriracha.Deploy.Data
{
    public interface IUserManager
    {
        PagedSortedList<SrirachaUser> GetUserList(ListOptions listOptions, List<string> userNameList=null);
        SrirachaUser GetUser(string userId);
        SrirachaUser GetUserByUserName(string userName);
        SrirachaUser TryGetUserByUserName(string userName);

        SrirachaUser CreateUser(string userName, string emailAddress, string password);
        SrirachaUser UpdateUser(string userId, string userName, string emailAddress, string password);

        SrirachaUser DeleteUser(string userId);
    }
}
